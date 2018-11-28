using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningProject.FrankWolfe
{
    public class AvgGradientFrankWolfe
    {
        public static (double[], List<double[]>) NewFrankWolfe(Func<double[], double[]> gradient, int T, double[] x0)
        {
            double[] x = x0;

            var points = new List<double[]>();

            for (int t = 1; t <= T; t++)
            {
                points.Add(x);
                //Console.WriteLine("Iteration: " + t);
                //Console.WriteLine("x: " + string.Join(",", x));

                var grad = gradient(x);
                //Console.WriteLine("Gradient: " + string.Join(",", grad));

                double gamma = 2 / ((double)t + 2);

                var s = NewSorting(gradient, x, t);
                //Console.WriteLine("s: " + string.Join(",", s));
                

                x = UpdateX(x, s, gamma);
            }

            Console.WriteLine("End-------------");

            return (x, points);
        }
        static double[] NewSorting(Func<double[], double[]> gradient, double[] x, int iteration)
        {
            double epsilon = 0.001;
            double[] s = new double[x.Length];
            var grad = gradient(x);
            var normalizedGrad = grad.Select(g => g / grad.Max()).ToArray();

            var sorted = Enumerable.Range(0, grad.Length).
                Select(i => new KeyValuePair<int, double>(i, grad[i])).
                OrderByDescending(p => p.Value).
                ToList();
            for (int i = 1; i <= x.Length; i++)
            {
                s[sorted[i - 1].Key] = i;
            }

            List<List<int>> colorings = new List<List<int>>();
            colorings.Add(new List<int>() { sorted[0].Key });
            for (int i = 1; i < x.Length; i++)
            {
                if (normalizedGrad[sorted[i - 1].Key] - normalizedGrad[sorted[i].Key] < epsilon)
                    colorings.Last().Add(sorted[i].Key);
                else
                    colorings.Add(new List<int>() { sorted[i].Key });
            }
            double[] newS = new double[x.Length];
            foreach (var group in colorings)
            {
                double average = group.Select(i => s[i]).Average();
                foreach (int i in group)
                {
                    newS[i] = average;
                }
            }
            return newS;
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
