using System;

namespace PalicneKonstrukcijeMKE.Utility;

public struct BooleanVector
{
    public bool X { get; set; }
    public bool Y { get; set; }
    public bool Z { get; set; }

    public BooleanVector(bool x = false, bool y = false, bool z = false)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static bool Equals(BooleanVector booleanVector1, BooleanVector booleanVector2)
    {
        if (booleanVector1.X == booleanVector2.X && booleanVector1.Y == booleanVector2.Y)
            return booleanVector1.Z == booleanVector2.Z;

        return false;
    }

    public override bool Equals(object o)
    {
        if (o == null || !(o is BooleanVector)) return false;

        return Equals(this, (BooleanVector)o);
    }

    public static bool operator ==(BooleanVector booleanVector1, BooleanVector booleanVector2)
    {
        if (booleanVector1.X == booleanVector2.X && booleanVector1.Y == booleanVector2.Y)
            return booleanVector1.Z == booleanVector2.Z;

        return false;
    }

    public static bool operator !=(BooleanVector booleanVector1, BooleanVector booleanVector2)
    {
        return !(booleanVector1 == booleanVector2);
    }

    /// <summary>
    /// Converts <see cref="BooleanVector"/> to <see cref="string"/>
    /// </summary>
    /// <param name="SurroundWithBrackets">If <see langword="true"/> output will be surrounded with brackets <code>(True True False)</code></param>
    /// <param name="SeparateWithComma">If <see langword="true"/> values will be separated with commas <code>True,True,False</code>
    /// If <see langword="false"/> values will be separated with spaces <code>True True False</code>
    /// </param>
    /// <returns>values as one string:<code>True True False</code></returns>
    public string ToString(bool SurroundWithBrackets = false, bool SeparateWithComma = false)
    {
        string returnValue;
        if (SeparateWithComma)
            returnValue = $"{X.ToString()},{Y.ToString()},{Z.ToString()}";
        else
            returnValue = $"{X.ToString()} {Y.ToString()} {Z.ToString()}";

        if (SurroundWithBrackets) return $"({returnValue})";
        return returnValue;
    }
}
