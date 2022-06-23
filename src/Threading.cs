using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class Threading
    {
        [Benchmark]
        public async Task Threading_ValueTaskCost()
        {
            for (int i = 0; i < 1_000; i++)
                await YieldOnce();
        }

        private static async ValueTask YieldOnce() => await Task.Yield();

        const int Iters = 1_000_000;

        private AsyncTaskMethodBuilder[] tasks = new AsyncTaskMethodBuilder[Iters];

        [IterationSetup]
        public void Setup()
        {
            Array.Clear(tasks, 0, tasks.Length);
            for (int i = 0; i < tasks.Length; i++)
                _ = tasks[i].Task;
        }

        [Benchmark(OperationsPerInvoke = Iters)]
        public void Threading_Cancel()
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].Task.ContinueWith(_ => { }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
                tasks[i].SetResult();
            }
        }
    }
}
