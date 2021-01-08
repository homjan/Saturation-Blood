using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZedGraph;

namespace Saturation_Blood
{
    public partial class Form1 : Form
    {
        UseZedgraph usergraph;

        //System.IO.FileInfo file = new System.IO.FileInfo(путь к вашему файлу);
        //long size = file.Length;
        //
        public Form1()
        {
            InitializeComponent();
        }

       
        delegate void SetTextCallback(int nomer);

        const double PI_ = 3.1415926;
        const int time_numerical = 60;
        double N_shift_axis = 0;
        double max_time = 0;

        public String texti = "50";
               
        void Clear_list_zed()
        {
            ZedGraph.MasterPane masterPane = zedGraph1.MasterPane;

            masterPane.PaneList.Clear();

            // Создаем экземпляр класса GraphPane, представляющий собой один график
            GraphPane pane = new GraphPane();

            pane.CurveList.Clear();

            // Добавим новый график в MasterPane
            masterPane.Add(pane);

            // Установим масштаб по умолчанию для оси X
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;

            // Установим масштаб по умолчанию для оси Y
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;

            using (Graphics g = CreateGraphics())
            {               
                // Графики будут размещены в один столбец друг под другом
                masterPane.SetLayout(g, PaneLayout.SingleColumn);
            }

            zedGraph1.AxisChange();
            zedGraph1.Invalidate();

        }//Очистить график - функция

        private void button4_Click(object sender, EventArgs e)
        {
            N_shift_axis = 0;

            string adres = "q";
            string adres2 = "q";
            string datapath = "w";

            int da5 = 0;

            StringBuilder buffer2 = new StringBuilder();

            FolderBrowserDialog fbd = new FolderBrowserDialog();

            OpenFileDialog qqq = new OpenFileDialog
            {
                Filter = "Файлы txt|*.txt"
            };

            if (qqq.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //adres = qqq.SelectedPath;
                adres = qqq.FileName;
                //       textBox4.Text = adres;
                buffer2.Insert(0, qqq.FileName);

                da5 = buffer2.Length;
                buffer2.Remove(da5 - 8, 8);
                adres2 = buffer2.ToString();
                datapath = Path.Combine(Application.StartupPath);

                System.IO.File.Copy(Path.Combine(qqq.InitialDirectory, qqq.FileName), Path.Combine(datapath, "test.txt"), true);
                try
                {
                    System.IO.File.Copy(adres2 + "Информация о пациенте.txt", Path.Combine(datapath, "Информация о пациенте.txt"), true);
                }
                catch
                {
                }
            }

           
            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = System.Convert.ToInt32(this.textBox2.Text);

            try
            {
                Initial_Data init_data = new Initial_Data("test.txt", reg, ekg);
                init_data.Row1_Shift_Time_To_0();//Сдвигаем время к 0
                init_data.Row1_Smothing();// Сглаживаем полученные данные
                init_data.Row1_Write_In_File("test3.txt");

                usergraph = new UseZedgraph(zedGraph1);
                usergraph.ClearAll();//Очищаем полотно
                usergraph.MakeGraph_4_Kanal(init_data.Get_row1(), init_data.Get_b());//Строим график
                usergraph.Install_Pane("t, мc", "R, Ом", "Каналы");//Устанавливаем оси и загавие
                usergraph.ResetGraph();//Обновляем
                max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Выбран неправильный файл");
            }
        }

        /// <summary>
        /// Обновить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            int reg = System.Convert.ToInt32(this.textBox1.Text);
            int ekg = System.Convert.ToInt32(this.textBox2.Text);
            try
            {
                Initial_Data init_data = new Initial_Data("test3.txt", reg, ekg);
                //      init_data.row1_shift_time_0();
                //       init_data.row1_smothing();

                usergraph = new UseZedgraph(zedGraph1);
                usergraph.ClearAll();//Очищаем полотно
                usergraph.MakeGraph_4_Kanal(init_data.Get_row1(), init_data.Get_b());//Строим график
                usergraph.Install_Pane("t, мc", "R, Ом", "Каналы");//Устанавливаем оси и загавие
                usergraph.ResetGraph();//Обновляем
                max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Выбран неправильный файл");
            }
        }
        /// <summary>
        /// Рассчитать точки макс-мин
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            int red_const = System.Convert.ToInt32(this.textBox1.Text);
            int red_diff = System.Convert.ToInt32(this.textBox2.Text);
            int IK_const = System.Convert.ToInt32(this.textBox3.Text);
            int IK_diff = System.Convert.ToInt32(this.textBox4.Text);

