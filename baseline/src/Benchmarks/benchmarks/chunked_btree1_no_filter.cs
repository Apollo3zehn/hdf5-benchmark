using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using HDF.PInvoke;

namespace Benchmark;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter
{
    const string FILE_NAME = "chunked_no_filter.h5";

    private long _fileId = -1;
    private long _datasetId = -1;
    private nint _buffer = Marshal.AllocHGlobal(1024 * 100 * sizeof(long));

    [GlobalSetup]
    public void GlobalSetup()
    {
        using (var client = new HttpClient())
        {
            using var sourceStream = client.GetStreamAsync("https://raw.githubusercontent.com/hdf5-benchmark/data/main/data/chunked_btree1_no_filter.h5");
            using var targetStream = new FileStream(FILE_NAME, FileMode.Create);

            sourceStream.Result.CopyTo(targetStream);
        }

        _fileId = H5F.open(FILE_NAME, H5F.ACC_RDONLY);

        if (_fileId < 0)
            throw new Exception("Could not open file");
        
        _datasetId = H5D.open(_fileId, "chunked");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        if (H5I.is_valid(_datasetId) > 0)
            _ = H5D.close(_datasetId);

        if (H5I.is_valid(_fileId) > 0)
            _ = H5F.close(_fileId);

        if (File.Exists(FILE_NAME))
            File.Delete(FILE_NAME);
    }

    [Benchmark]
    public unsafe void Run()
    {
        var result = H5D.read(_datasetId, H5T.NATIVE_INT64, H5S.ALL, H5S.ALL, H5P.DEFAULT, _buffer);

        if (result < 0)
            throw new Exception("Could not read data");
    }
}