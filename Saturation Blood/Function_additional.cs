using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    static class Function_additional
    {
        public static double[,] Convert_Long_Double(long[,] sloj, int x, int y)
        {
            double[,] rowx = new double[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    rowx[i, j] = System.Convert.ToDouble(sloj[i, j]);
                }
            }

            return rowx;
        }

        public static double[,] Proizvodnaja_Massiv(double[,] sloj, int x, int y)
        {
            double[,] rowx = new double[x, y];

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y - 1; j++)
                {
                    rowx[i, j] = sloj[i, j + 1] - sloj[i, j];
                }
            }
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (rowx[i, j] < -100)
                    {
                        rowx[i, j] = 0;
                    }
                }
            }

            return rowx;

        }

        public static double[] Get_One_Line_1024(double[,] sloj, int x, int N_nejron)
        {
            double[] y = new double[N_nejron];

            for (int j = 0; j < N_nejron; j++)
            {
                y[j] = sloj[x, j] / 1024;
            }

            return y;

        }

        public static double[] Get_One_Line(double[,] sloj, int x)
        {
            double[] y = new double[1000];

            for (int j = 0; j < 1000; j++)
            {
                y[j] = sloj[x, j];
            }

            return y;

        }

        public static int Found_Max(double[] layer, int x)
        {
            double max_y = layer[1];
            int max_x = 1;

            for (int i = x; i < x + 10; i++)
            {
                if (max_y < layer[i])
                {
                    max_y = layer[i];
                    max_x = i;
                }
            }
            return max_x;
        }

        public static int Found_Min(double[] layer, int x)
        {
            double min_y = layer[1];
            int min_x = 1;

            for (int i = x; i < x + 10; i++)
            {
                if (min_y > layer[i])
                {
                    min_y = layer[i];
                    min_x = i;
                }
            }
            return min_x;
        }

        public static int Return_Max_Element_Neural_Network(double[] layer)
        {
            double max = 0;
            int a = 0;

            if (layer.Length < 500)
            {
                for (int i = 0; i < layer.Length; i++)
                {
                    if (max < layer[i])
                    {
                        max = layer[i];
                        a = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 500; i++)
                {
                    if (max < layer[i])
                    {
                        max = layer[i];
                        a = i;
                    }
                }
            }
            return a;
        }

        public static double[] Layer_1000(double[] result_NS, int number)
        {
            double[] result = new double[1000];

            for (int i = 0; i < 1000; i++)
            {
                result[i] = 0;
            }

            for (int i = number; i < (number + result_NS.Length); i++)
            {
                result[i] = result_NS[i - number];
            }

            return result;

        }

        public static double[] Multiple_Ten_B2_B4(double[] layer, double[] row, int x)
        {

            double[] y = new double[layer.Length * 10];
            int z = 0;
            double layer_max = layer[0];


            for (int i = 0; i < layer.Length; i++)
            {
                if (layer_max < layer[i])
                {
                    layer_max = layer[i];
                    z = i;
                }
            }
            int coor_0 = z * 10;
            int coor_1 = Found_Max(row, coor_0);

            for (int i = 0; i < y.Length; i++)
            {
                if (i == coor_1)
                {
                    y[i] = 1;
                }
            }

            return y;

        }

        public static long[,] Set_Const_Special_Point_from_Diff(Initial_Data init_data, long[,] osob_point_diff, int number_canal_const)
        {
            int length_osob_point_diff = osob_point_diff.Length / 15;
            long[,] osob_point_const = new long[15, length_osob_point_diff];

            for (int i = 0; i < length_osob_point_diff; i++)
            {
                long time_B1 = osob_point_diff[3, i];
                int position_B1 = init_data.Find_Position_in_Time(time_B1);
                osob_point_const[3, i] = time_B1;
                osob_point_const[2, i] = init_data.Get_row1_x_y(position_B1, number_canal_const);
                osob_point_const[10, i] = osob_point_const[2, i];

                long time_B2 = osob_point_diff[5, i];
                int position_B2 = init_data.Find_Position_in_Time(time_B2);
                osob_point_const[5, i] = time_B2;
                osob_point_const[4, i] = init_data.Get_row1_x_y(position_B2, number_canal_const)- osob_point_const[10, i];

                long time_B3 = osob_point_diff[7, i];
                int position_B3 = init_data.Find_Position_in_Time(time_B3);
                osob_point_const[7, i] = time_B3;
                osob_point_const[6, i] = init_data.Get_row1_x_y(position_B3, number_canal_const)- osob_point_const[10, i];

                long time_B4 = osob_point_diff[9, i];
                int position_B4 = init_data.Find_Position_in_Time(time_B4);
                osob_point_const[9, i] = time_B4;
                osob_point_const[8, i] = init_data.Get_row1_x_y(position_B4, number_canal_const)- osob_point_const[10, i];
            }
            return osob_point_const;
        
        }


        public static double[] Multiple_Ten_B3(double[] layer, double[] row, int x)
        {

            double[] y = new double[layer.Length * 10];
            int z = 0;
            double layer_max = layer[0];


            for (int i = 0; i < layer.Length; i++)
            {
                if (layer_max > layer[i])
                {
                    layer_max = layer[i];
                    z = i;
                }
            }
            int coor_0 = z * 10;
            int coor_1 = Found_Min(row, coor_0);

            for (int i = 0; i < y.Length; i++)
            {
                if (i == coor_1)
                {
                    y[i] = 1;
                }
            }

            return y;

        }

    }
}
