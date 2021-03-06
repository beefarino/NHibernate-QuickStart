using CodeOwls.PowerShell.Host.Executors;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class ProviderPathAutoCompleteProvider : CommandAutoCompleteProvider
    {
        private const string Command = @"resolve-path '{0}' -relative";

        public ProviderPathAutoCompleteProvider(Executor executor) : base(Command, executor)
        {
        }
    }
}