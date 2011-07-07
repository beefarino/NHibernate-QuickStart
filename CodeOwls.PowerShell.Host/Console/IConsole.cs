using System;
using System.Management.Automation.Host;
using System.Threading;
using CodeOwls.PowerShell.Host.AutoComplete;
using CodeOwls.PowerShell.Host.Configuration;
using CodeOwls.PowerShell.Host.History;

namespace CodeOwls.PowerShell.Host.Console
{
    public interface IConsole
    {
        IAutoCompleteWalker AutoCompleteWalker { get; set; }
        IHistoryStackWalker HistoryStackWalker { get; set; }
        WaitHandle CommandEnteredEvent { get; }
        int EndOfLinePosition { get; }
        ConsoleSize ConsoleSizeInCharacters { get; }
        bool IsInputEntryModeEnabled { get; set; }
        bool KeyAvailable { get; }
        ConsoleColor ConsoleForeColor { get; set; }
        ConsoleColor ConsoleBackColor { get; set; }
        void Apply(UISettings settings);
        void ClearBuffer();
        ConsoleKeyInfo ReadNextKey();
        void FlushInputBuffer();
        void WritePrompt(string str);
        void Write(string str);
        void Write(string str, ConsoleColor fore, ConsoleColor back);
        void WriteLine(string value);
        void WriteErrorLine(string msg);
        void WriteWarningLine(string msg);
        void WriteDebugLine(string msg);
        void WriteVerboseLine(string msg);
        string ReadLine();
        IntPtr GetSafeWindowHandle();
    }
}