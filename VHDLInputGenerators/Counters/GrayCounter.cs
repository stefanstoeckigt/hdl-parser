//
//  Copyright (C) 2010-2014  Denis Gavrish
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Text;
using VHDLRuntime;

/*
         Dec  Gray   Binary
         0    000    000
         1    001    001
         2    011    010
         3    010    011
         4    110    100
         5    111    101
         6    101    110
         7    100    111
 */

namespace VHDLInputGenerators.Counters
{
    public class GrayCounter : Counter
    {
        public override bool[] Next(bool[] value)
        {
            bool[] res = new bool[value.Length];
            value.CopyTo(res, 0);

            int index = 0;
            bool count_up = true;
            for (int i = 0; i < value.Length; i++)
            {
                if (((res[i] == false) && count_up) || ((res[i] == true) && !count_up))
                    index = i;
                if (res[i] == true)
                    count_up = !count_up;
            }
            res[index] = !res[index];
            return res;
        }


        public override bool[] Next(bool[] value, uint step_count)
        {
            if (step_count == 1)
            {
                return this.Next(value);
            }
            else
            {
                bool[] res = new bool[value.Length];
                value.CopyTo(res, 0);

                
                // convert to decimal
                bool[] r1 = ConvertGrayToDecimal(res);

                BinaryCounter counter = new BinaryCounter();

                bool[] r2 = counter.Next(r1, step_count);
                //convert to gray
                return ConvertDecimalToGray(r2);
            }
        }   


        public override bool[] Prev(bool[] value)
        {
            bool[] res = new bool[value.Length];
            value.CopyTo(res, 0);
            
            int index = 0;
            bool count_up = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (((res[i] == false) && count_up) || ((res[i] == true) && !count_up))
                    index = i;
                if (res[i] == true)
                    count_up = !count_up;
            }
            res[index] = !res[index];
            
            return res;
        }


        public override bool[] Prev(bool[] value, uint step_count)
        {
            if (step_count == 1)
            {
                return this.Prev(value);
            }
            else
            {
                bool[] res = new bool[value.Length];
                value.CopyTo(res, 0);

                
                // convert to decimal
                bool[] r1 = ConvertGrayToDecimal(res);

                BinaryCounter counter = new BinaryCounter();

                bool[] r2 = counter.Prev(r1, step_count);
                //convert to gray
                return ConvertDecimalToGray(r2);
            }
        }

        public override bool CheckForCorrect(bool[] value)
        {
            return true;
        }

        /// <summary>
        /// Перевод из системы счисления Грея в десятичную систему
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ConvertGrayToDecimal(bool[] value)
        {
            bool[] res = new bool[value.Length];
            value.CopyTo(res, 0);

            bool c = value[0];
            // convert to decimal
            for (int i = 1; i < value.Length; i++)
            {
                if (c == true)
                    res[i] = !res[i];
                if (value[i] == true)
                    c = !c;
            }

            return res;
        }

        /// <summary>
        /// Перевод изи десятичной системы счисления в код Грея
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool[] ConvertDecimalToGray(bool[] value)
        {
            bool[] res = new bool[value.Length];
            value.CopyTo(res, 0);

            for (int i = 1; i < value.Length; i++)
            {
                if (value[i - 1] == true)
                    res[i] = !res[i];
            }

            return res;
        }
        public override string ToString()
        {
            return "Counter : GrayCounter";
        }
        public override StringBuilder StringVhdlRealization(KeyValuePair<string, TimeInterval> param)
        {
            StringBuilder res = new StringBuilder();
            res.Append("\n").Append(param.Key).Append(" <= GrayCounter(").Append(param.Key).Append(", ").Append(stepCount).Append(") after ").Append(timeStep.TimeNumber).Append(" ").Append(timeStep.Unit).Append(" when now < END_TIME ").Append(" ;").Append("\n");   
            //s1<=GrayCouner(s1, delta) after 10 ns;
            return res;
        }

    }
}
