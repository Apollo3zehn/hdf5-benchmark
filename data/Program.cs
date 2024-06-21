using HDF.PInvoke;

var faplId = H5P.create(H5P.FILE_ACCESS);
var version = H5F.libver_t.V110;

H5P.set_libver_bounds(faplId, version, version);

var fileId = H5F.create("data/chunked_new_chunk_indices_no_filter_2d.h5", H5F.ACC_TRUNC, H5P.DEFAULT, faplId);

TestUtils.AddChunkedDataset_Single_Chunk(fileId);
TestUtils.AddChunkedDataset_Implicit(fileId);
TestUtils.AddChunkedDataset_Fixed_Array(fileId);
TestUtils.AddChunkedDataset_Fixed_Array_Paged(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Elements(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Data_Blocks(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Secondary_Blocks(fileId);
TestUtils.AddChunkedDataset_BTree2(fileId);

H5F.close(fileId);