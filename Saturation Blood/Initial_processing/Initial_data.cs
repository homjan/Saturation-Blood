using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    public class Initial_Data
    {
        const int numerical = 60;

        private long[,] row1;
        private long[,] row2;
        private long[] row3;
        private long[] row4;

        private long[][] row_divided;

        private int b;

        public int REG;
        public int EKG;
        private int POW_DIFF = 1000000;
       
       // private int RED_miracle;

        const int potok2 = 12;

        String name_file;

        Initial_processing.Reader_data re_data;

        /// <summary>
        /// Конструктор открывает файл c именем ss и заполняет содержимым 2-мерный массив 
        /// </summary>
        /// <param name="ss">Имя файла</param>
        /// <param name="reg">номер каналов с РЭГ</param>
        /// <param name="ekg">номер каналов с ЕКГ</param>
        public Initial_Data(String ss, int reg, int ekg) //
        {
            this.name_file = ss;
            this.REG = reg;
            this.EKG = ekg;
          

            re_data = new Initial_processing.Reader_data(name_file);

            row1 = re_data.Return_Read_Massiv();
            b = re_data.Return_Read_Strings();

            row2 = new long[b + 200, potok2];
            row3 = new long[b + 200];
            row4 = new long[b + 200];

            Row3_Average_Kanal_reg();
        }

        public Initial_Data(String ss, int reg, int ekg, int power) //
        {
            this.name_file = ss;
            this.REG = reg;
            this.EKG = ekg;
          
            this.POW_DIFF = power;

            re_data = new Initial_processing.Reader_data(name_file);

            row1 = re_data.Return_Read_Massiv();
            b = re_data.Return_Read_Strings();

            row2 = new long[b + 200, potok2];
            row3 = new long[b + 200];
            row4 = new long[b + 200];

            Row3_Average_Kanal_reg();
        }

        /// <summary>
        /// Конструктор открывает файл c именем ss, заполняет содержимым 2-мерный массив и разрезает данные с выбранный канал РЭГ на периоды
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="reg"></param>
        /// <param name="ekg"></param>
        /// <param name="div"></param>
        public Initial_Data(String ss, int reg, int ekg, bool div) //
        {
            this.name_file = ss;
            this.REG = reg;
            this.EKG = ekg;

            re_data = new Initial_processing.Reader_data(name_file);

            row_divided = re_data.Return_Read_Massiv_Divided_Data();

            // b = re_data.return_read_stroki();
        }
        /// <summary>
        /// Сдвинуть к 0 1 столбик c временем
        /// </summary>
        public void Row1_Shift_Time_To_0() 
        {
            for (int j = 3; j < b; j++)
            {
                row1[j, 0] = row1[j, 0] - row1[2, 0];
            }
            row1[2, 0] = 0;
            row1[1, 0] = 0;
            row1[0, 0] = 0;

        }

        /// <summary>
        /// Сглаживаем по 7 точкам все кроме времени
        /// </summary>
        public void Row1_Smothing() //
        {
            long[,] rw11 = row1;

            for (int d = 1; d < potok2; d++)
            {
                for (int q = 4; q < b - 4; q++)
                {
                    row1[q, d] = (rw11[q + 3, d] + rw11[q + 2, d] + rw11[q + 1, d] + rw11[q, d] + rw11[q - 1, d] + rw11[q - 2, d] + rw11[q - 3, d]) / 7;
                }
            }

        }

        /// <summary>
        /// Отражаем данные с канала РЭГ относительно среднего значения этого канала
        /// </summary>
        public void Row1_Reflect() {

            long sum_row = 0;
             double sred = 0.0;

             for (int q = 3; q < b - 8; q++)
             {
                sum_row = sum_row + row1[q, REG]; 
             }
             sred = Convert.ToDouble(sum_row)/ Convert.ToDouble(b);
             sum_row = Convert.ToInt64(sred);
             for (int q = 3; q < b - 8; q++)
             {
                 row1[q, REG] = sum_row + (sum_row - row1[q, REG]);
             }

        }
        /// <summary>
        /// Отражаем данные с выбранного канала относительно среднего значения этого канала
        /// </summary>
        /// <param name="canal">Номер канала</param>
        public void Row1_Reflect_Chosen(int canal)
        {
               long sum_row = 0;
                double sred = 0.0;

                for (int q = 3; q < b - 8; q++)
                {
                    sum_row = sum_row + row1[q, canal];
                }
                sred = Convert.ToDouble(sum_row) / Convert.ToDouble(b);
                sum_row = Convert.ToInt64(sred);
                for (int q = 3; q < b - 8; q++)
                {
                    row1[q, canal] = sum_row + (sum_row - row1[q, canal]);
                }

         //   reflection_row1(canal);

          

        }

        private void Reflection_row1(int canal) 
        {
            long sum_row = 0;
            double sred = 0.0;
            int shift = 500;            
            int q = 3;

            while (q<(b-8))
            {                
                int i = 0;
                while (i < shift)
                {
                    sum_row = sum_row + row1[q, canal];
                    i++;
                    q++;
                }
                q = q - shift;
                sred = Convert.ToDouble(sum_row) / Convert.ToDouble(shift);
                sum_row = Convert.ToInt64(sred);
                i = 0;
                while (i < shift)
                {
                    row1[q, canal] = sum_row + (sum_row - row1[q, canal]);
                    i++;
                    q++;
                }
                
            }                               

        }

        /// <summary>
        /// Считать производную и усиливать ее
        /// </summary>
        public void Row2_Calculate()// 
        {
            for (int d = 1; d <= potok2; d++)
                {
                for (int q = 3; q < b - 3; q++)
                {
                    row2[q, d - 1] = POW_DIFF * (row1[q + 1, d] - row1[q - 1, d]) / (row1[q + 1, 0] - row1[q - 1, 0]);
                }
            }                       
        }
        /// <summary>
        /// Считать производную и усиливать ее в pow_DIFF_1 раз
        /// </summary>
        /// <param name="pow_DIFF_1"></param>
        public void Row2_re_Calculate(int pow_DIFF_1) {

            for (int d = 1; d <= potok2; d++)
            {
                for (int q = 3; q < b - 3; q++)
                {
                    row2[q, d - 1] = pow_DIFF_1 * (row1[q + 1, d] - row1[q - 1, d]) / (row1[q + 1, 0] - row1[q - 1, 0]);
                }
            }

        }
        /// <summary>
        /// Усреднить производную канала РЭГ
        /// </summary>
        public void Row3_Average_Kanal_reg()
        {
            for (int q = 4; q < b - 4; q++)
                {
                    row3[q] = (row2[q + 3, REG - 1] + row2[q + 2, REG - 1] + row2[q + 1, REG - 1] + row2[q, REG - 1] + row2[q - 1, REG - 1] + row2[q - 2, REG - 1] + row2[q - 3, REG - 1]) / 7;
                }         
        }

        /// <summary>
        /// Усреднить производную выбранного канала
        /// </summary>
        /// <param name="number_kanal"></param>
        public void Row3_Average_Chosen_Kanal(int number_kanal) {

            for (int q = 4; q < b - 4; q++)
            {
                row3[q] = (row2[q + 3, number_kanal - 1] + row2[q + 2, number_kanal - 1] + row2[q + 1, number_kanal - 1] + row2[q, number_kanal - 1] + row2[q - 1, number_kanal - 1] + row2[q - 2, number_kanal - 1] + row2[q - 3, number_kanal - 1]) / 7;
            }
            REG = number_kanal;

        }
        /// <summary>
        /// Сгладить ЭКГ
        /// </summary>
        public void Row4_Smoothing_ekg()
        {
            for (int q = 3; q < b - 8; q++)
            {
                row4[q] = (row1[q, EKG] + row1[q + 7, EKG]) / 2;
            }
        }

        /// <summary>
        /// Найти положение элемента массива с выбранным временем
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int Find_Position_in_Time(long time) {
            int position = 0;

            long min_time = 0;
            long max_time = row1[b-201,0];

            int min_position = 0;
            int max_position = b;
            int current_position = (max_position + min_position) / 2;

            long current_time = row1[current_position, 0];

            if (time == 0) {
                return 0;
            
            }

            if (time>max_time)
            {
                return 0;
            }

            while (true)
            {
                if (time < current_time)
                {
                    max_time = current_time;
                    max_position = current_position;
                    current_position = (max_position + min_position) / 2;
                    current_time = row1[current_position, 0];
                }
                else if (time > current_time)
                {
                    min_time = current_time;
                    min_position = current_position;
                    current_position = (max_position + min_position) / 2;
                    current_time = row1[current_position, 0];
                }
                else if (time == current_time) 
                {
                    position = current_position;
                    break;
                }               
            }

            return position;
        }
        /// <summary>
        /// Записать даннык в файл
        /// </summary>
        /// <param name="name_file">Имя файла</param>
        public void Row1_Write_In_File(String name_file)
        {
            StreamWriter rw2 = new StreamWriter(name_file);
            for (int j = 3; j < b; j++)
            {
                rw2.Write(System.Convert.ToString(row1[j, 0]));

                for (int z = 0; z < potok2; z++)
                {
                    rw2.Write(System.Convert.ToString("\t"));
                    rw2.Write(System.Convert.ToString(row1[j, z + 1]));
                }
                rw2.WriteLine();
            }
            rw2.Close();
        }


        //Геттеры
        public long[,] Get_row1()
        {
            return row1;
        }
        public long[,] Get_row2()
        {
            return row2;
        }
        public long[] Get_row3()
        {
            return row3;
        }
        public long[] Get_row4()
        {
            return row4;
        }
        public int Get_b()
        {
            return b;
        }

        public long[][] Get_row_divided()
        {
            return row_divided;
        }

        /// <summary>
        /// Возвращает выбранный элемент с выбранного канала
        /// </summary>
        /// <param name="x">номер элемента</param>
        /// <param name="kanal">номер канала</param>
        /// <returns></returns>
        public long Get_row1_x_y(int x, int kanal)
        {
            return row1[x, kanal];
        }
        public void Set_row1(long[,] row_1_new)
        {
            row1 = row_1_new;
        }

        public void Set_b(int bx)
        {
            b = bx;
        }







    }
}
