﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using app.core.application.error;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.routepipeline
{
    /// <summary>
    /// Route Pipe line Engine.
    /// </summary>
    public class CameContextConfigFileInitializer
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
                ISystemLogProvider logProvider = null;

                //read handler.
                var logProviderXml = leafContextXml.Attribute("logProvider");
                if (logProviderXml != null && logProviderXml.Value != string.Empty)
                {
                    try
                    {
                        var logger = Camel.Registry[logProviderXml.Value];
                        if (logger is ISystemLogProvider)
                        {
                            logProvider = logger as ISystemLogProvider;
                        }
                    }
                    catch (Exception exception)
                    {

                    }
                }

                var routeNode = leafContextXml.Elements("route");
                foreach (var route in routeNode)
                {
                    var xmlRoute = route;
                    RouteStepAnalyzer.ProcessRouteInformation(xmlRoute, autoExec, logProvider);
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

                    var type = SimpleExpression.GetBean(@class);// Type.GetType(@class);
                    if (type == null) continue;

                    var bean = Activator.CreateInstance(type);

                    //process product.
                    IList<XElement> xmlColl = null;

                    try
                    {
                        var propertyXmlColl = beanXml.Elements("propertys").Elements("property");
                        xmlColl = propertyXmlColl as IList<XElement> ?? propertyXmlColl.ToList();
                    }
                    catch (Exception)
                    { }

                    if (xmlColl.Any())
                    {
                        foreach (var item in xmlColl)
                        {
                            try
                            {
                                var key = item.Attribute("key").Value;
                                var @value = item.Attribute("value").Value;

                                var prop = bean.GetType().GetProperty(key);
                                if (prop == null) continue;

                                //does rubbish.. change
                                var res = SimpleExpression.ResolveObjectFromRegistry(@value);
                                prop.SetValue(bean, Convert.ChangeType(res, prop.PropertyType), null);
                            }
                            catch (Exception exception)
                            {

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
