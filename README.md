# hdf5-benchmark

This repository contains a set of benchmarks to compare the performance of different independent HDF5 implementations against the original HDF5 C library.

Every time there is a commit to the master branch of this repository, the benchmarks are run in the GitHub Actions CI pipeline and the graphs are updated with the new results.

The original C library is not included directly, but through the thin C# wrapper `HDF5.PInvoke.1.10`, which should not affect the benchmark performance.

Please create a PR if you want to add more benchmarks or improve existing ones.

You can view the graphs here: https://apollo3zehn.github.io/hdf5-benchmark/

# Development
Add `--recurse-submodules` to your `git clone` command to clone the submodules as well. See `.github/workflows/benchmark.yml` to see how to run the individual benchmarks.

# Remarks
Chunk caches are very important to improve performance. However, they also impact benchmarks negatively in that when the benchmark repeats the read operation on the very same dataset multiple times, no real read operation happens anymore and the benchmark result becomes useless. Thus the following benchmark principles need to be applied:

- 1D array: no cache needed as all data are read only once = disable cache

- 2D array: make cache size exactly the size of the dataset, so that when the dataset is being re-read again in the next loop iteration, the cache is full which forces real read operations to happen