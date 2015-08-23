using System;
using System.Collections.Generic;
using System.ComponentModel;
using app.core.application.error;
using app.core.nerve.dto;
using app.core.nerve.expression;

namespace app.core.nerve.facade
{
    public class UriDescriptor
    {
        public string ComponentName { get; set; }
        public string ComponentPath { get; set; }
        public string ComponentQueryPath { get; set; }
        public string FullUri { get; set; }

        public T GetUriProperty<T>(string key, T defaultResult = default (T), Exchange exchange = null)
        {
            var data = GetUriProperty(key);
            if (string.IsNullOrEmpty(data))
                return defaultResult;

            try
            {
                if (exchange != null)
                {
                    data = SimpleExpression.ResolveSpecifiedUriPart(data, exchange);
                }

                var foo = TypeDescriptor.GetConverter(typeof(T));
                return (T)(foo.ConvertFromInvariantString(data));
            }
            catch
            {
                return defaultResult;
            }
        }

        public string GetUriProperty(string key)
        {
            var mParts = ComponentQueryPath.Split(new[] { ';' });
            foreach (var mPart in mParts)
            {
                var keyValue = mPart.Split(new[] { '=' });
                if (keyValue[0].Equals(key, StringComparison.InvariantCultureIgnoreCase))
                    return keyValue[1];
            }
            return "";
        }

        public static List<KeyValuePair<string, string>> BuildKeyValueListWithEquality(string data, char splitChar = ';')
        {
            if (string.IsNullOrEmpty(data))
                return null;

            var mParts = data.Split(new[] { splitChar });
            var postData = new List<KeyValuePair<string, string>>();

            foreach (var mPart in mParts)
            {
                var keyValue = mPart.Split(new[] { '=' });
                postData.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]));
            }
            return postData;
        }

        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public static UriDescriptor Parse(string uri, Exchange exchange = null)
        {
            var parts = new UriDescriptor();

            if (exchange != null)
                uri = SimpleExpression.ResolveSpecifiedUriPart(uri, exchange);

            if (string.IsNullOrEmpty(uri))
                throw new AppCoreException("uri data error: cannot be empty");

            var mainParts = uri.Split(new[] { '?' });

            var uriPrimaryParts = mainParts[0].Split(new[] { ':' }, 2);

            parts.ComponentName = uriPrimaryParts.Length >= 1 ? uriPrimaryParts[0] : "";
            parts.ComponentPath = uriPrimaryParts.Length >= 2 ? uriPrimaryParts[1] : "";
            parts.ComponentQueryPath = mainParts.Length > 1 ? mainParts[1] : "";
            parts.FullUri = uri;

            return parts;
        }
    }
}
