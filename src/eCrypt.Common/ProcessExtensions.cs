namespace eVision.eCrypt.Common
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ProcessExtensions
    {
        public static Task<int> WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(((Process)sender).ExitCode);

            if (cancellationToken != default(CancellationToken))
            {
                cancellationToken.Register(tcs.SetCanceled);
            }

            return tcs.Task;
        }
    }
}