using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood.Initial_processing
{
    public class Reader_data
    {
        const int time_numerical = 60;
        private String name;

        public Reader_data(String s)
        {
            this.name = s;
        }


        public long[,] Return_Read_Massiv() //Считывает файл и записывает в массив (только цифры)
        {
            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10

            int k = 0;//счетчик столбцов 2


            int m = 0;//смещение буффера

            int potok = 5;
            int potok2 = 12;
            int ckor = time_numerical * (230400 / 400) * (10 + potok2 * potok / 3);

            long[,] row11 = new long[ckor, 1 + potok2];

            StreamReader sw = new StreamReader(name);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    // rw2.WriteLine(n1);
                    row11[j, k] = System.Convert.ToInt64(n1);// пишем в массив
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    row11[j, k] = System.Convert.ToInt64(n1);// пишем в массив
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                }


            }
            sw.Close();
            return row11;

        }

        public int Return_Read_Strings() // Выдает число строк в считанном файле
        {
            //  StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк


            int l1;
            int j = 0;// счетчик строк 10

            int k = 0;//счетчик столбцов 2


            int m = 0;//смещение буффера

            int potok = 5;
            int potok2 = 12;
            int ckor = time_numerical * (230400 / 400) * (10 + potok2 * potok / 3);

            long[,] row11 = new long[ckor, 1 + potok2];

            StreamReader sw = new StreamReader(name);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57)
                {
                    m++;
                }
                else
                {
                    a++;
                }
            }
            sw.Close();
            return b;

        }

        public long[,] Return_Read_Massiv_Special_Point(int ckor) //Считывает файл и записывает в массив (только цифры)
        {
            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10

            int k = 0;//счетчик столбцов 2


            int m = 0;//смещение буффера

            int potok2 = 12;
            //  int ckor = time_numerical * (230400 / 400) * (10 + potok2 * potok / 3);

            long[,] row11 = new long[1 + potok2, ckor];

            long[,] row22 = new long[1 + potok2, ckor];

            StreamReader sw = new StreamReader(name);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    // rw2.WriteLine(n1);
                    row11[k, j] = System.Convert.ToInt64(n1);// пишем в массив
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    row11[k, j] = System.Convert.ToInt64(n1);// пишем в массив
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                }


            }
            sw.Close();


            for (int i = 0; i < ckor; i++)
            {
                row22[1, i] = row11[0, i];
                row22[0, i] = row11[1, i];
                row22[3, i] = row11[2, i];
                row22[2, i] = row11[3, i];
                row22[5, i] = row11[4, i];
                row22[4, i] = row11[5, i];
                row22[7, i] = row11[6, i];
                row22[6, i] = row11[7, i];
                row22[9, i] = row11[8, i];
                row22[8, i] = row11[9, i];
                row22[10, i] = row11[10, i];

            }

            return row22;

        }


        public long[][] Return_Read_Massiv_Divided_Data() //Считывает файл и записывает в массив (только цифры)
        {
            StringBuilder buffer = new StringBuilder();
            int a = 0;
            int b = 0;//счетчик строк

            string n1;

            int l1;
            int j = 0;// счетчик строк 10

            int k = 0;//счетчик столбцов 2


            int m = 0;//смещение буффера

            int ckor = 1363;

            long[][] row11 = new long[ckor][];
            for (int i = 0; i < ckor; i++)
            {
                row11[i] = new long[1000];
            }


            StreamReader sw = new StreamReader(name);

            while (sw.Peek() != -1)
            {
                l1 = sw.Read();


                if (l1 == 13)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                                                 // rw2.WriteLine(n1);
                    if (n1 != "")
                    {
                        row11[j][k] = System.Convert.ToInt64(n1);// пишем в массив
                    }
                    j++; // переходим на следующую строку
                    k = 0; // переходим на первый столбец
                    m = 0;
                    b++;
                }
                if (l1 == 9)
                {
                    n1 = buffer.ToString(); // пищем цифру в строку
                    buffer.Remove(0, n1.Length); //очищаем буффер
                    row11[j][k] = System.Convert.ToInt64(n1);// пишем в массив
                    k++; // переходим на следующий столбец
                    m = 0;
                }
                if (l1 == 48 || l1 == 49 || l1 == 50 || l1 == 51 || l1 == 52 || l1 == 53 || l1 == 54 || l1 == 55 || l1 == 56 || l1 == 57)
                {
                    buffer.Insert(m, System.Convert.ToChar(l1)); // пишем символ
                    m++;
                }
                else
                {
                    a++;
                }


            }
            sw.Close();
            return row11;

        }

    }



}
