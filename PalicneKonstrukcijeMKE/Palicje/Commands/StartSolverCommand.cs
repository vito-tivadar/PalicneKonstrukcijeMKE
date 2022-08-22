using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;
using PalicneKonstrukcijeMKE.MVVMBase;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Solver;

namespace PalicneKonstrukcijeMKE.Palicje.Commands;

public class StartSolverCommand : CommandBase
{
    private readonly TrussSolver _solver;
    private readonly NavigationBase _navigation;
    private readonly Collection<ModelVisual3D> _viewportItems;
    private readonly Collection<NodeModel> nodeModels;
    private readonly Collection<ElementModel> elementModels;

    public StartSolverCommand(TrussSolver solver, NavigationBase navigation, Collection<ModelVisual3D> viewportItems, Collection<NodeModel> nodeModels, Collection<ElementModel> elementModels)
    {
        _solver = solver;
        _navigation = navigation;
        this._viewportItems = viewportItems;
        this.nodeModels = nodeModels;
        this.elementModels = elementModels;
        _navigation.CurrentViewModelChanged += () => OnCanExecuteChanged();
    }

    public override void Execute(object parameter)
    {
        Tuple<Collection<int>, double[], double[]>? resultsTuple = _solver.Solve();
        if(resultsTuple == null) App.MessageBlock.SetError("Konstrukcija ni bila preračunana!"); 
        else
        {
            SolverResults resultsWindow = new SolverResults(resultsTuple, _viewportItems, nodeModels, elementModels);
            resultsWindow.Show();
        }

    }

    public override bool CanExecute(object parameter)
    {
        return _navigation.CurrentViewModel == null && base.CanExecute(parameter);
    }
}
