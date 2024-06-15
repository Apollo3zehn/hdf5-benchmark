from typing import Any

import pyfive

FILE_NAME = "../data/data/chunked_btree1_no_filter_2d.h5"

_file: dict[str, Any]

def setup():

    global _file
    _file = pyfive.File(FILE_NAME)

def run():
    dataset = _file["dataset"]
    return dataset

def test_chunked_btree1_no_filter_2d(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=1000)

    assert result[0, 1] == 1
    assert result[0, 100] == 100
    assert result[999, 999] == 1000 * 1000 - 1
