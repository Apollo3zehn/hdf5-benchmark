package org.java_jhdf;

import java.nio.file.Paths;
import java.util.concurrent.TimeUnit;

import org.openjdk.jmh.annotations.Benchmark;
import org.openjdk.jmh.annotations.BenchmarkMode;
import org.openjdk.jmh.annotations.Fork;
import org.openjdk.jmh.annotations.Measurement;
import org.openjdk.jmh.annotations.Mode;
import org.openjdk.jmh.annotations.OutputTimeUnit;
import org.openjdk.jmh.annotations.Scope;
import org.openjdk.jmh.annotations.Setup;
import org.openjdk.jmh.annotations.State;
import org.openjdk.jmh.annotations.TearDown;
import org.openjdk.jmh.annotations.Warmup;

import io.jhdf.HdfFile;
import io.jhdf.api.Dataset;

public class chunked_implicit_no_filter_2d {

    @State(Scope.Thread)
    public static class MyState {
        public HdfFile HdfFile;
        public Dataset Dataset;

        @Setup()
        public void setup() {
            HdfFile = new HdfFile(Paths.get("../../data/data/chunked_new_chunk_indices_no_filter_2d.h5"));
            Dataset = HdfFile.getDatasetByPath("/chunked_implicit");
        }
        
        @TearDown()
        public void tearDown() {
            HdfFile.close();
        }
    }

    @Warmup(iterations = 10, time = 500, timeUnit = TimeUnit.MILLISECONDS)
    @Measurement(iterations = 20, time = 500, timeUnit = TimeUnit.MILLISECONDS)
    @Benchmark
    @BenchmarkMode(Mode.SampleTime)
    @OutputTimeUnit(TimeUnit.MICROSECONDS)
    @Fork(1)
    public void run(MyState state) {
        int[][] data = (int[][])state.Dataset.getData();

        if (data[0][1] != 1 || data[1][0] != 4)
            throw new RuntimeException("Invalid data");
    }

}