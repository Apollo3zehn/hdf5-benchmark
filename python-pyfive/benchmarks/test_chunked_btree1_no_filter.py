import urllib.request
from typing import Any

import pyfive

_file: dict[str, Any]

def setup():

    global _file

    FILE_NAME = "/tmp/chunked_btree1_no_filter.h5"

    with urllib.request.urlopen('https://raw.githubusercontent.com/hdf5-benchmark/data/main/data/chunked_btree1_no_filter.h5') as response:
        with open(FILE_NAME,'wb') as target_file:
            target_file.write(response.read())

    _file = pyfive.File(FILE_NAME)

def run():
    dataset = _file["chunked"]
    return dataset

def test(benchmark):
    result = benchmark.pedantic(run, setup=setup, iterations=1, rounds=100)
    assert result[1] == 1
    assert result[-1] == result.len() - 1

