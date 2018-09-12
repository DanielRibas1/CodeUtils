using System;

namespace Snippets
{
    public static class RandomizeHelper
    {
        public static bool RandomizeBooleanFromPercentage(uint percentage)
        {
            if (percentage == 100) return true;

            double probability = (1.0 - percentage) / 100.0;

            return new Random().NextDouble() >= probability;
        }
    }
}
