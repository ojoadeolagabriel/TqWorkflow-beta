﻿using System;
using app.core.nerve.facade;

namespace app.core.nerve.dto
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