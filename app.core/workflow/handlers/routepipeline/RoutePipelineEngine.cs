using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using app.core.application.error;
using app.core.workflow.dto;
using app.core.workflow.facade;
using app.core.workflow.registry;

namespace app.core.workflow.handlers.routepipeline
{
    /// <summary>
    /// Route Pipe line Engine.
    /// </summary>
    public class RoutePipelineEngine
    {
        /// <summary>
        /// Load Route Config File
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="autoExec"></param>
        public static void Initialize(string filePath, bool autoExec = false)
        {
            if (!File.Exists(filePath))
                throw new AppCoreException("route config file [" + filePath + "] not found!");

            XElement routeConfigFile;

            try
            {
                routeConfigFile = XElement.Load(filePath);
            }
            catch (Exception exception)
            {
                throw new AppCoreException("error loading route-config", exception);
            }

            InjestContextBeans(routeConfigFile);

            //get context node
            var leafContextXml = routeConfigFile.Element(CamelConstant.LeafContext);

            //get first route
            if (leafContextXml != null)
            {
                var routeNode = leafContextXml.Elements("route");
                foreach (var route in routeNode)
                {
                    var xmlRoute = route;
                    RoutePipelineDefaultProcessor.ProcessRouteInformation(xmlRoute, autoExec);
                }
            }
            else
            {
                throw new AppCoreException("error loading route-config: leaf context node not found!");
            }
        }

        /// <summary>
        /// Injest Context Beans.
        /// </summary>
        /// <param name="routeConfigFile"></param>
        private static void InjestContextBeans(XContainer routeConfigFile)
        {
            var beansXml = routeConfigFile.Elements("bean");
            var xElements = beansXml as XElement[] ?? beansXml.ToArray();
            if (!xElements.Any()) return;

            foreach (var beanXml in xElements)
            {
                try
                {
                    var id = beanXml.Attributes("id").First().Value;
                    var @class = beanXml.Attributes("class").First().Value;

                    var type = Type.GetType(@class);
                    if (type == null) continue;
                      
                    var bean = Activator.CreateInstance(type);
                    //process product.
                    var propertyXmlColl = beanXml.Elements("property");
                    if (propertyXmlColl.Any())
                    {
                        foreach (var item in propertyXmlColl)
                        {
                            var key = item.Attribute("key").Value;
                            var @value = item.Attribute("value").Value;

                            var prop = bean.GetType().GetProperty(key);
                            if (prop != null)
                            {
                                var checkValue = @value.Replace("{{", "");
                                checkValue = checkValue.Replace("}}", "");

                                //check for bean
                                if (!Camel.Registry.ContainsKey(checkValue))
                                    prop.SetValue(bean, Convert.ChangeType(@value, prop.PropertyType), null);
                                else
                                {
                                    if (@value != string.Empty && @value.StartsWith("{{") && @value.EndsWith("}}"))
                                    {
                                        try
                                        {
                                            var obj = Camel.Registry[checkValue];
                                            prop.SetValue(bean, obj, null);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                        }
                    }

                    Camel.Registry.TryAdd(id, bean);
                }
                catch (Exception exc)
                {
                    var msg = exc.Message;
                }
            }
        }
    }
}
