using LINQPad;
using System.Reflection;
using System.Text.RegularExpressions;
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace LINQPadHelpers.Azure;

public partial class KeyVaultSecrets
{
    public static string? TenantId { get; set; }
    public static string KeyVaultUrl { get; set; } = "https://lcp-msvs-common-dev-kv.vault.azure.net/";
    public static string KeyVaultName
    {
        get => AzureKeyVaultRegex().Replace(new Uri(KeyVaultUrl).Host, string.Empty);
        set => KeyVaultUrl = $"https://{value}.vault.azure.net/";
    }

    public static Lazy<SecretClient> SecretClient { get; set; } =
        new(() =>
            {
                var cloud = Util.AzureCloud.FindByVaultHostOrUri(KeyVaultUrl) ?? Util.AzureCloud.PublicCloud;
                var credential =
                    new LINQPadTokenCredential(cloud.AuthenticationEndpoint + (TenantId ?? Util.LoadString("DefaultTenantId")),
                                               PInvoke.GetCurrentUsername.GetCurrentUPN());
                return new SecretClient(new Uri(KeyVaultUrl), credential);
            });

    public static async Task<string?> GetSecretAsync(string key, bool forceRefresh = false) =>
        await Util.Cache(async () =>
                         {
                             try
                             {
                                 var s = await SecretClient.Value.GetSecretAsync(key);
                                 return s.Value.Value;
                             }
                             catch (RequestFailedException ex)
                             {
                                 ex.Dump();
                                 return string.Empty;
                             }
                         }, $"{KeyVaultUrl}_{key}", forceRefresh);

    public static async Task<KeyVaultSecret> SetSecretAsync(string secretName, string secretValue, string contentType = "text/plain", bool enabled = true, DateTimeOffset? expiresOn = null, DateTimeOffset? notBefore = null)
    {
        Console.WriteLine("{0}: Setting secret \"{1}\" to \"{2}\".", KeyVaultUrl, secretName, secretValue);
        var response = await SecretClient.Value.SetSecretAsync(secretName, secretValue);
        try
        {
            var value = response.Value;
            value.Dump();
            var properties = value.Properties;
            if (contentType == string.Empty && enabled && expiresOn == null && notBefore == null) 
                return value;
            properties.ContentType = contentType;
            properties.Enabled = enabled;
            properties.ExpiresOn = expiresOn;
            properties.NotBefore = notBefore;
            properties = await SecretClient.Value.UpdateSecretPropertiesAsync(properties);
            var secret =
                (KeyVaultSecret)typeof(KeyVaultSecret)
                                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                null, [typeof(SecretProperties)], null)!
                                .Invoke([properties]);
            return secret;
        }
        catch (Exception ex)
        {
            // source doesn't say what exception is thrown if there's no value. *shrug*
            ex.Dump();
            throw;
        }
    }
    public static async Task<SecretProperties> SetSecretPropertiesAsync(
        string secretName, 
        string contentType = "text/plain", 
        bool enabled = true, 
        DateTimeOffset? expiresOn = null, 
        DateTimeOffset? notBefore = null
        )
    {
        var properties = await SecretClient.Value
                                           .UpdateSecretPropertiesAsync(new SecretProperties(secretName)
                                                                        {
                                                                            ContentType = contentType,
                                                                            Enabled = enabled,
                                                                            ExpiresOn = expiresOn,
                                                                            NotBefore = notBefore
                                                                        });
        return properties.Value;
    }
    public static async Task DeleteSecretAsync(string secretName)
    {
        var operation = await SecretClient.Value.StartDeleteSecretAsync(secretName);
        await operation.WaitForCompletionAsync();
    }
    public static async Task PurgeSecretAsync(string secretName)
        => await SecretClient.Value.PurgeDeletedSecretAsync(secretName);
    
    [GeneratedRegex(@"\.vault\.azure\.net/?")]
    private static partial Regex AzureKeyVaultRegex();
}