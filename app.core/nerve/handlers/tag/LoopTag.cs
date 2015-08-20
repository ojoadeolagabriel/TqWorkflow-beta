using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.tag
{
    /// <summary>
    /// Loop Tag Class.
    /// </summary>
    public class LoopTag
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="loopTag"></param>
        /// <param name="exchange"></param>
        /// <param name="route"></param>
        public static void Execute(XElement loopTag, Exchange exchange, Route route)
        {
            var expressionTag = loopTag.Elements().FirstOrDefault();
            if (expressionTag == null)
                return;

            var xpression = expressionTag.Value;
            var count = SimpleExpression.ObjectExpressionResolver(xpression);

            var mCount = Convert.ToInt32(count);
            for (var i = 0; i < mCount; i++)
            {
                var data = loopTag.Elements().Skip(1);
                foreach (var dataItem in data)
                {
                    RouteStep.ProcessStep(dataItem, route, exchange);
                }
            }
        }
    }
}
