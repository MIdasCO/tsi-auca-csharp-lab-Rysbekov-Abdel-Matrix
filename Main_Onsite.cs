// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

class Main_Home
{
    private static readonly string calcDir = "calcDir";
    private static readonly string textDir = "textDir";
    private static readonly string binaryDir = "binaryDir";
    private static readonly string jsonDir = "jsonDir";
    private static readonly string aPrefix = "a";
    private static readonly string bPrefix = "b";
    private static readonly string extension = ".tsv";
    private static readonly string textExtension = ".csv";
    private static readonly string jsonExtension = ".json";
    private static readonly string sep = "\t";
    private static readonly string textSep = ";";

    public static async Task Main(string[] args)
    {
        Matrix[] a = CreateMatrixArray(50, 500, 100);
        Matrix[] b = CreateMatrixArray(50, 100, 500);

        PrepareDirectory(calcDir);
        PrepareDirectory(textDir);
        PrepareDirectory(binaryDir);
        PrepareDirectory(jsonDir);

        Task calcTask = Task.Run(async () =>
        {
            var tasks = new Task[]
            {
                PerformCalculation(a, b, "abScalar", MatrixOperations.ScalarProduct),
                PerformCalculation(a, b, "abMatrix", MatrixOperations.Multiply),
                PerformCalculation(b, a, "baScalar", MatrixOperations.ScalarProduct),
                PerformCalculation(b, a, "baMatrix", MatrixOperations.Multiply)
            };

            await Task.WhenAll(tasks);
        });

        Task writeAsyncTask = Task.Run(async () =>
        {
            Task aCsvTask = WriteToDirAsync(a, textDir, aPrefix, textExtension, (matrix, stream) => MatrixIO.WriteMatrixAsync(matrix, stream, textSep));
            Task bCsvTask = WriteToDirAsync(b, textDir, bPrefix, textExtension, (matrix, stream) => MatrixIO.WriteMatrixAsync(matrix, stream, textSep));
            Task aJsonTask = WriteToDirAsync(a, jsonDir, aPrefix, jsonExtension, MatrixIO.WriteMatrixJsonAsync);
            Task bJsonTask = WriteToDirAsync(b, jsonDir, bPrefix, jsonExtension, MatrixIO.WriteMatrixJsonAsync);

            await Task.WhenAll(aCsvTask, bCsvTask, aJsonTask, bJsonTask);
            Console.WriteLine("Write async finished");

            Task<Matrix[]> csvRead = ReadFromDirAsync(textDir, aPrefix, textExtension, stream => MatrixIO.ReadMatrixAsync(stream, textSep));
            Task<Matrix[]> jsonRead = ReadFromDirAsync(jsonDir, aPrefix, jsonExtension, MatrixIO.ReadMatrixJsonAsync);

            var readTasks = new Task<Matrix[]>[] { csvRead, jsonRead };
            await HandleReadTasks(readTasks, a);
        });

        WriteToDir(a, binaryDir, aPrefix, extension, MatrixIO.WriteMatrixBinary);
        WriteToDir(b, binaryDir, bPrefix, extension, MatrixIO.WriteMatrixBinary);
        var binA = ReadFromDir(binaryDir, aPrefix, extension, MatrixIO.ReadMatrixBinary);
        bool binEquals = CompareMatrixArrays(a, binA);
        Console.WriteLine($"Binary a equals: {binEquals}");

        await Task.WhenAll(calcTask, writeAsyncTask);
    }

    private static Matrix[] CreateMatrixArray(int count, int rows, int cols)
    {
        var matrices = new Matrix[count];
        for (int i = 0; i < count; i++)
        {
            matrices[i] = CreateRandomMatrix(rows, cols);
        }
        return matrices;
    }

    private static Matrix CreateRandomMatrix(int rows, int cols)
    {
        var rand = new Random();
        var values = new double[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                values[i, j] = rand.NextDouble() * 20 - 10;
            }
        }
        return new Matrix(values);
    }

    private static async Task PerformCalculation(Matrix[] a, Matrix[] b, string name, Func<Matrix, Matrix, Matrix> operation)
    {
        var results = new Matrix[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            results[i] = operation(a[i], b[i]);
        }
        await SaveResult(results, name);
    }

    private static async Task SaveResult(Matrix[] results, string name)
    {
        Console.WriteLine($"{name} calculation is finished");
        for (int i = 0; i < results.Length; i++)
        {
            string fileName = Path.Combine(calcDir, $"{name}_{i}{extension}");
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                await MatrixIO.WriteMatrixAsync(results[i], stream, sep);
            }
        }
    }

    private static void WriteToDir(Matrix[] matrices, string directory, string prefix, string extension, Action<Matrix, Stream> writeAction)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            using (var stream = new FileStream(Path.Combine(directory, $"{prefix}{i}{extension}"), FileMode.Create))
            {
                writeAction(matrices[i], stream);
            }
            if ((i + 1) % 10 == 0)
                Console.WriteLine($"{prefix}{i}{extension} write finished");
        }
        Console.WriteLine("Write finished");
    }

    private static Matrix[] ReadFromDir(string directory, string prefix, string extension, Func<Stream, Matrix> readAction)
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

    private static async Task WriteToDirAsync(Matrix[] matrices, string directory, string prefix, string extension, Func<Matrix, Stream, Task> writeAction)
    {
        for (int i = 0; i < matrices.Length; i++)
        {
            using (var stream = new FileStream(Path.Combine(directory, $"{prefix}{i}{extension}"), FileMode.Create))
            {
                await writeAction(matrices[i], stream);
            }
            if ((i + 1) % 10 == 0)
                Console.WriteLine($"{prefix}{i}{extension} write async finished");
        }
    }

    private static async Task<Matrix[]> ReadFromDirAsync(string directory, string prefix, string extension, Func<Stream, Task<Matrix>> readAction)
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

    private static async Task HandleReadTasks(Task<Matrix[]>[] readTasks, Matrix[] original)
    {
        var readResults = new List<Matrix[]>();
        foreach (var readTask in readTasks)
        {
            var result = await readTask;
            readResults.Add(result);
            Console.WriteLine($"{(readTask == readTasks[0] ? "CSV" : "JSON")} finished");
        }

        bool csvEquals = CompareMatrixArrays(original, readResults[0]);
        bool jsonEquals = CompareMatrixArrays(original, readResults[1]);
        Console.WriteLine($"CSV array equals: {csvEquals}");
        Console.WriteLine($"JSON array equals: {jsonEquals}");
    }

    private static bool CompareMatrixArrays(Matrix[] first, Matrix[] second)
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

    private static void PrepareDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
        Directory.CreateDirectory(directory);
    }
}
// Я тот пацан с MacBook я не смог скачать Visual Studio Code, вы сказали что бы я написал о том что я тот пацан и чуточку вы строго не будете проверать