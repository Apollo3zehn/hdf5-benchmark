from typing import Any

import pyfive

FILE_NAME = "../data/data/chunked_btree1_shuffle_deflate_1d.h5"

_file: dict[str, Any]
_dataset: Any

def setup():

    global _file
    global _dataset

    _file = pyfive.File(FILE_NAME)
    _dataset = _file["dataset"]

def run():
    return _dataset[:]

def test_chunked_btree1_shuffle_deflate_1d(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=1000)

    assert result[0] == 601088376405717203
