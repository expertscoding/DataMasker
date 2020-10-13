using System.Linq;
using Bogus;

namespace DataMasker.Extensions
{
    public static class BogusExtensions
    {
        public static string SpanishNif(this Faker faker)
        {
            const string controlDigits = "TRWAGMYFPDXBNJZSQVHLCKE";
            string[] firstCharacterNif = { "K", "L", "M", "X", "Y", "Z" };
            
            var randomizer = faker.Random;

            var isSpecial = randomizer.Bool(0.15F);

            var numDigits = isSpecial ? 7 : 8;
            var digits = string.Join("", randomizer.Digits(numDigits));
            var first = isSpecial ? randomizer.ArrayElement(firstCharacterNif) : string.Empty;
            var number = int.Parse(digits);
            var controlDigit = controlDigits[number % 23];

            var result = $"{first}{digits}{controlDigit}";

            return result;
        }
        
        public static string SpanishCif(this Faker faker)
        {
            const string firstCharacterCif = "ABCDEFGHJPQRSUV";
            var randomizer = faker.Random;

            var number = string.Join("", randomizer.Digits(7));

            var firstDigit = randomizer.ArrayElement(firstCharacterCif.ToCharArray());
            var controlDigit = GetCifDigitControl(firstDigit, number);

            return firstDigit + number + controlDigit;
        }

        private static string GetCifDigitControl(char firstDigit, string number)
        {
            string[] codeChars = { "J", "A", "B", "C", "D", "E", "F", "G", "H", "I" };

            var odd = 0;
            var even = 0;
            var total = 0;

            for (var i = 0; i < number.Length; i++)
            {
                int.TryParse(number[i].ToString(), out var aux);

                if ((i + 1) % 2 == 0)
                {
                    odd += aux;
                }
                else
                {
                    aux *= 2;

                    even += SumDigits(aux);
                }
            }
            total += odd + even;

            var units = total % 10;

            if (units != 0)
                units = 10 - units;
            switch (firstDigit)
            {
                case 'A':
                case 'B':
                case 'E':
                case 'H':
                    return units.ToString();

                case 'K':
                case 'P':
                case 'Q':
                case 'S':
                    return codeChars[units];

                default:
                    return units.ToString();
            }
        }

        private static int SumDigits(int digits)
        {
            return digits.ToString()
                .Select(d => int.Parse(d.ToString()))
                .Sum();
        }

        
        public static string SpanishNaf(this Faker faker)
        {
            var provinceCode = faker.Address.ZipCode().Substring(0, 2);
            var code = faker.Random.Replace("########-##");

            return $"{provinceCode}/{code}";
        }
    }
}