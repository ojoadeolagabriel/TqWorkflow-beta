using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace app.core.nerve.utility
{
    public class ConsoleProfiler : IDisposable
    {

        private Stopwatch watch;

        public ConsoleProfiler()
        {
            watch = new Stopwatch();
            watch.Restart();
            watch.Start();
        }

        public void Dispose()
        {
            watch.Stop();
            Console.WriteLine("Total Time: {0}ms", watch.ElapsedMilliseconds );
        }
    }
}
