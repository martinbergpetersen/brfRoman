﻿using System;
using System.Collections;
using RomeNumberConverter.App.Interfaces;
using static RomeNumberConverter.App.Enums.RomanDigits;

namespace RomeNumberConverter.App
{
    public class RomanDecimalConveter : IConverter
    {
        private string _input;
        public RomanDecimalConveter(string input)
        {
            if (!TypeOf.IsDecimal(input) && !TypeOf.IsRoman(input))
                throw new ArgumentException("Argument is not valid");

            _input = input.ToUpper().Trim();
        }

        public string GetResult()
        {
            if (TypeOf.IsDecimal(_input))
                return ToRoman(decimal.Parse(_input));

            if (TypeOf.IsRoman(_input))
                return ToDecimal(_input).ToString();

            else
                throw new ArgumentException("Argument is not valid");

        }

        private decimal ToDecimal(string roman)
        {
            //There is very little information that suggests that the system originally had a notation for zero.
            //However, the letter N has been used to represent zero in a text from around 725AD. This will be used in the algorithm.
            // Rule 7
            if (roman == "N") return 0;

            // Rule 4
            //The numerals that represent numbers beginning with a '5' (V, L and D) may only appear once in each Roman numeral.
            //This rule permits XVI but not VIV.
            if (roman.Split('V').Length > 2 ||
                roman.Split('L').Length > 2 ||
                roman.Split('D').Length > 2)
                //The numerals that represent numbers beginning with a '5'(V, L and D) may only appear once in each Roman numeral.
                //This rule permits XVI but not VIV.
                //return "input is not valid - according to rule 4";
                throw new ArgumentException("Rule 4");

            // Rule 1
            //A single letter may be repeated up to three times consecutively with each occurrence of the value being additive.
            //This means that I is one, II means two and III is three. However, IIII is incorrect for four.
            int count = 1;
            char last = 'Z';
            foreach (char numeral in roman)
            {
                // Valid character?
                if ("IVXLCDM".IndexOf(numeral) == -1)
                    throw new ArgumentException("Invalid numeral");

                // Duplicate?
                if (numeral == last)
                {
                    count++;
                    if (count == 4)
                        //A single letter may be repeated up to three times consecutively with each occurrence of the value being additive.
                        //This means that I is one, II means two and III is three. However, IIII is incorrect for four.
                        throw new ArgumentException("input is not valid - Rule 1");
                }
                else
                {
                    count = 1;
                    last = numeral;
                }
            }

            // Create an ArrayList containing the values
            int ptr = 0;
            ArrayList values = new ArrayList();
            int maxDigit = 1000;
            while (ptr < roman.Length)
            {
                // Base value of digit
                char numeral = roman[ptr];
                int digit = (int)Enum.Parse(typeof(RomanDigit), numeral.ToString());

                // Rule 3
                if (digit > maxDigit)
                    //A small-value numeral may be placed to the left of a larger value.
                    //Where this occurs, for example IX, the smaller numeral is subtracted from the larger.
                    //This means that IX is nine and IV is four. 
                    //The subtracted digit must be at least one tenth of the value of the larger numeral and must be either I, X or C.
                    //Accordingly, ninety-nine is not IC but rather XCIX. The XC part represents ninety and the IX adds the nine.
                    //In addition, once a value has been subtracted from another, no further numeral or pair may match or exceed the subtracted value.
                    //This disallows values such as MCMD or CMC.
                    throw new ArgumentException("Input is not valid  - Rule 3");

                // Next digit
                int nextDigit = 0;
                if (ptr < roman.Length - 1)
                {
                    char nextNumeral = roman[ptr + 1];
                    nextDigit = (int)Enum.Parse(typeof(RomanDigit), nextNumeral.ToString());

                    if (nextDigit > digit)
                    {
                        if ("IXC".IndexOf(numeral) == -1 ||
                            nextDigit > (digit * 10) ||
                            roman.Split(numeral).Length > 3)
                            //A small-value numeral may be placed to the left of a larger value.
                            //Where this occurs, for example IX, the smaller numeral is subtracted from the larger.
                            //This means that IX is nine and IV is four. 
                            //The subtracted digit must be at least one tenth of the value of the larger numeral and must be either I, X or C.
                            //Accordingly, ninety-nine is not IC but rather XCIX. The XC part represents ninety and the IX adds the nine.
                            //In addition, once a value has been subtracted from another, no further numeral or pair may match or exceed the subtracted value.
                            //This disallows values such as MCMD or CMC.
                            throw new ArgumentException("Input is not valid - Rule 3");

                        maxDigit = digit - 1;
                        digit = nextDigit - digit;
                        ptr++;
                    }
                }

                values.Add(digit);

                // Next digit
                ptr++;
            }

            // Rule 5
            for (int i = 0; i < values.Count - 1; i++)
                if ((int)values[i] < (int)values[i + 1])
                    //The fourth rule compares the size of value of each the numeral as read from left to right.
                    //The value must never increase from one letter to the next.
                    //Where there is a subtractive numeral, this rule applies to the combined value of the two numerals involved in the subtraction
                    //when compared to the previous letter. This means that XIX is acceptable but XIM and IIV are not.
                    throw new ArgumentException("Input is not valid  - Rule 5");

            // Rule 2
            var total = 0;
            foreach (int digit in values)
                total += digit;

            return total;
        }

        private string ToRoman(decimal number)
        {
            if ((number < 0) || (number > 3999)) return "Argument is not valid - Value must be between 1 and 3999";
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            return "Argument is not valid - Value must be between 1 and 3999";
        }
    }
}