            string red_const_regul = this.comboBox1.Text;
            string red_diff_regul = this.comboBox3.Text;
            string IK_const_regul = this.comboBox4.Text;
            string IK_diff_regul = this.comboBox5.Text;

            int N_smoth = System.Convert.ToInt32(this.textBox11.Text);

            Initial_Data init_data = new Initial_Data("test3.txt", red_const, red_const, 1000000);
            init_data.Row1_Shift_Time_To_0();//Сдвигаем время к 0
            for (int i = 0; i < N_smoth; i++)
            {
                init_data.Row1_Smothing();
            }  

            init_data.Row1_Reflect_Chosen(red_const);
            init_data.Row1_Reflect_Chosen(IK_const);
            init_data.Row2_Calculate();
            init_data.Row3_Average_Kanal_reg();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.ClearAll();//Очищаем полотно
          
            Calculate_ALL_point_kanal_new(init_data, red_const, red_const_regul, 10000000, label11);
            Calculate_ALL_point_kanal_new(init_data, red_diff, red_diff_regul, 1000000, label12);
            Calculate_ALL_point_kanal_new(init_data, IK_const, IK_const_regul, 10000000, label13);
            Calculate_ALL_point_kanal_new(init_data, IK_diff, IK_diff_regul, 1000000, label14);

