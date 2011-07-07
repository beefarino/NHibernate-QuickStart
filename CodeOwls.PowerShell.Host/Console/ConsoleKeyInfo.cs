namespace CodeOwls.PowerShell.Host.Console
{
    public struct ConsoleKeyInfo
    {
        public char Character { get; set; }
        public ConsoleControlKeyStates ControlKeyState { get; set; }
        public bool KeyDown { get; set; }
        public int VirtualKeyCode { get; set; }
    }
}