# hdf5-benchmark

This repository contains a set of benchmarks to compare the performance of different independent HDF5 implementations against the original HDF5 C library.

Every time there is a commit to the master branch of this repository, the benchmarks are run in the GitHub Actions CI pipeline and the graphs are updated with the new results.

The original C library is not included directly, but through the thin C# wrapper `HDF5.PInvoke.1.10`, which should not affect the benchmark performance.

Please create a PR if you want to add more benchmarks or improve existing ones.

You can view the graphs here: https://apollo3zehn.github.io/hdf5-benchmark/

# Development
Add `--recurse-submodules` to your `git clone` command to clone the submodules as well. See `.github/workflows/benchmark.yml` to see how to run the individual benchmarks.