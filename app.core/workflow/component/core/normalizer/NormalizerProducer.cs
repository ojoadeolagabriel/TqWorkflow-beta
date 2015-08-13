using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.normalizer
{
    /// <summary>
    /// Normalizer Producer.
    /// </summary>
    public class NormalizerProducer : DefaultProducer
    {
        public NormalizerProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                var method = endPointDescriptor.GetUriProperty("method");
                var objectTypePath = endPointDescriptor.ComponentPath;
                var objectType = Type.GetType(objectTypePath);

                if (objectType != null)
                {
                    var objectInstance = Activator.CreateInstance(objectType, null);
                    if (!string.IsNullOrEmpty(method))
                    {
                        //process normalizer method
                        var methodMember = objectInstance.GetType()
                            .GetMethod(method, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        methodMember.Invoke(objectInstance, null);
                    }
                }
            }
            catch (Exception exception)
            {
                
            }

            return base.Process(exchange, endPointDescriptor);
        }
    }
}
