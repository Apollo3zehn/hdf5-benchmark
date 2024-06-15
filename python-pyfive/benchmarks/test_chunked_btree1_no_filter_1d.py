import urllib.request
from typing import Any

import pyfive

FILE_NAME = "../data/data/chunked_btree1_no_filter_1d.h5"

_file: dict[str, Any]

def setup():

    global _file
    _file = pyfive.File(FILE_NAME)

def run():
    dataset = _file["chunked"]
    return dataset

def test(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=100)
    assert result[1] == 1
    assert result[-1] == result.len() - 1
