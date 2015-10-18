using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.application.error
{
    [Serializable]
    public class AppCoreException : Exception
    {
        public AppCoreException(string message = "", Exception exception = null) :
            base(message, exception)
        {

        }
    }
}
