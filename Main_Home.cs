// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать
using System;
using System.IO;
using System.Threading.Tasks;

public class Program
{
    public static Matrix CreateRandomMatrix(int rows, int columns)
    {
        Random random = new Random();
        double[,] values = new double[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                values[i, j] = random.NextDouble() * 20.0 - 10.0;
            }
        }
        return new Matrix(values);
    }

    public static Matrix[] MultiplyMatrixArrays(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length)
            throw new ArgumentException("Arrays must be of equal length.");

        Matrix[] result = new Matrix[first.Length];
        for (int i = 0; i < first.Length; i++)
        {
            result[i] = MatrixOperations.Multiply(first[i], second[i]);
        }
        return result;
    }


    public static double CalculateScalarProduct(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length)
            throw new ArgumentException("Arrays must be of equal length.");

        double totalSum = 0.0;
        for (int i = 0; i < first.Length; i++)
        {
            Matrix productMatrix = MatrixOperations.Multiply(first[i], second[i]);
            double sumOfProductMatrix = 0.0;
            for (int row = 0; row < productMatrix.Rows; row++)
            {
                for (int col = 0; col < productMatrix.Columns; col++)
                {
                    sumOfProductMatrix += productMatrix[row, col];
                }
            }
            totalSum += sumOfProductMatrix;
        }
        return totalSum;
    }

    public static void WriteMatrixArray(Matrix[] matrices, string directory, string prefix, string extension, Action<Matrix, Stream> writeAction)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            using (var stream = File.Create(Path.Combine(directory, $"{prefix}{i}.{extension}")))
            {
                writeAction(matrices[i], stream);
            }
            if ((i + 1) % 10 == 0)
                Console.WriteLine($"Written {i + 1} matrices.");
        }
    }

    public static async Task WriteMatrixArrayAsync(Matrix[] matrices, string directory, string prefix, string extension, Func<Matrix, Stream, Task> writeAction)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            using (var stream = File.Create(Path.Combine(directory, $"{prefix}{i}.{extension}")))
            {
                await writeAction(matrices[i], stream);
            }
            if ((i + 1) % 10 == 0)
                Console.WriteLine($"Written {i + 1} matrices.");
        }
    }

    public static Matrix[] ReadMatrixArray(string directory, string prefix, string extension, Func<Stream, Matrix> readAction)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles($"{prefix}*.{extension}");
        Matrix[] matrices = new Matrix[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            using (var stream = files[i].OpenRead())
            {
                matrices[i] = readAction(stream);
            }
        }
        return matrices;
    }

    public static async Task<Matrix[]> ReadMatrixArrayAsync(string directory, string prefix, string extension, Func<Stream, Task<Matrix>> readAction)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(directory);
        FileInfo[] files = dirInfo.GetFiles($"{prefix}*.{extension}");
        Matrix[] matrices = new Matrix[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            using (var stream = files[i].OpenRead())
            {
                matrices[i] = await readAction(stream);
            }
        }
        return matrices;
    }

    public static bool CompareMatrixArrays(Matrix[] first, Matrix[] second)
    {
        if (first.Length != second.Length)
            return false;

        for (int i = 0; i < first.Length; i++)
        {
            if (!first[i].Equals(second[i]))
                return false;
        }
        return true;
    }
}
// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать