using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineLearningProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Canvas Diagram;
        TextBox box;
        Random rng;
        public MainWindow()
        {
            InitializeComponent();
            //BitonicSortingExperiments.RunSeriesOfExperiments("BitonicSorting.txt");
            //FrankWolfe.FrankWolfeExperiments.CompareVanillaVsAvg();
            
            rng = new Random();
        }
        private void GenerateNewNetwork(int Size)
        {
            var randoms = Enumerable.Range(0, Size).Select(x => rng.NextDouble()).ToList();
            var input = randoms.OrderBy(x => x).Select(x => (double)randoms.IndexOf(x)).ToArray();

            
            var bitonicSorter = new BitonicSorter();
            bitonicSorter.Sort(true, input);
            var comps = bitonicSorter.AllComparatorsWithSplitStage;
            Diagram = SortingNetworkPlotter.GenerateDiagram(comps, bitonicSorter.ActiveComparatorsWithSplitStage);

            var sPanel = new StackPanel();
            sPanel.Orientation = Orientation.Horizontal;
            sPanel.Children.Add(Diagram);

            var VertSPanel = new StackPanel();
            var smallHPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            var prevExample = new Label() { Content = string.Join(" ", input) };
            VertSPanel.Children.Add(prevExample);
            var label = new Label { Content = "InputSize" };
            box = new TextBox()
            {
                Width = 30
            };
            smallHPanel.Children.Add(label);
            smallHPanel.Children.Add(box);


            var button = new Button()
            {
                Content = "Generate New Input",
                Height = 20,
                Width = 120
            };

            button.Click += GenerateClick;

            VertSPanel.Children.Add(smallHPanel);
            VertSPanel.Children.Add(button);
            sPanel.Children.Add(VertSPanel);
            this.Content = sPanel;
        }
        private void GenerateClick(object sender, RoutedEventArgs e)
        {
            int size;
            bool isNum = int.TryParse(box.Text, out size);
            if (isNum)
                GenerateNewNetwork(size);
        }
        private void Test(object sender, RoutedEventArgs e)
        {
            GenerateNewNetwork(8);
        }
    }
}
