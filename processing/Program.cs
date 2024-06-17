using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

// Load or create database
const string GH_PAGES_FOLDER = "../.gh-pages";
const string DATABASE_FILE_PATH = $"{GH_PAGES_FOLDER}/database.json";

Directory.CreateDirectory(GH_PAGES_FOLDER);

var database = File.Exists(DATABASE_FILE_PATH)
    ? JsonSerializer.Deserialize<List<CommitData>>(File.ReadAllText(DATABASE_FILE_PATH)) 
        ?? throw new Exception("invalid database")
    : new();

// Get commit info
var (commit, commitMessage) = GetCommitInfo();

if (database.Any(current => current.Commit == commit))
    throw new Exception("Commit already exists");

// Initialize new commit data object
var commitData = new CommitData(
    Commit: commit,
    CommitMessage: commitMessage,
    DateTime: DateTime.UtcNow,
    TestData: new()
);

database.Add(commitData);

// Extract test results
{
    // baseline
    const string BASELINE_ROOT = "../baseline/BenchmarkDotNet.Artifacts/results";
    ExtractDotNetResult(BASELINE_ROOT, "baseline" /* HDF5 1.10.6 (C) */);

    // dotnet-purehdf
    const string DOTNET_PUREHDF_ROOT = "../dotnet-purehdf/BenchmarkDotNet.Artifacts/results";
    ExtractDotNetResult(DOTNET_PUREHDF_ROOT, "dotnet-purehdf" /* PureHDF (.NET) */);

    // java-jhdf
    const string JAVA_JHDF_FILE_PATH = "../java-jhdf/test/jmh-result.json";
    ExtractJavaResult(JAVA_JHDF_FILE_PATH, "java-jhdf" /* jHDF (Java) */);

    // python-pyfive
    const string PYTHON_PYFIVE_FILE_PATH = "../python-pyfive/output.json";
    ExtractPythonResult(PYTHON_PYFIVE_FILE_PATH, "python-pyfive" /* pyfive (Python) */);
}

// Save database
var options = new JsonSerializerOptions { WriteIndented = true };
var jsonString = JsonSerializer.Serialize(database, options);

File.WriteAllText(DATABASE_FILE_PATH, jsonString);

// Methods and class definitions
void ExtractDotNetResult(string root, string library)
{
    var filePaths = Directory.GetFiles(root, "*.json");

    foreach (var filePath in filePaths)
    {
        var jsonString = File.ReadAllText(filePath);
        var report = JsonSerializer.Deserialize<JsonElement>(jsonString);
        var benchmark = report.GetProperty("Benchmarks")[0];

        var testName = benchmark
            .GetProperty("Type")
            .GetString() ?? throw new Exception("invalid raw data");

        var statistics = benchmark
            .GetProperty("Statistics");

        var mean = statistics
            .GetProperty("Mean")
            .GetDouble() / 1000;

        var benchmarkResult = new BenchmarkResult(
            Mean: mean
        );

        SetBenchmarkResult(testName, library, benchmarkResult);
    }
}

void ExtractJavaResult(string filePath, string library)
{
    var jsonString = File.ReadAllText(filePath);
    var result = JsonSerializer.Deserialize<JsonElement>(jsonString);
    
    foreach (var item in result.EnumerateArray())
    {
        var testFullName = item
            .GetProperty("benchmark")
            .GetString() ?? throw new Exception("invalid raw data");

        var testName = Regex
            .Match(testFullName, @"^.*\.([^\.]+)\..*$")
            .Groups[1].Value;

        var primaryMetric = item
            .GetProperty("primaryMetric");

        var mean = primaryMetric
            .GetProperty("score")
            .GetDouble();

        var benchmarkResult = new BenchmarkResult(
            Mean: mean
        );

        SetBenchmarkResult(testName, library, benchmarkResult);
    }
}

void ExtractPythonResult(string filePath, string library)
{
    var jsonString = File.ReadAllText(filePath);
    var output = JsonSerializer.Deserialize<JsonElement>(jsonString);

    var benchmarks = output
        .GetProperty("benchmarks");

    foreach (var item in benchmarks.EnumerateArray())
    {
        var testFullName = item
            .GetProperty("name")
            .GetString() ?? throw new Exception("invalid raw data");

        var testName = testFullName.Substring(5);

        var stats = item
            .GetProperty("stats");

        var mean = stats
            .GetProperty("mean")
            .GetDouble() * 1000 * 1000;

        var benchmarkResult = new BenchmarkResult(
            Mean: mean
        );

        SetBenchmarkResult(testName, library, benchmarkResult);
    }
}

void SetBenchmarkResult(string testName, string library, BenchmarkResult benchmarkResult)
{
    if (!commitData.TestData.TryGetValue(testName, out var libraryData))
    {
        libraryData = new();
        commitData.TestData[testName] = libraryData;
    }

    libraryData[library] = benchmarkResult;
}

(string commit, string commitMessage) GetCommitInfo()
{
    var result = default(string);

    var gitLogProcess = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "log --oneline -n 1",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = false,
            CreateNoWindow = true
        }
    };

    gitLogProcess.Start();

    while (!gitLogProcess.StandardOutput.EndOfStream)
    {
        var line = gitLogProcess.StandardOutput.ReadLine();

        if (result is null)
            result = line;

        else
            result += Environment.NewLine + line;
    }

    if (result is null)
        throw new Exception("Failed to get commit info.");

    var match = Regex.Match(result, @"^(.*?)\s(.*)$");

    if (!match.Success)
        throw new Exception("Failed to get commit info.");

    return (match.Groups[1].Value, match.Groups[2].Value);
}

record CommitData(
    string Commit,
    string CommitMessage,
    DateTime DateTime,
    //         test               lib     result
    Dictionary<string, Dictionary<string, BenchmarkResult>> TestData
);

record BenchmarkResult(
    double Mean /* µs */
);