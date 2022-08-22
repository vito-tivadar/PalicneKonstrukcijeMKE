using HelixToolkit.Wpf;
using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Commands;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Palicje.Services;
using PalicneKonstrukcijeMKE.Solver;
using PalicneKonstrukcijeMKE.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace PalicneKonstrukcijeMKE.Palicje.ViewModels;

public class TrussMainViewModel : ViewModelBase
{
    private readonly NavigationBase _editControlNavigation;

    private readonly TrussMainModel TrussModel;
    public ProjectionCamera ViewportCamera { get; set; }
    public ObservableCollection<ModelVisual3D> ViewportItems { get; }
    public NodeService NodeService { get; set; }
    public ElementService ElementService { get; set; }

    public ViewModelBase CurrentEditControlViewModel => _editControlNavigation.CurrentViewModel;

    public TrussSolver Solver { get; set; }
    private bool _isSolverSolving;
    public bool IsSolverSolving
    {
        get { return _isSolverSolving; }
        set
        {
            _isSolverSolving = value;
            OnPropertyChanged(nameof(IsSolverSolving));
        }
    }


    public HelixViewport3D viewport3D;


    public string DebugText => 
        $"Število členkov:\t{NodeService.NodeModels.Count}\n" +
        $"Število palic:\t{ElementService.ElementModels.Count}\n" +
        $"Število vizualnih elementov:\t{NodeService.ViewportItems.Count}";

    public ICommand DisplayEditControl { get; }
    public ICommand AddNodeControl { get; }
    public ICommand AddElementControl { get; }
    public ICommand FileNew { get; }
    public ICommand FileOpen { get; }
    public ICommand FileSave { get; }
    public ICommand FileSaveAs { get; }
    public ICommand OpenGitHub { get; }
    public ICommand CloseApp { get; }
    public ICommand StartSolver { get; }


    public TrussMainViewModel()
    {
        ViewportItems = new ObservableCollection<ModelVisual3D>();
        //ViewportItems.Add(new CameraController());
        ViewportItems.Add(new DefaultLights());
        //ViewportItems.Add(new CoordinateSystemVisual3D());

        TrussModel = new TrussMainModel(ViewportItems);

        NodeService = new NodeService(TrussModel.NodeModels, TrussModel.ElementModels, ViewportItems);
        ElementService = new ElementService(TrussModel.ElementModels, TrussModel.NodeModels, ViewportItems);


        _editControlNavigation = new NavigationBase() { CurrentViewModel = null };
        _editControlNavigation.CurrentViewModelChanged += () => OnPropertyChanged(nameof(CurrentEditControlViewModel));

        Solver = new TrussSolver(TrussModel.NodeModels, TrussModel.ElementModels, ViewportItems);
        Solver.IsSolvingChanged += (isSolving) => IsSolverSolving = isSolving;

        DisplayEditControl = new UpdateEditControlCommand(_editControlNavigation, NodeService, ElementService);
        AddNodeControl = new AddNodeControlCommand(_editControlNavigation, NodeService, ElementService);
        AddElementControl = new AddElementControlCommand(_editControlNavigation, ElementService, NodeService);
        FileNew = new FileNewCommand(TrussModel);
        FileOpen = new FileOpenCommand(TrussModel);
        FileSave = new FileSaveCommand(TrussModel);
        FileSaveAs = new FileSaveAsCommand(TrussModel);
        StartSolver = new StartSolverCommand(Solver, _editControlNavigation, ViewportItems, TrussModel.NodeModels, TrussModel.ElementModels);
        OpenGitHub = new OpenURLCommand("https://github.com/vito-tivadar/Palicne-Konstrukcije-MKE/");
        CloseApp = new CloseAppCommand();

        //ViewportCamera = CameraHelper.



        NodeService.NodeModels.CollectionChanged += OnCollectionChanged;
        ElementService.ElementModels.CollectionChanged += OnCollectionChanged;
        NodeService.ViewportItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(DebugText));   
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(DebugText));
        _editControlNavigation.CurrentViewModel = null;
    }
}
