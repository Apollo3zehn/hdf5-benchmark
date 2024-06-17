from typing import Any

import pyfive

FILE_NAME = "../data/data/chunked_new_chunk_indices_no_filter_2d.h5"

_file: dict[str, Any]
_dataset: Any

def setup():

    global _file
    global _dataset

    _file = pyfive.File(FILE_NAME)
    _dataset = _file["chunked_fixed_array"]

def run():
    return _dataset[:]

def test_chunked_fixed_array_no_filter_2d(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=1000)

    assert result[0, 1] == 1
    assert result[1, 0] == 4
