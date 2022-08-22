using System;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Palicje;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using PalicneKonstrukcijeMKE.Utility;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using System.Windows.Controls;
using System.Windows.Media;

namespace PalicneKonstrukcijeMKE.Solver
{
    /// <summary>
    /// Interaction logic for SolverResults.xaml
    /// </summary>
    public partial class SolverResults : Window
    {
        public Collection<int> KeptDOF { get; private set; }
        public double[] Displacements { get; private set; }
        public Collection<ModelVisual3D> TrussVisualItems { get; private set; }
        public Collection<ResultNodeModel> ResultNodeModels { get; }
        public Collection<ElementModel> ResultElementModels { get; }
        public Collection<ModelVisual3D> ResultVisualItems { get; private set; }

        public SolverResults(Tuple<Collection<int>, double[], double[]>? resultsTuple, Collection<ModelVisual3D> trussVisualItems, Collection<NodeModel> nodeModels, Collection<ElementModel> elementModels)
        {
            DataContext = this;
            if(resultsTuple == null)
            {
                this.Close();
                return;
            };
            ResultNodeModels = new Collection<ResultNodeModel>();
            ResultElementModels = new Collection<ElementModel>();
            ResultVisualItems = new ObservableCollection<ModelVisual3D>();
            TrussVisualItems = new Collection<ModelVisual3D>();
            DeepCopyVisualItems(trussVisualItems);

            KeptDOF = resultsTuple.Item1;
            //Displacements = resultsTuple.Item2;
            Displacements = new double[nodeModels.Count * 3];
            for (int i = 0; i < KeptDOF.Count; i++)
            {
                Displacements[KeptDOF[i]] = resultsTuple.Item2[i];
            }
            
            DeepCopyNodesAndElements(nodeModels, elementModels);

            InitializeComponent();
            createDisplacedVisualItems(1);
        }


        private void createDisplacedVisualItems(double scale)
        {
            if(ScaleText == null)
            {
                scale = 1;
            }
            else
            {
                ScaleText.Text = "( " + scale.ToString($"0.##") + "x )";
            }
            
            ResultVisualItems.Clear();
            foreach (ModelVisual3D mv3D in TrussVisualItems)
            {
                ResultVisualItems.Add(mv3D);
            }
            if (ShowResults.IsChecked == false) return;


            foreach (ResultNodeModel nodeModel in ResultNodeModels)
            {
                Point3D displacedCoordinates = nodeModel.CalculateDisplacedCoordinates(scale);
                if (nodeModel.Coordinates == displacedCoordinates) continue;

                ResultVisualItems.Add(new NodeVisual3D()
                {
                    Coordinates = displacedCoordinates,
                    Material = Materials.Blue,
                    Supports = nodeModel.Supports,
                    //Forces = nodeModel.Forces

                });
                if ((bool)ShowBillBoards.IsChecked!)
                {
                    ResultVisualItems.Add(new BillboardTextVisual3D()
                    {
                        Position = new Point3D(displacedCoordinates.X + 4, displacedCoordinates.Y - 1, displacedCoordinates.Z),
                        Text = nodeModel.CalculateTotalDisplacements().ToString("0.####") + "mm",
                        Foreground = Brushes.OrangeRed,
                        Background = Brushes.Transparent,
                        FontSize = 14,
                    });
                }
            }

            foreach (ElementModel elementModel in ResultElementModels)
            {
                ResultNodeModel node1 = (ResultNodeModel)elementModel.FirstNode;
                ResultNodeModel node2 = (ResultNodeModel)elementModel.SecondNode;

                Point3D FirstDisplacedPoint = node1.CalculateDisplacedCoordinates(scale);
                Point3D SecondDisplacedPoint = node2.CalculateDisplacedCoordinates(scale);

                ResultVisualItems.Add(new PipeVisual3D()
                {
                    Point1 = FirstDisplacedPoint,
                    Point2 = SecondDisplacedPoint,
                    Material = MaterialHelper.CreateMaterial(Color.FromRgb(0x00, 0x9b, 0xf4)),
                    Diameter = 0.5,
                });
                if ((bool)ShowBillBoards.IsChecked!)
                {
                    ResultVisualItems.Add(new BillboardTextVisual3D()
                    {
                        Position = CalculateElementMidpoint(FirstDisplacedPoint, SecondDisplacedPoint, new Point3D(1, 1, 0.2)),
                        Text = $"σ = {CalculateElementStress(elementModel).ToString("0.####")}N",
                        Foreground = Brushes.MediumVioletRed,
                        Background = Brushes.Transparent,
                        FontSize = 14,
                    });
                }
            }


        }

