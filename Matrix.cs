// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать
using System;

public class Matrix
{
    private double[,] _values;
    private int? _lazyHashCode = null;

    public int Rows => _values.GetLength(0);
    public int Columns => _values.GetLength(1);

    public Matrix(double[,] values)
    {
        _values = values;
    }

    public double this[int i, int j]
    {
        get { return _values[i, j]; }
    }

    public static Matrix Zero(int r, int c)
    {
        return new Matrix(new double[r, c]);
    }

    public static Matrix Zero(int n)
    {
        return Zero(n, n);
    }

    public static Matrix Identity(int n)
    {
        var result = new Matrix(new double[n, n]);
        for (int i = 0; i < n; i++)
        {
            result._values[i, i] = 1;
        }
        return result;
    }

    public override string ToString()
    {
        var result = "";
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
                result += _values[i, j] + " ";
            result += "\\n";
        }
        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix m && m.Rows == Rows && m.Columns == Columns)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (_values[i, j] != m._values[i, j]) return false;
                }
            }
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        if (_lazyHashCode is null)
        {
            int hashCode = 17;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    hashCode = hashCode * 31 + _values[i, j].GetHashCode();
                }
            }
            _lazyHashCode = hashCode;
        }
        return _lazyHashCode.Value;
    }

    public static Matrix operator +(Matrix a, Matrix b) => Operate(a, b, (x, y) => x + y);
    public static Matrix operator -(Matrix a, Matrix b) => Operate(a, b, (x, y) => x - y);
    public static Matrix operator *(Matrix a, double scalar) => Operate(a, scalar, (x, s) => x * s);
    public static Matrix operator /(Matrix a, double scalar) => Operate(a, scalar, (x, s) => x / s);
    public static Matrix operator ~(Matrix a) => a.Transpose();

    public Matrix Transpose()
    {
        double[,] result = new double[Columns, Rows];
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                result[i, j] = _values[j, i];
            }
        }
        return new Matrix(result);
    }

    private static Matrix Operate(Matrix a, Matrix b, Func<double, double, double> operation)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("Matrices must be of the same dimensions");
        double[,] result = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < a.Columns; j++)
                result[i, j] = operation(a._values[i, j], b._values[i, j]);
        return new Matrix(result);
    }

    private static Matrix Operate(Matrix a, double scalar, Func<double, double, double> operation)
    {
        double[,] result = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++)
            for (int j = 0; j < a.Columns; j++)
                result[i, j] = operation(a._values[i, j], scalar);
        return new Matrix(result);
    }
}
// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать