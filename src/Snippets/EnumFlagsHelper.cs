using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snippets
{
    public static class EnumFlagsHelper
    {
        public static IEnumerable<string> GetFlagsValues(Enum input)
        {
            ValidateEnum(input);

            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))

                    yield return value.ToString();
        }

        public static IEnumerable<TEnum> GetFlags<TEnum>(Enum input)
        {
            ValidateEnum(input);

            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return (TEnum)Enum.Parse(typeof(TEnum), value.ToString());
        }

        private static void ValidateEnum(Enum input)
        {
            if (input.GetType().CustomAttributes.All(x => x.AttributeType != typeof(FlagsAttribute)))
                throw new ArgumentException(string.Format("Enum {0} hasn't FlagsAttribute", input.GetType().Name));
        }
    }
}
