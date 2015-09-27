using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace camelcontext.server.facade.util
{
    public class TaskManager
    {
        public void Run()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var corporateTasks = new List<Task<int>>();
            var corporateCodes = "TBN,ASP,REP".Split(',');
            foreach (var corporateCode in corporateCodes)
            {
                var code = corporateCode;
                var corporateTask = Task.Factory.StartNew(() => ProcessActivity(code, token),TaskCreationOptions.LongRunning);
                corporateTasks.Add(corporateTask);
            }

            Task.Factory.StartNew(() => CancelIn5Secs(cts));
        }

        private static int ProcessActivity(string code, CancellationToken token)
        {
            while (true)
            {
                Thread.Sleep(1000);
                Console.WriteLine("processing: {0}", code);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("cancelling: {0}", code);
                    break;
                }
            }

            return 0;
        }

        private void CancelIn5Secs(CancellationTokenSource cts)
        {
            Thread.Sleep(5000);
            cts.Cancel();
        }
    }
}
