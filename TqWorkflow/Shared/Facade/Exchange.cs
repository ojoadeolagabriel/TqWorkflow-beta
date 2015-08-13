using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TqWorkflow.Shared.Facade
{
    /// <summary>
    /// Exchange Pattern.
    /// </summary>
    public class Exchange
    {
        public enum Mep
        {
            InOnly, InOut
        }

        private readonly DateTime _lastAccessTime;

        public Dictionary<string, string> PropertyCollection { get; set; }

        public Mep MepPattern { get; set; }

        public readonly Guid ExchangeId;

        public Message InMessage { get; set; }

        public Message OutMessage { get; set; }

        public Exchange()
        {
            ExchangeId = Guid.NewGuid();
            _lastAccessTime = DateTime.Now;
            PropertyCollection = new Dictionary<string, string>();
        }
    }
}