            usergraph.MakeGraph_4_Kanal(init_data.Get_row1(), init_data.Get_b());
          
          
            max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));

            usergraph.Install_Pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
            usergraph.ResetGraph();//Обновляем
        }
        
        void Calculate_ALL_point_kanal_new(Initial_Data init_data, int fpg, string regul, int pow_int, System.Windows.Forms.Label label11)
        { 
          //  init_data.row1_smothing();// Сглаживаем полученные данные
           
            init_data.Row2_re_Calculate(pow_int);
            init_data.Row3_Average_Chosen_Kanal(fpg);
         
            //Разделяем 
            Initial_processing.Divided_By_Periods_Data divided_row_red_const = new Initial_processing.Divided_By_Periods_Data(init_data, regul);
            divided_row_red_const.Calculate_Data_In_Period();
           
            Special_Point osob_point_1 = new Special_Point(divided_row_red_const, init_data);

            long[,] osob_1 = null;
            if (radioButton8.Checked)
            {
                osob_point_1.Return_Special_Point(regul);
            }
            if (radioButton7.Checked)
            {
                osob_point_1.Return_Special_Point_Neural_Network();
            }
            if (radioButton4.Checked)
            {
                osob_point_1.Return_Special_Point_Statistic_Num();
            }

            osob_point_1.Delete_Zero_In_Data();
            
            osob_1 = osob_point_1.Get_Spec_Point();

            int arre = osob_1.Length;
            int N_Period_red_const = arre / 15;//счетчик найденных максимумов
            label11.Text = $"Особых точек: {N_Period_red_const}";

            //  osob_1 = osob_point_1.shift_osob_point(osob_1, N_Period_red_const);
           usergraph.Make_Graph_Special_Point_B1_B2(osob_1);           

        }

        long[,] Calculate_osob_point(Initial_Data init_data, int fpg, string regul, int pow_int, System.Windows.Forms.Label label11)
        {
            //  init_data.row1_smothing();// Сглаживаем полученные данные

            init_data.Row2_re_Calculate(pow_int);
            init_data.Row3_Average_Chosen_Kanal(fpg);

            //Разделяем 
            Initial_processing.Divided_By_Periods_Data divided_row_red_const = new Initial_processing.Divided_By_Periods_Data(init_data, regul);
            divided_row_red_const.Calculate_Data_In_Period();
           // divided_row_red_const.Delete_zero_in_period();

            Special_Point osob_point_1 = new Special_Point(divided_row_red_const, init_data);

            long[,] osob_1 = null;
            if (radioButton8.Checked)
            {
                osob_point_1.Return_Special_Point(regul);
            }
            if (radioButton7.Checked)
            {
                osob_point_1.Return_Special_Point_Neural_Network();
            }
            if (radioButton4.Checked)
            {
                osob_point_1.Return_Special_Point_Statistic_Num();
            }

            osob_point_1.Delete_Zero_In_Data();
            osob_point_1.Delete_Equal_Data();

            osob_1 = osob_point_1.Get_Spec_Point();

            int arre = osob_1.Length;
            int N_Period_red_const = arre / 15;//счетчик найденных максимумов
            label11.Text = $"Особых точек: {N_Period_red_const}";

            //  osob_1 = osob_point_1.shift_osob_point(osob_1, N_Period_red_const);


            /*   long[,] osob_x_red_const = new long[5, N_Period_red_const];// список особых точек для вывода на график
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
               usergraph.Make_Graph_Max_Min(osob_x_red_const, osob_y_red_const, N_Period_red_const, "", "");
            */

           usergraph.Make_Graph_Special_Point_B1_B2(osob_1);
            return osob_1;

        }

      

        long[,] Calculate_osob_point_without_graph(Initial_Data init_data, int fpg, string regul, int pow_int, System.Windows.Forms.Label label11)
        {
            
            init_data.Row2_re_Calculate(pow_int);
            init_data.Row3_Average_Chosen_Kanal(fpg);

            //Разделяем 
            Initial_processing.Divided_By_Periods_Data divided_row_red_const = new Initial_processing.Divided_By_Periods_Data(init_data, regul);
            divided_row_red_const.Calculate_Data_In_Period();
          //  divided_row_red_const.Delete_zero_in_period();

            Special_Point osob_point_1 = new Special_Point(divided_row_red_const, init_data);

            long[,] osob_1 = null;
            if (radioButton8.Checked)
            {
                osob_point_1.Return_Special_Point(regul);
            }
            if (radioButton7.Checked)
            {
                osob_point_1.Return_Special_Point_Neural_Network();
            }
            if (radioButton4.Checked)
            {
                osob_point_1.Return_Special_Point_Statistic_Num();
            }

            osob_point_1.Delete_Zero_In_Data();
            osob_point_1.Delete_Equal_Data();

            osob_1 = osob_point_1.Get_Spec_Point();

            int arre = osob_1.Length;
            int N_Period_red_const = arre / 15;//счетчик найденных максимумов
            label11.Text = $"Особых точек: {N_Period_red_const}";
                       
            
            return osob_1;

        }


        /// <summary>
        /// Сохранить график
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            usergraph.SaveGraph();
        }
        /// <summary>
        /// Сохранить данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Очистить график
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            usergraph.ClearAll();
            Clear_list_zed();
            richTextBox1.Clear();
        }

        /// <summary>
        /// Рассчитать особые точки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
           
            int reg = System.Convert.ToInt32(this.textBox8.Text);
            int ekg = System.Convert.ToInt32(this.textBox8.Text);

            Initial_Data init_data = new Initial_Data("test3.txt", reg, ekg);
            init_data.Row1_Shift_Time_To_0();//Сдвигаем время к 0
            init_data.Row1_Smothing();// Сглаживаем полученные данные
            init_data.Row2_Calculate();
            init_data.Row3_Average_Kanal_reg();

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.ClearAll();//Очищаем полотно
            usergraph.MakeGraph_On_Canal_FPG();

            //Разделяем 
            Initial_processing.Divided_By_Periods_Data divided_row = new Initial_processing.Divided_By_Periods_Data(init_data, this.comboBox3.Text);
            divided_row.Calculate_Data_In_Period();
            //divided_row.delete_zero_in_period();

            Special_Point osob_point = new Special_Point(divided_row, init_data);

            long[,] osob = null;
            if (radioButton8.Checked)
            {
                osob_point.Return_Special_Point(this.comboBox3.Text);
            }
            if (radioButton7.Checked)
            {
                osob_point.Return_Special_Point_Neural_Network();
            }
            if (radioButton4.Checked)
            {
                osob_point.Return_Special_Point_Statistic_Num();
            }

            osob_point.Delete_Zero_In_Data();
            osob_point.Delete_Equal_Data();


            osob = osob_point.Get_Spec_Point();

                int arre = osob.Length;
                int ew = arre / 15;//счетчик найденных максимумов

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

                ////////////////////////

                long[,] osob_x = new long[5, ew];// список особых точек для вывода на график
                long[,] osob_y = new long[5, ew];

                for (int i = 0; i < ew - 1; i++)
                {
                    osob_x[0, i] = osob[1, i];
                    osob_y[0, i] = osob[0, i];

                    osob_x[1, i] = osob[3, i];
                    osob_y[1, i] = osob[2, i];

                    osob_x[2, i] = osob[5, i];
                    osob_y[2, i] = osob[4, i] + osob[10, i];

                    osob_x[3, i] = osob[7, i];
                    osob_y[3, i] = osob[6, i] + osob[10, i];

                    osob_x[4, i] = osob[9, i];
                    osob_y[4, i] = osob[8, i] + osob[10, i];

                }
                usergraph.MakeGraph_Special_Point(osob_x, osob_y, ew);
                usergraph.Install_Pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
                usergraph.ResetGraph();//Обновляем
                max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));


                Save_data save_data = new Save_data();

                save_data.Save_Osob_Point_Postr(osob, ew);
                /////////////////////////////////////////

                osob = osob_point.Shift_Special_Point(osob, ew);

                save_data.Save_Osob_Point_Clear(osob, ew);
            }

        /// <summary>
        /// Влево
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (N_shift_axis > 0)
            {
                N_shift_axis = N_shift_axis - 10000.0;
            }

            double value_Min = N_shift_axis;
            double value_Max = N_shift_axis + 10000.0;

            usergraph = new UseZedgraph(zedGraph1);
            usergraph.Shift_Axis(value_Min, value_Max);

            progressBar1.Maximum = Convert.ToInt32(max_time / 1000000.0);
            progressBar1.Minimum = 0;
            double value_2 = N_shift_axis / 1000.0;
            if (value_2 < 0)
            {
                value_2 = progressBar1.Minimum;
            }
            if (value_2 > (max_time / 1000000.0))
            {
                value_2 = progressBar1.Maximum;

            }

            progressBar1.Value = Convert.ToInt32(value_2);

        }
        /// <summary>
        /// Вправо
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (N_shift_axis < (max_time / 1000.0))
            {
                N_shift_axis = N_shift_axis + 10000.0;
            }
            double value_Min = N_shift_axis;
            double value_Max = N_shift_axis + 10000.0;

            usergraph = new UseZedgraph(zedGraph1);
            usergraph.Shift_Axis(value_Min, value_Max);

            progressBar1.Maximum = Convert.ToInt32(max_time / 1000000.0);
            progressBar1.Minimum = 0;
            double value_2 = N_shift_axis / 1000.0;
            if (value_2 < 0)
            {
                value_2 = progressBar1.Minimum;
            }
            if (value_2 > (max_time / 1000000.0))
            {
                value_2 = progressBar1.Maximum;

            }

            progressBar1.Value = Convert.ToInt32(value_2);
        }
    
       
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Рассчитать сатурацию крови 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
           
            Clear_list_zed();

            int red_const = System.Convert.ToInt32(this.textBox1.Text);
            int red_diff = System.Convert.ToInt32(this.textBox2.Text);
            int IK_const = System.Convert.ToInt32(this.textBox3.Text);
            int IK_diff = System.Convert.ToInt32(this.textBox4.Text);

            string red_const_regul = this.comboBox1.Text;
            string red_diff_regul = this.comboBox3.Text;
            string IK_const_regul = this.comboBox4.Text;
            string IK_diff_regul = this.comboBox5.Text;

            int N_smoth = System.Convert.ToInt32(this.textBox11.Text);

            double atten = Convert.ToDouble(this.textBox7.Text);
            double k_const_RED = Convert.ToDouble(this.textBox5.Text);
            double k_const_IK = Convert.ToDouble(this.textBox6.Text);

            Initial_Data init_data = new Initial_Data("test3.txt", red_const, red_const, 1000000);
            init_data.Row1_Shift_Time_To_0();//Сдвигаем время к 0
            //Сглаживаем N_smoth раз
            for (int i = 0; i < N_smoth; i++)
            {
                init_data.Row1_Smothing();
            }
           
            init_data.Row2_Calculate();
            init_data.Row3_Average_Kanal_reg();

            //Считаем особые точки переменных составляющих
            long[,] osob_IK_diff = Calculate_osob_point_without_graph(init_data, IK_diff, IK_diff_regul, 1000000, label14);
            long[,] osob_Red_diff = Calculate_osob_point_without_graph(init_data, red_diff, red_diff_regul, 1000000, label12);

            if (checkBox2.Checked)
            {
                Reflector_Const_Data reflector_IK_const = new Reflector_Const_Data(init_data, osob_IK_diff, IK_const);

                reflector_IK_const.Set_Const_Special_Point_from_Diff(init_data);
                reflector_IK_const.Calculate_line_reflection();
                reflector_IK_const.Calculate_reflected_row();
                init_data.Set_row1(reflector_IK_const.Get_row1());

                Reflector_Const_Data reflector_Red_const = new Reflector_Const_Data(init_data, osob_Red_diff, red_const);

                reflector_Red_const.Set_Const_Special_Point_from_Diff(init_data);
                reflector_Red_const.Calculate_line_reflection();
                reflector_Red_const.Calculate_reflected_row();
                init_data.Set_row1(reflector_Red_const.Get_row1());

                GC.Collect();
            }
            int h = 5;

            long[,] osob_IK_const;
            long[,] osob_Red_const;

            if (checkBox1.Checked)// Если - считаем на основе точек переменного сигнала
            {
                //Заменить reflector на function_additional??
                Reflector_Const_Data reflector_Const_IK_2 = new Reflector_Const_Data(osob_IK_diff, IK_const);
                reflector_Const_IK_2.Set_Const_Special_Point_from_Diff(init_data);
                osob_IK_const = reflector_Const_IK_2.Get_Special_Point_Const();
                
                int N_Period_IK_const = osob_IK_const.Length / 15;//счетчик найденных максимумов
                label13.Text = $"Особых точек: {N_Period_IK_const}";

                Reflector_Const_Data reflector_Const_Red_2 = new Reflector_Const_Data(osob_Red_diff, red_const);
                reflector_Const_Red_2.Set_Const_Special_Point_from_Diff(init_data);
                osob_Red_const = reflector_Const_Red_2.Get_Special_Point_Const();

                int N_Period_Red_const = osob_Red_const.Length / 15;//счетчик найденных максимумов
                label11.Text = $"Особых точек: {N_Period_Red_const}";

                GC.Collect();

            }
            else // Иначе - как положено
            {
                osob_IK_const = Calculate_osob_point_without_graph(init_data, IK_const, IK_const_regul, 10000000, label13);
                osob_Red_const = Calculate_osob_point_without_graph(init_data, red_const, red_const_regul, 10000000, label11);
            }
        
            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.ClearAll();//Очищаем полотно

            usergraph.MakeGraph_4_Kanal(init_data.Get_row1(), init_data.Get_b());

            usergraph.Make_Graph_Special_Point_B1_B2(osob_IK_diff);
            usergraph.Make_Graph_Special_Point_B1_B2(osob_IK_const);
            usergraph.Make_Graph_Special_Point_B1_B2(osob_Red_diff);
            usergraph.Make_Graph_Special_Point_B1_B2(osob_Red_const);


            max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));

            Saturation saturation = new Saturation(osob_Red_const, osob_Red_diff, osob_IK_const, osob_IK_diff);


            if (radioButton1.Checked == true)
            {

            saturation.Set_K_Pow_Const(atten, k_const_RED, k_const_IK);
            saturation.Subscribe_Length_Special();
            saturation.Calculate_K_Pow_Diff();
            saturation.Calculate_Intensity();
            saturation.Calculate_Saturation_Time();

                saturation.Calculate_Saturation_1_Kalinina_Const();
                saturation.Calculate_Saturation_2_New_Const();
            }

            if (radioButton2.Checked == true)
            {
                saturation.Set_K_Pow_Const(atten, k_const_RED, k_const_IK);
                saturation.Subscribe_Length_Osob_Full();
                saturation.Calculate_K_Pow_Diff();
                saturation.Calculate_Intensity();
                saturation.Calculate_Saturation_Time();

                saturation.Calculate_Saturation_1_Kalinina_Diff();
                saturation.Calculate_Saturation_2_New_Diff();
            }

            double[] saturation_Time = saturation.Get_Saturation_Time();
            double[] saturation_One = saturation.Get_Saturation_1();
            double[] saturation_Two = saturation.Get_Saturation_2();

            int final_length = saturation_Time.Length;
            if (saturation_One.Length < final_length)
            {
                final_length = saturation_One.Length;
            }
            if (saturation_Two.Length < final_length)
            {
                final_length = saturation_Two.Length;
            }


            for (int i = 0; i < final_length; i++)
            {
                richTextBox1.AppendText(Math.Round(saturation_Time[i] / 1000, 3) + "\t" +
                    Math.Round(saturation_One[i], 3)  + "\n");
            }

            usergraph.Make_Graph_Saturation(saturation_Time, saturation_One, saturation_Two, final_length);
       
            usergraph.Install_Pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
            usergraph.ResetGraph();//Обновляем

            GC.Collect();

        }

        /// <summary>
        /// Рассчитать и вывести только сатурацию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            Clear_list_zed();

            int red_const = System.Convert.ToInt32(this.textBox1.Text);
            int red_diff = System.Convert.ToInt32(this.textBox2.Text);
            int IK_const = System.Convert.ToInt32(this.textBox3.Text);
            int IK_diff = System.Convert.ToInt32(this.textBox4.Text);

            string red_const_regul = this.comboBox1.Text;
            string red_diff_regul = this.comboBox3.Text;
            string IK_const_regul = this.comboBox4.Text;
            string IK_diff_regul = this.comboBox5.Text;

            int N_smoth = System.Convert.ToInt32(this.textBox11.Text);

            double atten = Convert.ToDouble(this.textBox7.Text);
            double k_const_RED = Convert.ToDouble(this.textBox5.Text);
            double k_const_IK = Convert.ToDouble(this.textBox6.Text);

            Initial_Data init_data = new Initial_Data("test3.txt", red_const, red_const, 1000000);
            init_data.Row1_Shift_Time_To_0();//Сдвигаем время к 0
            //Сглаживаем N_smoth раз
            for (int i = 0; i < N_smoth; i++)
            {
                init_data.Row1_Smothing();
            }

            init_data.Row2_Calculate();
            init_data.Row3_Average_Kanal_reg();

            //Считаем особые точки переменных составляющих
            long[,] osob_IK_diff = Calculate_osob_point_without_graph(init_data, IK_diff, IK_diff_regul, 1000000, label14);
            long[,] osob_Red_diff = Calculate_osob_point_without_graph(init_data, red_diff, red_diff_regul, 1000000, label12);

            if (checkBox2.Checked)
            {
                Reflector_Const_Data reflector_IK_const = new Reflector_Const_Data(init_data, osob_IK_diff, IK_const);

                reflector_IK_const.Set_Const_Special_Point_from_Diff(init_data);
                reflector_IK_const.Calculate_line_reflection();
                reflector_IK_const.Calculate_reflected_row();
                init_data.Set_row1(reflector_IK_const.Get_row1());

                Reflector_Const_Data reflector_Red_const = new Reflector_Const_Data(init_data, osob_Red_diff, red_const);

                reflector_Red_const.Set_Const_Special_Point_from_Diff(init_data);
                reflector_Red_const.Calculate_line_reflection();
                reflector_Red_const.Calculate_reflected_row();
                init_data.Set_row1(reflector_Red_const.Get_row1());
            }
            int h = 5;

            long[,] osob_IK_const;
            long[,] osob_Red_const;

            if (checkBox1.Checked)// Если - считаем на основе точек переменного сигнала
            {
                //Заменить reflector на function_additional??
                Reflector_Const_Data reflector_Const_IK_2 = new Reflector_Const_Data(osob_IK_diff, IK_const);
                reflector_Const_IK_2.Set_Const_Special_Point_from_Diff(init_data);
                osob_IK_const = reflector_Const_IK_2.Get_Special_Point_Const();

                int N_Period_IK_const = osob_IK_const.Length / 15;//счетчик найденных максимумов
                label13.Text = $"Особых точек: {N_Period_IK_const}";

                Reflector_Const_Data reflector_Const_Red_2 = new Reflector_Const_Data(osob_Red_diff, red_const);
                reflector_Const_Red_2.Set_Const_Special_Point_from_Diff(init_data);
                osob_Red_const = reflector_Const_Red_2.Get_Special_Point_Const();

                int N_Period_Red_const = osob_Red_const.Length / 15;//счетчик найденных максимумов
                label11.Text = $"Особых точек: {N_Period_Red_const}";

            }
            else // Иначе - как положено
            {
                osob_IK_const = Calculate_osob_point_without_graph(init_data, IK_const, IK_const_regul, 10000000, label13);
                osob_Red_const = Calculate_osob_point_without_graph(init_data, red_const, red_const_regul, 10000000, label11);
            }

            usergraph = new UseZedgraph(zedGraph1, init_data);
            usergraph.ClearAll();//Очищаем полотно
             
            max_time = Convert.ToDouble(init_data.Get_row1_x_y(init_data.Get_b() - 1, 0));

            Saturation saturation = new Saturation(osob_Red_const, osob_Red_diff, osob_IK_const, osob_IK_diff);

            // Saturation saturation = new Saturation(Calculate_osob_point(init_data, red_const, red_const_regul, 10000000, label11),
            //     Calculate_osob_point(init_data, red_diff, red_diff_regul, 1000000, label12),
            //     Calculate_osob_point(init_data, IK_const, IK_const_regul, 10000000, label13),
            //    Calculate_osob_point(init_data, IK_diff, IK_diff_regul, 1000000, label14));


            if (radioButton1.Checked == true)
            {

                saturation.Set_K_Pow_Const(atten, k_const_RED, k_const_IK);
                saturation.Subscribe_Length_Special();
                saturation.Calculate_K_Pow_Diff();
                saturation.Calculate_Intensity();
                saturation.Calculate_Saturation_Time();

                saturation.Calculate_Saturation_1_Kalinina_Const();
             
            }

            if (radioButton2.Checked == true)
            {
                saturation.Set_K_Pow_Const(atten, k_const_RED, k_const_IK);
                saturation.Subscribe_Length_Osob_Full();
                saturation.Calculate_K_Pow_Diff();
                saturation.Calculate_Intensity();
                saturation.Calculate_Saturation_Time();

                saturation.Calculate_Saturation_1_Kalinina_Diff();                
            }

            double[] saturation_Time = saturation.Get_Saturation_Time();
            double[] saturation_One = saturation.Get_Saturation_1();
            double[] saturation_Two = saturation.Get_Saturation_2();

            int final_length = saturation_Time.Length;
            if (saturation_One.Length < final_length)
            {
                final_length = saturation_One.Length;
            }
            if (saturation_Two.Length < final_length)
            {
                final_length = saturation_Two.Length;
            }


            for (int i = 0; i < final_length; i++)
            {
                richTextBox1.AppendText(Math.Round(saturation_Time[i] / 1000, 3) + "\t" +
                    Math.Round(saturation_One[i], 3) + "\n");
            }

            usergraph.Make_Graph_Saturation(saturation_Time, saturation_One, saturation_Two, final_length);

            usergraph.Install_Pane("t, мc", "R, Ом", " ");//Устанавливаем оси и заглавие
            usergraph.ResetGraph();//Обновляем
        }

      
    }
}

