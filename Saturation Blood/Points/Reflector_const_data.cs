using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    
    public class Reflector_Const_Data
    {
        private long[,] row_1;
        private long[,] osob_point_diff;
        private long[,] osob_point_const;
        private long[] line_reflection;

        private long[] row_chosen_reflected;

        private int length_row_1;
        private int length_osob_point_diff;
        private int number_canal_const;

        public Reflector_Const_Data(long[,] osob_point_diff, int number_canal_const)
        {
            this.osob_point_diff = osob_point_diff;
            this.number_canal_const = number_canal_const;           
            this.length_osob_point_diff = osob_point_diff.Length / 15;

            osob_point_const = new long[15, length_osob_point_diff];
        }

        public Reflector_Const_Data(Initial_Data initial_Data, long[,] osob_point_diff, int number_canal_const)
        {
            this.row_1 = initial_Data.Get_row1();
            this.osob_point_diff = osob_point_diff;
            this.number_canal_const = number_canal_const;
            this.length_row_1 = initial_Data.Get_b();
            this.length_osob_point_diff = osob_point_diff.Length / 15;

            osob_point_const = new long[15, length_osob_point_diff];

            line_reflection = new long[length_row_1];
            row_chosen_reflected = new long[length_row_1];
        }

        public long[,] Get_Special_Point_Const() {

            return osob_point_const;
        }

        public void Set_row_1(Initial_Data initial_Data) {
            this.row_1 = initial_Data.Get_row1();          
            this.length_row_1 = initial_Data.Get_b();

        }

        /////////////////////////
        /////////////////////////
        // новое
        //ЭКГ мах -     0
        //ЭКГ мах -х -  1
        // В1, В5 -     2
        // В1x, В5x -   3
        // В2 -         4
        // В2x -        5
        // В3 -         6
        // В3x -        7
        // В4 -         8  
        // В4x -        9
        //osob_10  -    Изначальная высота
        // schet[4, w - 1] = schet[4, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[5, w - 1], schet[2, w - 1], schet[2, w]);// максимум В2

        ////////////////////////


        public void Set_Const_Special_Point_from_Diff(Initial_Data init_data)
        {
            osob_point_const = Function_additional.Set_Const_Special_Point_from_Diff(init_data, osob_point_diff, number_canal_const);
                                 
        }       

        public void Calculate_line_reflection() {

            int s = 1;
            for (int i = 0; i < line_reflection.Length; i++)
            {               

                if (row_1[i, 0] < osob_point_const[3, 0])
                {
                    line_reflection[i] = row_1[i, number_canal_const];
                }
                

                if (row_1[i, 0]>=osob_point_const[3, s-1] && row_1[i, 0] <= osob_point_const[3, s])
                {
                    line_reflection[i] = ((osob_point_const[2, s-1] + (osob_point_const[4, s-1]+osob_point_const[10, s-1])) / 2) 
                        + Shift_BX(osob_point_const[3, s-1], osob_point_const[3, s], 
                        row_1[i, 0], osob_point_const[2, s-1], osob_point_const[2, s]);
                 
                }

                if (row_1[i, 0] > osob_point_const[3, length_osob_point_diff - 2])
                {
                    line_reflection[i] = row_1[i, number_canal_const];
                }

                if (row_1[i, 0] == osob_point_const[3, s])
                {
                    s++;
                    if (s>= length_osob_point_diff)
                    {
                        s--;
                    }
                }
            }  
        }

        public void Calculate_reflected_row() {
                       
            for (int i = 0; i < length_row_1; i++)
            {
                row_1[i, number_canal_const] = line_reflection[i] + (line_reflection[i] - row_1[i, number_canal_const]);
            }
        }


        public long[] Get_line_reflection() 
        {
            return line_reflection;
        }

        public long[,] Get_row1() {

            return row_1;
        }
        public long[] Get_row_chosen_reflected() {

            return row_chosen_reflected;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1">Координата х для B1 первого цикла</param>
        /// <param name="x2">Координата х для В1 второго цикла</param>
        /// <param name="x3_b">Точка, сдвиг которой надо найти</param>
        /// <param name="y1">Координата y для B1 первого цикла</param>
        /// <param name="y2">Координата y для B1 первого цикла</param>
        /// <returns></returns>
        private long Shift_BX(long x1, long x2, long x3_b, long y1, long y2)
        {
            double shift_y = 0;

            if (x2 == x1)
            {
                shift_y = 0;
            }
            else
            {
                shift_y = (System.Convert.ToDouble(x3_b - x1) / System.Convert.ToDouble(x2 - x1)) * System.Convert.ToDouble(y2 - y1);
            }

            long shift_2 = System.Convert.ToInt64(shift_y);

            return shift_2;
        }


    }
}
