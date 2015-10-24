using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace paymentnotification.generic.bundle.codebase
{
    public class StatusObject
    {
        public int A { get; set; }
        public int B { get; set; }

        public StatusObject(int a, int b)
        {
            A = a;
            B = b;
        }
    }
}
