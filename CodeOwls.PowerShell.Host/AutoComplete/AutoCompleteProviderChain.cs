using System.Collections.Generic;
using System.Linq;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class AutoCompleteProviderChain : IAutoCompleteProvider
    {
        private readonly IEnumerable<IAutoCompleteProvider> _providers;

        public AutoCompleteProviderChain(params IAutoCompleteProvider[] providers)
        {
            _providers = providers;
        }

        public AutoCompleteProviderChain(IEnumerable<IAutoCompleteProvider> providers)
        {
            _providers = providers;
        }

        #region IAutoCompleteProvider Members

        public IEnumerable<string> GetSuggestions(string guess)
        {
            if (!_providers.Any())
            {
                return new string[] {};
            }

            Queue<IAutoCompleteProvider> queue = new Queue<IAutoCompleteProvider>(_providers);

            List<string> suggestions = new List<string>();
            while (queue.Any() && ! suggestions.Any())
            {
                IAutoCompleteProvider current = queue.Dequeue();
                var currentStuggestions = current.GetSuggestions(guess);
                if (!currentStuggestions.Any())
                {
                    continue;
                }
                suggestions.AddRange(currentStuggestions);
            }

            return suggestions;
        }

        #endregion
    }
}