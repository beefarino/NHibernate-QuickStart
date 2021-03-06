using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Host.Executors;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class PowerShellTabExansionAutoCompleteProvider : IAutoCompleteProvider
    {
        private const string TabExpansionFunctionName = "TabExpansion";
        private const string LineArgumentName = "line";
        private const string LastWordArgumentName = "lastWord";
        private readonly Executor _executor;

        public PowerShellTabExansionAutoCompleteProvider(Executor executor)
        {
            _executor = executor;
        }

        #region IAutoCompleteProvider Members

        public IEnumerable<string> GetSuggestions(string guess)
        {
            try
            {
                Dictionary<string, object> arguments = SplitGuessIntoArguments(guess);
                Exception error;
                var results = _executor.ExecuteCommand(TabExpansionFunctionName, arguments, out error,
                                                       ExecutionOptions.None);
                if (null == results)
                {
                    return new string[] {};
                }

                return results.ToList().ConvertAll(pso => pso.ToStringValue());
            }
            catch
            {
            }
            return null;
        }

        #endregion

        private Dictionary<string, object> SplitGuessIntoArguments(string guess)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add(LineArgumentName, guess);
            //todo: add more logic to split, handle quotations
            var lastWord = Regex.Split(guess, @"\s+").LastOrDefault();
            args.Add(LastWordArgumentName, lastWord);
            return args;
        }
    }
}