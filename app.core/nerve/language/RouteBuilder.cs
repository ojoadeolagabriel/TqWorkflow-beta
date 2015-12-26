using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace app.core.nerve.language
{
    public class RouteBuilder
    {
        public XElement RouteXml  = new XElement("container",
            new XElement("routecontext"));

        public RouteBuilder From(string uri)
        {
            
            return this;//from('uri')
                            //.choice('$s=1')
                                //to('uri')
        }
        
        public RouteBuilder To(string uri)
        {
            return this;
        }
    }
}
