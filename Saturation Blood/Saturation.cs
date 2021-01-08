using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    class Saturation
    {
        private const double E_Hb_650_1 = 3750.12;
        private const double E_Hb02_650_2 = 761.84;
        private const double E_Hb_900_1 = 368;
        private const double E_Hb02_900_2 = 1198;

        private const double shift_zero_scale = 512;

        private long[,] osob_red_const;
        private long[,] osob_red_diff;
        private long[,] osob_IK_const;
        private long[,] osob_IK_diff;

        private int length_osob_red_const;
        private int length_osob_red_diff;
        private int length_osob_IK_const;
        private int length_osob_IK_diff;
              
        private double Atten; 
        private double K_pow_const_RED;
        private double K_pow_const_IK;

        private double[] K_Pow_diff_RED;
        private double[] K_Pow_diff_IK;


        double[,] Intensity_RED_const;
        double[,] Intensity_RED_diff;
        double[,] Intensity_IK_const;
        double[,] Intensity_IK_diff;


        double[] Saturation1;
        double[] Saturation2; 
        double[] Saturation_t;
        /// <summary>
        /// Конструктор для расчета сатурации крови
        /// </summary>
        /// <param name="osob_red_const">массив особых точек красного постоянного сигнала</param>
        /// <param name="osob_red_diff">массив особых точек красного переменного сигнала</param>
        /// <param name="osob_IK_const">массив особых точек ИК постоянного сигнала</param>
        /// <param name="osob_IK_diff">массив особых точек ИК переменного сигнала</param>
        public Saturation(long[,] osob_red_const, long[,] osob_red_diff, long[,] osob_IK_const, long[,] osob_IK_diff)
        {
            this.osob_red_const = osob_red_const;
            this.osob_red_diff = osob_red_diff;
            this.osob_IK_const = osob_IK_const;
            this.osob_IK_diff = osob_IK_diff;

            length_osob_red_const = osob_red_const.Length / 15;
            length_osob_red_diff = osob_red_diff.Length / 15;
            length_osob_IK_const = osob_IK_const.Length / 15;
            length_osob_IK_diff = osob_IK_diff.Length / 15;

            Saturation1 = new double[length_osob_red_const];
            Saturation2 = new double[length_osob_red_const];
            Saturation_t = new double[length_osob_red_const];

            K_Pow_diff_RED = new double[length_osob_red_const];
            K_Pow_diff_IK = new double[length_osob_IK_const];


            Intensity_RED_const = new double[length_osob_red_const, 3];
            Intensity_RED_diff = new double[length_osob_red_diff, 3];
            Intensity_IK_const = new double[length_osob_IK_const, 3];
            Intensity_IK_diff = new double[length_osob_IK_diff, 3];

        }
        /// <summary>
        /// Установить данные красного канала
        /// </summary>
        private void Set_Red_Const() {

            length_osob_red_const = osob_red_const.Length / 15;
            length_osob_red_diff = osob_red_diff.Length / 15;
           
            Saturation1 = new double[length_osob_red_const];
            Saturation2 = new double[length_osob_red_const];
            Saturation_t = new double[length_osob_red_const];

            K_Pow_diff_RED = new double[length_osob_red_const];          

            Intensity_RED_const = new double[length_osob_red_const, 3];
            Intensity_RED_diff = new double[length_osob_red_diff, 3];           

        }
        /// <summary>
        /// Установить данные ИК канала
        /// </summary>
        private void Set_IK_Const() {
          
            length_osob_IK_const = osob_IK_const.Length / 15;
            length_osob_IK_diff = osob_IK_diff.Length / 15;
            
            K_Pow_diff_IK = new double[length_osob_IK_const];

            Intensity_IK_const = new double[length_osob_IK_const, 3];
            Intensity_IK_diff = new double[length_osob_IK_diff, 3];

        }

        public void Set_Special_Red_Const(long[,] red_const) {

            osob_red_const = red_const;
        }

        public void Set_Special_IK_Const(long[,] IK_const)
        {

            osob_IK_const = IK_const;
        }
        /*
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
        */

     
      /// <summary>
      /// Установить Коэффициенты усиления постоянного сигнала
      /// </summary>
      /// <param name="atten"></param>
      /// <param name="k_pow_const_RED"></param>
      /// <param name="k_pow_const_IK"></param>
        public void Set_K_Pow_Const( double atten, double k_pow_const_RED, double k_pow_const_IK) 
        {
             Atten = atten;
             K_pow_const_RED = k_pow_const_RED/Atten;
             K_pow_const_IK= k_pow_const_IK/Atten;
        }

        /// <summary>
        /// Выставить минимальное число особых точек для каждого цвета
        /// </summary>
        public void Subscribe_Length_Special() {

            if (length_osob_red_const < length_osob_red_diff)
            {
                length_osob_red_diff = length_osob_red_const;
            }
            else
            {
                length_osob_red_const = length_osob_red_diff;
            }

            if (length_osob_IK_const < length_osob_IK_diff)
            {
                length_osob_IK_diff = length_osob_IK_const;
            }
            else
            {
                length_osob_IK_const = length_osob_IK_diff;
            }

        }

        /// <summary>
        /// Выставить минимальное число особых точек 
        /// </summary>
        public void Subscribe_Length_Osob_Full() {

            int min_length = length_osob_red_const;
            if (min_length > length_osob_IK_const)
            {
                min_length = length_osob_IK_const;
            }
            if (min_length>length_osob_red_diff)
            {
                min_length = length_osob_red_diff;
            }
            if (min_length>length_osob_IK_diff)
            {
                min_length = length_osob_IK_diff;
            }

            length_osob_IK_const = min_length;
            length_osob_IK_diff = min_length;
            length_osob_red_const = min_length;
            length_osob_red_diff = min_length;

        }
        /// <summary>
        /// Рассчитать коэффициент усиления переменного сигнала
        /// </summary>
        public void Calculate_K_Pow_Diff() {
            int length_min_red = length_osob_red_const;
            if (length_osob_red_diff<length_osob_red_const)
            {
                length_min_red = length_osob_red_diff;
            }

            int length_min_IK = length_osob_IK_const;
            if (length_osob_IK_diff<length_osob_IK_const)
            {
                length_min_IK = length_osob_IK_diff;
            }


            for (int i = 1; i < length_min_red; i++)
            {
                K_Pow_diff_RED[i] = System.Convert.ToDouble((osob_red_diff[4, i] + osob_red_diff[10, i]) - osob_red_diff[2, i]) / System.Convert.ToDouble((osob_red_const[4, i] + osob_red_const[10, i]) - osob_red_const[2, i]);
            }

            for (int i = 1; i < length_min_IK; i++)
            {
                K_Pow_diff_IK[i] = System.Convert.ToDouble((osob_IK_diff[4, i] + osob_IK_diff[10, i]) - osob_IK_diff[2, i]) / System.Convert.ToDouble((osob_IK_const[4, i] + osob_IK_const[10, i]) - osob_IK_const[2, i]);
            }

        }

        /// <summary>
        /// Рассчитать размерную интенсивность
        /// </summary>
        public void Calculate_Intensity() {

            for (int i = 0; i < length_osob_red_const - 1; i++)
            {
                Intensity_RED_const[i, 0] = System.Convert.ToDouble(osob_red_const[2, i] - shift_zero_scale) * 2.5 / 512;//минимум
                Intensity_RED_const[i, 1] = System.Convert.ToDouble((osob_red_const[4, i] + osob_red_const[10, i]) - shift_zero_scale) * 2.5 / 512;//Максимум
                Intensity_RED_const[i, 2] = System.Convert.ToDouble(((osob_red_const[2, i]+ osob_red_const[4, i] + osob_red_const[10, i]) /2) - shift_zero_scale) * 2.5 / 512;//среднее

            }

            for (int i = 0; i < length_osob_IK_const - 1; i++)
            {
                Intensity_IK_const[i, 0] = System.Convert.ToDouble(osob_IK_const[2, i] - shift_zero_scale) * 2.5 / 512;//минимум
                Intensity_IK_const[i, 1] = System.Convert.ToDouble((osob_IK_const[4, i] + osob_IK_const[10, i]) - shift_zero_scale) * 2.5 / 512;//Максимум
                Intensity_IK_const[i, 2] = System.Convert.ToDouble(((osob_IK_const[2, i]+ osob_IK_const[4, i] + osob_IK_const[10, i]) /2) - shift_zero_scale) * 2.5 / 512;//среднее
            }

            for (int i = 0; i < length_osob_red_diff - 1; i++)
            {
                Intensity_RED_diff[i, 0] = System.Convert.ToDouble(osob_red_diff[2, i] - shift_zero_scale) * 2.5 / 512;//минимум
                Intensity_RED_diff[i, 1] = System.Convert.ToDouble((osob_red_diff[4, i] + osob_red_diff[10, i]) - shift_zero_scale) * 2.5 / 512;//Максимум
                Intensity_RED_diff[i, 2] = System.Convert.ToDouble(((osob_red_diff[2, i]+osob_red_diff[4, i] + osob_red_diff[10, i]) /2) - shift_zero_scale) * 2.5 / 512;//среднее

            }

            for (int i = 0; i < length_osob_IK_diff - 1; i++)
            {
                Intensity_IK_diff[i, 0] = System.Convert.ToDouble(osob_IK_diff[2, i] - shift_zero_scale) * 2.5 / 512;//минимум
                Intensity_IK_diff[i, 1] = System.Convert.ToDouble((osob_IK_diff[4, i] + osob_IK_diff[10, i]) - shift_zero_scale) * 2.5 / 512;//Максимум
                Intensity_IK_diff[i, 2] = System.Convert.ToDouble(((osob_IK_diff[2, i]+osob_IK_diff[4, i] + osob_IK_diff[10, i]) /2) - shift_zero_scale) * 2.5 / 512;//среднее

            }


        }
        /// <summary>
        /// Рассчитать временные координаты точек сатурации
        /// </summary>
        public void Calculate_Saturation_Time() {

            for (int i = 0; i < length_osob_red_const; i++)//Временные координаты
            {
                Saturation_t[i] = System.Convert.ToDouble(osob_red_const[3, i]) / 1000;
            }
        }

        /// <summary>
        /// Рассчитать насыщение постоянного сигнала (1 способ)
        /// </summary>
        public void Calculate_Saturation_1_Kalinina_Const() {
            double L_2;
            double L_1;
                        
            for (int i = 0; i < length_osob_red_const - 1; i++)//Амплитудные координаты
            {
                L_1 = Math.Log((K_pow_const_IK * Intensity_IK_const[i, 1] / 10000) / (K_pow_const_IK * Intensity_IK_const[i, 0] / 10000));
                L_2 = Math.Log((K_pow_const_RED * Intensity_RED_const[i, 1] / 10000) / (K_pow_const_RED * Intensity_RED_const[i, 0] / 10000));

                Saturation1[i] = (E_Hb_650_1 * (L_2 - (E_Hb_900_1 / E_Hb_650_1) * L_1)) / (E_Hb_650_1 * (L_2 - (E_Hb_900_1 / E_Hb_650_1) * L_1) + E_Hb02_900_2 * L_1 - E_Hb02_650_2 * L_2);                               
            }
        }

        /// <summary>
        /// Рассчитать насыщение постоянного сигнала (2 способ)
        /// </summary>
        public void Calculate_Saturation_2_New_Const()
        {
            double L_2;
            double L_1;
            double T_12;

            for (int i = 0; i < length_osob_red_const - 1; i++)//Амплитудные координаты
            {
                L_1 = Math.Log((K_pow_const_IK * Intensity_IK_const[i, 1] / 10000) / (K_pow_const_IK * Intensity_IK_const[i, 0] / 10000));
                L_2 = Math.Log((K_pow_const_RED * Intensity_RED_const[i, 1] / 10000) / (K_pow_const_RED * Intensity_RED_const[i, 0] / 10000));
                               
                T_12 = L_1 / L_2;
                Saturation2[i] = (T_12 * E_Hb_900_1 - E_Hb_650_1) / (E_Hb02_650_2 - E_Hb_650_1 + T_12 * (E_Hb_900_1 - E_Hb02_900_2));
            }
        }

        /// <summary>
        /// Рассчитать насыщение переменного сигнала (1 способ)
        /// </summary>
        public void Calculate_Saturation_1_Kalinina_Diff()
        {
            double L_2;
            double L_1;

            for (int i = 0; i < length_osob_red_const - 1; i++)//Амплитудные координаты
            {
                  L_1 = Math.Log(((K_Pow_diff_IK[i] * (Intensity_IK_const[i, 2] + Intensity_IK_diff[i, 2] - Intensity_IK_diff[i, 1])) / 10000) / ((K_Pow_diff_IK[i] * (Intensity_IK_const[i, 2] + Intensity_IK_diff[i, 2] - Intensity_IK_diff[i, 0])) / 10000));
                  L_2 = Math.Log(((K_Pow_diff_RED[i] * (Intensity_RED_const[i, 2] + Intensity_RED_diff[i, 2] - Intensity_RED_diff[i, 1])) / 10000) / ((K_Pow_diff_RED[i] * (Intensity_RED_const[i, 2] + Intensity_RED_diff[i, 2] - Intensity_RED_diff[i, 0])) / 10000));

                //  L_2 = Math.Log((( (Intensity_IK_const[i, 2] +  Intensity_IK_diff[i, 1])) / 10000) / (( (Intensity_IK_const[i, 2] +  Intensity_IK_diff[i, 0])) / 10000));
                //  L_1 = Math.Log((( (Intensity_RED_const[i, 2] + Intensity_RED_diff[i, 1])) / 10000) / (( (Intensity_RED_const[i, 2] +  Intensity_RED_diff[i, 0])) / 10000));


                //Первый вариант Калининой
                Saturation1[i] = (E_Hb_650_1 * (L_2 - (E_Hb_900_1 / E_Hb_650_1) * L_1)) / (E_Hb_650_1 * (L_2 - (E_Hb_900_1 / E_Hb_650_1) * L_1) + E_Hb02_900_2 * L_1 - E_Hb02_650_2 * L_2);
            }
        }

        /// <summary>
        /// Рассчитать насыщение переменного сигнала (2 способ)
        /// </summary>
        public void Calculate_Saturation_2_New_Diff()
        {
            double L_2;
            double L_1;
            double T_12;

            for (int i = 0; i < length_osob_red_const - 1; i++)//Амплитудные координаты
            {

                L_1 = Math.Log(((K_Pow_diff_IK[i] * (Intensity_IK_const[i, 2] + Intensity_IK_diff[i, 2] - Intensity_IK_diff[i, 1])) / 10000) / ((K_Pow_diff_IK[i] * (Intensity_IK_const[i, 2] + Intensity_IK_diff[i, 2] - Intensity_IK_diff[i, 0])) / 10000));
                L_2 = Math.Log(((K_Pow_diff_RED[i] * (Intensity_RED_const[i, 2] + Intensity_RED_diff[i, 2] - Intensity_RED_diff[i, 1])) / 10000) / ((K_Pow_diff_RED[i] * (Intensity_RED_const[i, 2] + Intensity_RED_diff[i, 2] - Intensity_RED_diff[i, 0])) / 10000));

                //Второй вариант 17,11,19
                T_12 = L_1 / L_2;
                Saturation2[i] = (T_12 * E_Hb_900_1 - E_Hb_650_1) / (E_Hb02_650_2 - E_Hb_650_1 + T_12 * (E_Hb_900_1 - E_Hb02_900_2));
            }
        }

        public double[] Get_Saturation_Time() 
        {
            return Saturation_t;
        }

        public double[] Get_Saturation_1()
        {
            return Saturation1;
        }
        public double[] Get_Saturation_2()
        {
            return Saturation2;
        }


        public int Get_Length_Special_Red_Const() {

            return length_osob_red_const;
        }

        public int Get_Length_Special_Red_Diff()
        {

            return length_osob_red_diff;
        }
        public int Get_Length_Special_IK_Const()
        {

            return length_osob_IK_const;
        }
        public int Get_Length_Osob_IK_Diff()
        {

            return length_osob_IK_diff;
        }





    }
}
