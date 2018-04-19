using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using 配色pro.Model;

namespace 配色pro.Library
{
 public    class Method
    {  
        //根据向量求角度
        public static float Tanh(Vector2 vector)
        {
            float tan = (float)Math.Atan(Math.Abs(vector.Y / vector.X));

            if (vector.X > 0 && vector.Y > 0)//第一象限
                return tan;
            else if (vector.X > 0 && vector.Y < 0)//第二象限
                return -tan;
            else if (vector.X < 0 && vector.Y > 0)//第三象限  
                return (float)Math.PI - tan;
            else
                return tan - (float)Math.PI;
        }
        public static float Tanh(float vectorX, float vectorY)
        {
            float tan = (float)Math.Atan(Math.Abs(vectorY / vectorX));

            if (vectorX > 0 && vectorY > 0)//第一象限
                return tan;
            else if (vectorX > 0 && vectorY < 0)//第二象限
                return -tan;
            else if (vectorX < 0 && vectorY > 0)//第三象限  
                return (float)Math.PI - tan;
            else
                return tan - (float)Math.PI;
        }






        //输出A与B的距离
        public static double 两点距(Point A, Point B)
        {
            return Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y));//椭圆半焦距
        }

        //在直线AB上，使得输出点P与A点距离为distance
        public static Point 距在线上点(double distance, Point A, Point B)
        {
            var Scale = distance / 两点距(A, B);
            return new Point(-Scale * (B.X - A.X) + A.X, -Scale * (B.Y - A.Y) + A.Y);
        }

        //在直线AB上，使得输出点P与A点距离的比例为scale（0~1）
        public static Point 比在线上点(float scale, Point A, Point B)
        {
            return new Point((B.X - A.X) * scale + A.X, (B.Y - A.Y) * scale + A.Y);
        }

        //输出P与线AB的距离
        public static double 点线距(Point P, Point A, Point B)
        {
            double dis = 0;
            if (A.X == B.X)
            {
                dis = Math.Abs(P.X - A.X);
                return dis;
            }
            double lineK = (B.Y - A.Y) / (B.X - A.X);
            double lineC = (B.X * A.Y - A.X * B.Y) / (B.X - A.X);
            dis = Math.Abs(lineK * P.X - P.Y + lineC) / (Math.Sqrt(lineK * lineK + 1));
            return dis;
        }

        //P在直线AB上的映射点，输出距离A的距离
        public static double 点在线上距(Point P, Point A, Point B)
        {
            var 斜边 = 两点距(P, A);
            var 高 = 点线距(P, A, B);
            return Math.Sqrt(斜边 * 斜边 - 高 * 高);
        }





        public static double 两边距(double A, double B)//求c2（c2=a2+b2）
        {
            return Math.Sqrt(A * A + B * B);//椭圆半焦距
        }

        public static double 点线距(Point P, Point A, Point B, double 两点距)//两点距是AB的距离
        {
            var 上距离 = (A.X - B.X) * (P.X - A.X) + (A.Y - B.Y) * (P.Y - A.Y);

            return Math.Abs(上距离) / 两点距;
        }

        public static Point 点与线垂直点(Point P, Point A, Point B)//P与线AB的距离
        {
            double XX = A.X - B.X; //缓存值：X1 - X2
            double YY = A.Y - B.Y; // 缓存值：Y1 - Y2
            double XY = A.X * B.Y - A.Y * B.X;// 缓存值：X1*Y2 - X2*Y1

            double 上式 = P.Y + XX / YY * P.X - XY / XX;//算式：分子
            double 下式 = YY / XX + XX / YY;//算式：分母

            double X = 上式 / 下式;//垂直点：X
            double Y = YY / XX * X + XY / XX;//垂直点：Y

            return new Point(X, Y);
        }

        public static double 点角度(Point O, Point P)//返回360度
        {
            //除数不能为0
            double tan = Math.Atan(Math.Abs((P.Y - O.Y) / (P.X - O.X))) * 180 / Math.PI;

            if (P.X > O.X && P.Y > O.Y)//第一象限
                return -tan;
            else if (P.X > O.X && P.Y < O.Y)//第二象限
                return tan;
            else if (P.X < O.X && P.Y > O.Y)//第三象限
                return tan - 180;
            else
                return 180 - tan;
        }








        //混合颜色

        public static Color ColorBlend(Color 上色, Color 下色)//正常叠加：两个像素叠加
        {
            Color 中间色;

            double 上色占比; //计算Alpha通道的百分数30%和1-30%
            double 下色占比;

            double 上色透光度;//透光度=1-0.8，即透过80%的光线
            double 下色透光度;

            if (上色.A == 255)
            {
                中间色 = 上色; //上组不透明，直接用上图代替下图
            }
            else if (上色.A == 0 && 下色.A == 0)
            {
                中间色 = Color.FromArgb(0, 0, 0, 0); //上组和下组都透明，直接透明
            }
            else if (上色.A == 0)
            {
                中间色 = 下色; //上组透明，直接用上图代替
            }
            else if (下色.A == 0)
            {
                中间色 = 上色; //下组透明，直接用下图代替
            }
            else //上组透明，用混合算法
            {
                //RGB（上色占比、下色占比）
                上色占比 = (上色.A) / 255d; //根据Alpha通道计算上下RGB的百分数占比：30%和1-30%
                下色占比 = 1.0d - 上色占比;

                中间色.R = (byte)(上色.R * 上色占比 + 下色.R * 下色占比);
                中间色.G = (byte)(上色.G * 上色占比 + 下色.G * 下色占比);
                中间色.B = (byte)(上色.B * 上色占比 + 下色.B * 下色占比);


                //A（上色透光度、下色透光度）：
                上色透光度 = (255.0d - 上色.A) / 255.0d;
                下色透光度 = (255.0d - 下色.A) / 255.0d;

                中间色.A = (byte)(255 - 255 * 上色透光度 * 下色透光度);
            }
            return 中间色;
        }

        public static Color ColorBlend(Color 上色,  Color 下色, double 下alpha) //正常叠加：两个像素叠加(Color[,] 上组, Color[,] 下组, i)
        {
            if (下alpha > 100) 下alpha = 100;

            double 上组A = (double)上色.A;
            double 下组A = (double)下alpha / 100.0d * (double)下色.A;

            Color 上组色 = Color.FromArgb((byte)上组A, 上色.R, 上色.G, 上色.B);
            Color 下组色 = Color.FromArgb((byte)下组A, 下色.R, 下色.G, 下色.B);

            return ColorBlend(上组色, 下组色);
        }
        public static Color ColorBlend(Color 上色, double 上alpha, Color 下色, double 下alpha) //正常叠加：两个像素叠加(Color[,] 上组, Color[,] 下组, i)
        {
            if (上alpha > 100) 上alpha = 100;
            if (下alpha > 100) 下alpha = 100;

            double 上组A = (double)上alpha / 100.0d * (double)上色.A;
            double 下组A = (double)下alpha / 100.0d * (double)下色.A;

            Color 上组色 = Color.FromArgb((byte)上组A, 上色.R, 上色.G, 上色.B);
            Color 下组色 = Color.FromArgb((byte)下组A, 下色.R, 下色.G, 下色.B);

            return ColorBlend(上组色, 下组色);
        }



        //混合颜色
        public static Color BlendColor(double Opacity, Color Backgroud)
        {
            byte R = (byte)(Opacity * Backgroud.R);
            byte G = (byte)(Opacity * Backgroud.G);
            byte B = (byte)(Opacity * Backgroud.B);

            return Color.FromArgb(255, R, G, B);
        }
        public static Color BlendColor(double Opacity, Color Backgroud, Color Foreground)
        {
            double UnOpacity = 1 - Opacity;

            byte R = (byte)(Opacity * Foreground.R + UnOpacity * Backgroud.R);
            byte G = (byte)(Opacity * Foreground.G + UnOpacity * Backgroud.G);
            byte B = (byte)(Opacity * Foreground.B + UnOpacity * Backgroud.B);

            return Color.FromArgb(255, R, G, B);
        }





        //GRB
        public static Color HSLtoRGB(double H)  //输入色相（0~360），输出最艳的颜色
        {
            double hh = H / 60;
            double x = 1 - Math.Abs(hh % 2 - 1);  //6个色相内的内部渐变的数值
            double R, G, B;

            if (hh < 1) { R = 1; G = x; B = 0; }
            else if (hh < 2) { R = x; G = 1; B = 0; }
            else if (hh < 3) { R = 0; G = 1; B = x; }
            else if (hh < 4) { R = 0; G = x; B = 1; }
            else if (hh < 5) { R = x; G = 0; B = 1; }
            else { R = 1; G = 0; B = x; }

            return Color.FromArgb(255, (byte)(255 * R), (byte)(255 * G), (byte)(255 * B));
        }
        public static Color HSLtoRGB(HSL hsl, byte aplha = 255)   //HSL转换为RGB   H:0~360,S:0~1,L:0~1 
        {
            double hh = hsl.H % 360.0;

            if (hsl.S == 0.0)
            {
                byte lllllllll = (byte)(hsl.L * 255.0);
                return Color.FromArgb(255, lllllllll, lllllllll, lllllllll);
            }
            else
            {
                double dhh = hh / 60.0;
                int nhh = (int)Math.Floor(dhh);
                double rhh = dhh - nhh;

                double rr = hsl.L * (1.0 - hsl.S);
                double gg = hsl.L * (1.0 - (hsl.S * rhh));
                double bb = hsl.L * (1.0 - (hsl.S * (1.0 - rhh)));

                switch (nhh)
                {
                    case 0: return Color.FromArgb(aplha, (byte)(hsl.L * 255.0), (byte)(bb * 255.0), (byte)(rr * 255.0));
                    case 1: return Color.FromArgb(aplha, (byte)(gg * 255.0), (byte)(hsl.L * 255.0), (byte)(rr * 255.0));
                    case 2: return Color.FromArgb(aplha, (byte)(rr * 255.0), (byte)(hsl.L * 255.0), (byte)(bb * 255.0));
                    case 3: return Color.FromArgb(aplha, (byte)(rr * 255.0), (byte)(gg * 255.0), (byte)(hsl.L * 255.0));
                    case 4: return Color.FromArgb(aplha, (byte)(bb * 255.0), (byte)(rr * 255.0), (byte)(hsl.L * 255.0));
                    default: return Color.FromArgb(aplha, (byte)(hsl.L * 255.0), (byte)(rr * 255.0), (byte)(gg * 255.0));
                }
            }
        }
        public static Color HSLtoRGB(double H, double S, double L)   //HSL转换为RGB   H:0~360,S:0~1,L:0~1 
        {
            double hh = H % 360.0;

            if (S == 0.0)
            {
                byte lllllllll = (byte)(L * 255.0);
                return Color.FromArgb(255, lllllllll, lllllllll, lllllllll);
            }
            else
            {
                double dhh = hh / 60.0;
                int nhh = (int)Math.Floor(dhh);
                double rhh = dhh - nhh;

                double rr = L * (1.0 - S);
                double gg = L * (1.0 - (S * rhh));
                double bb = L * (1.0 - (S * (1.0 - rhh)));

                switch (nhh)
                {
                    case 0: return Color.FromArgb(255, (byte)(L * 255.0), (byte)(bb * 255.0), (byte)(rr * 255.0));
                    case 1: return Color.FromArgb(255, (byte)(gg * 255.0), (byte)(L * 255.0), (byte)(rr * 255.0));
                    case 2: return Color.FromArgb(255, (byte)(rr * 255.0), (byte)(L * 255.0), (byte)(bb * 255.0));
                    case 3: return Color.FromArgb(255, (byte)(rr * 255.0), (byte)(gg * 255.0), (byte)(L * 255.0));
                    case 4: return Color.FromArgb(255, (byte)(bb * 255.0), (byte)(rr * 255.0), (byte)(L * 255.0));
                    default: return Color.FromArgb(255, (byte)(L * 255.0), (byte)(rr * 255.0), (byte)(gg * 255.0));
                }
            }
        }


        public static HSL RGBtoHSL(Color coo)
        {
            double r = coo.R / 255.0;
            double g = coo.G / 255.0;
            double b = coo.B / 255.0;
            double max;//大
            double min;//小
            double dist;//差
            double r2, g2, b2;

            double h = 0; // default to black
            double s = 0;
            double l = 0;

            max = Math.Max(Math.Max(r, g), b);
            min = Math.Min(Math.Min(r, g), b);
            l = (min + max) / 2.0;

            if (l <= 0.0) return new HSL { H = 0, S = 0, L = 0 };

            dist = max - min;
            s = dist;

            if (s > 0.0) s /= (l <= 0.5) ? (max + min) : (2.0 - max - min);
            else return new HSL { H = 0, S = 0, L = 0 };

            r2 = (max - r) / dist;
            g2 = (max - g) / dist;
            b2 = (max - b) / dist;

            if (r == max) h = (g == min ? 5.0 + b2 : 1.0 - g2);
            else if (g == max) h = (b == min ? 1.0 + r2 : 3.0 - b2);
            else h = (r == min ? 3.0 + g2 : 5.0 - r2);

            return new HSL { H = h * 60d, S = s, L = l };
        }


        public static double RGBtoH(Color co)//色转色相：输入颜色，输出色相H（0~360）
        {
            double max = Math.Max(Math.Max(co.R, co.G), co.B);//最max值（0~255）
            double min = Math.Min(Math.Min(co.R, co.G), co.B);//最小值（0~255）
            double dist = max - min;//差值（0~255）
            double sum = max + min;//和值（0~512）

            double H;

            if (dist == 0)//差值为零：灰色
            {
                H = 0;//饱和度：0
            }
            else
            {
                //色相
                double r = (max - co.R) / dist;//红差值（0~1）
                double g = (max - co.G) / dist;//绿差值（0~1）
                double b = (max - co.G) / dist;//蓝差值（0~1）
                H = b - g;

                if (co.R == max) H = b - g;
                else if (co.G == max) H = 2 + r - b;
                else if (co.B == max) H = 4 + g - r;

                H *= 60;
                if (H < 0) H += 360;
                else if (H >= 360) H -= 360;
            }

            return H;
        }
        public static double RGBtoL(Color co)//色转亮度：输入颜色，输出亮度L（0~1）
        {
            double max = Math.Max(Math.Max(co.R, co.G), co.B) / 255.00d;//最大值（0~1）
            double min = Math.Min(Math.Min(co.R, co.G), co.B) / 255.00d;//最小值（0~1）

            return (max + min) / 2.00;
        }
        public static double RGBtos(Color co)//色转饱和度：输入颜色，输出饱和度S（0~1）
        {
            double max = Math.Max(Math.Max(co.R, co.G), co.B) / 255.00d;//最大值（0~1）
            double min = Math.Min(Math.Min(co.R, co.G), co.B) / 255.00d;//最小值（0~1）

            double dist = max - min;//差值（0~1）
            double sum = max + min;//和值（0~2）


            if (dist == 0) return 0;     //差为零：灰色，无饱和度


            if (sum < 1) return dist / sum;    //和小于1：暗色
            else return dist / (2 - sum);    //和max;于1：亮色
        }


        public static Color ChangeL(Color co, double L)//提高亮度（亮度值）：-1~1
        {
            double R = co.R;
            double G = co.G;
            double B = co.B;

            if (L > 0)//亮度值：0~1（变亮）
            {
                if (L > 1) return Color.FromArgb(co.A, 255, 255, 255);

                R = R + (255 - R) * L;
                G = G + (255 - G) * L;
                B = B + (255 - B) * L;

                return Color.FromArgb(co.A, (byte)R, (byte)G, (byte)B);
            }
            else if (L < 0)//亮度值：-1~0（变暗）
            {
                if (L < -1) return Color.FromArgb(co.A, 0, 0, 0);

                double 暗度 = 1 + L;//暗度：0~1

                R = R * 暗度;
                G = G * 暗度;
                B = B * 暗度;

                return Color.FromArgb(co.A, (byte)R, (byte)G, (byte)B);
            }
            else //if (亮度 == 0)
            {
                return Color.FromArgb(co.A, (byte)R, (byte)G, (byte)B);
            }
        }

    }
}
