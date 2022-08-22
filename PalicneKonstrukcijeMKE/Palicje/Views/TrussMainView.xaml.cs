using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.Palicje.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.Views
{
    /// <summary>
    /// Interaction logic for TrussMainView.xaml
    /// </summary>
    public partial class TrussMainView : UserControl
    {
        public TrussMainView()
        {
            InitializeComponent();
            CameraHelper.ChangeDirection(viewport3D.Camera, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 0);
        }

        private void HeilxViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var thisViewModel = (TrussMainViewModel)DataContext;
            if (thisViewModel == null || thisViewModel.DisplayEditControl.CanExecute(null) == false) return;

            HelixViewport3D vp = sender as HelixViewport3D;
            var firstHitElement = vp.Viewport.FindHits(e.GetPosition(vp)).FirstOrDefault();

            thisViewModel.DisplayEditControl.Execute(firstHitElement);
        }

        private void ButtonShow3D(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraHelper.ChangeDirection(viewport3D.Camera, new Vector3D(-10, -10, -10), new Vector3D(0, 1, 0), 0);
        }
        
        private void ButtonShow2D(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraHelper.ChangeDirection(viewport3D.Camera, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 0);
        }

        private void ButtonFitView(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraHelper.FitView(viewport3D.Camera, viewport3D.Viewport, 0.5);
            //viewport3D.Camera.;
        }
    }

    
}
