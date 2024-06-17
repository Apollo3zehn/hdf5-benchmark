using HDF.PInvoke;

var fileId = H5F.create("data/chunked_new_chunk_indices_no_filter_2d.h5", H5F.ACC_TRUNC);

TestUtils.AddChunkedDataset_Single_Chunk(fileId);
TestUtils.AddChunkedDataset_Implicit(fileId);
TestUtils.AddChunkedDataset_Fixed_Array(fileId);
TestUtils.AddChunkedDataset_Fixed_Array_Paged(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Elements(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Data_Blocks(fileId);
TestUtils.AddChunkedDataset_Extensible_Array_Secondary_Blocks(fileId);
TestUtils.AddChunkedDataset_BTree2(fileId);

H5F.close(fileId);