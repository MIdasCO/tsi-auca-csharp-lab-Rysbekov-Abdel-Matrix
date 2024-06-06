// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class MatrixIO
{
    public static async Task WriteMatrixAsync(Matrix matrix, Stream stream, string sep = " ")
    {
        using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            await writer.WriteLineAsync($"{matrix.Rows}{sep}{matrix.Columns}");
            for (int i = 0; i < matrix.Rows; i++)
            {
                string[] row = new string[matrix.Columns];
                for (int j = 0; j < matrix.Columns; j++)
                    row[j] = matrix[i, j].ToString();
                await writer.WriteLineAsync(string.Join(sep, row));
            }
        }
    }

    public static async Task<Matrix> ReadMatrixAsync(Stream stream, string sep = " ")
    {
        using (var reader = new StreamReader(stream))
        {
            string[] dimensions = (await reader.ReadLineAsync()).Split(sep);
            int rows = int.Parse(dimensions[0]);
            int columns = int.Parse(dimensions[1]);
            double[,] values = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                string[] rowValues = (await reader.ReadLineAsync()).Split(sep);
                for (int j = 0; j < columns; j++)
                    values[i, j] = double.Parse(rowValues[j]);
            }
            return new Matrix(values);
        }
    }

    public static void WriteMatrixBinary(Matrix matrix, Stream stream)
    {
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            writer.Write(matrix.Rows);
            writer.Write(matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
                for (int j = 0; j < matrix.Columns; j++)
                    writer.Write(matrix[i, j]);
        }
    }

    public static Matrix ReadMatrixBinary(Stream stream)
    {
        using (var reader = new BinaryReader(stream))
        {
            int rows = reader.ReadInt32();
            int columns = reader.ReadInt32();
            double[,] values = new double[rows, columns];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    values[i, j] = reader.ReadDouble();
            return new Matrix(values);
        }
    }

    public static async Task WriteMatrixJsonAsync(Matrix matrix, Stream stream)
    {
        double[][] array = new double[matrix.Rows][];
        for (int i = 0; i < matrix.Rows; i++)
        {
            array[i] = new double[matrix.Columns];
            for (int j = 0; j < matrix.Columns; j++)
            {
                array[i][j] = matrix[i, j];
            }
        }

        await JsonSerializer.SerializeAsync(stream, array);
    }

    public static async Task<Matrix> ReadMatrixJsonAsync(Stream stream)
    {
        double[][] array = await JsonSerializer.DeserializeAsync<double[][]>(stream);
        int rows = array.Length;
        int columns = rows > 0 ? array[0].Length : 0;
        double[,] values = new double[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            if (array[i].Length != columns)
                throw new ArgumentException("All rows must have the same number of columns.");
            for (int j = 0; j < columns; j++)
            {
                values[i, j] = array[i][j];
            }
        }

        return new Matrix(values);
    }

    public static void WriteMatrixToFile(string directory, string filename, Matrix matrix, Action<Matrix, Stream> writeAction)
    {
        using (var fileStream = new FileStream(Path.Combine(directory, filename), FileMode.Create, FileAccess.Write))
        {
            writeAction(matrix, fileStream);
        }
    }

    public static async Task WriteMatrixToFileAsync(string directory, string filename, Matrix matrix, Func<Matrix, Stream, Task> writeAction)
    {
        using (var fileStream = new FileStream(Path.Combine(directory, filename), FileMode.Create, FileAccess.Write))
        {
            await writeAction(matrix, fileStream);
        }
    }

    public static Matrix ReadMatrixFromFile(string file, Func<Stream, Matrix> readAction)
    {
        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            return readAction(fileStream);
        }
    }

    public static async Task<Matrix> ReadMatrixFromFileAsync(string file, Func<Stream, Task<Matrix>> readAction)
    {
        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
        {
            return await readAction(fileStream);
        }
    }
}
// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать