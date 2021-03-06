using System.Linq;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Host.Executors;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class DriveListAutoCompleteProvider : CommandAutoCompleteProvider
    {
        private const string Command =
            @"get-psdrive |  ?{{ $_.Name.ToLowerInvariant().StartsWith( '{0}' ) }} | %{{ $_.name + ':' }}";

        public DriveListAutoCompleteProvider(Executor executor)
            : base(Command, executor)
        {
        }

        protected override FormattedGuessInformation FormatGuessInfo(string guess)
        {
            var guessTemplate = Regex.Split(guess.Trim(), @"\s+").LastOrDefault();

            var commandFormat = guess.Replace(guessTemplate, "{0}");

            return new FormattedGuessInformation(guessTemplate, commandFormat);
        }
    }
}