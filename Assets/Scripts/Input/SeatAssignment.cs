namespace MarioPie.Input
{
    public readonly struct ControlSeat
    {
        public ControlSeat(string scheme, int gamepadIndex)
        {
            Scheme = scheme;
            GamepadIndex = gamepadIndex;
        }

        public string Scheme { get; }
        public int GamepadIndex { get; }
        public bool UsesGamepad => GamepadIndex >= 0;
    }

    public static class SeatAssignment
    {
        public const string KeyboardWasd = "KeyboardWASD";
        public const string KeyboardArrows = "KeyboardArrows";
        public const string Gamepad = "Gamepad";
        public const int SeatCount = 2;

        public static ControlSeat[] Assign(int connectedGamepads)
        {
            if (connectedGamepads < 0)
                connectedGamepads = 0;
            if (connectedGamepads > SeatCount)
                connectedGamepads = SeatCount;

            var seats = new ControlSeat[SeatCount];
            for (var i = 0; i < connectedGamepads; i++)
                seats[i] = new ControlSeat(Gamepad, i);

            if (connectedGamepads == 0)
            {
                seats[0] = new ControlSeat(KeyboardWasd, -1);
                seats[1] = new ControlSeat(KeyboardArrows, -1);
            }
            else if (connectedGamepads == 1)
            {
                seats[1] = new ControlSeat(KeyboardWasd, -1);
            }

            return seats;
        }
    }
}
