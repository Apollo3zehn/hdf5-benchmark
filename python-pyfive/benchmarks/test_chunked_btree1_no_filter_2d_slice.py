from typing import Any

import pyfive

FILE_NAME = "../data/data/chunked_btree1_no_filter_2d.h5"

_file: dict[str, Any]
_dataset: Any

def setup():

    global _file
    global _dataset

    _file = pyfive.File(FILE_NAME)
    _dataset = _file["dataset"]

def run():
    return _dataset[0, 100:200]

def test_chunked_btree1_no_filter_2d_slice(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=1000)

    assert result[0] == 100
    assert result[99] == 199
