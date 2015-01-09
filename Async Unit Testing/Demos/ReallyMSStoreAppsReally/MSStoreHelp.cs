using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

public static class MSStoreHelp
{
    public static async Task ExecuteAsync(Func<Task> testLogic)
    {
        var tcs = new TaskCompletionSource<object>();
        await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
        {
            // This is actually an async void method. :(
            try
            {
                await testLogic().ConfigureAwait(false);
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        await tcs.Task.ConfigureAwait(false);
    }
}