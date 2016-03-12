using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace app.core.nerve.dto
{
    [Serializable]
    public class Message
    {
        public object GetHeader(string key)
        {
            try
            {
                return HeaderCollection[key];
            }
            catch
            {
                return "";
            }
        }

        public T GetHeader<T>(string key, T defaultResult = default (T))
        {
            try
            {
                var result = HeaderCollection[key];
                return (T)Convert.ChangeType(result, typeof (T));
            }
            catch
            {
                return default(T);
            }
        }


        public void SetHeader(string key, Object data)
        {
            try
            {
                HeaderCollection[key] = data;
            }
            catch
            {
                
            }
        }

        public bool IsFault { get; set; }

        public ConcurrentDictionary<string, object> HeaderCollection = new ConcurrentDictionary<string, object>();

        public Object Body  = new object();

        public List<Object> AttachmentCollection { get; set; }
    }
}
