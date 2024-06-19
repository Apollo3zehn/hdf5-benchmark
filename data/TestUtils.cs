using System.Text;
using HDF.PInvoke;

// copy from https://github.com/Apollo3zehn/PureHDF/blob/dev/tests/PureHDF.Tests/TestUtils/TestUtils%40chunked.cs

public static class TestUtils
{
    private static int[] _testData = Enumerable.Range(0, 100_000).ToArray();

    public static unsafe void AddChunkedDataset_Single_Chunk(long fileId)
    {
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);
        var length = (ulong)_testData.Length / 4;
        var dims = new ulong[] { length, 4 };

        _ = H5P.set_chunk(dcpl_id, 2, [length, 4]);

        Add(fileId, "chunked_single_chunk", H5T.NATIVE_INT32, _testData.AsSpan(), dims, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Implicit(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims = new ulong[] { length, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [1000, 3]);
        _ = H5P.set_alloc_time(dcpl_id, H5D.alloc_time_t.EARLY);

        Add(fileId, "chunked_implicit", H5T.NATIVE_INT32, _testData.AsSpan(), dims, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Fixed_Array(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims = new ulong[] { length, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [1000, 3]);

        Add(fileId, "chunked_fixed_array", H5T.NATIVE_INT32, _testData.AsSpan(), dims, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Fixed_Array_Paged(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims = new ulong[] { length, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [1, 3]);

        Add(fileId, "chunked_fixed_array_paged", H5T.NATIVE_INT32, _testData.AsSpan(), dims, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Extensible_Array_Elements(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims0 = new ulong[] { length, 4 };
        var dims1 = new ulong[] { H5S.UNLIMITED, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [1000, 3]);

        Add(fileId, "chunked_extensible_array_elements", H5T.NATIVE_INT32, _testData.AsSpan(), dims0, dims1, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Extensible_Array_Data_Blocks(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims0 = new ulong[] { length, 4 };
        var dims1 = new ulong[] { H5S.UNLIMITED, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [100, 3]);

        Add(fileId, "chunked_extensible_array_data_blocks", H5T.NATIVE_INT32, _testData.AsSpan(), dims0, dims1, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_Extensible_Array_Secondary_Blocks(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims0 = new ulong[] { length, 4 };
        var dims1 = new ulong[] { H5S.UNLIMITED, 4 };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [3, 3]);

        Add(fileId, "chunked_extensible_array_secondary_blocks", H5T.NATIVE_INT32, _testData.AsSpan(), dims0, dims1, cpl: dcpl_id);
        _ = H5P.close(dcpl_id);
    }

    public static unsafe void AddChunkedDataset_BTree2(long fileId)
    {
        var length = (ulong)_testData.Length / 4;
        var dims0 = new ulong[] { length, 4 };
        var dims1 = new ulong[] { H5S.UNLIMITED, H5S.UNLIMITED };
        var dcpl_id = H5P.create(H5P.DATASET_CREATE);

        _ = H5P.set_chunk(dcpl_id, 2, [1000, 3]);

        Add(fileId, "chunked_btree2", H5T.NATIVE_INT32, _testData.AsSpan(), dims0, dims1, cpl: dcpl_id);

        _ = H5P.close(dcpl_id);
    }

    public static unsafe void Add<T>(long fileId, string elementName, long typeId, Span<T> data, ulong[] dims0, ulong[]? dims1 = default, long cpl = 0, long apl = 0)
        where T : unmanaged
    {
        fixed (void* dataPtr = data)
        {
            Add(fileId, elementName, typeId, dataPtr, dims0, dims1, cpl, apl);
        }
    }

    public static unsafe void Add(long fileId, string elementName, long typeId, void* dataPtr, ulong[] dims, ulong[]? maxDims = default, long cpl = 0, long apl = 0)
    {
        maxDims ??= dims;

        var spaceId = H5S.create_simple(dims.Length, dims, maxDims);
        Add(fileId, elementName, typeId, dataPtr, spaceId, cpl, apl);
        _ = H5S.close(spaceId);
    }

    public static unsafe void Add(long fileId, string elementName, long typeId, void* dataPtr, long spaceId, long cpl = 0, long apl = 0)
    {
        long id;

        id = H5D.create(fileId, Encoding.UTF8.GetBytes(elementName), typeId, spaceId, dcpl_id: cpl, dapl_id: apl);

        if (id == -1)
            throw new Exception("Could not create dataset.");

        if ((int)dataPtr != 0)
            _ = H5D.write(id, typeId, spaceId, H5S.ALL, 0, new IntPtr(dataPtr));

        _ = H5D.close(id);
    }
}