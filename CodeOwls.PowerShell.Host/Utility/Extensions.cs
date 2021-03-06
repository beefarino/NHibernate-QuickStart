using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Text;
using CodeOwls.PowerShell.Host.Console;

namespace CodeOwls.PowerShell.Host.Utility
{
    public static class Extensions
    {
        public static ControlKeyStates ToControlKeyState( this Console.ConsoleControlKeyStates state )
        {
            var map = new Dictionary<Console.ConsoleControlKeyStates, ControlKeyStates>
                          {
                              {ConsoleControlKeyStates.CapsLockOn, ControlKeyStates.CapsLockOn},
                              {ConsoleControlKeyStates.EnhancedKey, ControlKeyStates.EnhancedKey},
                              {ConsoleControlKeyStates.LeftAltPressed, ControlKeyStates.LeftAltPressed},
                              {ConsoleControlKeyStates.LeftCtrlPressed, ControlKeyStates.LeftCtrlPressed},
                              {ConsoleControlKeyStates.NumLockOn, ControlKeyStates.NumLockOn},
                              {ConsoleControlKeyStates.RightAltPressed, ControlKeyStates.RightAltPressed},
                              {ConsoleControlKeyStates.RightCtrlPressed, ControlKeyStates.RightCtrlPressed},
                              {ConsoleControlKeyStates.ScrollLockOn, ControlKeyStates.ScrollLockOn},
                              {ConsoleControlKeyStates.ShiftPressed, ControlKeyStates.ShiftPressed},
                          };

            ControlKeyStates controlKeyState = 0;
            map.ToList().ForEach(pair =>
                                     {
                                         if (0 != (pair.Key & state))
                                         {
                                             controlKeyState |= pair.Value;
                                         }
                                     });
            return controlKeyState;
        }

        public static KeyInfo ToKeyInfo(this Console.ConsoleKeyInfo cki)
        {
            return new KeyInfo( cki.VirtualKeyCode, cki.Character, cki.ControlKeyState.ToControlKeyState(), cki.KeyDown );
        }
        
        public static PSObject ToPSObject(this object o)
        {
            return PSObject.AsPSObject(o);
        }

        public static string ToDotSource(this FileInfo fileInfo)
        {
            return String.Format(". \"{0}\"", fileInfo.FullName).EscapeTicks();
        }

        public static string EscapeTicks(this string str)
        {
            StringBuilder builder = new StringBuilder(str.Length*2);
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                switch (ch)
                {
                    case '`':
                    case '\'':
                        builder.Append('`');
                        break;
                }
                builder.Append(ch);
            }
            string format = builder.ToString();
            return format;
        }
    }
}