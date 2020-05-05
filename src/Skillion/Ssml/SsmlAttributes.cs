namespace Skillion.Ssml
{
    public static class SsmlAttributes
    {
        public static class Level
        {
            public const string Strong = "strong";
            public const string Moderate = "moderate";
            public const string Reduced = "reduced";

            internal static bool TryValidate(string level, out string validLevel)
            {
                var levelLower = level.ToLower();
                if (levelLower == Strong ||
                    levelLower == Moderate ||
                    levelLower == Reduced)
                {
                    validLevel = levelLower;
                    return true;
                }

                validLevel = Moderate;
                return false;
            }
        }
        
        public static class Strength
        {
            public const string None = "none";
            public const string XWeak = "x-weak";
            public const string Weak = "weak";
            public const string Medium = "medium";
            public const string Strong = "strong";
            public const string XStrong = "x-strong";

            internal static bool TryValidate(string strength, out string validStrength)
            {
                var strengthLower = strength.ToLower();
                if (strengthLower == None ||
                    strengthLower == XWeak ||
                    strengthLower == Weak ||
                    strengthLower == Medium ||
                    strengthLower == Strong ||
                    strengthLower == XStrong)
                {
                    validStrength = strengthLower;
                    return true;
                }

                validStrength = Medium;
                return false;
            }
        }

        public static class Language
        {
            
        }
    }
}