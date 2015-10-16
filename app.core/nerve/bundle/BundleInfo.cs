using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;

namespace app.core.nerve.bundle
{
    public class BundleInfo
    {
        public XElement RouteXml { get; set; }
        public DescriptorObject DescriptorObject { get; set; }
    }
}
