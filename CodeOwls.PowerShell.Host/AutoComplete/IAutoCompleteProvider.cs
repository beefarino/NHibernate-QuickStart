using System.Collections.Generic;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal interface IAutoCompleteProvider
    {
        IEnumerable<string> GetSuggestions(string guess);
    }
}