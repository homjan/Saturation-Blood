using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    class Special_Point
    {
        Initial_processing.Divided_By_Periods_Data Period_job;
        Initial_Data initial_data;

        private int mass = 10;

        private long[,] spec_point;

        private void Set_Spec_Point(long[,] value)
        {
            spec_point = value;
        }

        const int shift_B1 = 2;

        public long[,] Get_Spec_Point()
        {
            return spec_point;
        }

        public Special_Point(Initial_processing.Divided_By_Periods_Data per, Initial_Data init)
        {
            this.Period_job = per;
            this.initial_data = init;

        }

        /// <summary>
        /// Вернуть точки ЭКГ
        /// </summary>
        /// <param name="combobox3">Данные регулировки</param>
        public void Return_Point_EKG(String combobox3)
        {

            int b = initial_data.Get_b();

            long[,] row1 = initial_data.Get_row1();

            long[,] row2 = initial_data.Get_row2();
            long[] row3 = initial_data.Get_row3();
            long[] row4 = initial_data.Get_row4();

            int reg = initial_data.REG;

            /////////////////////////

            int b0 = 0; //второй счетчик строк
            int ew = 0;//счетчик найденных максимумов
            int est = 0;
            int maxim = 0;// счетчик массива
            long[] max1_y = new long[2000]; // счетчик максимума
            long[] max1_x = new long[2000];
            long[] max1_coor = new long[2000];

            for (int u = 0; u < 1000; u++)
            {
                max1_x[u] = 1;
                max1_y[u] = 1;
            }
            // while (ew<2)
            int N_propusk = 0;

            while (b0 < b)/////////////поиск опорных точек
            {
                for (int t = 0; t < 200; t++)
                {
                    b0++;

                    if ((row3[t + 1 + est]) > max1_y[maxim])
                    {
                        max1_y[maxim] = (row3[t + 1 + est]);
                        max1_x[maxim] = row1[t + 1 + est, 0];
                        max1_coor[maxim] = t + 1 + est;
                    }

                }

                if (max1_y[maxim] > System.Convert.ToInt64(combobox3) * mass)////////////////////!!!!!!
                {
                    ew++;// счетчик пиков производной
                    maxim++;
                    N_propusk = 0;
                }
                est += 200;
                N_propusk++;

                /*    if (N_propusk == 10) {
                         b0 = b0 - (200 * 10);
                         est = est - 2000;
                         mass = mass-3;

                         N_propusk = 0;

                         dtr = true;
                     }

                     if (dtr) {
                         dtch++;
                     }

                     if (dtch == 10) {
                         dtr = false;
                         mass = 10;
                         dtch = 0;
                     }*/

            }

            ////////////////////////////////////////////////////////////////
            int period = 0;
            double period2 = 0;
            for (int u = 1; u < ew; u++)
            {
                period2 = period2 + (max1_coor[u] - max1_coor[u - 1]);
            }
            period = System.Convert.ToInt32(Math.Round(period2 / (ew - 1)));

            double Left_shift = 0.1 * period;
            double Right_shift = 0.75 * period;

            double Shift_03n = 300;



            if (period < 400)
            {
                Shift_03n = 0.65 * period;
            }


            int Left_Border = Convert.ToInt32(Math.Round(Left_shift));
            int Right_Border = System.Convert.ToInt32(Math.Round(Right_shift));

            int Shift_03 = System.Convert.ToInt32(Math.Round(Shift_03n));

            /////////////////////////////////////////////////////

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)

            long[] EKG_max = new long[ew];
            long[] EKG_max_x = new long[ew];

            for (int w = 0; w < ew; w++) //н.у.
            {
                for (int i = 0; i < 14; i++)
                {
                    osob_x[i, w] = 1;
                    osob_y[i, w] = 512;
                }


                EKG_max[w] = 512;
                EKG_max_x[w] = 0;
            }



            for (int w = 2; w < ew - 1; w++)//перебираем пики
            {
                ///////////////////////////// Ищем максимум ЭКГ--1

                for (long i = max1_coor[w]; i > max1_coor[w] - Shift_03; i--)//2
                {
                    if (row4[i] > EKG_max[w])
                    {
                        EKG_max[w] = row4[i];
                        EKG_max_x[w] = row1[i, 0];

                    }
                }
            }


            for (int w = 1; w < ew - 1; w++)//перебираем пики
            {
                spec_point[0, w] = EKG_max[w];
                spec_point[1, w] = EKG_max_x[w];
            }


        }

        /// <summary>
        /// Удалить нули из периода
        /// </summary>
        public void Delete_Zero_In_Data()
        {
            int arre = spec_point.Length;
            int ew = arre / 15;

            for (int i = 1; i < ew; i++)
            {

                if (spec_point[2, i] == 0 && spec_point[3, i] == 0 && spec_point[4, i] == 0 && spec_point[5, i] == 0
                   )
                {
                    for (int j = i; j < ew - 1; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            spec_point[k, j] = spec_point[k, j + 1];
                        }
                    }
                }
            }
            int s = 1;

            for (int i = 1; i < ew; i++)
            {
                s++;
                if (spec_point[2, i] == 0 && spec_point[3, i] == 0 && spec_point[4, i] == 0 && spec_point[5, i] == 0
                    )
                {
                    break;
                }
            }

            long[,] period_new = new long[15, s];

            for (int i = 0; i < s; i++)
            {
                for (int k = 0; k < 15; k++)
                {
                    period_new[k, i] = spec_point[k, i];
                }
            }

            Set_Spec_Point(period_new);

        }

        /// <summary>
        /// Удалить периоды нулевой длительности 
        /// </summary>
        public void Delete_Equal_Data() {

            int arre = spec_point.Length;
            int ew = arre / 15;

            for (int i = 1; i < ew-1; i++)
            {

                if (spec_point[3, i] == spec_point[3, i+1])
                {
                    for (int j = i; j < ew - 1; j++)
                    {
                        for (int k = 0; k < 15; k++)
                        {
                            spec_point[k, j] = spec_point[k, j + 1];
                        }
                    }
                }
            }
            int s = 1;

            for (int i = 1; i < ew-1; i++)
            {
                s++;
                if (spec_point[3, i] == spec_point[3, i + 1])
                {
                    break;
                }
            }

            long[,] period_new = new long[15, s];

            for (int i = 0; i < s; i++)
            {
                for (int k = 0; k < 15; k++)
                {
                    period_new[k, i] = spec_point[k, i];
                }
            }

            Set_Spec_Point(period_new);

        }

        /// <summary>
        /// Вернуть особые точки
        /// </summary>
        /// <param name="combobox3">Данные регулировки</param>
        public void Return_Special_Point(String combobox3)
        {
            int b = initial_data.Get_b();

            long[,] row1 = initial_data.Get_row1();

            long[,] row2 = initial_data.Get_row2();
            long[] row3 = initial_data.Get_row3();
            long[] row4 = initial_data.Get_row4();

            int reg = initial_data.REG;

            /////////////////////////

            int b0 = 0; //второй счетчик строк
            int ew = 0;//счетчик найденных максимумов
            int est = 0;
            int maxim = 0;// счетчик массива
            long[] max1_y = new long[1000]; // счетчик максимума
            long[] max1_x = new long[1000];
            long[] max1_coor = new long[1000];

            for (int u = 0; u < 1000; u++)
            {
                max1_x[u] = 1;
                max1_y[u] = 1;
            }
         
            while (b0 < b)
            {
                for (int t = 0; t < 200; t++)
                {
                    b0++;

                    if ((row3[t + 1 + est]) > max1_y[maxim])
                    {
                        max1_y[maxim] = (row3[t + 1 + est]);
                        max1_x[maxim] = row1[t + 1 + est, 0];
                        max1_coor[maxim] = t + 1 + est;
                    }
                }

                if (max1_y[maxim] > System.Convert.ToInt64(combobox3) * 10)////////////////////!!!!!!
                {
                    ew++;// счетчик пиков производной
                    maxim++;
                }
                est += 200;
            }

            ////////////////////////////////////////////////////////////////
            int period = 0;
            double period2 = 0;
            for (int u = 1; u < ew; u++)
            {
                period2 = period2 + (max1_coor[u] - max1_coor[u - 1]);
            }
            period = System.Convert.ToInt32(Math.Round(period2 / (ew - 1)));

            double Left_shift = 0.1 * period;
            double Right_shift = 0.75 * period;

            double Shift_005n = 30;//разные сдвиги
            double Shift_01n = 45;
            double Shift_015n = 100;
            double Shift_02n = 120;
            double Shift_025n = 150;
            double Shift_03n = 180;
            double Shift_04n = 240;
            double Shift_05n = 300;
            double Shift_065n = 400;
            double Shift_075n = 450;


            if (period < 400)

            {
                Shift_005n = 0.05 * period;//разные сдвиги
                Shift_01n = 0.1 * period;
                Shift_015n = 0.15 * period;
                Shift_02n = 0.2 * period;
                Shift_025n = 0.25 * period;
                Shift_03n = 0.3 * period;
                Shift_04n = 0.4 * period;
                Shift_05n = 0.5 * period;
                Shift_065n = 0.65 * period;
                Shift_075n = 0.75 * period;
            }

            int Left_Border = System.Convert.ToInt32(Math.Round(Left_shift));
            int Right_Border = System.Convert.ToInt32(Math.Round(Right_shift));

            int Shift_005 = System.Convert.ToInt32(Math.Round(Shift_005n));
            int Shift_01 = System.Convert.ToInt32(Math.Round(Shift_01n));
            int Shift_015 = System.Convert.ToInt32(Math.Round(Shift_015n));
            int Shift_02 = System.Convert.ToInt32(Math.Round(Shift_02n));
            int Shift_025 = System.Convert.ToInt32(Math.Round(Shift_025n));
            int Shift_03 = System.Convert.ToInt32(Math.Round(Shift_03n));
            int Shift_04 = System.Convert.ToInt32(Math.Round(Shift_04n));
            int Shift_05 = System.Convert.ToInt32(Math.Round(Shift_05n));
            int Shift_065 = System.Convert.ToInt32(Math.Round(Shift_065n));
            int Shift_075 = System.Convert.ToInt32(Math.Round(Shift_075n));

            /////////////////////////////////////////////////////

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек
            double[] schet_sum_final = new double[15];

            double[,] time_dif = new double[3, ew];
            double[] time_dif_sum = new double[3];
            double[] time_dif_sum_final = new double[3];

            long[] EKG_max = new long[ew];
            long[] EKG_max_x = new long[ew];

            long[] Begin_coor = new long[ew];
            long[] x_min_1 = new long[ew];
            long[] y_min_1 = new long[ew];

            long[] x_min_2 = new long[ew];
            long[] y_min_2 = new long[ew];

            long[] max_1_1 = new long[ew];
            long[] coor_max_1_1 = new long[ew];

            long[] local_min = new long[ew];
            long[] coor_local_min = new long[ew];
            long[] nomer_local_min = new long[ew];

            long[] local_max = new long[ew];
            long[] coor_local_max = new long[ew];

            long[] local_min_diff = new long[ew];
            long[] coor_local_min_diff = new long[ew];
            long[] nomer_local_min_diff = new long[ew];

            long[] global_max = new long[ew];
            long[] coor_global_max = new long[ew];

            long[] chetv_ot_end = new long[ew];
            long[] coor_chetv_ot_end = new long[ew];

            bool B3_diff = false;
            long Diff_3_max = 0;
            long Diff_3_max_coor = 0;

            for (int w = 0; w < ew; w++) //н.у.
            {
                for (int i = 0; i < 14; i++)
                {
                    osob_x[i, w] = 1;
                    osob_y[i, w] = 512;
                }
                x_min_1[w] = 0;//3
                y_min_1[w] = 512;
                x_min_2[w] = 0;
                y_min_2[w] = 512;

                max_1_1[w] = 1;//7
                coor_max_1_1[w] = 0;

                local_min[w] = 512;//10
                coor_local_min[w] = 0;

                local_max[w] = 512;//11
                coor_local_max[w] = 0;

                local_min_diff[w] = 512;//13
                coor_local_min_diff[w] = 0;

                global_max[w] = 512;
                coor_global_max[w] = 0;

                EKG_max[w] = 512;
                EKG_max_x[w] = 0;
            }


            for (int w = 2; w < ew - 2; w++)//перебираем пики
            {

                //////////////////////////////ищем начало подъема--2

                osob_x[1, w] = row1[max1_coor[w], 0];
                osob_y[1, w] = row1[max1_coor[w], reg];// Данные с соответствующего канала (№4)

                for (long i = max1_coor[w]; i > max1_coor[w] - Shift_03; i--)//2
                {

                    if (row1[i, reg] < osob_y[1, w])
                    {
                        osob_x[1, w] = row1[i + shift_B1, 0];
                        osob_y[1, w] = row1[i + shift_B1, reg];// Данные с соответствующего канала (№4)
                        Begin_coor[w] = i + shift_B1;
                        y_min_2[w] = row1[i + shift_B1, reg];
                        x_min_2[w] = row1[i + shift_B1, 0];
                    }
                }

                ////////////////////////////////////  // Ищем 1 максимум--7

                for (long i = max1_coor[w]; i < max1_coor[w] + Shift_03; i++)
                {
                    if (row1[i, reg] > max_1_1[w])
                    {
                        max_1_1[w] = row1[i, reg];
                        coor_max_1_1[w] = row1[i, 0];
                    }
                }

                //Ищем локальный минимум производной---12

                for (long i = max1_coor[w] + Shift_015; i < max1_coor[w] + Shift_025; i++)
                {
                    if (row3[i] < local_min_diff[w])
                    {
                        local_min_diff[w] = row3[i + 1];
                        coor_local_min_diff[w] = row1[i + 1, 0];
                        nomer_local_min_diff[w] = i + 1;
                    }
                }


                //ищем первый локальный минимум---10

                for (long i = nomer_local_min_diff[w]; i < nomer_local_min_diff[w] + Shift_03; i++)
                {
                    if (row3[i + 1] > 0)
                    {
                        local_min[w] = row1[i, reg];
                        coor_local_min[w] = row1[i, 0];
                        nomer_local_min[w] = i;
                        B3_diff = true;
                        break;
                    }
                }

                if (B3_diff == false)
                {
                    Diff_3_max = row3[nomer_local_min_diff[w]];
                    Diff_3_max_coor = nomer_local_min_diff[w];

                    for (long i = nomer_local_min_diff[w]; i < nomer_local_min_diff[w] + Shift_03; i++)
                    {
                        if (Diff_3_max < row3[i])
                        {
                            Diff_3_max = row3[i];
                            Diff_3_max_coor = i;
                        }
                    }

                    local_min[w] = row1[Diff_3_max_coor, reg];
                    coor_local_min[w] = row1[Diff_3_max_coor, 0];
                    nomer_local_min[w] = Diff_3_max_coor;

                }

                B3_diff = false;
                // ищем 2 локальный максимум  ---11

                local_max[w] = local_min[w];
                coor_local_max[w] = coor_local_min[w];

                for (long i = nomer_local_min[w] + Shift_005 + 10; i < nomer_local_min[w] + Shift_025; i++)///////////////
                {
                    if (i == nomer_local_min[w] + 10)
                    {
                        local_max[w] = row1[i + 1, reg];
                        coor_local_max[w] = row1[i + 1, 0];
                    }

                    if (row1[i + 1, reg] > local_max[w])
                    {
                        local_max[w] = row1[i + 1, reg];
                        coor_local_max[w] = row1[i + 1, 0];
                    }


                }

                //Ищем глобальный максимум ---14

                for (long i = max1_coor[w] - Shift_01; i < max1_coor[w] + Shift_075; i++)
                {
                    if (row1[i, reg] > global_max[w])
                    {
                        global_max[w] = row1[i, reg];
                        coor_global_max[w] = row1[i, 0];
                    }
                }

                //Ищем амплитуда за 0,25 до конца
                chetv_ot_end[w] = row1[max1_coor[w] + Shift_065, reg];
                coor_chetv_ot_end[w] = row1[max1_coor[w] + Shift_065, 0];


                if (w < ew - 1)
                {
                    time_dif[0, w] = max1_x[w + 1] - max1_x[w];
                }
                time_dif[1, w] = coor_global_max[w] - max1_x[w];
                time_dif[2, w] = max1_x[w] - osob_x[1, w];


                //  schet[0, w] = EKG_max[w];// пик экг
                //   schet[1, w] = EKG_max_x[w];// положение пика экг

                schet[2, w] = osob_y[1, w];// минимум В1
                schet[3, w] = osob_x[1, w];// положение минимума В1 - начала отсчета

                schet[4, w] = max_1_1[w] - y_min_2[w];// максимум В2
                schet[5, w] = coor_max_1_1[w];// положение максимума В2 - начала отсчета - EKG_max_x[w]

                schet[6, w] = local_min[w] - y_min_2[w];// минимум В3
                schet[7, w] = coor_local_min[w];// положение минимума В3 - начала отсчета- EKG_max_x[w]

                schet[8, w] = local_max[w] - y_min_2[w];// максимум В4
                schet[9, w] = coor_local_max[w];// положение максимума В4 - начала отсчета - EKG_max_x[w]

                schet[10, w] = y_min_2[w];


            }

            spec_point = schet;
        }

        /// <summary>
        /// Рассчитать особые точки используя нейронную сеть 1000-100-35
        /// </summary>
        public void Return_Special_Point_Neural_Network()
        {

            long[][] periods = Period_job.Get_Period();//Конвертируем выбранную поток с рег в массив периодов

            long periods_full_length = Period_job.Return_Period_In_Data_Length();

            long[,] periods_1000 = Period_job.Return_Periods_1000();//Добавляем 0 в массиве до одинаковой длины=1000

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек

            int N_nejron_in = 1000;

            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;
                     
            double[,] row0001 = Function_additional.Convert_Long_Double(periods_1000, ew, N_nejron_in);

            double[,] row01 = Function_additional.Calculate_Derivative_Array(row0001, ew, N_nejron_in);



            double[] sloj2B2 = new double[100];
            double[] sloj3B2 = new double[35];

            double[] sloj2B3 = new double[100];
            double[] sloj3B3 = new double[45];

            double[] sloj2B4 = new double[100];
            double[] sloj3B4 = new double[40];

            double[] sloj3B2_final = new double[N_nejron_in];
            double[] sloj3B3_final = new double[N_nejron_in];
            double[] sloj3B4_final = new double[N_nejron_in];

            Job_Net job_net = new Job_Net(N_nejron_in, 100, 35);
            job_net.Read_In_File_Bias_1("Сеть1/bias0B2.txt");
            job_net.Read_In_File_Bias_2("Сеть1/bias1B2.txt");

            job_net.Read_In_File_Weight_1("Сеть1/kernel0B2.txt");
            job_net.Read_In_File_Weight_2("Сеть1/kernel1B2.txt");

            /////////////////////////////////////////////
            Job_Net job_net2 = new Job_Net(N_nejron_in, 100, 45);
            job_net2.Read_In_File_Bias_1("Сеть1/bias0B3.txt");
            job_net2.Read_In_File_Bias_2("Сеть1/bias1B3.txt");

            job_net2.Read_In_File_Weight_1("Сеть1/kernel0B3.txt");
            job_net2.Read_In_File_Weight_2("Сеть1/kernel1B3.txt");

            /////////////////////////////////////
            Job_Net job_net3 = new Job_Net(N_nejron_in, 100, 40);
            job_net3.Read_In_File_Bias_1("Сеть1/bias0B4.txt");
            job_net3.Read_In_File_Bias_2("Сеть1/bias1B4.txt");

            job_net3.Read_In_File_Weight_1("Сеть1/kernel0B4.txt");
            job_net3.Read_In_File_Weight_2("Сеть1/kernel1B4.txt");

            //////////////////////////////////////////////////////


            for (int i = 0; i < ew; i++)
            {
                if (periods[i].Length < 240)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }

                else
                {
                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[3, i] = row1[Period_job.Return_Length_x_Zero(i, 0 + shift_B1), 0]; // положение минимума В1 - начала отсчета
                    schet[10, i] = periods[i][0 + shift_B1];

                    double[] r1 = Function_additional.Get_One_Line(row01, i);

                    //////////////////////////////////////////////////////
                    //Нейронная сеть
                    ////////////////////////////////////////////////////////                   


                    sloj2B2 = job_net.Perzertron_Forward(r1, 1000, 100);
                    sloj3B2 = job_net.Perzertron_Forward_Softmax(sloj2B2, 100, 35);
                    sloj3B2_final = Function_additional.Layer_1000(sloj3B2, 65);

                    int B2 = Function_additional.Return_Max_Element_Neural_Network(sloj3B2_final);

                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[4, i] = row1[Period_job.Return_Length_x_Zero(i, B2), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B3 = job_net2.Perzertron_Forward(r1, N_nejron_in, 100);
                    sloj3B3 = job_net2.Perzertron_Forward_Softmax(sloj2B3, 100, 45);
                    sloj3B3_final = Function_additional.Layer_1000(sloj3B3, 170);

                    int B3 = Function_additional.Return_Max_Element_Neural_Network(sloj3B3_final);

                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = row1[Period_job.Return_Length_x_Zero(i, B3), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B4 = job_net3.Perzertron_Forward(r1, 1000, 100);
                    sloj3B4 = job_net3.Perzertron_Forward_Softmax(sloj2B4, 100, 40);
                    sloj3B4_final = Function_additional.Layer_1000(sloj3B4, 200);

                    int B4 = Function_additional.Return_Max_Element_Neural_Network(sloj3B4_final);

                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[8, i] = row1[Period_job.Return_Length_x_Zero(i, B4), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                }

            }
            spec_point = schet;


        }

        /// <summary>
        /// Рассчитать особые точки используя вторую нейронную сеть 1000-100-8 + поиск в 10 элементах
        /// </summary>
        public void Return_Special_Point_Neural_Network_10()
        {

            long[][] periods = Period_job.Get_Period();

            long periods_full_length = Period_job.Return_Period_In_Data_Length();
            long[,] periods_1000 = Period_job.Return_Periods_1000();//Добавляем 0 в массиве до одинаковой длины=1000

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек

            int N_nejron_in = 1000;

            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;

            double[,] row0001 = Function_additional.Convert_Long_Double(periods_1000, ew, N_nejron_in);

            double[,] row01 = Function_additional.Calculate_Derivative_Array(row0001, ew, N_nejron_in);



            double[] sloj2B2 = new double[100];
            double[] sloj3B2 = new double[8];
            double[] sloj3fullB2 = new double[80];

            double[] sloj2B3 = new double[100];
            double[] sloj3B3 = new double[10];
            double[] sloj3fullB3 = new double[100];

            double[] sloj2B4 = new double[100];
            double[] sloj3B4 = new double[11];
            double[] sloj3fullB4 = new double[110];


            double[] sloj3B2_final = new double[N_nejron_in];
            double[] sloj3B3_final = new double[N_nejron_in];
            double[] sloj3B4_final = new double[N_nejron_in];

            Job_Net job_net = new Job_Net(N_nejron_in, 100, 8);
            job_net.Read_In_File_Bias_1("Сеть10/bias0B2.txt");
            job_net.Read_In_File_Bias_2("Сеть10/bias1B2.txt");

            job_net.Read_In_File_Weight_1("Сеть10/kernel0B2.txt");
            job_net.Read_In_File_Weight_2("Сеть10/kernel1B2.txt");

            /////////////////////////////////////////////
            Job_Net job_net2 = new Job_Net(N_nejron_in, 100, 10);
            job_net2.Read_In_File_Bias_1("Сеть10/bias0B3.txt");
            job_net2.Read_In_File_Bias_2("Сеть10/bias1B3.txt");

            job_net2.Read_In_File_Weight_1("Сеть10/kernel0B3.txt");
            job_net2.Read_In_File_Weight_2("Сеть10/kernel1B3.txt");

            /////////////////////////////////////
            Job_Net job_net3 = new Job_Net(N_nejron_in, 100, 11);
            job_net3.Read_In_File_Bias_1("Сеть10/bias0B4.txt");
            job_net3.Read_In_File_Bias_2("Сеть10/bias1B4.txt");

            job_net3.Read_In_File_Weight_1("Сеть10/kernel0B4.txt");
            job_net3.Read_In_File_Weight_2("Сеть10/kernel1B4.txt");

            //////////////////////////////////////////////////////

            for (int i = 0; i < ew; i++)
            {
                if (periods[i].Length < 240)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }

                else
                {
                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[3, i] = row1[Period_job.Return_Length_x_Zero(i, 0), 0 + shift_B1]; // положение минимума В1 - начала отсчета
                    schet[10, i] = periods[i][0 + shift_B1];

                    double[] r1 = Function_additional.Get_One_Line(row01, i);
                    double[] rxx = Function_additional.Get_One_Line_1024(row0001, i, 1000);

                    //////////////////////////////////////////////////////
                    //Нейронная сеть
                    ////////////////////////////////////////////////////////                   


                    sloj2B2 = job_net.Perzertron_Forward(r1, 1000, 100);
                    sloj3B2 = job_net.Perzertron_Forward_Softmax(sloj2B2, 100, 8);

                    sloj3fullB2 = Function_additional.Multiple_Ten_B2_B4(sloj3B2, rxx, 37);
                    sloj3B2_final = Function_additional.Layer_1000(sloj3fullB2, 37);

                    int B2 = Function_additional.Return_Max_Element_Neural_Network(sloj3B2_final);

                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[4, i] = row1[Period_job.Return_Length_x_Zero(i, B2), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B3 = job_net2.Perzertron_Forward(r1, N_nejron_in, 100);
                    sloj3B3 = job_net2.Perzertron_Forward_Softmax(sloj2B3, 100, 10);

                    sloj3fullB3 = Function_additional.Multiple_Ten_B3(sloj3B3, rxx, 127);
                    sloj3B3_final = Function_additional.Layer_1000(sloj3fullB3, 127);

                    int B3 = Function_additional.Return_Max_Element_Neural_Network(sloj3B3_final);

                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = row1[Period_job.Return_Length_x_Zero(i, B3), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B4 = job_net3.Perzertron_Forward(r1, 1000, 100);
                    sloj3B4 = job_net3.Perzertron_Forward_Softmax(sloj2B4, 100, 11);


                    sloj3fullB4 = Function_additional.Multiple_Ten_B2_B4(sloj3B4, rxx, 165);
                    sloj3B4_final = Function_additional.Layer_1000(sloj3fullB4, 165);

                    int B4 = Function_additional.Return_Max_Element_Neural_Network(sloj3B4_final);

                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[8, i] = row1[Period_job.Return_Length_x_Zero(i, B4), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////

                }

            }

            spec_point = schet;
        }

        /// <summary>
        /// Рассчитать особые точки используя вторую нейронную сеть 1000-300-80
        /// </summary>
        public void Return_Special_Point_Neural_Network_100()
        {
            long[][] periods = Period_job.Get_Period();

            long periods_full_length = Period_job.Return_Period_In_Data_Length();


            long[,] periods_1000 = Period_job.Return_Periods_1000();//Добавляем 0 в массиве до одинаковой длины=1000

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек

            int N_nejron_in = 1000;

            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;

            double[,] row0001 = Function_additional.Convert_Long_Double(periods_1000, ew, N_nejron_in);

            double[,] row01 = Function_additional.Calculate_Derivative_Array(row0001, ew, N_nejron_in);



            double[] sloj2B2 = new double[300];
            double[] sloj3B2 = new double[80];

            double[] sloj2B3 = new double[300];
            double[] sloj3B3 = new double[100];

            double[] sloj2B4 = new double[300];
            double[] sloj3B4 = new double[110];

            double[] sloj3B2_final = new double[N_nejron_in];
            double[] sloj3B3_final = new double[N_nejron_in];
            double[] sloj3B4_final = new double[N_nejron_in];

            Job_Net job_net = new Job_Net(N_nejron_in, 300, 80);
            job_net.Read_In_File_Bias_1("Сеть100/bias0B2.txt");
            job_net.Read_In_File_Bias_2("Сеть100/bias1B2.txt");

            job_net.Read_In_File_Weight_1("Сеть100/kernel0B2.txt");
            job_net.Read_In_File_Weight_2("Сеть100/kernel1B2.txt");

            /////////////////////////////////////////////
            Job_Net job_net2 = new Job_Net(N_nejron_in, 300, 100);
            job_net2.Read_In_File_Bias_1("Сеть100/bias0B3.txt");
            job_net2.Read_In_File_Bias_2("Сеть100/bias1B3.txt");

            job_net2.Read_In_File_Weight_1("Сеть100/kernel0B3.txt");
            job_net2.Read_In_File_Weight_2("Сеть100/kernel1B3.txt");

            /////////////////////////////////////
            Job_Net job_net3 = new Job_Net(N_nejron_in, 300, 110);
            job_net3.Read_In_File_Bias_1("Сеть100/bias0B4.txt");
            job_net3.Read_In_File_Bias_2("Сеть100/bias1B4.txt");

            job_net3.Read_In_File_Weight_1("Сеть100/kernel0B4.txt");
            job_net3.Read_In_File_Weight_2("Сеть100/kernel1B4.txt");



            //////////////////////////////////////////////////////

            for (int i = 0; i < ew; i++)
            {
                if (periods[i].Length < 240)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;
                }

                else
                {
                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[3, i] = row1[Period_job.Return_Length_x_Zero(i, 0 + shift_B1), 0]; // положение минимума В1 - начала отсчета
                    schet[10, i] = periods[i][0 + shift_B1];

                    double[] r1 = Function_additional.Get_One_Line(row01, i);

                    //////////////////////////////////////////////////////
                    //Нейронная сеть
                    ////////////////////////////////////////////////////////                   


                    sloj2B2 = job_net.Perzertron_Forward(r1, 1000, 300);
                    sloj3B2 = job_net.Perzertron_Forward_Softmax(sloj2B2, 300, 80);

                    sloj3B2_final = Function_additional.Layer_1000(sloj3B2, 37);

                    int B2 = Function_additional.Return_Max_Element_Neural_Network(sloj3B2_final);

                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[4, i] = row1[Period_job.Return_Length_x_Zero(i, B2), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B3 = job_net2.Perzertron_Forward(r1, N_nejron_in, 300);
                    sloj3B3 = job_net2.Perzertron_Forward_Softmax(sloj2B3, 300, 100);
                    sloj3B3_final = Function_additional.Layer_1000(sloj3B3, 128);

                    int B3 = Function_additional.Return_Max_Element_Neural_Network(sloj3B3_final);

                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = row1[Period_job.Return_Length_x_Zero(i, B3), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////
                    sloj2B4 = job_net3.Perzertron_Forward(r1, 1000, 300);
                    sloj3B4 = job_net3.Perzertron_Forward_Softmax(sloj2B4, 300, 110);


                    sloj3B4_final = Function_additional.Layer_1000(sloj3B4, 165);

                    int B4 = Function_additional.Return_Max_Element_Neural_Network(sloj3B4_final);

                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[8, i] = row1[Period_job.Return_Length_x_Zero(i, B4), reg] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////

                }

            }

            spec_point = schet;
        }

        /// <summary>
        /// Рассчитать особые точки используя статистику
        /// </summary>
        public void Return_Special_Point_Statistic()
        {
            long[][] periods = Period_job.Get_Period();

            long periods_full_length = Period_job.Return_Period_In_Data_Length();

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек

            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;
           
            for (int i = 0; i < ew; i++)
            {

                if (periods[i].Length < 200)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }
                else
                {

                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[3, i] = row1[Period_job.Return_Length_x_Zero(i, 0 + shift_B1), 0]; // положение минимума В1 - начала отсчета
                    schet[10, i] = periods[i][0 + shift_B1];

                    int B2 = Methods_Statistics.Statistic_Point_B2(periods[i].Length);
                    int B3 = Methods_Statistics.Statistic_Point_B3(periods[i].Length);
                    int B4 = Methods_Statistics.Statistic_Point_B4(periods[i].Length);

                    schet[4, i] = periods[i][B2] - periods[i][0 + shift_B1]; // максимум В2
                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]

                    schet[6, i] = periods[i][B3] - periods[i][0 + shift_B1]; // минимум В3
                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение минимума В3 - начала отсчета- EKG_max_x[w]

                    schet[8, i] = periods[i][B4] - periods[i][0 + shift_B1]; // максимум В4
                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В4 - начала отсчета - EKG_max_x[w]


                }

            }

            spec_point = schet;

        }

        /// <summary>
        /// Рассчитать особые точки используя статистику и дополнительные расчеты
        /// </summary>
        public void Return_Special_Point_Statistic_Num()
        {
            long[][] periods = Period_job.Get_Period();

            long periods_full_length = Period_job.Return_Period_In_Data_Length();

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек


            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;

            for (int i = 0; i < ew; i++)
            {

                if (periods[i].Length <= 150)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }
                else
                {

                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[3, i] = row1[Period_job.Return_Length_x_Zero(i, 0 + shift_B1), 0]; // положение минимума В1 - начала отсчета
                    schet[10, i] = periods[i][0 + shift_B1];

                    int B2 = Methods_Statistics.Statistic_Point_B2(periods[i].Length);
                    int B3 = Methods_Statistics.Statistic_Point_B3(periods[i].Length);
                    int B4 = Methods_Statistics.Statistic_Point_B4(periods[i].Length);

                    schet[4, i] = periods[i][B2] - periods[i][0 + shift_B1]; // максимум В2
                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]

                    schet[6, i] = periods[i][B3] - periods[i][0 + shift_B1]; // минимум В3
                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение минимума В3 - начала отсчета- EKG_max_x[w]

                    schet[8, i] = periods[i][B4] - periods[i][0 + shift_B1]; // максимум В4
                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В4 - начала отсчета - EKG_max_x[w]

                }

            }

            for (int i = 0; i < ew; i++)
            {

                if (periods[i].Length > 150)
                {
                    int B2 = Methods_Statistics.Statistic_Point_B2(periods[i].Length);
                    int B3 = Methods_Statistics.Statistic_Point_B3(periods[i].Length);
                    int B4 = Methods_Statistics.Statistic_Point_B4(periods[i].Length);
                    //В2
                    long max_B2 = periods[i][B2];
                    int coor_B2 = B2;
                    int coor_B2_shift;
                    if (B2 >= 35)
                    {
                        coor_B2_shift = 35;
                    }
                    else
                    {
                        coor_B2_shift = B2;
                    }
                    for (int j = B2 - coor_B2_shift; j < B2 + coor_B2_shift; j++)
                    {

                        if (max_B2 < periods[i][j])
                        {
                            max_B2 = periods[i][j];
                            coor_B2 = j;
                        }
                    }

                    B2 = coor_B2;
                    schet[4, i] = periods[i][B2] - periods[i][0 + shift_B1]; // максимум В2
                    schet[5, i] = row1[Period_job.Return_Length_x_Zero(i, B2), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]


                    ////////////////////////////////////////////////////////////////////
                    //В4
                    long max_B4 = periods[i][B4];
                    int coor_B4 = B4;
                    int coor_B4_shift;
                    if (B4 >= 45)
                    {
                        coor_B4_shift = 45;
                    }
                    else
                    {
                        coor_B4_shift = B4;
                    }

                    if ((B4 + coor_B4_shift) > periods[i].Length)
                    {

                        coor_B4_shift = periods[i].Length - B4 - 2;
                    }

                    double[] diff_B4 = new double[coor_B4_shift + 1];

                    for (int d = 0; d < diff_B4.Length - 1; d++)
                    {
                        diff_B4[d] = periods[i][B4 - coor_B4_shift + 1 + d] - periods[i][B4 - coor_B4_shift + d];

                    }

                    double diff_b4max = diff_B4[0];
                    int d1 = 0;
                    for (int j = B4; j < B4 + coor_B4_shift; j++)
                    {
                        d1++;
                        if (diff_b4max < diff_B4[d1])
                        {
                            diff_b4max = diff_B4[d1];
                            max_B4 = periods[i][j];
                            coor_B4 = j;
                        }
                    }

                    B4 = coor_B4;
                    schet[8, i] = periods[i][B4] - periods[i][0 + shift_B1]; // максимум В2
                    schet[9, i] = row1[Period_job.Return_Length_x_Zero(i, B4), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]


                    /////////////////////////////////////////////////////////////////////
                    //В3
                    //Ищем особые точки точнее
                    B3 = B4 - 32;
                    if (B3 < 0)
                    {
                        B3 = 0;
                    }
                    long min_B3 = periods[i][B3];
                    int coor_B3 = B3;

                    int coor_B3_shift;
                    if (B3 >= 14)
                    {
                        coor_B3_shift = 14;
                    }
                    else
                    {
                        coor_B3_shift = B3;
                    }

                    for (int j = B3 - coor_B3_shift; j < B3 + coor_B3_shift; j++)
                    {

                        if (min_B3 > periods[i][j])
                        {
                            min_B3 = periods[i][j];
                            coor_B3 = j;
                        }
                    }

                    B3 = coor_B3;
                    schet[6, i] = periods[i][B3] - periods[i][0 + shift_B1]; // максимум В2
                    schet[7, i] = row1[Period_job.Return_Length_x_Zero(i, B3), 0]; // положение максимума В2 - начала отсчета - EKG_max_x[w]

                }
            }
            spec_point = schet;
        }

        /// <summary>
        /// Рассчитать особые точки используя статистику 
        /// </summary>
        public void Return_Special_Point_Statistic_Num_2()
        {
            long[][] periods = Period_job.Get_Period();

            long periods_full_length = Period_job.Return_Period_In_Data_Length();

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек


            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;

            StreamWriter rwx = new StreamWriter("Положение особых точек.txt");

            for (int i = 0; i < ew; i++)
            {

                if (periods[i].Length <= 150)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }
                else
                {

                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[10, i] = periods[i][0 + shift_B1];

                    int B2 = Methods_Statistics.Statistic_Point_B2(periods[i].Length);
                    int B3 = Methods_Statistics.Statistic_Point_B3(periods[i].Length);
                    int B4 = Methods_Statistics.Statistic_Point_B4(periods[i].Length);

                    schet[4, i] = periods[i][B2] - periods[i][0 + shift_B1]; // максимум В2

                    schet[6, i] = periods[i][B3] - periods[i][0 + shift_B1]; // минимум В3

                    schet[8, i] = periods[i][B4] - periods[i][0 + shift_B1]; // максимум В4

                }

            }

            for (int i = 0; i < ew; i++)
            {

                if (periods[i].Length > 150)
                {
                    int B2 = Methods_Statistics.Statistic_Point_B2(periods[i].Length);
                    int B3 = Methods_Statistics.Statistic_Point_B3(periods[i].Length);
                    int B4 = Methods_Statistics.Statistic_Point_B4(periods[i].Length);
                    //В2
                    long max_B2 = periods[i][B2];
                    int coor_B2 = B2;
                    int coor_B2_shift;
                    if (B2 >= 35)
                    {
                        coor_B2_shift = 35;
                    }
                    else
                    {
                        coor_B2_shift = B2;
                    }
                    for (int j = B2 - coor_B2_shift; j < B2 + coor_B2_shift; j++)
                    {

                        if (max_B2 < periods[i][j])
                        {
                            max_B2 = periods[i][j];
                            coor_B2 = j;
                        }
                    }

                    B2 = coor_B2;
                    schet[4, i] = periods[i][B2] - periods[i][0 + shift_B1]; // максимум В2


                    ////////////////////////////////////////////////////////////////////
                    //В4
                    long max_B4 = periods[i][B4];
                    int coor_B4 = B4;
                    int coor_B4_shift;
                    if (B4 >= 45)
                    {
                        coor_B4_shift = 45;
                    }
                    else
                    {
                        coor_B4_shift = B4;
                    }

                    if ((B4 + coor_B4_shift) > periods[i].Length)
                    {

                        coor_B4_shift = periods[i].Length - B4 - 2;
                    }

                    double[] diff_B4 = new double[coor_B4_shift + 1];

                    for (int d = 0; d < diff_B4.Length - 1; d++)
                    {
                        diff_B4[d] = periods[i][B4 - coor_B4_shift + 1 + d] - periods[i][B4 - coor_B4_shift + d];

                    }

                    double diff_b4max = diff_B4[0];
                    int d1 = 0;
                    for (int j = B4; j < B4 + coor_B4_shift; j++)
                    {
                        d1++;
                        if (diff_b4max < diff_B4[d1])
                        {
                            diff_b4max = diff_B4[d1];
                            max_B4 = periods[i][j];
                            coor_B4 = j;
                        }
                    }

                    B4 = coor_B4;
                    schet[8, i] = periods[i][B4] - periods[i][0 + shift_B1]; // максимум В2


                    /////////////////////////////////////////////////////////////////////
                    //В3
                    //Ищем особые точки точнее
                    B3 = B4 - 32;
                    if (B3 < 0)
                    {
                        B3 = 0;
                    }
                    long min_B3 = periods[i][B3];
                    int coor_B3 = B3;

                    int coor_B3_shift;
                    if (B3 >= 14)
                    {
                        coor_B3_shift = 14;
                    }
                    else
                    {
                        coor_B3_shift = B3;
                    }

                    for (int j = B3 - coor_B3_shift; j < B3 + coor_B3_shift; j++)
                    {

                        if (min_B3 > periods[i][j])
                        {
                            min_B3 = periods[i][j];
                            coor_B3 = j;
                        }
                    }

                    B3 = coor_B3;
                    schet[6, i] = periods[i][B3] - periods[i][0 + shift_B1]; // максимум В2

                    rwx.WriteLine(i + "\t" + periods[i].Length + "\t" + B2 + "\t" + B3 + "\t" + B4);


                }
            }
            spec_point = schet;
            rwx.Close();
        }

        /// <summary>
        /// Рассчитать особые точки используя нейронную сеть 1000-100-35 с пересчетом диапазона
        /// </summary>
        public void Return_Special_Point_Neural_Network_2()
        {
            StreamWriter rwx = new StreamWriter("Положение особых точек.txt");

            long[][] periods = Period_job.Get_Period();//Конвертируем выбранную поток с рег в массив периодов

            long periods_full_length = Period_job.Return_Period_In_Data_Length();

            long[,] periods_1000 = Period_job.Return_Periods_1000();//Добавляем 0 в массиве до одинаковой длины=1000

            int ew = periods.Length;//счетчик найденных максимумов

            long[,] osob_x = new long[14, ew];// список особых точек для вывода на график
            long[,] osob_y = new long[14, ew];

            long[,] schet = new long[15, ew];// список особых точек для расчета (должны отличаться!!!!!)
            long[] schet_sum = new long[15];// список особых точек

            int N_nejron_in = 1000;

            long[,] row1 = initial_data.Get_row1();
            int reg = initial_data.REG;

            double[,] row0001 = Function_additional.Convert_Long_Double(periods_1000, ew, N_nejron_in);

            double[,] row01 = Function_additional.Calculate_Derivative_Array(row0001, ew, N_nejron_in);



            double[] sloj2B2 = new double[100];
            double[] sloj3B2 = new double[35];

            double[] sloj2B3 = new double[100];
            double[] sloj3B3 = new double[45];

            double[] sloj2B4 = new double[100];
            double[] sloj3B4 = new double[40];

            double[] sloj3B2_final = new double[N_nejron_in];
            double[] sloj3B3_final = new double[N_nejron_in];
            double[] sloj3B4_final = new double[N_nejron_in];

            Job_Net job_net = new Job_Net(N_nejron_in, 100, 35);
            job_net.Read_In_File_Bias_1("Сеть1/bias0B2.txt");
            job_net.Read_In_File_Bias_2("Сеть1/bias1B2.txt");

            job_net.Read_In_File_Weight_1("Сеть1/kernel0B2.txt");
            job_net.Read_In_File_Weight_2("Сеть1/kernel1B2.txt");

            /////////////////////////////////////////////
            Job_Net job_net2 = new Job_Net(N_nejron_in, 100, 45);
            job_net2.Read_In_File_Bias_1("Сеть1/bias0B3.txt");
            job_net2.Read_In_File_Bias_2("Сеть1/bias1B3.txt");

            job_net2.Read_In_File_Weight_1("Сеть1/kernel0B3.txt");
            job_net2.Read_In_File_Weight_2("Сеть1/kernel1B3.txt");

            /////////////////////////////////////
            Job_Net job_net3 = new Job_Net(N_nejron_in, 100, 40);
            job_net3.Read_In_File_Bias_1("Сеть1/bias0B4.txt");
            job_net3.Read_In_File_Bias_2("Сеть1/bias1B4.txt");

            job_net3.Read_In_File_Weight_1("Сеть1/kernel0B4.txt");
            job_net3.Read_In_File_Weight_2("Сеть1/kernel1B4.txt");

            //////////////////////////////////////////////////////


            for (int i = 0; i < ew; i++)
            {
                if (periods[i].Length < 240)
                {
                    schet[2, i] = 0;// минимум В1
                    schet[3, i] = 0;// положение минимума В1 - начала отсчета
                    schet[4, i] = 0;// максимум В2
                    schet[5, i] = 0;// положение максимума В2 - начала отсчета - EKG_max_x[w]
                    schet[6, i] = 0;// минимум В3
                    schet[7, i] = 0;// положение минимума В3 - начала отсчета- EKG_max_x[w]
                    schet[8, i] = 0;// максимум В4
                    schet[9, i] = 0;// положение максимума В4 - начала отсчета - EKG_max_x[w]
                    schet[10, i] = 0;

                }

                else
                {
                    schet[2, i] = periods[i][0 + shift_B1]; // минимум В1
                    schet[10, i] = periods[i][0 + shift_B1];

                    double[] r1 = Function_additional.Get_One_Line(row01, i);

                    //////////////////////////////////////////////////////
                    //Нейронная сеть
                    ////////////////////////////////////////////////////////                   


                    sloj2B2 = job_net.Perzertron_Forward(r1, 1000, 100);
                    sloj3B2 = job_net.Perzertron_Forward_Softmax(sloj2B2, 100, 35);
                    sloj3B2_final = Function_additional.Layer_1000(sloj3B2, 65);

                    int B2 = Function_additional.Return_Max_Element_Neural_Network(sloj3B2_final);



                    /////////////////////////////////////////////////////////
                    sloj2B3 = job_net2.Perzertron_Forward(r1, N_nejron_in, 100);
                    sloj3B3 = job_net2.Perzertron_Forward_Softmax(sloj2B3, 100, 45);
                    sloj3B3_final = Function_additional.Layer_1000(sloj3B3, 170);

                    int B3 = Function_additional.Return_Max_Element_Neural_Network(sloj3B3_final);



                    /////////////////////////////////////////////////////////
                    sloj2B4 = job_net3.Perzertron_Forward(r1, 1000, 100);
                    sloj3B4 = job_net3.Perzertron_Forward_Softmax(sloj2B4, 100, 40);
                    sloj3B4_final = Function_additional.Layer_1000(sloj3B4, 200);

                    int B4 = Function_additional.Return_Max_Element_Neural_Network(sloj3B4_final);


                    rwx.WriteLine(i + "\t" + periods[i].Length + "\t" + B2 + "\t" + B3 + "\t" + B4);

                    /////////////////////////////////////////////////////////
                }

            }
            spec_point = schet;

            rwx.Close();

        }

        /// <summary>
        /// Рассчитать сдвиг особых точек
        /// </summary>
        /// <param name="sche"></param>
        /// <param name="ew"></param>
        /// <returns></returns>
        public long[,] Shift_Special_Point(long[,] sche, long ew)
        {
            long[,] schet = sche;
           
            for (int w = 0; w < ew; w++)
            {
                if (w > 2)
                {   
                    schet[4, w - 1] = schet[4, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[5, w - 1], schet[2, w - 1], schet[2, w]);// максимум В2
                    schet[6, w - 1] = schet[6, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[7, w - 1], schet[2, w - 1], schet[2, w]); // минимум В3
                    schet[8, w - 1] = schet[8, w - 1] - Shift_BX(schet[3, w - 1], schet[3, w], schet[9, w - 1], schet[2, w - 1], schet[2, w]);// максимум В4
                }
            }

            return schet;
        }


        public long Shift_BX(long x1, long x2, long x3_b, long y1, long y2)
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
