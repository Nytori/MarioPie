namespace MarioPie.Match
{
    public static class MatchResult
    {
        public const int Tie = -1;

        public static int Winner(int leftScore, int rightScore)
        {
            if (leftScore > rightScore)
                return 0;
            if (rightScore > leftScore)
                return 1;
            return Tie;
        }
    }
}
