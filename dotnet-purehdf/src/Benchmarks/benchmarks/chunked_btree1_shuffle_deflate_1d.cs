using BenchmarkDotNet.Attributes;
using PureHDF;
using PureHDF.Filters;
using PureHDF.VOL.Native;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_shuffle_deflate_1d
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_shuffle_deflate_1d.h5";

    private NativeFile _h5File = default!;
    
    private NativeDataset _dataset = default!;

    private readonly long[] _buffer = new long[1000 * 1000];

    private H5DatasetAccess _datasetAccess = new H5DatasetAccess(
        ChunkCache: new SimpleReadingChunkCache(chunkSlotCount: 0, byteCount: 0)
    );

    [GlobalSetup]
    public void GlobalSetup()
    {
        H5Filter.Register(new DeflateISALFilter());

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

        if (_buffer[0] != 601088376405717203)
            throw new Exception("Invalid data");
    }
}