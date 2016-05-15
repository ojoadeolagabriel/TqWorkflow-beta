using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.handlers.tag;
using app.core.nerve.pattern.eip;

namespace app.core.nerve.language
{
    public class LanguageUtil
    {
        public Exchange Exchange { get; set; }

        public LanguageUtil(Exchange exchange)
        {
            Exchange = exchange;
        }

        public LanguageUtil From(string uri)
        {
            FromTag.Execute(uri, Exchange, Exchange.Route);
            return this;
        }

        public LanguageUtil To(string uri)
        {
            ToTag.Execute(uri, Exchange, Exchange.Route);
            return this;
        }

        public WhenUtil Choice()
        {
            MessageRouterPattern.Execute(Exchange.Route.CurrentRouteStep.XmlRaw, Exchange);
            return new WhenUtil(Exchange);
        }
    }
}
