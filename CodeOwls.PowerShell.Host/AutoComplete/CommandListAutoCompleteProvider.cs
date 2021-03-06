using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Host.Executors;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class CommandListAutoCompleteProvider : CommandAutoCompleteProvider
    {
        private const string Command = @"get-command '{0}'";

        public CommandListAutoCompleteProvider(Executor executor) : base(Command, executor)
        {
        }

        public override IEnumerable<string> GetSuggestions(string guess)
        {
            // apply only when the guess is the first word in the line
            var words = BreakIntoWords(guess);
            if (1 != words.Count())
            {
                return new string[] {};
            }

            return base.GetSuggestions(guess);
        }
    }
}