using PalicneKonstrukcijeMKE.Utility;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Models
{
    public interface INodeModel
    {
        Point3D Coordinates { get; set; }
        Vector3D Forces { get; set; }
        string Name { get; }
        BooleanVector Supports { get; set; }
    }
}