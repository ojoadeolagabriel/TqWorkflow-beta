﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.workflow.dto
{
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

        public Dictionary<string, object> HeaderCollection = new Dictionary<string, object>();

        public Object Body  = new object();

        public List<Object> AttachmentCollection { get; set; }
    }
}