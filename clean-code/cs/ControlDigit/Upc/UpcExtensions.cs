using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ControlDigit
{
    public static class UpcExtensions
    {
        public static int CalculateUpc(this long number) {

            /*UPC: 03600029145.
                Находим значение контрольного числа:

            Цифры номера 0 3 6 0 0 0 2 9 1 4 5
            Множитель    3 1 3 1 3 1 3 1 3 1 3

            sum = 0×3 + 3×1 + 6×3 + 0×1 + 0×3 + 0×1 + 2×3 + 9×1 + 1×3 + 4×1 + 5×3 = 58
            M = 58 % 10 = 8
            M ≠ 0 => res = 10 – 8 = 2
            */
            LongAripmetic numbersSequence = new LongAripmetic(number);

            int sum = 0;
            for (int i = 0; i < numbersSequence.Length; i++) {
                if (i%2 == 0)
                    sum += numbersSequence.At(i) * 3;
                else {
                    sum += numbersSequence.At(i) * 1;
                }
            }

            var M = sum % numbersSequence.Length;
            if (M != 0) {
                return  numbersSequence.Length - M;
            }

            return 0;
        }
    }
}
