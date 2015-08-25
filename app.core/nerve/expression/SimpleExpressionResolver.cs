using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using app.core.nerve.dto;

namespace app.core.nerve.expression
{
    public static class SimpleExpression
    {
        public static bool Evaluate(Exchange exchange, string expression)
        {
            var expressionParts = expression.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var operatorType = expressionParts[1];
            var lhs = expressionParts[0];
            var rhs = expressionParts[2];

            var lhsResult = ResolveSpecifiedUriPart(lhs, exchange);
            var rhsResult = ResolveSpecifiedUriPart(rhs, exchange);

            switch (operatorType)
            {
                case "=":
                    return lhsResult == rhsResult;
                case "!=":
                    return lhsResult != rhsResult;

            }

            return false;
        }

        public static Object ResolveObjectFromRegistry(string objectExpression)
        {
            if (!objectExpression.StartsWith("${") || !objectExpression.EndsWith("}"))
                return objectExpression;

            var mData = objectExpression.Replace("${", "");
            mData = mData.Replace("}", "");

            var mDataParts = mData.Split(new []{"."}, StringSplitOptions.RemoveEmptyEntries);
            var objectKey = mDataParts[0];

            var objectData = Camel.Registry[objectKey];
            return objectData;
        } 

        /// <summary>
        /// Resolve path via exchange.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static string ResolveSpecifiedUriPart(string path, Exchange exchange)
        {
            var matchColl = Regex.Matches(path, @"\${(.*?)\}", RegexOptions.IgnoreCase);

            return matchColl
                .Cast<Match>()
                .Where(match => match.Success).Aggregate(path, (current1, match) => match.Groups.Cast<Group>().Aggregate(current1, (current, @group) => HandleMatch(@group, current, exchange)));
        }

        /// <summary>
        /// Handle match
        /// </summary>
        /// <param name="group"></param>
        /// <param name="componentQueryPath"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        private static string HandleMatch(Capture @group, string componentQueryPath, Exchange exchange)
        {
            try
            {
                var originalData = group.Value;

                var mData = group.Value.Replace("${", "");
                mData = mData.Replace("{{", "");
                mData = mData.Replace("}", "");

                var parts = mData.Split(new[] { ':' });
                if (parts.Length != 2)
                {
                    parts = mData.Split(new[] { '.' });
                    if (parts.Length > 3)
                        return "";
                }

                var objectData = parts.Length >= 1 ? parts[0] : "";
                var objectDataProperty = parts.Length >= 2 ? parts[1] : "";
                var objectDataKey = parts.Length >= 3 ? parts[2] : "";

                Object replacementData;

                switch (objectData)
                {
                    case "in":
                        replacementData = ReadMessageData(exchange.InMessage, objectDataProperty, objectDataKey);
                        break;
                    case "out":
                        replacementData = ReadMessageData(exchange.OutMessage, objectDataProperty, objectDataKey);
                        break;
                    case "property":
                        replacementData = exchange.PropertyCollection[objectDataKey];
                        break;
                    case "enum":
                        replacementData = ReadEnumData(objectDataProperty);
                        break;
                    default:
                        replacementData = ReadComplexData(objectData, objectDataProperty, objectDataKey);
                        break;
                }

                //return
                return componentQueryPath.Replace(originalData, replacementData.ToString());
            }
            catch (Exception)
            {
                return componentQueryPath;
            }
        }

        private static object ReadEnumData(string objectDataProperty)
        {
            try
            {
                var parts = objectDataProperty.Split(new[] { '.' });
                var partsLessOne = parts.Take(parts.Length - 1).ToList();
                var last = parts.Last();
                var fullName = "";

                partsLessOne.ForEach(c =>
                {
                    fullName += string.Format("{0}.", c);
                });

                fullName = fullName.Remove(fullName.Length - 1, 1);

                var @enum = Type.GetType(fullName);
                if (@enum != null)
                {
                    var val = (int)Enum.Parse(@enum, last);
                    return val;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static object ReadComplexData(string objectData, string objectDataProperty, string objectDataKey)
        {
            var objectInRegistry = Camel.Registry.FirstOrDefault(c => c.Key == objectData);
            if (objectInRegistry.Value == null)
                return "";

            var prop = objectInRegistry.Value.GetType().GetProperty(objectDataProperty);

            if (prop == null)
                return null;

            var data = prop.GetValue(objectInRegistry.Value, BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.CurrentCulture);

            return data.ToString();
        }

        private static object ReadMessageData(Message message, string objectDataProperty, string objectDataKey)
        {
            try
            {
                switch (objectDataProperty)
                {
                    case "header":
                        return message.HeaderCollection[objectDataKey];
                    case "body":
                        return message.Body;
                }

                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
