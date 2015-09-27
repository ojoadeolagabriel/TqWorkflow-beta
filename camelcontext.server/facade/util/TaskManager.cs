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
        public class CorporateProcess
        {
            public bool IsSuspended { get; set; }

            public string CorporateCode { get; set; }
            public CancellationToken CancelToken { get; set; }

            public CorporateProcess(string ccode, CancellationToken token)
            {
                CorporateCode = ccode;
                CancelToken = token;
            }

            public void Suspend()
            {
                IsSuspended = true;
                Console.WriteLine("corporate [{0}] : offline", CorporateCode);
            }

            public void Resume()
            {
                IsSuspended = false;
                Console.WriteLine("resuming: {0}", CorporateCode);
            }

            public int Execute()
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);

                        if (CancelToken.IsCancellationRequested)
                        {
                            Console.WriteLine("Kill request on {0}", CorporateCode);
                            break;
                        }

                        if (IsSuspended)
                            continue;

                        var m = 0;
                    }
                    catch (Exception exception)
                    {

                    }
                }

                return 0;
            }
        }
        readonly List<CorporateProcess> _corporateProcesses = new List<CorporateProcess>();
        readonly List<string> _corporateCodes = "TBN,ASP,REP,FFF,TIN,FIRS,PROC".Split(',').ToList();

        public void Run()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            Task.Factory.StartNew(() => Monitor(token), token);
            Task.Factory.StartNew(AddCorporate);
            Task.Factory.StartNew(() => Kill(cts));
        }

        private void Kill(CancellationTokenSource cSource)
        {
            Thread.Sleep(25000);
            Console.WriteLine("Kill monitor: requested!");
            cSource.Cancel();
        }

        private void Monitor(CancellationToken token)
        {
            //while processing
            while (true)
            {
                Thread.Sleep(1000);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Warning: Kill monitor request detected!");
                    break;
                }

                //register processes.
                foreach (var code in _corporateCodes)
                {
                    if (_corporateProcesses.Exists(c => c.CorporateCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))) 
                        continue;

                    Console.WriteLine("Adding: {0}", code);
                    var process = new CorporateProcess(code, token);
                    Task.Factory.StartNew(() => process.Execute(), TaskCreationOptions.LongRunning);
                    _corporateProcesses.Add(process);
                }

                //remove tasks not configured.
                var corporatesDeactivated = _corporateProcesses.Where(c => !_corporateCodes.Contains(c.CorporateCode) && !c.IsSuspended).ToList();
                var reactivationRequested = _corporateProcesses.Where(c => _corporateCodes.Contains(c.CorporateCode) && c.IsSuspended).ToList();

                //corporate process.
                foreach (var corporateProcess in corporatesDeactivated)
                {
                    corporateProcess.Suspend();
                }

                //resume
                foreach (var corporateProcess in reactivationRequested)
                {
                    corporateProcess.Resume();
                }
            }
        }

        private void AddCorporate()
        {
            Thread.Sleep(5000);
            _corporateCodes.Add("ZENITH");
            Thread.Sleep(5000);
            _corporateCodes.Add("COCO");
            Thread.Sleep(5000);
            _corporateCodes.Remove("COCO");
            Thread.Sleep(5000);
            _corporateCodes.Add("COCO");
        }
    }
}
