using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.data.lightfoot
{
    public class LightFootMetadataAttribute : Attribute
    {
        public string ColumnNameOverride { get; set; }
        public string MapsTo { get; set; }
        public string MapsToSp { get; set; }
        public string MapsToSpParam { get; set; }
    }
}
