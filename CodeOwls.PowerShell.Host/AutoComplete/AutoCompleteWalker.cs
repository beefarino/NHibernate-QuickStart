using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class AutoCompleteWalker : IAutoCompleteWalker
    {
        private readonly List<IAutoCompleteProvider> _providers;
        private string _currentGuess;
        private List<string> _currentSuggestions;
        private int _index;

        public AutoCompleteWalker(List<IAutoCompleteProvider> providers)
        {
            _providers = providers;
        }

        #region IAutoCompleteWalker Members

        public string NextUp(string guess)
        {
            bool isReset = LoadSuggestionsForGuess(guess);

            if (! _currentSuggestions.Any())
            {
                return null;
            }

            if (! isReset)
            {
                ++_index;

                _index = Math.Min(_currentSuggestions.Count - 1, Math.Max(0, _index));
            }

            return _currentSuggestions[_index];
        }

        public string NextDown(string guess)
        {
            bool isReset = LoadSuggestionsForGuess(guess);

            if (!_currentSuggestions.Any())
            {
                return null;
            }

            if (!isReset)
            {
                --_index;
                _index = Math.Min(_currentSuggestions.Count - 1, Math.Max(0, _index));
            }

            return _currentSuggestions[_index];
        }

        #endregion

        public void Reset()
        {
            _currentGuess = null;
        }


        private bool LoadSuggestionsForGuess(string guess)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(guess, _currentGuess))
            {
                return false;
            }

            ReloadSuggestionsForGuess(guess);

            return true;
        }

        private void ReloadSuggestionsForGuess(string guess)
        {
            if (null == _providers || !_providers.Any())
            {
                return;
            }

            _currentGuess = guess;
            _index = 0;
            _currentSuggestions = new List<string>();
            _providers.ForEach(
                p => _currentSuggestions.AddRange(p.GetSuggestions(guess))
                );
            _currentSuggestions.Sort(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}