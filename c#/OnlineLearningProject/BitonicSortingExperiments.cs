using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningProject
{
    public static class BitonicSortingExperiments
    {
        public static void RunSeriesOfExperiments(string output)
        {
            var experiments = new List<(int size, int nTrials, double c)>()
            {
                (8,1000, 1),
                (8,1000,0.5),
                (8, 1000, 5),
                (32,1000, 1),
                (32,1000,0.5),
                (32, 1000, 5),
                (256,1000, 1),
                (256,1000,0.5),
                (256, 1000, 5),
                (512,1000, 1),
                (512,1000,0.5),
                (512, 1000, 5),
                (512, 1000, 20),
                (512, 1000, 50)
            };

            var results = new List<(int size, int nTrials, double c, double averageInconsistencies)>();

            foreach(var exp in experiments)
            {
                double inconsist = ReducedNetworkExperiment(exp.size, exp.nTrials, exp.c);
                results.Add((exp.size, exp.nTrials, exp.c, inconsist));
            }

            using(var writer = new System.IO.StreamWriter(output))
            {
                writer.WriteLine("Size\t nTrials\t C\t Avg.Inconsist.");
                foreach(var result in results)
                {
                    writer.WriteLine(string.Format("{0}\t {1}\t {2}\t {3}",
                        result.size, result.nTrials, result.c, result.averageInconsistencies));
                }   
            }
        }
        static double ReducedNetworkExperiment(int size, int nTrials, double c)
        {

            var rng = new Random();
            var distinctNumbers = Enumerable.Range(1, size);
            var randomOrdering = distinctNumbers.Select(x => rng.NextDouble()).ToList();
            var randomX = distinctNumbers.OrderBy(x => randomOrdering[x - 1]).Select(x => (double)x).ToArray();
            //var randomX = Enumerable.Range(1, vectorLength).Select(x => (double)rng.Next(1, vectorLength)).ToArray();

            var regularSorter = new BitonicSorter();
            var sortedX = regularSorter.Sort(true, randomX);

            int nTotalComparators = regularSorter.AllComparatorsWithSplitStage.Count;
            int nActiveComparators = regularSorter.ActiveComparatorsWithSplitStage.Count;
            var activeComparators = regularSorter.ActiveComparatorsWithSplitStage;

            Console.WriteLine("Random Vector:");
            Console.WriteLine(string.Join(",", randomX));
            Console.WriteLine("Sorted Random Vector:");
            Console.WriteLine(string.Join(",", sortedX));
            Console.WriteLine("Total Number of Comparators: " + nTotalComparators);
            Console.WriteLine("Number of Active Comparators: " + nActiveComparators);

            Console.WriteLine("Experiment");
            List<int> inconsistencies = new List<int>();

            Parallel.For(1, nTrials, i => {
                var perturbedVector = randomX.Select(x => x + (rng.NextDouble() - 0.5) * c).ToArray();
                var reducedSorter = new ReducedBitonicSorter(activeComparators);
                var sortedPerturbed = reducedSorter.Sort(true, perturbedVector);
                int nInconsistencies = CountInconsistencies(sortedPerturbed);
                inconsistencies.Add(nInconsistencies);
            });


            Console.WriteLine("Constant: " + c);
            Console.WriteLine("Average Number of Inconsistencies: " + inconsistencies.Average());
            return inconsistencies.Average();
        }
        static int CountInconsistencies(double[] sortedArray)
        {
            int nInconsistencies = 0;
            for (int i = 0; i < sortedArray.Length - 1; i++)
            {
                if (sortedArray[i] > sortedArray[i + 1])
                    nInconsistencies++;
            }
            return nInconsistencies;
        }
    }
}
