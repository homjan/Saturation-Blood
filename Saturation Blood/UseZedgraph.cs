using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Saturation_Blood
{
    class UseZedgraph
    {
        const int potok2 = 13; //Общее Число потоков (работае только 4 + время)
        long shift_grafh = 200;
        long shift_grafh_ekg = 200;
        long maximum = 0;
        long minimum = 1024;

        private GraphPane pane;
        ZedGraphControl zedgraph;

        Initial_Data initdata;

        public UseZedgraph(ZedGraphControl zedd)
        {
            this.zedgraph = zedd;
            pane = zedd.GraphPane;

        }

        public UseZedgraph(ZedGraphControl zedd, Initial_Data data)
        {
            this.zedgraph = zedd;
            pane = zedd.GraphPane;

            this.initdata = data;

        }

      
        public void ClearGraph()
        {
            pane.CurveList.Clear();
        }

        public void MakeGraph_4_Kanal(long[,] xxx, int b)
        {
            PointPairList f1_list = new PointPairList();
            PointPairList f2_list = new PointPairList();
            PointPairList f3_list = new PointPairList();
            PointPairList f4_list = new PointPairList();

            for (int i = 3; i < b; i++)
            {
                f1_list.Add(xxx[i, 0] / 1000, xxx[i, 1]);
                f2_list.Add(xxx[i, 0] / 1000, xxx[i, 2]);
                f3_list.Add(xxx[i, 0] / 1000, xxx[i, 3]);
                f4_list.Add(xxx[i, 0] / 1000, xxx[i, 4]);
            }

            LineItem myCurve1 = pane.AddCurve("Канал1", f1_list, Color.Blue, SymbolType.None);
            LineItem myCurve2 = pane.AddCurve("Канал2", f2_list, Color.Red, SymbolType.None);
            LineItem myCurve3 = pane.AddCurve("Канал3", f3_list, Color.Green, SymbolType.None);
            LineItem myCurve4 = pane.AddCurve("Канал4", f4_list, Color.Black, SymbolType.None);

        }
        public void Install_Pane(String Xaxis, String Yaxis, String Title_text)
        {
            pane.Title.Text = Title_text;
            pane.XAxis.Title.Text = Xaxis;
            pane.YAxis.Title.Text = Yaxis;
        }

        public void MakeGraph_One_Function(long[,] xxx, long[] yyy, int b)
        {
            PointPairList f1_list = new PointPairList();           

            for (int i = 3; i < b; i++)
            {
                f1_list.Add(xxx[i,0] / 1000, yyy[i]);               
            }

            LineItem myCurve1 = pane.AddCurve("Канал1", f1_list, Color.Blue, SymbolType.None);          

        }


        public void MakeGraph_of_Chosen_Kanal(int number_Kanal) {

            long[,] row_1 = initdata.Get_row1();
           
            int b = initdata.Get_b();


            for (int y = 100; y < b - 10; y++)
            {
                if (maximum < row_1[y, number_Kanal])
                {
                    maximum = row_1[y, number_Kanal];
                }

                if (minimum > row_1[y, number_Kanal])
                {
                    minimum = row_1[y, number_Kanal];
                }

            }

            if ((maximum - minimum) < 200)
            {
                shift_grafh = 200;
                shift_grafh_ekg = 200;
            }
            else if ((maximum - minimum) > 500)
            {
                shift_grafh = -500;
                shift_grafh_ekg = 400;
            }
            else if ((maximum - minimum) > 1000)
            {
                shift_grafh = -5500;
                shift_grafh_ekg = 5500;
            }
            else
            {
                shift_grafh_ekg = 200;
                shift_grafh = -300;
            }


            // Создадим список точек для кривой f1(x)
            PointPairList f1_list = new PointPairList();
          

            // Заполним массив точек для кривой f1-3(x)
            for (int y = 3; y < b - 10; y++)
            {    
                f1_list.Add(row_1[y, 0] / 1000, row_1[y, number_Kanal]);               
            }

            pane.XAxis.Title.Text = "t, мc";
            pane.YAxis.Title.Text = "R, Ом";
            pane.Title.Text = "Данные";

            LineItem f1_curve = pane.AddCurve("ФПГ", f1_list, Color.Blue, SymbolType.None);
           

        }

        public void MakeGraph_On_Canal_FPG()
        {

            long[,] row_1 = initdata.Get_row1();
            long[] row_3 = initdata.Get_row3();
            long[] row_4 = initdata.Get_row4();
            int b = initdata.Get_b();


            for (int y = 100; y < b - 10; y++)
            {
                if (maximum < row_1[y, initdata.REG])
                {
                    maximum = row_1[y, initdata.REG];
                }

                if (minimum > row_1[y, initdata.REG])
                {
                    minimum = row_1[y, initdata.REG];
                }

            }

            if ((maximum - minimum) < 200)
            {
                shift_grafh = 200;
                shift_grafh_ekg = 200;
            }
            else if ((maximum - minimum) > 500)
            {
                shift_grafh = -500;
                shift_grafh_ekg = 400;
            }
            else if ((maximum - minimum) > 1000)
            {
                shift_grafh = -5500;
                shift_grafh_ekg = 5500;
            }
            else
            {
                shift_grafh_ekg = 200;
                shift_grafh = -300;
            }


            // Создадим список точек для кривой f1(x)
            PointPairList f1_list = new PointPairList();
            PointPairList f2_list = new PointPairList();
            PointPairList f3_list = new PointPairList();
            PointPairList f4_list = new PointPairList();
            PointPairList f5_list_diff = new PointPairList();

            // Заполним массив точек для кривой f1-3(x)
            for (int y = 3; y < b - 10; y++)
            {
                f1_list.Add(row_1[y, 0] / 1000, row_4[y] + (shift_grafh_ekg));
                f2_list.Add(row_1[y, 0] / 1000, 570);
                f3_list.Add(row_1[y, 0] / 1000, shift_grafh);

                f4_list.Add(row_1[y, 0] / 1000, row_1[y, initdata.REG]);
                f5_list_diff.Add(row_1[y, 0] / 1000, row_3[y] / 10 + shift_grafh);
            }

            pane.XAxis.Title.Text = "t, мc";
            pane.YAxis.Title.Text = "R, Ом";
            pane.Title.Text = "Данные";

            LineItem f1_curve = pane.AddCurve("ЭКГ", f1_list, Color.Blue, SymbolType.None);
            LineItem f2_curve = pane.AddCurve("", f2_list, Color.Black, SymbolType.None);
            LineItem f3_curve = pane.AddCurve("", f3_list, Color.Black, SymbolType.None);
            LineItem f4_curve = pane.AddCurve(" РЭГ", f4_list, Color.Red, SymbolType.None);
            LineItem f5_curve_diff = pane.AddCurve("Производная РЭГ", f5_list_diff, Color.Green, SymbolType.None);


        }

        public void MakeGraph_Special_Point(long[,] osob_x, long[,] osob_y, int ew)
        {

            // Выводим точки на экран
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();
            PointPairList list7 = new PointPairList();
            PointPairList list8 = new PointPairList();
            PointPairList list9 = new PointPairList();

            for (int w = 2; w < ew - 2; w++)
            {
                /*   for (int i = 0; i < 14; i++)
                   {
                       list4.Add(osob_x[i, w] / 1000, osob_y[i, w]);
                   }*/
                list5.Add(osob_x[1, w] / 1000, osob_y[1, w]);
                list6.Add(osob_x[2, w] / 1000, osob_y[2, w]);
                list7.Add(osob_x[3, w] / 1000, osob_y[3, w]);
                list8.Add(osob_x[4, w] / 1000, osob_y[4, w]);

                list9.Add(osob_x[0, w] / 1000, osob_y[0, w] + (shift_grafh_ekg));


            }
            LineItem myCurve5 = pane.AddCurve("B1", list5, Color.Blue, SymbolType.Diamond);
            LineItem myCurve6 = pane.AddCurve("B2", list6, Color.Black, SymbolType.Diamond);
            LineItem myCurve7 = pane.AddCurve("B3", list7, Color.DarkRed, SymbolType.Diamond);
            LineItem myCurve8 = pane.AddCurve("B4", list8, Color.Green, SymbolType.Diamond);
            LineItem myCurve9 = pane.AddCurve("ЭКГ", list9, Color.Brown, SymbolType.Diamond);


            // !!!
            // У кривой линия будет невидимой
            myCurve5.Line.IsVisible = false;
            myCurve6.Line.IsVisible = false;
            myCurve7.Line.IsVisible = false;
            myCurve8.Line.IsVisible = false;
            myCurve9.Line.IsVisible = false;

            // !!!
            // Цвет заполнения отметок (ромбиков) - голубой
            myCurve5.Symbol.Fill.Color = Color.Blue;
            myCurve6.Symbol.Fill.Color = Color.Black;
            myCurve7.Symbol.Fill.Color = Color.DarkRed;
            myCurve8.Symbol.Fill.Color = Color.Green;
            myCurve9.Symbol.Fill.Color = Color.Brown;

            // !!!
            // Тип заполнения - сплошная заливка
            myCurve5.Symbol.Fill.Type = FillType.Solid;
            myCurve6.Symbol.Fill.Type = FillType.Solid;
            myCurve7.Symbol.Fill.Type = FillType.Solid;
            myCurve8.Symbol.Fill.Type = FillType.Solid;
            myCurve9.Symbol.Fill.Type = FillType.Solid;

            // !!!
            // Размер ромбиков
            myCurve5.Symbol.Size = 8;
            myCurve6.Symbol.Size = 8;
            myCurve7.Symbol.Size = 8;
            myCurve8.Symbol.Size = 8;
            myCurve9.Symbol.Size = 8;

            pane.YAxis.MajorGrid.IsZeroLine = false;

        }

        public void Make_Graph_Max_Min(long[,] osob_x, long[,] osob_y, int ew, string title_1, string title_2)
        {
            // Выводим точки на экран
            PointPairList list5 = new PointPairList();
            PointPairList list6 = new PointPairList();           

            for (int w = 2; w < ew - 2; w++)
            {               
                list5.Add(osob_x[1, w] / 1000, osob_y[1, w]);
                list6.Add(osob_x[2, w] / 1000, osob_y[2, w]); 
            }

            LineItem myCurve5 = pane.AddCurve(title_1, list5, Color.Blue, SymbolType.Diamond);
            LineItem myCurve6 = pane.AddCurve(title_2, list6, Color.Black, SymbolType.Diamond);
          
            // !!!
            // У кривой линия будет невидимой
            myCurve5.Line.IsVisible = false;
            myCurve6.Line.IsVisible = false;           

            // !!!
            // Цвет заполнения отметок (ромбиков) - голубой
            myCurve5.Symbol.Fill.Color = Color.Blue;
            myCurve6.Symbol.Fill.Color = Color.Black;
            
            // !!!
            // Тип заполнения - сплошная заливка
            myCurve5.Symbol.Fill.Type = FillType.Solid;
            myCurve6.Symbol.Fill.Type = FillType.Solid;
          
            // !!!
            // Размер ромбиков
            myCurve5.Symbol.Size = 8;
            myCurve6.Symbol.Size = 8;
           
            pane.YAxis.MajorGrid.IsZeroLine = false;

        }

        public void Make_Graph_Saturation(double[] Saturation_t, double[] Saturation_y1, double[] Saturation_y2, int ew)
        {
            PointPairList f_saturation = new PointPairList();
          //  PointPairList f_saturation2 = new PointPairList();

            for (int y = 3; y < ew - 1; y++)
            {
                f_saturation.Add(Saturation_t[y], Saturation_y1[y] * 100);                
            //    f_saturation2.Add(Saturation_t[y], Saturation_y2[y] * 100);
            }

            int axis2 = pane.AddYAxis("Степень оксигенации, %");

            LineItem f_satur = pane.AddCurve("Степень оксигенации %", f_saturation, Color.DarkGoldenrod, SymbolType.Circle);
         //   LineItem f_satur2 = pane.AddCurve("Степень оксигенации %", f_saturation2, Color.Green, SymbolType.Diamond);

            f_satur.YAxisIndex = axis2;          

            f_satur.Symbol.Fill.Type = FillType.Solid;
            f_satur.Symbol.Size = 8;

         //   f_satur2.Symbol.Fill.Type = FillType.Solid;
        //    f_satur2.Symbol.Size = 8;

           // pane.YAxisList[axis2].Title.FontSpec.FontColor = Color.Black;

            pane.YAxis.MajorGrid.IsZeroLine = false;

        }

        public void Make_Graph_Special_Point_B1_B2(long[,] srec_point)
        {

            long[,] osob_1 = srec_point;

            int N_Period_red_const = osob_1.Length / 15;//счетчик найденных максимумов

            //  osob_1 = osob_point_1.shift_osob_point(osob_1, N_Period_red_const);

            long[,] osob_x_red_const = new long[5, N_Period_red_const];// список особых точек для вывода на график
            long[,] osob_y_red_const = new long[5, N_Period_red_const];

            for (int i = 0; i < N_Period_red_const - 1; i++)
            {
                osob_x_red_const[0, i] = osob_1[1, i];
                osob_y_red_const[0, i] = osob_1[0, i];

                osob_x_red_const[1, i] = osob_1[3, i];
                osob_y_red_const[1, i] = osob_1[2, i];

                osob_x_red_const[2, i] = osob_1[5, i];
                osob_y_red_const[2, i] = osob_1[4, i] + osob_1[10, i];

                osob_x_red_const[3, i] = osob_1[7, i];
                osob_y_red_const[3, i] = osob_1[6, i] + osob_1[10, i];

                osob_x_red_const[4, i] = osob_1[9, i];
                osob_y_red_const[4, i] = osob_1[8, i] + osob_1[10, i];

            }

            // usergraph.makeGraph_osob_point(osob_x_red_const, osob_y_red_const, N_Period_red_const);
            Make_Graph_Max_Min(osob_x_red_const, osob_y_red_const, N_Period_red_const, "", "");



        }

        public void Make_Graph_Special_Point_All(long[,] srec_point)
        {

            long[,] osob_1 = srec_point;

            int N_Period_red_const = osob_1.Length / 15;//счетчик найденных максимумов

            //  osob_1 = osob_point_1.shift_osob_point(osob_1, N_Period_red_const);

            long[,] osob_x_red_const = new long[5, N_Period_red_const];// список особых точек для вывода на график
            long[,] osob_y_red_const = new long[5, N_Period_red_const];

            for (int i = 0; i < N_Period_red_const - 1; i++)
            {
                osob_x_red_const[0, i] = osob_1[1, i];
                osob_y_red_const[0, i] = osob_1[0, i];

                osob_x_red_const[1, i] = osob_1[3, i];
                osob_y_red_const[1, i] = osob_1[2, i];

                osob_x_red_const[2, i] = osob_1[5, i];
                osob_y_red_const[2, i] = osob_1[4, i] + osob_1[10, i];

                osob_x_red_const[3, i] = osob_1[7, i];
                osob_y_red_const[3, i] = osob_1[6, i] + osob_1[10, i];

                osob_x_red_const[4, i] = osob_1[9, i];
                osob_y_red_const[4, i] = osob_1[8, i] + osob_1[10, i];

            }

            MakeGraph_Special_Point(osob_x_red_const, osob_y_red_const, N_Period_red_const);
          


        }

        public void Make_Graph_Saturation_2(double[] Saturation_t, double[] Saturation_y1, double[] Saturation_y2, int ew)
        {
            PointPairList f_saturation = new PointPairList();
            PointPairList f_saturation2 = new PointPairList();

            for (int y = 3; y < ew - 1; y++)
            {
                f_saturation.Add(Saturation_t[y], Saturation_y1[y] * 100);
                f_saturation2.Add(Saturation_t[y], Saturation_y2[y] * 100);
            }

           
            LineItem f_satur = pane.AddCurve("Степень оксигенации %", f_saturation, Color.DarkGoldenrod, SymbolType.Circle);
            LineItem f_satur2 = pane.AddCurve("Степень оксигенации %", f_saturation2, Color.Green, SymbolType.Diamond);

            
            f_satur.Symbol.Fill.Type = FillType.Solid;
            f_satur.Symbol.Size = 8;

            f_satur2.Symbol.Fill.Type = FillType.Solid;
            f_satur2.Symbol.Size = 8;

            pane.YAxis.MajorGrid.IsZeroLine = false;
            pane.XAxis.Title.Text = "t, мс";
            pane.YAxis.Title.Text = "Степень оксигенации, %";

        }



        public void Make_Graph_Spectr_Fast(String name_curve, int N_point, double dw, double[] y1)
        {

            int n = N_point;
            double[] y = y1;

            if (y.Length < N_point)
            {
                n = y.Length;
            }
            // Создадим список точек для кривой f1(x)
            PointPairList f1_list = new PointPairList();

            // Заполним массив точек для кривой f1-3(x)
            for (int i = 0; i < n; i++)
            {
                f1_list.Add(Math.Round((dw * (i + 1)) / (2 * 3.14), 3), Math.Round(y[i], 3));
            }

            LineItem f1_curve = pane.AddCurve(name_curve, f1_list, Color.Blue, SymbolType.None);
        }

        public void ResetGraph()
        {
            zedgraph.AxisChange();

            // Обновляем график
            zedgraph.Invalidate();
        }

        public void SaveGraph()
        {
            zedgraph.SaveAsBitmap();
        }

        public void ClearAll()
        {

            pane.CurveList.Clear();

            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;

            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;

            zedgraph.AxisChange();
            zedgraph.Invalidate();

        }

        public void Shift_Axis(double value_min, double value_max)
        {
            // Устанавливаем интересующий нас интервал по оси Y
            pane.XAxis.Scale.Min = value_min;
            pane.XAxis.Scale.Max = value_max;

            pane.YAxis.Scale.Min = 0;
            pane.YAxis.Scale.Max = 1250;

            // Вызываем метод AxisChange (), чтобы обновить данные об осях.
            // В противном случае на рисунке будет показана только часть графика,
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedgraph.AxisChange();
            zedgraph.Invalidate();

           

        }




    }
}
