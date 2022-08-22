using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Models;

public class NodeModel : INotifyPropertyChanged, INodeModel
{
    public string Name => GenerateName();
    public Point3D Coordinates { get; set; }

    public BooleanVector Supports { get; set; }
    public Vector3D Forces { get; set; }

    public int? GlobalNodeNumber { get; set; } = null;
    public bool IsTemporary { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(Name));
        }
    }



    public event PropertyChangedEventHandler? PropertyChanged;

    public NodeModel(Point3D coordinates, Vector3D force, BooleanVector support, bool isSelected = false, bool isTemporary = true)
    {
        Coordinates = coordinates;
        Forces = force;
        Supports = support;
        IsSelected = isSelected;
        IsTemporary = isTemporary;
    }

    private string GenerateName()
    {
        if (IsSelected) return $"({Coordinates}) - izbran";
        else return $"({Coordinates})";
    }

    public void LocalToGlobalForce(ref double[] globalForceVector)
    {
        globalForceVector[3 * (int)GlobalNodeNumber! - 3] += Forces.X;
        globalForceVector[3 * (int)GlobalNodeNumber! - 2] += Forces.Y;
        globalForceVector[3 * (int)GlobalNodeNumber! - 1] += Forces.Z;
    }



    private void OnPropertyChanged(string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
