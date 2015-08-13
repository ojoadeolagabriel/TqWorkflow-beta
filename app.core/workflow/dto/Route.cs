using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.facade;

namespace app.core.workflow.dto
{
    public class Route
    {
        public RouteStep RouteProcess { get; set; }

        public string RouteId = Guid.NewGuid().ToString();

        public Route()
        {
            RouteId = Guid.NewGuid().ToString();
        }

        public void TriggerRoute()
        {
            RouteProcess.Execute();
        }
    }
}
