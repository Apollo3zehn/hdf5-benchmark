using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using HDF.PInvoke;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter_1d
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_no_filter_1d.h5";

    private long _fileId = -1;

    private long _datasetId = -1;

    private long _daplId = -1;

    private nint _buffer = Marshal.AllocHGlobal(1000 * 100 * sizeof(long));

    [GlobalSetup]
    public void GlobalSetup()
    {
        _fileId = H5F.open(FILE_PATH, H5F.ACC_RDONLY);

        if (_fileId < 0)
            throw new Exception("Could not open file");
        
        _datasetId = H5D.open(_fileId, "dataset");
        _daplId = H5P.create(H5P.DATASET_XFER);

        H5P.set_chunk_cache(_daplId, rdcc_nslots: 0, rdcc_nbytes: 0, rdcc_w0: 0);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
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
        var result = H5D.read(_datasetId, H5T.NATIVE_INT64, H5S.ALL, H5S.ALL, _daplId, _buffer);

        if (result < 0)
            throw new Exception("Could not read data");

        var ptr = (long*)_buffer;

        if (ptr[1] != 1 || ptr[100] != 100)
            throw new Exception("Invalid data");
    }
}