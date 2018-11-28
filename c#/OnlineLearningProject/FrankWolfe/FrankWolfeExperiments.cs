using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace OnlineLearningProject.FrankWolfe
{
    class FrankWolfeExperiments
    {
        public static void CompareVanillaVsAvg()
        {
            int dim = 6;
            int T = 1000;
            double[,] A = GeneratePositiveDefiniteMatrix(dim);

            var chol = new Accord.Math.Decompositions.CholeskyDecomposition(A);
            var isPosDef = chol.IsPositiveDefinite;
            if (!isPosDef)
                throw new Exception();

            var aMat = Matrix.Create(A);
            var sumMat = Elementwise.Add(aMat, aMat.Transpose());

            Func<double[], double[]> gradient = (x =>
            {
                var v = Vector.Create(x);
                return sumMat.Dot(v);
            });
            Func<double[], double> obj = (x =>
            {
                return Matrix.Dot(x, aMat).Dot(x);
            });

            var x0 = ConstructCentroid(dim);

            var vResult = VanillaFrankWolfe.FrankWolfe(gradient, T, x0);
            Console.WriteLine(string.Join(",", vResult.Item1));

            using (var writer = new System.IO.StreamWriter("ObjProgressionVanilla.txt"))
            {
                foreach (var p in vResult.Item2)
                {
                    writer.WriteLine(obj(p));
                }
            }

            var aResult = AvgGradientFrankWolfe.NewFrankWolfe(gradient, T, x0);
            Console.WriteLine(string.Join(",", aResult.Item1));

            using (var writer = new System.IO.StreamWriter("ObjProgressionAvg.txt"))
            {
                foreach (var p in aResult.Item2)
                {
                    writer.WriteLine(obj(p));
                }
            }

        }
        public static void TestVanillaFrankWolfe()
        {
            int dim = 6;
            int T = 1000;
            double[,] A = GeneratePositiveDefiniteMatrix(dim);
            
            var chol = new Accord.Math.Decompositions.CholeskyDecomposition(A);
            var isPosDef = chol.IsPositiveDefinite;
            if (!isPosDef)
                throw new Exception();

            var gapProg = new List<double>();

            var aMat = Matrix.Create(A);
            var sumMat = Elementwise.Add(aMat, aMat.Transpose());
            
            Func<double[], double[]> gradient = (x =>
            {
                var v = Vector.Create(x);
                return sumMat.Dot(v);
            });
            Func<double[], double> obj = (x =>
            {
                return Matrix.Dot(x, aMat).Dot(x);
            });

            var x0 = ConstructCentroid(dim);
            var result = VanillaFrankWolfe.FrankWolfe(gradient, T, x0);
            Console.WriteLine(string.Join(",", result.Item1));

            using (var writer = new System.IO.StreamWriter("ObjProgressionVanilla.txt"))
            {
                foreach (var p in result.Item2)
                {
                    writer.WriteLine(obj(p));
                }
            }

        }
        public static void TestAvgGradientFrankWolfe()
        {
            int dim = 6;
            int T = 1000;
            double[,] A = GeneratePositiveDefiniteMatrix(dim);

            var chol = new Accord.Math.Decompositions.CholeskyDecomposition(A);
            var isPosDef = chol.IsPositiveDefinite;
            if (!isPosDef)
                throw new Exception();
            

            var aMat = Matrix.Create(A);
            var sumMat = Elementwise.Add(aMat, aMat.Transpose());

            Func<double[], double[]> gradient = (x =>
            {
                var v = Vector.Create(x);
                return sumMat.Dot(v);
            });
            Func<double[], double> obj = (x =>
            {
                return Matrix.Dot(x, aMat).Dot(x);
            });

            var x0 = ConstructCentroid(dim);
            var result = AvgGradientFrankWolfe.NewFrankWolfe(gradient, T, x0);
            Console.WriteLine(string.Join(",", result.Item1));

            using (var writer = new System.IO.StreamWriter("ObjProgressionAvg.txt"))
            {
                foreach (var p in result.Item2)
                {
                    writer.WriteLine(obj(p));
                }
            }

        }
        static double[] ConstructCentroid(int dim)
        {
            var range = Enumerable.Range(1, dim);
            var avg = range.Average();
            double[] centroid = range.Select(x => avg).ToArray();

            return centroid;
        }
        static double[,] GeneratePositiveDefiniteMatrix(int dim)
        {
            var rng = new Random();
            double[,] matrix = new double[dim, dim];

            //Construct random matrix
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    matrix[i, j] = rng.NextDouble();
                }
            }

            //Construct A*A
            double[,] product = new double[dim, dim];
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    double entry = 0;
                    for (int k = 0; k < dim; k++)
                    {
                        entry += matrix[i, k] * matrix[k, j];
                    }
                    product[i, j] = entry;
                }
            }

            //Construct AA + nI
            for (int i = 0; i < dim; i++)
            {
                product[i, i] += dim + 2 * i;
            }

            return product;
        }
    }
}
