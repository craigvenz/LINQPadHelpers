using System.Reflection;
using Azure;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using FluentAssertions;
using LINQPadHelpers.Azure;

namespace LINQPadHelpers.Tests;

public class KeyVaultSecretsTests 
{
    [Fact]
    public void KeyVaultSecrets_SettingName_ReturnsCorrectUrl()
    {
        KeyVaultSecrets.KeyVaultName = "sometestvalue";
        KeyVaultSecrets.KeyVaultUrl.Should().Be("https://sometestvalue.vault.azure.net/");
        KeyVaultSecrets.KeyVaultName.Should().Be("sometestvalue");
    }

    [Fact]
    public async Task GetSecretAsync_WhenSecretClientThrows_ReturnsEmptyString()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.GetSecretImplementation = (_, _, _) => throw new RequestFailedException("Throwing exception on purpose");
        var result = await KeyVaultSecrets.GetSecretAsync("dummyvalue");
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSecretAsync_ReturnsValue()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.GetSecretImplementation =
            (_, _, _) => new DummySecretClient.DummyResponse<KeyVaultSecret>(new KeyVaultSecret("key", "value"));
        var value = await KeyVaultSecrets.GetSecretAsync("key");
        value.Should().Be("value");
    }

    [Fact]
    public async Task WhenSettingAValueAndExceptionIsThrown_Rethrows()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.SetSecretImplementation =
            (_, _, _) => throw new RequestFailedException("Throwing exception on purpose");

        var act = () => KeyVaultSecrets.SetSecretAsync("key", "value");
        await act.Should().ThrowAsync<RequestFailedException>();
    }

    [Fact]
    public async Task
        KeyVaultSecrets_WhenSettingAValueAndUpdateSecretPropertiesAsyncThrowsException_DumpsAndRethrows()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.SetSecretImplementation =
            (_, _, _) => new DummySecretClient.DummyResponse<KeyVaultSecret>(new KeyVaultSecret("key", "value"));
        dummy.UpdateSecretPropertiesAsyncImplementation =
            (_, _) => throw new RequestFailedException("Throwing");
        var act = () => KeyVaultSecrets.SetSecretAsync("key","value");
        await act.Should().ThrowAsync<RequestFailedException>();
    }

    [Fact]
    public async Task WhenSettingAValueWithEmptyContentTypes_ReturnsEarly()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.SetSecretImplementation =
            (_, _, _) => new DummySecretClient.DummyResponse<KeyVaultSecret>(new KeyVaultSecret("key", "value"));
        dummy.UpdateSecretPropertiesAsyncImplementation =
            (_, _) => new DummySecretClient.DummyResponse<SecretProperties>(new SecretProperties("a"));
        var response = await KeyVaultSecrets.SetSecretAsync("", "", "");
        response.Value.Should().Be("value");
        response.Properties.ContentType.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task WhenSetSecretAsync_WithCustomProperties_ReturnsPropertiesInSecretProperties()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        dummy.SetSecretImplementation =
            (_, _, _) => new DummySecretClient.DummyResponse<KeyVaultSecret>(new KeyVaultSecret("key", "value"));
        dummy.UpdateSecretPropertiesAsyncImplementation =
            (sp , _) => new DummySecretClient.DummyResponse<SecretProperties>(sp);
        var response = await KeyVaultSecrets.SetSecretAsync("", "", "custom content type", false,
                                             new DateTimeOffset(2024, 1, 1, 0, 1, 2, new TimeSpan()),
                                             new DateTimeOffset(2023, 12, 31, 5, 6, 7, new TimeSpan()));
        response.Properties.Should()
                .Match<SecretProperties>(p => p.Enabled == false &&
                                              p.ContentType == "custom content type" &&
                                              p.Name == "key" &&
                                              p.NotBefore.HasValue &&
                                              p.ExpiresOn.HasValue);
    }

    [Fact]
    public async Task WhenDeleteSecretAsyncCalled_Succeeds()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        var act = () => KeyVaultSecrets.DeleteSecretAsync("");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task WhenPurgeSecretAsyncCalled_Succeeds()
    {
        var dummy = new DummySecretClient();
        KeyVaultSecrets.SecretClient = new Lazy<SecretClient>(() => dummy);
        var act = () => KeyVaultSecrets.PurgeSecretAsync("");
        await act.Should().NotThrowAsync();
    }

    private class DummySecretClient : SecretClient
    {
        public Func<string,string,CancellationToken,Response<KeyVaultSecret>> GetSecretImplementation { get; set; }
            = (n, v, ct) => new DummyResponse<KeyVaultSecret>(new KeyVaultSecret("dummyvalue","dummyvalue"));

        public Func<string, string, CancellationToken, Response<KeyVaultSecret>> SetSecretImplementation
        {
            get;
            set;
        }
            = (n, v, ct) => null!;

        public Func<SecretProperties, CancellationToken, Response<SecretProperties>>
            UpdateSecretPropertiesAsyncImplementation
        {
            get;
            set;
        } = (sp, ct) => null!;

        public sealed class DummyResponse<T>(T value) : global::Azure.Response<T>
        {
            public override T Value { get; } = value;
            public override Response GetRawResponse() => throw new NotImplementedException();
        }

        private sealed class DummyResponse : global::Azure.Response
        {
            public override void Dispose() => throw new NotImplementedException();
            protected override bool TryGetHeader(string name, out string value)
                => throw new NotImplementedException();
            protected override bool TryGetHeaderValues(string name, out IEnumerable<string> values)
                => throw new NotImplementedException();
            protected override bool ContainsHeader(string name)
                => throw new NotImplementedException();
            protected override IEnumerable<HttpHeader> EnumerateHeaders()
                => throw new NotImplementedException();
            public override int Status { get; } = 0;
            public override string ReasonPhrase { get; } = string.Empty;
            public override Stream? ContentStream { get; set; }
            public override string ClientRequestId { get; set; } = string.Empty;
        }
            
        public override Task<Response<KeyVaultSecret>> GetSecretAsync(string name, string version = null!, CancellationToken cancellationToken = new())
            => Task.FromResult(GetSecretImplementation(name, version, cancellationToken));

        public override Task<Response<KeyVaultSecret>> SetSecretAsync(string name, string value, CancellationToken cancellationToken = new())
            => Task.FromResult(SetSecretImplementation(name, value, cancellationToken));

        public override Task<Response<SecretProperties>> UpdateSecretPropertiesAsync(SecretProperties properties,
                                                                                     CancellationToken cancellationToken = new())
            => Task.FromResult(UpdateSecretPropertiesAsyncImplementation(properties, cancellationToken));

        public override Task<DeleteSecretOperation> StartDeleteSecretAsync(string name, CancellationToken cancellationToken = new())
            => Task.FromResult((DeleteSecretOperation) new DummyKeyVaultOperation());

        public override Task<Response> PurgeDeletedSecretAsync(string name, CancellationToken cancellationToken = new())
            => Task.FromResult((Response) new DummyResponse());

        private class DummyKeyVaultOperation : DeleteSecretOperation
        {
            /// <inheritdoc />
            public override ValueTask<Response<DeletedSecret>> WaitForCompletionAsync(CancellationToken cancellationToken = new())
            {
                var value = (DeletedSecret)typeof(DeletedSecret)
                    .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                    null, [typeof(SecretProperties)], null)!.Invoke([
                                 new SecretProperties("abc")
                             ]);
                return ValueTask.FromResult((Response<DeletedSecret>)new DummyResponse<DeletedSecret>(value));
            }
        }
    }
}
