using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningProject.FrankWolfe
{
    public class VanillaFrankWolfe
    {
        public static (double[], List<double[]>) FrankWolfe(Func<double[], double[]> gradient, int T, double[] x0)
        {
            double[] x = x0;

            var points = new List<double[]>();

            for (int t = 1; t <= T; t++)
            {
                //Console.WriteLine("Iteration: " + t);
                //Console.WriteLine("x: " + string.Join(",", x));
                points.Add(x);
                var grad = gradient(x);
                //Console.WriteLine("Gradient: " + string.Join(",", grad));

                double gamma = 2 / ((double)t + 2);

                var s = CalculateS(gradient, x);
                //Console.WriteLine("s: " + string.Join(",", s));

                double gap = CalcDualityGap(x, s, gradient);
                //Console.WriteLine("Gap: " + gap);
                

                x = UpdateX(x, s, gamma);
            }

            Console.WriteLine("End-------------");

            return (x, points);
        }
        
        static double CalcDualityGap(double[] x, double[] s, Func<double[], double[]> gradient)
        {
            double gap = 0;
            double[] diff = x.Zip(s, (a, b) => a - b).ToArray();
            double[] gradAtX = gradient(x);

            for (int i = 0; i < x.Length; i++)
            {
                gap += diff[i] * gradAtX[i];
            }

            return gap;
        }
        static double[] CalculateS(Func<double[], double[]> gradient, double[] x)
        {
            //Since the extreme points of the polytope are permutations, we simply make s[i] = 1 where grad[i]>= grad[j] for all j, and so on

            double[] s = new double[x.Length];
            var grad = gradient(x);
            var sorted = Enumerable.Range(0, grad.Length).Select(i => new KeyValuePair<int, double>(i, grad[i])).OrderByDescending(p => p.Value).ToList();

            for (int i = 1; i <= x.Length; i++)
            {
                s[sorted[i - 1].Key] = i;
            }

            return s;
        }
        static double[] UpdateX(double[] x, double[] s, double gamma)
        {
            double[] newX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                newX[i] = (1 - gamma) * x[i] + gamma * s[i];
            }

            return newX;
        }
        
    }
}
