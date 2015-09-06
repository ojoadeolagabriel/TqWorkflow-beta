using System;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.handlers.tag;
using app.core.nerve.pattern.eip;

namespace app.core.nerve.facade
{
    /// <summary>
    /// 
    /// </summary>
    public class RouteStep
    {
        public RouteStep NextTag;

        private readonly string _componentPathInfo = null;
        public string ComponentPathInfo
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_componentPathInfo) && _currentStepXml.Name == "from")
                    {
                        var uri = UriDescriptor.Parse(_currentStepXml.Attribute("uri").Value);
                        return uri.ComponentPath;
                    }
                    return _componentPathInfo;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public string ComponentPathInfoWithComponentFilter(string component = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_componentPathInfo) && _currentStepXml.Name == "from")
                {
                    var uri = UriDescriptor.Parse(_currentStepXml.Attribute("uri").Value);
                    return uri.ComponentPath;
                }
                return _componentPathInfo;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private readonly XElement _currentStepXml;
        public Route Route { get; set; }

        public RouteStep(XElement currentStepXml, Route route)
        {
            _currentStepXml = currentStepXml;
            Route = route;
            CheckIfRouteNameOverrideRequired();
        }

        private void CheckIfRouteNameOverrideRequired()
        {
            try
            {
                var stepName = _currentStepXml.Name.ToString();
                switch (stepName)
                {
                    case "from":
                        var val = _currentStepXml.Attribute("uri").Value;
                        var desc = UriDescriptor.Parse(val);
                        if (desc.ComponentName == "direct")
                        {
                            Route.RouteId = desc.ComponentPath;
                        }
                        if (desc.ComponentName == "seda")
                        {
                            Route.RouteId = desc.ComponentPath;
                        }
                        break;
                }
            }
            catch (Exception exception)
            {

            }
        }

        public void ForceEndPointProcess(Exchange exchange)
        {

        }

        public void ProcessChannel(Exchange exchange = null)
        {
            ProcessStep(_currentStepXml, Route, exchange);

            if (NextTag == null || exchange == null) return;

            switch (exchange.Route.PipelineMode)
            {
                case Route.MessagePipelineMode.Default:
                    NextTag.ProcessChannel(exchange);
                    break;
            }
        }

        /// <summary>
        /// Read and handle step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="routeObj"></param>
        /// <param name="exchange"></param>
        public static void ProcessStep(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var stepname = step.Name.ToString().ToLower();

                switch (stepname)
                {
                    case "from":
                        HandleFromProcessor(step, routeObj, exchange);
                        break;
                    case "to":
                        HandleToProcessor(step, routeObj, exchange);
                        break;
                    case "enrich":
                        HandleToEnricher(step, routeObj, exchange);
                        break;
                    case "split":
                        HandleSplit(step, exchange);
                        break;
                    case "multicast":
                        HandleMulticast(step, exchange);
                        break;
                    case "process":
                        HandleProcessor(step, routeObj, exchange);
                        break;
                    case "bean":
                        HandleBean(step, routeObj, exchange);
                        break;
                    case "convertbodyto":
                        HandleConvertBodyTo(step, routeObj, exchange);
                        break;
                    case "setheader":
                        HandleSetHeader(step, routeObj, exchange);
                        break;
                    case "removeheader":
                        HandleRemoveHeader(step, routeObj, exchange);
                        break;
                    case "setproperty":
                        HandleProperty(step, routeObj, exchange);
                        break;
                    case "choice":
                        HandleChoice(step, routeObj, exchange);
                        break;
                    case "loop":
                        HandleLoop(step, routeObj, exchange);
                        break;
                    case "delay":
                        HandleDelay(step, routeObj, exchange);
                        break;
                    case "wiretap":
                        HandleWireTap(step, routeObj, exchange);
                        break;
                    case "transform":
                        HandleTransform(step, routeObj, exchange);
                        break;
                    case "filter":
                        HandleFilter(step, routeObj, exchange);
                        break;
                }
            }
            catch (Exception exception)
            {

            }
        }

        private static void HandleToEnricher(XElement step, dto.Route routeObj, Exchange exchange)
        {
            EnricherPattern.Enrich(step, exchange, routeObj);
        }

        private static void HandleLoop(XElement step, Route routeObj, Exchange exchange)
        {
            LoopTag.Execute(step, exchange, routeObj);
        }

        private static void HandleMulticast(XElement step, Exchange exchange)
        {
            PublishSubscribePattern.Process(step, exchange);
        }

        private static void HandleSplit(XElement step, Exchange exchange)
        {
            SplitterPattern.Execute(step, exchange);
        }

        private static void HandleFilter(XElement step, Route routeObj, Exchange exchange)
        {
            throw new NotImplementedException();
        }

        private static void HandleRemoveHeader(XElement step, Route routeObj, Exchange exchange)
        {
            var headerName = step.Attribute("headerName").Value;
            exchange.InMessage.HeaderCollection.Remove(headerName);
        }

        private static void HandleTransform(XElement step, Route routeObj, Exchange exchange)
        {
            Transform.HandleTransform(step, routeObj, exchange);
        }

        private static void HandleWireTap(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var fullUri = step.Attribute("uri").Value;
                WireTapPattern.Execute(exchange, fullUri, routeObj);
            }
            catch (Exception)
            {

            }
        }

        private static void HandleDelay(XElement step, Route routeObj, Exchange exchange)
        {
            var delay = step.Attribute("value").Value;
            DelayTag.Execute(delay, exchange, null);
        }

        private static void HandleChoice(XElement step, Route routeObj, Exchange exchange)
        {
            MessageRouterPattern.Execute(step, exchange);
        }

        private static void HandleProperty(XElement step, Route routeObj, Exchange exchange)
        {
            var propertyName = step.Attribute("name").Value;
            var value = step.Attribute("value").Value;

            value = SimpleExpression.ResolveSpecifiedUriPart(value, exchange);
            exchange.SetProperty(propertyName, value);
        }

        private static void HandleSetHeader(XElement step, Route routeObj, Exchange exchange)
        {
            var headerName = step.Attribute("name").Value;
            var value = step.Attribute("value").Value;

            value = SimpleExpression.ResolveSpecifiedUriPart(value, exchange);
            exchange.InMessage.SetHeader(headerName, value);
        }

        private static void HandleConvertBodyTo(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var toType = step.Attribute("type").Value;
                var finalType = Type.GetType(toType);

                if (finalType != null)
                    exchange.InMessage.Body = Convert.ChangeType(exchange.InMessage.Body, finalType);
            }
            catch
            {

            }
        }

        /// <summary>
        /// ProcessRouteInformation Bean
        /// </summary>
        /// <param name="step"></param>
        /// <param name="routeObj"></param>
        /// <param name="exchange"></param>
        private static void HandleBean(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var reference = step.Attribute("ref").Value;
                var method = step.Attribute("method").Value;

                var bean = Camel.Registry[reference];
                var methodInfo = bean.GetType().GetMethod(method);

                methodInfo.Invoke(bean, null);
            }
            catch (Exception)
            {

            }
        }

        private static void HandleProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var reference = step.Attribute("ref").Value;
                var bean = Camel.Registry[reference] as ProcessorBase;
                if (bean != null)
                    bean.Process(exchange);
            }
            catch (Exception)
            {

            }
        }

        private static void HandleToProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            var uri = step.Attribute("uri").Value;
            ToTag.Execute(uri, exchange, routeObj);
        }

        private static void HandleFromProcessor(XElement step, Route routeObj, Exchange exchange)
        {
            var uri = step.Attribute("uri").Value;
            FromTag.Execute(uri, exchange, routeObj);
        }
    }
}
