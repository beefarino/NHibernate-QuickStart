namespace CodeOwls.PowerShell.Host.Executors
{
    public interface IRunnableCommandExecutor : ICommandExecutor
    {
        void Stop();
        void Stop(bool force);
        void Run();
    }
}