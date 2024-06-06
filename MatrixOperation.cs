// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать
using System;
using System.Threading.Tasks;

public static class MatrixOperations
{
    public static Matrix Transpose(Matrix matrix)
    {
        double[,] result = new double[matrix.Columns, matrix.Rows];
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[j, i] = matrix[i, j];
            }
        }
        return new Matrix(result);
    }

    public static Matrix Multiply(Matrix matrix, double scalar)
    {
        double[,] result = new double[matrix.Rows, matrix.Columns];
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                result[i, j] = matrix[i, j] * scalar;
            }
        }
        return new Matrix(result);
    }

    public static Matrix Add(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("Matrices must have the same dimensions for addition.");

        double[,] result = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] + b[i, j];
            }
        }
        return new Matrix(result);
    }

    public static Matrix Subtract(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("Matrices must have the same dimensions for subtraction.");

        double[,] result = new double[a.Rows, a.Columns];
        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] - b[i, j];
            }
        }
        return new Matrix(result);
    }

    public static Matrix Multiply(Matrix a, Matrix b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentException("The number of columns in the first matrix must be equal to the number of rows in the second matrix.");

        double[,] result = new double[a.Rows, b.Columns];
        Matrix bTransposed = Transpose(b);

        Parallel.For(0, a.Rows, i =>
        {
            for (int j = 0; j < b.Columns; j++)
            {
                double sum = 0;
                for (int k = 0; k < a.Columns; k++)
                {
                    sum += a[i, k] * bTransposed[j, k];
                }
                result[i, j] = sum;
            }
        });
        return new Matrix(result);
    }
    public static (Matrix Inverse, double Determinant) Inverse(Matrix matrix)
    {
        if (matrix.Rows != matrix.Columns)
            throw new ArgumentException("Only square matrices can be inverted.");

        int n = matrix.Rows;
        double[,] a = (double[,])matrix.Values();
        double[,] inv = new double[n, n];
        double det = 1;

        for (int i = 0; i < n; i++)
            inv[i, i] = 1;

        for (int i = 0; i < n; i++)
        {
            int k = i;
            for (int j = i + 1; j < n; j++)
                if (Math.Abs(a[j, i]) > Math.Abs(a[k, i]))
                    k = j;

            if (Math.Abs(a[k, i]) < 1e-9)
                throw new InvalidOperationException("Matrix is not invertible.");

            for (int j = 0; j < n; j++)
            {
                double temp = a[i, j];
                a[i, j] = a[k, j];
                a[k, j] = temp;

                temp = inv[i, j];
                inv[i, j] = inv[k, j];
                inv[k, j] = temp;
            }

            det *= a[i, i] * (i != k ? -1 : 1);

            for (int j = 0; j < n; j++)
            {
                if (j != i)
                {
                    double c = a[j, i] / a[i, i];
                    for (int l = 0; l < n; l++)
                    {
                        a[j, l] -= a[i, l] * c;
                        inv[j, l] -= inv[i, l] * c;
                    }
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            double norm = a[i, i];
            for (int j = 0; j < n; j++)
                inv[i, j] /= norm;
        }

        return (new Matrix(inv), det);
    }

}
// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать