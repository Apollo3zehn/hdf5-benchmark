using BenchmarkDotNet.Attributes;
using PureHDF;
using PureHDF.VOL.Native;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter_2d
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_no_filter_2d.h5";

    private NativeFile _h5File = default!;

    private NativeDataset _dataset = default!;

    private H5DatasetAccess _datasetAccess = new H5DatasetAccess(
        ChunkCache: new SimpleReadingChunkCache(byteCount: 1000 * 10000 * sizeof(long))
    );

    private readonly long[,] _buffer = new long[1000, 10000];

    [GlobalSetup]
    public void GlobalSetup()
    {
        _h5File = H5File.OpenRead(FILE_PATH);
        _dataset = (NativeDataset)_h5File.Dataset("dataset");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _h5File.Dispose();
    }

    [Benchmark]
    public void Run()
    {
        _dataset.Read(buffer: _buffer, datasetAccess: _datasetAccess);

        if (_buffer[0, 1] != 1 || _buffer[0, 100] != 100 || _buffer[999, 9999] != 1000 * 10000 - 1)
            throw new Exception("Invalid data");
    }
}