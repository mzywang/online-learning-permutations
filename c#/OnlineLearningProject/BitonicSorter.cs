using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLearningProject
{
    public class BitonicSorter
    {
        public List<(int from, int to)> AllComparators;
        public List<(int from, int to, int splitStage)> AllComparatorsWithSplitStage;
        public List<(int from, int to)> ActiveComparators;
        public List<(int from, int to, int splitStage)> ActiveComparatorsWithSplitStage;

        protected const double dummyVar = double.MinValue;

        public double[] Sort(bool ascending, double[] array)
        {
            AllComparators = new List<(int from, int to)>();
            AllComparatorsWithSplitStage = new List<(int from, int to, int splitStage)>();
            ActiveComparators = new List<(int from, int to)>();
            ActiveComparatorsWithSplitStage = new List<(int from, int to, int splitStage)>();

            var input = FillWithDummyVariables(array);
            int nStages = (int)(Math.Log(input.Length) / Math.Log(2));
            var output = BitonicSort(ascending, input, 0, input.Length - 1, nStages);

            return RemoveDummyVariables(output);
        }
        protected double[] BitonicSort(bool ascending, double[] input, int a, int b, int splitStage)
        {
            if (input.Length <= 1)
                return input;

            int split = (int)Math.Floor((double)input.Length / 2);
            var first = BitonicSort(true, input.Take(split).ToArray(), a, a + split, splitStage - 1);
            var second = BitonicSort(false, input.Skip(split).ToArray(), a + split, b, splitStage - 1);

            return Merge(ascending, first.Concat(second).ToArray(), a, b, splitStage);
        }
        protected virtual double[] Merge(bool ascending, double[] array, int a, int b, int splitStage)
        {
            if (array.Length == 1)
                return array;

            int split = (int)Math.Floor((double)array.Length / 2);

            for (int i = 0; i < split; i++)
            {
                if (ascending)
                {
                    AllComparators.Add((a + i, a + i + split));
                    AllComparatorsWithSplitStage.Add((a + i, a + i + split, splitStage));
                }
                else
                {
                    AllComparators.Add((a + i + split, a + i));
                    AllComparatorsWithSplitStage.Add((a + i + split, a + i, splitStage));
                }


                if (array[i] > array[i + split] == ascending)
                {
                    if (ascending)
                    {
                        ActiveComparators.Add((a + i, a + i + split));
                        ActiveComparatorsWithSplitStage.Add((a + i, a + i + split, splitStage));
                    }
                    else
                    {
                        ActiveComparators.Add((a + i + split, a + i));
                        ActiveComparatorsWithSplitStage.Add((a + i + split, a + i, splitStage));
                    }

                    double temp = array[i];
                    array[i] = array[i + split];
                    array[i + split] = temp;
                }
            }

            var first = Merge(ascending, array.Take(split).ToArray(), a, a + split, splitStage);
            var second = Merge(ascending, array.Skip(split).ToArray(), a + split, b, splitStage);

            return first.Concat(second).ToArray();
        }
        protected static double[] FillWithDummyVariables(double[] input)
        {
            double power = Math.Log10(input.Length) / Math.Log10(2);
            int powerCeil = (int)Math.Ceiling(power);

            if (power == powerCeil)
                return input;
            else
            {
                var dummvars = Enumerable.Range(1, (int)Math.Pow(2, powerCeil) - input.Length).Select(x => dummyVar);
                return input.Concat(dummvars).ToArray();
            }
        }
        protected static double[] RemoveDummyVariables(double[] output)
        {
            var x = output.ToList();
            x.RemoveAll(y => y == dummyVar);
            return x.ToArray();
        }
    }
}
