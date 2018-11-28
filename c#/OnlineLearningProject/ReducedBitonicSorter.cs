using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineLearningProject
{
    public class ReducedBitonicSorter : BitonicSorter
    {
        public List<(int from, int to, int stage)> ReducedComparators;

        public ReducedBitonicSorter(List<(int from, int to, int stage)> comparators)
        {
            ReducedComparators = comparators;
        }

        protected override double[] Merge(bool ascending, double[] array, int a, int b, int splitStage)
        {
            if (array.Length == 1)
                return array;

            int split = (int)Math.Floor((double)array.Length / 2);

            for (int i = 0; i < split; i++)
            {
                (int from, int to, int stage) currentComp;
                if (ascending)
                    currentComp = (a + i, a + i + split, splitStage);
                else
                    currentComp = (a + i + split, a + i, splitStage);

                if (ReducedComparators.Contains(currentComp))
                {
                    if (array[i] > array[i + split] == ascending)
                    {
                        if (ascending)
                        {
                            AllComparators.Add((a + i, a + i + split));
                            AllComparatorsWithSplitStage.Add((a + i, a + i + split, splitStage));
                            ActiveComparators.Add((a + i, a + i + split));
                            ActiveComparatorsWithSplitStage.Add((a + i, a + i + split, splitStage));
                        }
                        else
                        {
                            AllComparators.Add((a + i + split, a + i));
                            AllComparatorsWithSplitStage.Add((a + i + split, a + i, splitStage));
                            ActiveComparators.Add((a + i + split, a + i));
                            ActiveComparatorsWithSplitStage.Add((a + i + split, a + i, splitStage));
                        }

                        double temp = array[i];
                        array[i] = array[i + split];
                        array[i + split] = temp;
                    }
                }
            }

            var first = Merge(ascending, array.Take(split).ToArray(), a, a + split, splitStage);
            var second = Merge(ascending, array.Skip(split).ToArray(), a + split, b, splitStage);

            return first.Concat(second).ToArray();
        }
    }
}
