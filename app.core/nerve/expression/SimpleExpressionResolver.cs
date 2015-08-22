using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using app.core.nerve.dto;
using app.core.nerve.facade;
using Configuration = app.core.nerve.utility.Configuration;

namespace app.core.nerve.expression
{
    public static class SimpleExpression
    {
        public static T ObjectExpressionResolver<T>(string expression)
            where T : class
        {
            if (string.IsNullOrEmpty(expression))
                return default(T);

            if (!expression.StartsWith("{{") && !expression.EndsWith("}}"))
                return (T)Convert.ChangeType(expression, typeof(T));            

            var expressionParts = expression.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            switch (expressionParts.Length)
            {
                case 1:
                    var checkValue = expression.Replace("{{", "").Replace("}}", "");
                    var regObj = Camel.Registry[checkValue];
                    if (regObj == null)
                        return null;

                    return regObj as T;
                case 2:
                    var checkValueColl = expression.Replace("{{", "").Replace("}}", "");
                    var checkValueCollParts = checkValueColl.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    var @class = checkValueCollParts[0];
                    var property = checkValueCollParts[1];
                    regObj = Camel.Registry[@class];
                    if (regObj == null)
                        return null;

                    var propertyValue = regObj.GetType().GetProperty(property).GetValue(regObj, null);
                    return propertyValue as T;
                default:
                    return default(T);
            }

            return default (T);
        }

        public static object ObjectExpressionResolver(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return null;

            if (!expression.StartsWith("{{") && !expression.EndsWith("}}"))
                return expression;

            var expressionParts = expression.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            switch (expressionParts.Length)
            {
                case 1:
                    var checkValue = expression.Replace("{{", "").Replace("}}", "");
                    var regObj = Camel.Registry[checkValue];
                    if (regObj == null)
                        return null;

                    return regObj;
                case 2:
                    var checkValueColl = expression.Replace("{{", "").Replace("}}", "");
                    var checkValueCollParts = checkValueColl.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    var @class = checkValueCollParts[0];
                    var property = checkValueCollParts[1];
                    regObj = Camel.Registry[@class];
                    if (regObj == null)
                        return null;

                    var propertyValue = regObj.GetType().GetProperty(property).GetValue(regObj, null);
                    return propertyValue;
                default:
                    return null;
            }

            return null;
        }

        public static string Resolve(string expression, Exchange exchange)
        {
            return "";
        }

        public static bool Evaluate(Exchange exchange, string expression)
        {
            var expressionParts = expression.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            var operatorType = expressionParts[1];
            var lhs = expressionParts[0];
            var rhs = expressionParts[2];

            var lhsResult = ResolveExpression(lhs, exchange);
            var rhsResult = ResolveExpression(rhs, exchange);

            switch (operatorType)
            {
                case "=":
                    return lhsResult == rhsResult;
                case "!=":
                    return lhsResult != rhsResult;

            }

            return false;
        }

        public static void ResolveExpression(UriDescriptor uriParts, Exchange exchange)
        {
            //exchange
            var match = Regex.Match(uriParts.ComponentQueryPath, @"\${(.+)}", RegexOptions.IgnoreCase);

            if (match.Success)
                foreach (Group @group in match.Groups)
                    HandleMatch(@group, uriParts, exchange);

            //beans
            match = Regex.Match(uriParts.ComponentQueryPath, @"\{{(.+)}}", RegexOptions.IgnoreCase);

            if (!match.Success) return;

            foreach (Group @group in match.Groups)
                HandleMatch(@group, uriParts, exchange);
        }

        public static string ResolveExpression(string componentQueryPath, Exchange exchange)
        {
            var match = Regex.Match(componentQueryPath, @"\${(.+)}", RegexOptions.IgnoreCase);

            if (match.Success)
                componentQueryPath = match.Groups.Cast<Group>().Aggregate(componentQueryPath, (current, @group) => HandleMatch(@group, current, exchange));

            match = Regex.Match(componentQueryPath, @"\{{(.+)}}", RegexOptions.IgnoreCase);

            if (match.Success)
                componentQueryPath = match.Groups.Cast<Group>().Aggregate(componentQueryPath, (current, @group) => HandleMatch(@group, current, exchange));

            return componentQueryPath;
        }

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

                Object replacementData = "";

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
                        replacementData = ReadEnumData(exchange, objectDataProperty);
                        break;
                    default:
                        replacementData = ReadComplexData(exchange, objectData, objectDataProperty, objectDataKey);
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

        private static object ReadEnumData(Exchange exchange, string objectDataProperty)
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

        private static string ReadComplexData(Exchange exchange, string objectData, string objectDataProperty, string objectDataKey)
        {
            var objectInRegistry = Camel.Registry.FirstOrDefault(c => c.Key == objectData);
            if (objectInRegistry.Value == null)
                return "";

            var prop = objectInRegistry.Value.GetType().GetProperty(objectDataProperty);

            if (prop == null)
                return null;

            var v = (Configuration)objectInRegistry.Value;

            var data = prop.GetValue(objectInRegistry.Value, BindingFlags.Public | BindingFlags.NonPublic, null,
                null, CultureInfo.CurrentCulture);

            return data.ToString();
        }

        private static void HandleMatch(Capture @group, UriDescriptor uriParts, Exchange exchange)
        {
            var originalData = group.Value;

            var mData = group.Value.Replace("${", "");
            mData = mData.Replace("}", "");

            var parts = mData.Split(new[] { '.' });
            if (parts.Length > 3)
                return;

            var objectData = parts.Length >= 1 ? parts[0] : "";
            var objectDataProperty = parts.Length >= 2 ? parts[1] : "";
            var objectDataKey = parts.Length >= 3 ? parts[2] : "";

            Object replacementData = "";

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
                default:
                    replacementData = ReadComplexData(exchange, objectData, objectDataProperty, objectDataKey);
                    break;
            }

            uriParts.ComponentQueryPath = uriParts.ComponentQueryPath.Replace(originalData, replacementData.ToString());
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
