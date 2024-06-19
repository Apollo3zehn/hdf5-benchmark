using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using HDF.PInvoke;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter_2d_slice
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_no_filter_2d.h5";

    private long _fileId = -1;

    private long _datasetId = -1;

    private long _fileSpaceId = -1;

    private long _memSpaceId = -1;
    
    private nint _buffer = Marshal.AllocHGlobal(100 * sizeof(long));

    [IterationSetup]
    public void IterationSetup()
    {
        _fileId = H5F.open(FILE_PATH, H5F.ACC_RDONLY);

        if (_fileId < 0)
            throw new Exception("Could not open file");

        _datasetId = H5D.open(_fileId, "dataset");
        _fileSpaceId = H5D.get_space(_datasetId);
        _memSpaceId = H5S.create_simple(rank: 1, dims: [100], maxdims: [100]);

        var result = H5S.select_hyperslab(
            _fileSpaceId, 
            H5S.seloper_t.SET,
            start: [0, 100], 
            stride: [1, 100], 
            count: [1, 1], 
            block: [1, 100]
        );

        if (result < 0)
            throw new Exception("Could not select hyperslab");
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        if (H5I.is_valid(_fileSpaceId) > 0)
            _ = H5S.close(_fileSpaceId);

        if (H5I.is_valid(_datasetId) > 0)
            _ = H5D.close(_datasetId);

        if (H5I.is_valid(_fileId) > 0)
            _ = H5F.close(_fileId);
    }

    [Benchmark]
    public unsafe void Run()
    {
        var result = H5D.read(
            _datasetId, 
            mem_type_id: H5T.NATIVE_INT64, 
            mem_space_id: _memSpaceId, 
            file_space_id: _fileSpaceId, 
            plist_id: H5P.DEFAULT, 
            _buffer
        );

        if (result < 0)
            throw new Exception("Could not read data");

        var ptr = (long*)_buffer;

        if (ptr[0] != 100 || ptr[99] != 199)
            throw new Exception("Invalid data");
    }
}