using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.facade;
using app.core.workflow.test;

namespace app.core.workflow.dto
{
    public class Route
    {
        public ISystemLogProvider LogProvider = null;

        public enum MessagePipelineMode
        {
            Default,
            Seda
        }

        public MessagePipelineMode PipelineMode = MessagePipelineMode.Default;

        public RouteStep RouteProcess { get; set; }

        public string RouteId = Guid.NewGuid().ToString();

        public Route()
        {
            RouteId = Guid.NewGuid().ToString();
        }

        public void TriggerRoute()
        {
            RouteProcess.ProcessChannel();
        }
    }
}
