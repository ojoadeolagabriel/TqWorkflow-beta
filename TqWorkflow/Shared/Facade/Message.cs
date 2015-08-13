using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TqWorkflow.Shared.Facade
{
    /// <summary>
    /// Message
    /// </summary>
    public class Message
    {
        public Dictionary<string,string> HeaderCollection { get; set; }
        public string Body { get; set; }
        public List<Object> AttachmentCollection { get; set; }
    }
}
