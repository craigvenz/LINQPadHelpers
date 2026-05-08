using FluentAssertions;
using LINQPadHelpers.Extensions;
using Moq;

namespace LINQPadHelpers.Tests;

public class DisposableExtensionsTests()
{
    [Fact]
    public void Using_TDisposable_TReturn_CallsDispose()
    {
        var disposable = new Mock<IDisposable>();

        var funcCalled = false;
        var returned = disposable.Object.Using(_ =>
                                               {
                                                   funcCalled = true;
                                                   return true;
                                               });
        returned.Should().BeTrue();
        disposable.Verify(d => d.Dispose(), Times.Once);
        funcCalled.Should().BeTrue();
    }

    [Fact]
    public void Using_TDisposable_CallsDispose()
    {
        var disposable = new Mock<IDisposable>();

        var funcCalled = false;
        disposable.Object.Using(_ => funcCalled = true);
        disposable.Verify(d => d.Dispose(), Times.Once);
        funcCalled.Should().BeTrue();
    }
    [Fact]
    public async Task AwaitUsing_TDisposable_TReturn_CallsDispose()
    {
        var disposable = new Mock<IAsyncDisposable>();

        var funcCalled = false;
        var returned = await disposable.Object.AwaitUsing(_ =>
                                                          {
                                                              funcCalled = true;
                                                              return Task.FromResult(true);
                                                          });
        returned.Should().BeTrue();
        disposable.Verify(d => d.DisposeAsync(), Times.Once);
        funcCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Using_TDisposable_AsyncAction_CallsDispose()
    {
        var disposable = new Mock<IAsyncDisposable>();
            
        var funcCalled = false;
        var delayed = false;
        await disposable.Object.AwaitUsing(async _ =>
                                           {
                                               funcCalled = true;
                                               await Task.Delay(100);
                                               delayed = true;
                                           });
        funcCalled.Should().BeTrue();
        delayed.Should().BeTrue();
        disposable.Verify(d => d.DisposeAsync(), Times.Once);
    }
    /*
    [Fact]
    public async Task Using_TaskTDisposable_TReturn_CallsDispose()
    {
        var disposable = new Mock<Task<IDisposable>>();
        disposable.Setup(x => x.Result)
                  .Returns();

        var funcCalled = false;
        var delayed = false;
        var returned = await disposable.Object.Using(t =>
                                                     {
                                                         
                                                         funcCalled = true;
                                                         await Task.Delay(100);
                                                         delayed = true;
                                                     });
        returned.Should().BeTrue();
        funcCalled.Should().BeTrue();
        delayed.Should().BeTrue();
        disposable.Verify(d => d.Dispose(), Times.Once);
    }
    */
}
