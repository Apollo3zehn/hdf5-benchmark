import os
import random
import h5py

test_name = os.path.splitext(os.path.basename(__file__))[0]
file_path = f"data/{test_name}.h5"

random.seed(10)

with h5py.File(file_path, "w") as h5_file:
    shape = (1000*100,)
    chunks = (1000*100,)
    total_length = shape[0]

    dataset = h5_file.create_dataset(
        name="dataset",
        data=[random.randint(0, 2**63 - 1) for _ in range(total_length)],
        shape=shape,
        chunks=chunks,
        shuffle=True,
        compression="gzip")
