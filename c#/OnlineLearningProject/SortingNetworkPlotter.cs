using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Drawing.Drawing2D;

namespace OnlineLearningProject
{
    class SortingNetworkPlotter
    {
        private const int imgLen = 500;
        private const int imgWidth = 500;
        public static Canvas GenerateDiagram(List<(int from, int to, int splitStage)> AllComparatorsWithSplitStage,
            List<(int from, int to, int splitStage)> ActiveComparatorsWithSplitStage)
        {
            AllComparatorsWithSplitStage = AllComparatorsWithSplitStage.
                OrderBy(x => x.splitStage).
                ThenByDescending(x => Math.Abs(x.from - x.to)).
                ToList();
            List<int> lineIndices = AllComparatorsWithSplitStage.
                Select(x => x.from).
                Union(AllComparatorsWithSplitStage.Select(x => x.to)).
                Distinct().
                OrderBy(x => x).
                ToList();

            var comps = new List<(int from, int to, int splitStage, int echelon)>();
            int echelon = 1;

            comps.Add((AllComparatorsWithSplitStage[0].from, AllComparatorsWithSplitStage[0].to,
                AllComparatorsWithSplitStage[0].splitStage, echelon));
            for (int i = 1; i < AllComparatorsWithSplitStage.Count; i++)
            {
                var cPrev = AllComparatorsWithSplitStage[i - 1];
                var c = AllComparatorsWithSplitStage[i];

                //if (c.splitStage != cPrev.splitStage | Math.Abs(c.from - c.to) != Math.Abs(cPrev.from - cPrev.to))
                //{
                //    echelon++;
                //    comps.Add((c.from, c.to, c.splitStage, echelon));
                //}
                //else
                //    comps.Add((c.from, c.to, c.splitStage, echelon));
                echelon++;
                comps.Add((c.from, c.to, c.splitStage, echelon));
            }

            int echelonWidth = (int)Math.Floor((double)imgWidth / echelon);
            int wireLen = (int)Math.Floor((double)imgLen / lineIndices.Count);

            var canvas = new Canvas
            {
                Width = imgWidth,
                Height = imgLen,
                Background = Brushes.White
            };


            //DRAW WIRES
            for (int i = 0; i < lineIndices.Count; i++)
            {
                var line = new Line
                {
                    X1 = 0,
                    Y1 =  i * wireLen,
                    X2 = imgWidth,
                    Y2 =  i * wireLen,
                    Width = imgWidth,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                };
                Canvas.SetLeft(line, 0);
                Canvas.SetTop(line, 0);
                canvas.Children.Add(line);

                var lineLabel = new TextBlock
                {
                    Text = lineIndices[i].ToString()
                };
                Canvas.SetLeft(lineLabel, 0);
                Canvas.SetTop(lineLabel, i * wireLen);
                canvas.Children.Add(lineLabel);
            }


            //DRAW ARROWS
            foreach (var comp in comps)
            {
                double tSize = 7;
               (double x, double y) tCenter;
                System.Windows.Point t1;
                System.Windows.Point t2;
                System.Windows.Point t3;

                if (comp.from< comp.to)
                {
                    tCenter = (comp.echelon * echelonWidth, lineIndices.IndexOf(comp.to) * wireLen - tSize / 2);
                    t1 = new System.Windows.Point(tCenter.x, tCenter.y + tSize / 2);
                    t2 = new System.Windows.Point(tCenter.x - tSize / 2, tCenter.y - tSize / 2);
                    t3 = new System.Windows.Point(tCenter.x + tSize / 2, tCenter.y - tSize / 2);
                }
                else
                {
                    tCenter = (comp.echelon * echelonWidth, lineIndices.IndexOf(comp.to) * wireLen + tSize / 2);
                    t1 = new System.Windows.Point(tCenter.x, tCenter.y - tSize / 2);
                    t2 = new System.Windows.Point(tCenter.x - tSize / 2, tCenter.y + tSize / 2);
                    t3 = new System.Windows.Point(tCenter.x + tSize / 2, tCenter.y + tSize / 2);
                }
                
                var arrowhead = new Polygon()
                {
                    Points = new PointCollection() { t1, t2, t3 },
                    Fill = Brushes.Black
                };

                if (ActiveComparatorsWithSplitStage.Contains((comp.from, comp.to, comp.splitStage)))
                    arrowhead.Fill = Brushes.Red;

                Canvas.SetLeft(arrowhead, 0);
                Canvas.SetTop(arrowhead, 0);
                canvas.Children.Add(arrowhead);

                var arrow = new Line
                {
                    StrokeEndLineCap = PenLineCap.Triangle,
                    StrokeStartLineCap = PenLineCap.Round,
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1,
                    X1 = comp.echelon* echelonWidth,
                    X2 = comp.echelon*echelonWidth,
                    Y1 = lineIndices.IndexOf(comp.from)*wireLen,
                    Y2 = lineIndices.IndexOf(comp.to) * wireLen
                };
                if (ActiveComparatorsWithSplitStage.Contains((comp.from, comp.to, comp.splitStage)))
                    arrow.Stroke = new SolidColorBrush(Colors.Red);

                Canvas.SetLeft(arrow, 0);
                Canvas.SetTop(arrow, 0);
                canvas.Children.Add(arrow);

            }

            return canvas;
        }

    }
}
