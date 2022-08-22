using PalicneKonstrukcijeMKE.Utility;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Interfaces
{
    public interface INodeModel
    {
        Point3D Coordinates { get; set; }
        Vector3D Forces { get; set; }
        int? GlobalNodeNumber { get; set; }
        bool IsSelected { get; set; }
        bool IsTemporary { get; set; }
        string Name { get; }
        BooleanVector Supports { get; set; }
    }
}