using BenchmarkDotNet.Attributes;
using PureHDF;
using PureHDF.VOL.Native;

namespace Benchmarks;

[JsonExporterAttribute.Full]
public class chunked_btree1_no_filter
{
    const string FILE_PATH = "../../../../../../../../../../data/data/chunked_btree1_no_filter.h5";

    private NativeFile _h5File = default!;
    private IH5Dataset _dataset = default!;
    private long[] _buffer = new long[1024 * 100];

    [GlobalSetup]
    public void GlobalSetup()
    {
        _h5File = H5File.OpenRead(FILE_PATH);
        _dataset = _h5File.Dataset("chunked");
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _h5File.Dispose();
    }

    [Benchmark]
    public void Run()
    {
        _dataset.Read(buffer: _buffer);
    }
}