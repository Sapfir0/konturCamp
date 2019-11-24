using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlDigit
{
    public static class SnilsExtensions
    {
        public static int CalculateSnils(this long number) {
            LongAripmetic numbersSequence = new LongAripmetic(number);
            
            int positionNumber = numbersSequence.Length;
            int sum = 0;
            for (int i = 0; i < numbersSequence.Length; i++) {
                sum += numbersSequence.At(i) * positionNumber;
                positionNumber--;
            }

            if (sum < 100) {
                return sum;
            }
            else if (sum == 100 || sum == 101) {
                return 0;
            }

            while (sum > 101) {
                long newNumber = sum % 101;
                sum = CalculateSnils(newNumber);
            }

            return -1;
        }
    }
}

