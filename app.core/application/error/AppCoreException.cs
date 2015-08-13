using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.application.error
{
    public class AppCoreException : Exception
    {
        public AppCoreException(string message = "", Exception exception = null) :
            base(message, exception)
        {

        }
    }
}
