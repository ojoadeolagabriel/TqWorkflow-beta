using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.security.onetime;

namespace karaf.console
{
    class Program
    {
        static void Main(string[] args)
        {
            var otp = new OTP();
            var otpVal = otp.GetNextOTP();
        }
    }
}
