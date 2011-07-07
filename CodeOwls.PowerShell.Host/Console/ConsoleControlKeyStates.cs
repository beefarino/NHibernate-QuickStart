using System;

namespace CodeOwls.PowerShell.Host.Console
{
    [Flags]
    public enum ConsoleControlKeyStates
    {
        None = 0x000, 
        
        RightAltPressed = 0x001,
        LeftAltPressed      = 0x002,
        RightCtrlPressed    = 0x004,
        LeftCtrlPressed     = 0x008,
        ShiftPressed        = 0x010,
        NumLockOn           = 0x020,
        ScrollLockOn        = 0x040,
        CapsLockOn          = 0x080,
        EnhancedKey         = 0x100,
        
    }
}