using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    class Job_Net
    {
        private double[,] weight_1;
        private double[,] weight_2;

        private double[] bias0;
        private double[] bias1;

        int razmer_data_in;
        int razmer_layer_1_in;
        int razmer_layer_2_in;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="razmer1">Дляна входного вектора</param>
        /// <param name="razmer2">Длина внутреннего вектора</param>
        /// <param name="razmer3">Длина выходного вектора</param>
        public Job_Net(int razmer1, int razmer2, int razmer3)
        {

            this.razmer_data_in = razmer1;
            this.razmer_layer_1_in = razmer2;
            this.razmer_layer_2_in = razmer3;

            weight_1 = new double[razmer_data_in, razmer_layer_1_in];
            weight_2 = new double[razmer_layer_1_in, razmer_layer_2_in];

            bias0 = new double[razmer_layer_1_in];
            bias1 = new double[razmer_layer_2_in];


        }
        /// <summary>
        /// Прочитать из файла смещения для первого слоя
        /// </summary>
        /// <param name="name_file">Имя файла</param>

        public void Read_In_File_Bias_1(String name_file)
        {

            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10  
            int m = 0;//смещение буффера

            StreamReader sw = new StreamReader(name_file);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку

                    buffer.Remove(0, n1.Length); //очищаем буффер

                    if (n1 != "")
                    {
                        bias0[j] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    j++; // переходим на следующую строку

                    m = 0;
                    b++;
                }

                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57 || l1 == 46)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                    continue;
                }

            }

            sw.Close();

        }
        /// <summary>
        /// Прочитать из файла смещения для второго слоя
        /// </summary>
        /// <param name="name_file">Имя файла</param>
        public void Read_In_File_Bias_2(String name_file)
        {

            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10   
            int m = 0;//смещение буффера

            StreamReader sw = new StreamReader(name_file);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер

                    if (n1 != "")
                    {
                        bias1[j] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    j++; // переходим на следующую строку

                    m = 0;
                    b++;
                }

                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57 || l1 == 46)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                    continue;
                }
            }
            sw.Close();

        }

        /// <summary>
        /// Прочитать из файла веса для первого слоя
        /// </summary>
        /// <param name="name_file">Имя файла</param>
        public void Read_In_File_Weight_1(String name_file)
        {
            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10
            int k = 0;//счетчик столбцов 2
            int m = 0;//смещение буффера

            long[,] rowx = new long[razmer_layer_1_in, razmer_data_in];

            StreamReader sw = new StreamReader(name_file);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();

                if (l1 == 13)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                                                 // rw2.WriteLine(n1);
                    if (n1 != "")
                    {
                        weight_1[j, k] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    if (n1 != "")
                    {
                        weight_1[j, k] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57 || l1 == 46)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                    continue;
                }
            }
            sw.Close();
        }

        /// <summary>
        /// Прочитать из файла веса для второг слоя
        /// </summary>
        /// <param name="name_file">Имя файла</param>
        public void Read_In_File_Weight_2(String name_file)
        {
            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10
            int k = 0;//счетчик столбцов 2
            int m = 0;//смещение буфера

            long[,] rowx = new long[razmer_layer_2_in, razmer_layer_1_in];

            StreamReader sw = new StreamReader(name_file);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();

                if (l1 == 13)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                                                 // rw2.WriteLine(n1);
                    if (n1 != "")
                    {
                        weight_2[j, k] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    buffer.Replace('.', ',');
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    if (n1 != "")
                    {
                        weight_2[j, k] = System.Convert.ToDouble(n1);// пишем в массив
                    }
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57 || l1 == 46)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                    continue;
                }

            }

            sw.Close();

        }

        /// <summary>
        /// Прямое распространение в перцептроне
        /// </summary>
        /// <param name="x">Входной вектор</param>
        /// <param name="data_in">Длина входного вектора</param>
        /// <param name="layer_1_in">Длина выходного вектора</param>
        /// <returns></returns>
        public double[] Perzertron_Forward(double[] x, int data_in, int layer_1_in)
        {
            double[] y = new double[layer_1_in];

            for (int i = 0; i < layer_1_in; i++)
            {
                for (int j = 0; j < data_in; j++)
                {
                    y[i] = y[i] + (weight_1[j, i] * x[j]);
                }
            }

            for (int i = 0; i < layer_1_in; i++)
            {
                y[i] = y[i] + bias0[i];
            }

            for (int i = 0; i < layer_1_in; i++)
            {

                y[i] = Sigmoid(y[i]);
            }

            return y;

        }
        /// <summary>
        /// Прямое распространение в перцептроне с функцией активации softmax
        /// </summary>
        /// <param name="x">Входной вектор</param>
        /// <param name="layer_1_in">Длина входного вектора</param>
        /// <param name="layer_2_in">Длина выходного вектора</param>
        /// <returns></returns>
        public double[] Perzertron_Forward_Softmax(double[] x, int layer_1_in, int layer_2_in)
        {
            double[] y = new double[layer_2_in];
            double[] z = new double[layer_2_in];


            for (int i = 0; i < layer_2_in; i++)
            {
                for (int j = 0; j < layer_1_in; j++)
                {
                    y[i] = y[i] + (weight_2[j, i] * x[j]);
                }
            }
            for (int i = 0; i < layer_2_in; i++)
            {
                y[i] = y[i] + bias1[i];
            }

            z = Softmax(y);        

            return z;

        }
        /// <summary>
        /// Функция активации Sigmoid
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Sigmoid(double x)
        {

            double y = 1 / (1 + Math.Exp((-1) * x));
            return y;
        }

        /// <summary>
        /// Функция активации Гиперболический тангенс
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Tanh(double x)
        {

            double y = (Math.Exp(x) - Math.Exp((-1) * x)) / (Math.Exp(x) + Math.Exp((-1) * x));
            return y;
        }

        /// <summary>
        /// Функция активации RELU
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double RELU(double x)
        {
            double y;
            if (x < 0) { y = 0; }
            else { y = x; }

            return y;
        }

        /// <summary>
        /// Функция активации Softmax
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double[] Softmax(double[] x)
        {
            double[] y = new double[x.Length];
            double maxi = x.Max();

            for (int i = 0; i < x.Length; i++)
            {
                y[i] = Math.Exp(x[i] - maxi);
            }
            double sum = y.Sum();

            for (int i = 0; i < x.Length; i++)
            {
                y[i] = y[i] / sum;

            }
            return y;
        }
    }
}
