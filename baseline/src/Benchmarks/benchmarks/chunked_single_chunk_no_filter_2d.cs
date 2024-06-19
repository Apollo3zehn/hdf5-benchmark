using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using HDF.PInvoke;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_single_chunk_no_filter_2d
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_new_chunk_indices_no_filter_2d.h5";

    private long _fileId = -1;

    private long _datasetId = -1;

    private long _daplId = -1;

    private nint _buffer = Marshal.AllocHGlobal(25000 * 4 * sizeof(int));

    [IterationSetup]
    public void IterationSetup()
    {
        _fileId = H5F.open(FILE_PATH, H5F.ACC_RDONLY);

        if (_fileId < 0)
            throw new Exception("Could not open file");
        
        _daplId = H5P.create(H5P.DATASET_ACCESS);

        H5P.set_chunk_cache(
            _daplId, 
            rdcc_nslots: nint.MaxValue, 
            rdcc_nbytes: 25000 * 4 * sizeof(int), 
            rdcc_w0: 0.75
        );

        _datasetId = H5D.open(_fileId, "chunked_single_chunk", _daplId);
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        if (H5I.is_valid(_daplId) > 0)
            _ = H5P.close(_daplId);

        if (H5I.is_valid(_datasetId) > 0)
            _ = H5D.close(_datasetId);

        if (H5I.is_valid(_fileId) > 0)
            _ = H5F.close(_fileId);
    }

    [Benchmark]
    public unsafe void Run()
    {
        var result = H5D.read(_datasetId, H5T.NATIVE_INT32, H5S.ALL, H5S.ALL, H5P.DEFAULT, _buffer);

        if (result < 0)
            throw new Exception("Could not read data");

        var ptr = (int*)_buffer;

        if (ptr[1] != 1)
            throw new Exception("Invalid data");
    }
}