using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saturation_Blood
{
    class Methods_Statistics
    {

        public int Statistic_Point_B2(long x1)
        {

            double x = System.Convert.ToDouble(x1);
            double y;
            int z;

            const double A_B2 = -70.56011;
            const double B1_X_B2 = 0.57614;
            const double B2_X_B2 = -5.27313E-4;

            y = A_B2 + B1_X_B2 * x + B2_X_B2 * x * x;
            if (y > 10)
            {
                z = System.Convert.ToInt32(y);
            }
            else { z = 0; }
            return z;

        }

        public int Statistic_Point_B3(long x1)
        {

            double x = System.Convert.ToDouble(x1);
            double y;
            int z;

            const double A_B3 = -0.16255;
            const double B1_X_B3 = 0.71636;
            const double B2_X_B3 = -6.14949E-4;

            y = A_B3 + B1_X_B3 * x + B2_X_B3 * x * x;
            if (y > 50)
            {
                z = System.Convert.ToInt32(y);
            }
            else { z = 0; }
            return z;

        }
        public int Statistic_Point_B4(long x1)
        {

            double x = System.Convert.ToDouble(x1);
            double y;
            int z;


            const double A_B4 = 1.05273;
            const double B1_X_B4 = 0.86666;
            const double B2_X_B4 = -7.970316E-4;

            y = A_B4 + B1_X_B4 * x + B2_X_B4 * x * x;

            if (y > 50)
            {
                z = System.Convert.ToInt32(y);
            }
            else { z = 0; }
            return z;

        }



    }
}
