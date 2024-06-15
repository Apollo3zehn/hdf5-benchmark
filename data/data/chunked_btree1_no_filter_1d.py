import os
import h5py

test_name = os.path.splitext(os.path.basename(__file__))[0]
file_path = f"data/{test_name}.h5"

with h5py.File(file_path, "w") as h5_file:
    chunk_size = 1024
    chunk_count = 100
    total_length = chunk_count * chunk_size

    dataset = h5_file.create_dataset(
        name="chunked",
        data=range(0, total_length),
        chunks=(chunk_size,))
