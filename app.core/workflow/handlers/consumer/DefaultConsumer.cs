using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.component.root;

namespace app.core.workflow.handlers.consumer
{
    /// <summary>
    /// Default Consumer
    /// </summary>
    public class DefaultConsumer
    {
        private RouteStep _stepProcessor;

        public DefaultConsumer(RouteStep stepProcessor, ComponentBase component)
        {
            _stepProcessor = stepProcessor;
            stepProcessor.Execute();
        }
    }
}
