namespace CodeOwls.PowerShell.Host.History
{
    public interface IHistoryStackWalker
    {
        string NextUp();
        string NextDown();

        string Oldest();
        string Newest();

        void Reset();
    }
}