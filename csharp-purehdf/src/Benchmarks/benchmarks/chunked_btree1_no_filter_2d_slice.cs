using BenchmarkDotNet.Attributes;
using PureHDF;
using PureHDF.Selections;
using PureHDF.VOL.Native;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter_2d_slice
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_no_filter_2d.h5";

    private NativeFile _h5File = default!;

    private IH5Dataset _dataset = default!;

    private readonly long[] _buffer = new long[100];

    private readonly HyperslabSelection _fileSelection = new(
        rank: 2, 
        starts: [0, 100], 
        blocks: [1, 100]
    );

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
        _dataset.Read(buffer: _buffer, fileSelection: _fileSelection);

        if (_buffer[0] != 100 || _buffer[99] != 199)
            throw new Exception("Invalid data");
    }
}