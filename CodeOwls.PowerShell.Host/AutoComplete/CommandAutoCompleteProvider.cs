using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Host.Executors;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host.AutoComplete
{
    internal class CommandAutoCompleteProvider : IAutoCompleteProvider
    {
        private readonly string _commandTemplate;
        private readonly Executor _executor;

        protected CommandAutoCompleteProvider(string commandTemplate, Executor executor)
        {
            _commandTemplate = commandTemplate;
            _executor = executor;
        }

        #region IAutoCompleteProvider Members

        public virtual IEnumerable<string> GetSuggestions(string guess)
        {
            if( String.IsNullOrEmpty(guess))
            {
                return new string[0];
            }

            var info = FormatGuessInfo(guess);
            Exception error;
            var items = _executor.ExecuteCommand(GetCommand(info), out error, ExecutionOptions.None);
            if (null == items)
            {
                return new string[] {};
            }

            return items.ToList()
                .ConvertAll(d => d.ToStringValue())
                .ConvertAll(d => d.Contains(" ") ? String.Format("'{0}'", d) : d)
                .ConvertAll(d => String.Format(info.CommandFormatString, d));
        }

        #endregion

        protected virtual string GetCommand(FormattedGuessInformation info)
        {
            return String.Format(_commandTemplate, info.Guess);
        }

        protected string[] BreakIntoWords(string guess)
        {
            guess = guess.Trim();
            Regex re = new Regex(@"('[^']+(?:'|$))|(""[^""]+(?:""|$))|([^\s'""]+)");
            var matches = re.Matches(guess);

            return (from Match match in matches
                    let value =
                        String.IsNullOrEmpty(match.Groups[1].Value) ? match.Groups[0].Value : match.Groups[1].Value
                    select value).ToArray();

            /*return Regex.Split(guess, @"\s+");
            /*char[] quotes = new char[] {'\'', '"'};
            
            if( (-1) != guess.IndexOfAny( quotes ))
            {
                List<string> quotedParts = new List<string>();
                Regex reQuotes = new Regex( @"['""]" );
                parts.ToList().ForEach(
                    part =>
                        {
                            
                        }
                );
            }
            return parts;*/
        }

        protected virtual FormattedGuessInformation FormatGuessInfo(string guess)
        {
            var guessTemplate = BreakIntoWords(guess).LastOrDefault();

            var commandFormat = guess.Replace(guessTemplate, "{0}");
            guessTemplate = guessTemplate.TrimStart('\'', '"').TrimEnd('\'', '"');
            if (! guessTemplate.Contains("*"))
            {
                guessTemplate += "*";
            }

            return new FormattedGuessInformation(guessTemplate, commandFormat);
        }

        #region Nested type: FormattedGuessInformation

        public class FormattedGuessInformation
        {
            public FormattedGuessInformation(string guess, string commandFormat)
            {
                Guess = guess;
                CommandFormatString = commandFormat;
            }

            public string Guess { get; private set; }
            public string CommandFormatString { get; private set; }
        }

        #endregion
    }
}