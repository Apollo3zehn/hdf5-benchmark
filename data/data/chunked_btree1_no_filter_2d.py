import os
import h5py

test_name = os.path.splitext(os.path.basename(__file__))[0]
file_path = f"data/{test_name}.h5"

with h5py.File(file_path, "w") as h5_file:
    shape = (1000, 1000)
    chunks = (100, 100)
    total_length = shape[0] * shape[1]

    dataset = h5_file.create_dataset(
        name="dataset",
        data=range(0, total_length),
        shape=shape,
        chunks=chunks)
