using System;
using System.Collections.Generic;

namespace ControlDigit {
    public class LongAripmetic {
        private List<int> numberSequense = new List<int>();
        public int Length = 0;
        public LongAripmetic(long number) {
            this.numberSequense = toList(number);
        }
        public List<int> toList(long number )  {
            string stringNumber = number.ToString();
            List<int> numbersSequenceTemp = new List<int>();
            for (int i = 0; i < stringNumber.Length; i++) {
                numbersSequenceTemp.Add(Int32.Parse(stringNumber[i].ToString()));
            }

            Length = numbersSequenceTemp.Count;
            return numbersSequenceTemp;
        }

        public sumWithWeights(int weitgh) {
            
        }

        public int Count() {
            return numberSequense.Count;
        }

        public int At(int index) {
            return numberSequense[index];
        }
    }

}
//this.numberSequense = numbersSequence;