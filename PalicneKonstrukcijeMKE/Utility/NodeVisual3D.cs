using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje;

public class NodeVisual3D : MeshElement3D
{
    #region Dependency Properties

    public static readonly DependencyProperty CoordinatesProperty = DependencyProperty.Register("Coordinates", typeof(Point3D), typeof(NodeVisual3D), new PropertyMetadata(new Point3D(0, 0, 0), MeshElement3D.GeometryChanged));
    public Point3D Coordinates
    {
        get { return (Point3D)GetValue(CoordinatesProperty); }
        set { SetValue(CoordinatesProperty, value); }
    }

    public static readonly DependencyProperty ForcesProperty = DependencyProperty.Register("Force", typeof(Vector3D), typeof(NodeVisual3D), new PropertyMetadata(new Vector3D(0, 0, 0), MeshElement3D.GeometryChanged));
    public Vector3D Forces
    {
        get { return (Vector3D)GetValue(ForcesProperty); }
        set { SetValue(ForcesProperty, value); }
    }

    public static readonly DependencyProperty MaxVisualForceSizeProperty = DependencyProperty.Register("MaxVisualForceSize", typeof(double), typeof(NodeVisual3D), new PropertyMetadata(5.0, MeshElement3D.GeometryChanged));
    public double MaxVisualForceSize
    {
        get { return (double)GetValue(MaxVisualForceSizeProperty); }
        set { SetValue(MaxVisualForceSizeProperty, value); }
    }

    public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register("Diameter", typeof(double), typeof(NodeVisual3D), new PropertyMetadata(1.0, MeshElement3D.GeometryChanged));
    public double Diameter
    {
        get { return (double)GetValue(DiameterProperty); }
        set { SetValue(DiameterProperty, value); }
    }

    /// <summary>
    /// Number of geometry divisions. higher numbers give smoother geometries, but are more resource heavy.
    /// <list type="table">
    ///     <item>
    ///         <term>less than 16</term>
    ///         <description>rough surface quality</description>
    ///     </item>
    ///     <item>
    ///         <term>16 - 32</term>
    ///         <description>medium surface quality</description>
    ///     </item>
    ///     <item>
    ///         <term>more than 32</term>
    ///         <description>smooth surface quality</description>
    ///     </item>
    /// </list>
    /// </summary>
    public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register("Resolution", typeof(int), typeof(NodeVisual3D), new PropertyMetadata(24, MeshElement3D.GeometryChanged));
    public int Resolution
    {
        get { return (int)GetValue(ResolutionProperty); }
        set { SetValue(ResolutionProperty, value); }
    }

    public BooleanVector Supports
    {
        get { return (BooleanVector)GetValue(SupportsProperty); }
        set { SetValue(SupportsProperty, value); }
    }
    public static readonly DependencyProperty SupportsProperty = DependencyProperty.Register("Supports", typeof(BooleanVector), typeof(NodeVisual3D), new PropertyMetadata(new BooleanVector(), MeshElement3D.GeometryChanged));
    #endregion // Dependency Properties

    protected override MeshGeometry3D Tessellate()
    {
        Point3D c = Coordinates;
        Vector3D f = Forces;
        double d = Diameter;
        int r = Resolution;
        BooleanVector s = Supports;
        string name = this.GetName();

        MeshBuilder builder = new MeshBuilder();



        double forceLength = f.Length;
        if (Forces != new Vector3D(0, 0, 0))
        {
            if (forceLength > MaxVisualForceSize) builder.AddArrow(c, DownScaleVector3D(c, f), d / 2, MaxVisualForceSize, r);
            else builder.AddArrow(c, c + f, d / 2, forceLength, r);
        }

        // builder.AddCone(c - new Vector3D(0.7, 0, 0), c, d / 2, true, r);
        // builder.AddCone(c - new Vector3D(0, 0.7, 0), c, d / 2, true, r);
        // builder.AddCone(c - new Vector3D(0, 0, 0.7), c, d / 2, true, r);

        if(s != new BooleanVector(true, true, true)) builder.AddSphere(c, d / 2, r, r);
        if (s.X)
        { 
            builder.AddCubeFace(c, new Vector3D(1, 0, 0), new Vector3D(0, 1, 0), d, d, d);
            builder.AddCubeFace(c, new Vector3D(-1, 0, 0), new Vector3D(0, 1, 0), d, d, d);
        }
        if (s.Y)
        { 
            builder.AddCubeFace(c, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), d, d, d);
            builder.AddCubeFace(c, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), d, d, d);
        }
        if (s.Z)
        {
            builder.AddCubeFace(c, new Vector3D(0, 0, 1), new Vector3D(1, 0, 0), d, d, d);
            builder.AddCubeFace(c, new Vector3D(0, 0, -1), new Vector3D(1, 0, 0), d, d, d);
        }

        return builder.ToMesh();
    }

    private Point3D DownScaleVector3D(Point3D startPoint, Vector3D vector)
    {
        double length = vector.Length;
        Point3D p = new Point3D()
        {
            //X = (vector.X > MaxVisualForceSize || vector.X < -MaxVisualForceSize) ? startPoint.X + MaxVisualForceSize : startPoint.X + vector.X,
            X = CalculateForceLogic(vector.X , startPoint.X),
            //Y = (vector.Y > MaxVisualForceSize || vector.Y < -MaxVisualForceSize) ? startPoint.Y + MaxVisualForceSize : startPoint.Y + vector.Y,
            Y = CalculateForceLogic(vector.Y , startPoint.Y),
            //Z = (vector.Z > MaxVisualForceSize || vector.Z < -MaxVisualForceSize) ? startPoint.Z + MaxVisualForceSize : startPoint.Z + vector.Z,
            Z = CalculateForceLogic(vector.Z , startPoint.Z),
        };
        return p;
    }

    private double CalculateForceLogic(double vector, double startpoint)
    {
        double result = 0;
        if (vector > MaxVisualForceSize)
        {
            result = startpoint + MaxVisualForceSize;
        }
        else if(vector < -MaxVisualForceSize)
        {
            result = startpoint - MaxVisualForceSize;
        }
        else
        {
            result = startpoint + vector;
        }

        return result;
    }
}
