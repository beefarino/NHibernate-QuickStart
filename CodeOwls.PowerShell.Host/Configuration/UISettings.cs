using System;

namespace CodeOwls.PowerShell.Host.Configuration
{
    public class UISettings
    {
        public ConsoleColor BackgroundColor;
        public ConsoleColor DebugBackgroundColor;
        public ConsoleColor DebugForegroundColor;
        public ConsoleColor ErrorBackgroundColor;
        public ConsoleColor ErrorForegroundColor;

        public string FontName;
        public int FontSize;
        public ConsoleColor ForegroundColor;

        public bool PromptForCredentialsInConsole;
        public ConsoleColor VerboseBackgroundColor;
        public ConsoleColor VerboseForegroundColor;
        public ConsoleColor WarningBackgroundColor;
        public ConsoleColor WarningForegroundColor;

        public UISettings()
        {
            BackgroundColor = ConsoleColor.Black;

            ForegroundColor = ConsoleColor.White;
            ErrorForegroundColor = ConsoleColor.Red;
            WarningForegroundColor = ConsoleColor.Yellow;
            VerboseForegroundColor = ConsoleColor.Green;
            DebugForegroundColor = ConsoleColor.Cyan;

            FontName = "Courier New";
            FontSize = 10;

            PromptForCredentialsInConsole = true;
        }
    }
}