using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using System;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Models;

public class ElementModel
{
    public INodeModel FirstNode { get; set; }
    public INodeModel SecondNode { get; set; }

    public double Length => CalculateLength();

    public double SectionArea;
    public double YoungsModulus;

    /// <param name="sectionArea">Section aria in <c>mm</c>²</param>
    /// <param name="youngsModulus">Young modulus in <c>MPa</c></param>
    public ElementModel(INodeModel firstNode, INodeModel secondNode, double sectionArea = 0.01, double youngsModulus = 210 * 100000)
    {
        FirstNode = firstNode;
        SecondNode = secondNode;

        SectionArea = sectionArea;
        YoungsModulus = youngsModulus;
    }

    private double CalculateLength()
    {
        Point3D c1 = FirstNode.Coordinates;
        Point3D c2 = SecondNode.Coordinates;

        double x = Math.Pow(c2.X - c1.X, 2);
        double y = Math.Pow(c2.Y - c1.Y, 2);
        double z = Math.Pow(c2.Z - c1.Z, 2);

        return Math.Sqrt(x + y + z);
    }

    public void LocalToGlobalStiffnesMatrix(ref double[,] globalStiffnessMatrix)
    {
        double l = Length;

        int firstNodeDOF = (int)FirstNode.GlobalNodeNumber!;
        int secondNodeDOF = (int)SecondNode.GlobalNodeNumber!;

        double[] t = TransformationMatrixElements();

        for (int r = 3; r > 0; r--)
        {
            for (int s = 3; s > 0; s--)
            {
                double a = ((SectionArea * YoungsModulus) / l) * t[-(r - 3)] * t[-(s - 3)];
                globalStiffnessMatrix[3 * firstNodeDOF - r, 3 * firstNodeDOF - s] += a;
                globalStiffnessMatrix[3 * firstNodeDOF - r, 3 * secondNodeDOF - s] += -a;
                globalStiffnessMatrix[3 * secondNodeDOF - r, 3 * firstNodeDOF - s] += -a;
                globalStiffnessMatrix[3 * secondNodeDOF - r, 3 * secondNodeDOF - s] += a;
            }
        }
    }

    public double[] TransformationMatrixElements()
    {
        double l = Length;

        return new double[] {
                (SecondNode.Coordinates.X - FirstNode.Coordinates.X) / l,      // cx
                (SecondNode.Coordinates.Y - FirstNode.Coordinates.Y) / l,      // cy
                (SecondNode.Coordinates.Z - FirstNode.Coordinates.Z) / l,      // cz
        };
    }
}
