using Fusion;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte MOUSEBUTTON1 = 2;
    public const byte BUTTONQ = 3;
    public const byte BUTTOND = 4;
    public const byte BUTTONF = 5;

    public NetworkButtons buttons;
}