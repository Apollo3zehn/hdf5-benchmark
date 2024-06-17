# hdf5-benchmark

This repository contains a set of benchmarks to compare the performance of different independent HDF5 implementations against the original HDF5 C library.

Every time there is a commit to the master branch of this repository, the benchmarks are run in the GitHub Actions CI pipeline and the graphs are updated with the new results.

The original C library is not included directly, but through the thin C# wrapper `HDF5.PInvoke.1.10`, which should not affect the benchmark performance.

Please create a PR if you want to add more benchmarks or improve existing ones.

You can view the graphs here: https://apollo3zehn.github.io/hdf5-benchmark/

# Development
Add `--recurse-submodules` to your `git clone` command to clone the submodules as well. See `.github/workflows/benchmark.yml` to see how to run the individual benchmarks.

# Remarks
Chunk caches are very important to improve performance. However, they also impact benchmarks negatively in that when the benchmark repeats the read operation on the very same dataset multiple times, no real read operation happens anymore and the benchmark result becomes useless. Thus the following benchmark principles need to be applied, especially to the baseline project:

- 1D array: no cache needed as all data are read only once = disable cache

- 2D array: make cache size match the size of the dataset and reopen the dataset for each loop iteration. This adds a slight overhead but since there is a metadata chache, it should be OK.

Closing and reopening is currently not required for `PureHDF` because the lifetime of the cache is only during the read operation itself. However, this might change in future.