        private double CalculateElementStress(ElementModel element)
        {
            double l = element.Length;
            double[] t = element.TransformationMatrixElements();
            int firstNodeGNN = (int)element.FirstNode.GlobalNodeNumber!;
            int secondNodeGNN = (int)element.SecondNode.GlobalNodeNumber!;

            double elementProperties = (element.YoungsModulus * element.SectionArea) / l;
            double xComponent = t[0] * (Displacements[3 * secondNodeGNN - 3] - Displacements[3 * firstNodeGNN - 3]);
            double yComponent = t[1] * (Displacements[3 * secondNodeGNN - 2] - Displacements[3 * firstNodeGNN - 2]);
            double zComponent = t[2] * (Displacements[3 * secondNodeGNN - 1] - Displacements[3 * firstNodeGNN - 1]);

            return elementProperties * (xComponent + yComponent + zComponent);
        }

        private Point3D CalculateElementMidpoint(Point3D FirstNode, Point3D SecondNode, Point3D DisplaceMidpointBy)
        {
            return new Point3D()
            {
                X = ((FirstNode.X + SecondNode.X) / 2) + DisplaceMidpointBy.X,
                Y = ((FirstNode.Y + SecondNode.Y) / 2) + DisplaceMidpointBy.Y,
                Z = ((FirstNode.Z + SecondNode.Z) / 2) + DisplaceMidpointBy.Z,
            };
        }



        public void DeepCopyNodesAndElements(Collection<NodeModel> nodeModels, Collection<ElementModel> elementModels)
        {
            
            foreach (NodeModel node in nodeModels)
            {
                int GNN = (int)node.GlobalNodeNumber!;
                Point3D displacements = new Point3D(0, 0, 0);
                if (KeptDOF.Contains(3 * GNN - 3)) displacements.X += Displacements[3 * GNN - 3];
                if (KeptDOF.Contains(3 * GNN - 2)) displacements.Y += Displacements[3 * GNN - 2];
                if (KeptDOF.Contains(3 * GNN - 1)) displacements.Z += Displacements[3 * GNN - 1];
                ResultNodeModels.Add(new ResultNodeModel(node.Coordinates, node.Forces, node.Supports, displacements, GNN, false, false));
            } 

            foreach (ElementModel element in elementModels)
            {
                IEnumerable<INodeModel> SelectedNodes = ResultNodeModels.Where(node => node.Coordinates == element.FirstNode.Coordinates || node.Coordinates == element.SecondNode.Coordinates);
                if (SelectedNodes.Count() == 2)
                {
                    ResultElementModels.Add(new ElementModel(SelectedNodes.ElementAt(0), SelectedNodes.ElementAt(1), element.SectionArea, element.YoungsModulus));
                }
            }
        }

        private void DeepCopyVisualItems(Collection<ModelVisual3D> trussVisualItems)
        {
            TrussVisualItems.Add(new DefaultLights());
            foreach (ModelVisual3D modelVisual3D in trussVisualItems)
            {
                if (modelVisual3D is PipeVisual3D)
                {
                    PipeVisual3D element = (PipeVisual3D)modelVisual3D;
                    TrussVisualItems.Add(new PipeVisual3D()
                    {
                        Point1 = element.Point1,
                        Point2 = element.Point2,
                        Material = element.Material,
                        Diameter = element.Diameter,
                        ThetaDiv = element.ThetaDiv,
                    });
                    continue;
                }

                if (modelVisual3D is NodeVisual3D)
                {
                    NodeVisual3D element = (NodeVisual3D)modelVisual3D;
                    TrussVisualItems.Add(new NodeVisual3D()
                    {
                        Coordinates = element.Coordinates,
                        Forces = element.Forces,
                        Supports = element.Supports,
                        Material = element.Material,
                        Resolution = element.Resolution,
                        Diameter = element.Diameter
                    });
                    continue;
                }
            }
        }

        private void ViewportScaleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ScaleText != null) createDisplacedVisualItems((sender as Slider)!.Value);
        }

        private void CheckboxChecked(object sender, RoutedEventArgs e)
        {
            if(ViewportScale != null && ScaleText != null) createDisplacedVisualItems(ViewportScale.Value);
        }
    }

    public class ResultNodeModel : NodeModel
    {
        public Point3D Displacements { get; set; }
        public ResultNodeModel(Point3D coordinates, Vector3D force, BooleanVector support, Point3D displacements, int globalNodeNumber, bool isSelected = false, bool isTemporary = true) : base(coordinates, force, support, isSelected, isTemporary)
        {
            base.Coordinates = coordinates;
            base.Forces = force;
            base.Supports = support;
            base.GlobalNodeNumber = globalNodeNumber;
            base.IsSelected = isSelected;
            base.IsTemporary = isTemporary;
            Displacements = displacements;
        }

        public Point3D CalculateDisplacedCoordinates(double scale)
        {
            return new Point3D(
                base.Coordinates.X + Displacements.X * scale,
                base.Coordinates.Y + Displacements.Y * scale,
                base.Coordinates.Z + Displacements.Z * scale
                );
        }

        public double CalculateTotalDisplacements()
        {
            double result = Math.Pow(Displacements.X, 2)
                          + Math.Pow(Displacements.Y, 2)
                          + Math.Pow(Displacements.Z, 2);
            return Math.Sqrt(result);
        }
    }
}
