using BenchmarkDotNet.Attributes;
using PureHDF;
using PureHDF.VOL.Native;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree2_no_filter_2d
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_new_chunk_indices_no_filter_2d.h5";

    private NativeFile _h5File = default!;

    private NativeDataset _dataset = default!;

    private H5DatasetAccess _datasetAccess = new H5DatasetAccess(
        ChunkCache: new SimpleReadingChunkCache(byteCount: 2500 * 4 * sizeof(int))
    );

    private int[,] _buffer = new int[2500, 4];

    [GlobalSetup]
    public void GlobalSetup()
    {
        _h5File = H5File.OpenRead(FILE_PATH);
        _dataset = (NativeDataset)_h5File.Dataset("chunked_btree2");
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

        if (_buffer[0, 1] != 1 || _buffer[1, 0] != 4)
            throw new Exception("Invalid data");
    }
}