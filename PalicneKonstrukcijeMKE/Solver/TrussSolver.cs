using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Media3D;
using PalicneKonstrukcijeMKE.Palicje.Interfaces;
using PalicneKonstrukcijeMKE.Palicje.Models;
using PalicneKonstrukcijeMKE.Utility;

namespace PalicneKonstrukcijeMKE.Solver;

public class TrussSolver
{
    private readonly Collection<NodeModel> _nodeModels;
    private readonly Collection<ElementModel> _elementModels;

    private readonly Collection<ModelVisual3D> _trussVisualItems;
    private readonly Collection<ModelVisual3D> _resultVisualItems;

    public Action<bool> IsSolvingChanged;
    private bool _isSolving;
    public bool IsSolving
    {
        get { return _isSolving; }
        set
        {
            if (_isSolving != value)
            {
                _isSolving = value;
                IsSolvingChanged?.Invoke(value);
            }
        }
    }

    public TrussSolver(Collection<NodeModel> nodeModels, Collection<ElementModel> elementModels, Collection<ModelVisual3D> trussVisualItems)
    {
        IsSolving = false;
        _nodeModels = nodeModels;
        _elementModels = elementModels;

        _trussVisualItems = trussVisualItems;
        _resultVisualItems = new Collection<ModelVisual3D>();
    }



    /// <returns>Returns <see langword="true"/> if is currently solving and <see langword="false"/> when done solving. </returns>
    public Tuple<Collection<int>, double[], double[]>? Solve()
    {
        if (IsSolving == true) return null;
        IsSolving = true;
        if (!IsSolvable())
        {
            IsSolving = false;
            return null;
        }
        int totalNumberOfDOFs = _nodeModels.Count();

        AssignDOF();
        double[] globalForceVector = LocalToGlobalForce(totalNumberOfDOFs);
        double[,] globalStiffnesMatrix = LocalToGlobalStiffnesMatrix(totalNumberOfDOFs);


        Collection<int> DOFtoKeepForDisplacements = new Collection<int>();
        for (int i = 0; i < _nodeModels.Count; i++)
        {
            BooleanVector nodeModelSupports = _nodeModels[i].Supports;
            if (!nodeModelSupports.X) DOFtoKeepForDisplacements.Add(3 * (i + 1) - 3);
            if (!nodeModelSupports.Y) DOFtoKeepForDisplacements.Add(3 * (i + 1) - 2);
            if (!nodeModelSupports.Z) DOFtoKeepForDisplacements.Add(3 * (i + 1) - 1);
        }
        Tuple<double[,], double[]> displacementTuple = RemoveRowsAndColumns(DOFtoKeepForDisplacements, globalStiffnesMatrix, globalForceVector);
        double[] displacements = GaussElimination(displacementTuple.Item1, displacementTuple.Item2, DOFtoKeepForDisplacements);
        
        /*
        double[] globalDisplacements = new double[totalNumberOfDOFs * 3];
        for (int i = 0; i < DOFtoKeepForDisplacements.Count; i++)
        {
            globalDisplacements[DOFtoKeepForDisplacements[i]] = displacements[i];
        }
        Collection<int> DOFtoKeepForReactions = new Collection<int>();
        for (int i = 0; i < _nodeModels.Count; i++)
        {
            BooleanVector nodeModelSupports = _nodeModels[i].Supports;
            if (nodeModelSupports.X) DOFtoKeepForReactions.Add(3 * (i + 1) - 3);
            if (nodeModelSupports.Y) DOFtoKeepForReactions.Add(3 * (i + 1) - 2);
            if (nodeModelSupports.Z) DOFtoKeepForReactions.Add(3 * (i + 1) - 1);
        }
        Tuple<double[,], double[]> reactionsTuple = RemoveRowsAndColumns(DOFtoKeepForReactions, globalStiffnesMatrix, globalForceVector);

        double[] reactions = CalculateReactions();
        */

        IsSolving = false;
        return Tuple.Create(DOFtoKeepForDisplacements, displacements, new double[] {});
    }

    private bool IsSolvable()
    {
        if (_nodeModels.Count > 1 && _elementModels.Count > 0) return true;
        else return false;
    }

    private void AssignDOF()
    {
        int dof = 1;
        foreach (NodeModel nodeModel in _nodeModels)
        {
            nodeModel.GlobalNodeNumber = dof;
            dof++;
        }
    }

    private double[] LocalToGlobalForce(int totalNumberOfDOFs)
    {
        double[] globalForceVector = new double[3 * totalNumberOfDOFs];
        foreach (NodeModel nodeModel in _nodeModels)
        {
            nodeModel.LocalToGlobalForce(ref globalForceVector);
        };
        return globalForceVector;
    }

    private double[,] LocalToGlobalStiffnesMatrix(int totalNumberOfDOFs)
    {
        double[,] globalStiffnessMatrix = new double[3 * totalNumberOfDOFs, 3 * totalNumberOfDOFs];
        foreach (ElementModel elementModel in _elementModels)
        {
            elementModel.LocalToGlobalStiffnesMatrix(ref globalStiffnessMatrix);
        }
        return globalStiffnessMatrix;
    }

    private Tuple<double[,], double[]> RemoveRowsAndColumns(Collection<int> DOFtoKeep, double[,] globalStiffnessMatrix, double[] globalForceVector/*, double[] globalDisplacementsVector*/)
    {
        double[,] finalStiffnessMatrix = new double[DOFtoKeep.Count, DOFtoKeep.Count];
        double[] finalForceVector = new double[DOFtoKeep.Count];

        for (int v = 0; v < DOFtoKeep.Count; v++)
        {
            for (int s = 0; s < DOFtoKeep.Count; s++)
            {
                finalStiffnessMatrix[v, s] = globalStiffnessMatrix[DOFtoKeep[v], DOFtoKeep[s]];
            }
            finalForceVector[v] = globalForceVector[DOFtoKeep[v]];
        }

        return Tuple.Create(finalStiffnessMatrix, finalForceVector);
    }

    private double[] GaussElimination(double[,] stiffnesMatrix, double[] forceVector, Collection<int> KeptDOF, bool findUnknowns = true)
    {
        int KeptDOFsize = KeptDOF.Count;
        for (int i = 0; i < KeptDOFsize; i++)
        {
            for (int v = i + 1; v < KeptDOFsize; v++)
            {
                double koef = stiffnesMatrix[v, i] / stiffnesMatrix[i, i];
                for (int s = 0; s < KeptDOFsize; s++)
                {
                    stiffnesMatrix[v, s] -= stiffnesMatrix[i, s] * koef;
                }
                forceVector[v] -= forceVector[i] * koef;
            }
        }

        if(findUnknowns == false) return forceVector;


        double temp = 0;
        double n = 0;
        double[] x = new double[KeptDOFsize];

        for (int row = KeptDOFsize - 1; row >= 0; row--)
        {
            for (int column = KeptDOFsize - 1; column >= 0; column--)
            {
                if (row == column)
                {
                    n = stiffnesMatrix[column, row];
                    break;
                }
                else
                {
                    temp += stiffnesMatrix[row, column] * forceVector[column];
                }
            }
            forceVector[row] = (forceVector[row] - temp) / n;
            x[row] = forceVector[row];
            temp = 0;
        }
        return forceVector;
    }

    private double[] CalculateReactions(double[,] stiffnesMatrix, double[] forceVector, double[] displacementsVector, Collection<int> KeptDOF)
    {





        return new double[] { };
    }
}
