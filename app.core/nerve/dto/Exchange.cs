using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using app.core.workflow.dto;
using Newtonsoft.Json;

namespace app.core.nerve.dto
{
    /// <summary>
    /// Exchange Pattern.
    /// </summary>
    public class Exchange
    {
        #region Constants

        #endregion

        public enum Mep
        {
            InOnly, InOut
        }

        public Exception CurrentException;
        public ConcurrentStack<Exception> Exception = new ConcurrentStack<Exception>();
        public ConcurrentStack<Object> AlternativeMessage = new ConcurrentStack<Object>();

        public void SetException(Exception value)
        {
            try
            {
                Exception.Push(value);
            }
            catch
            {

            }
        }

        public List<Exception> DequeueAllErrors
        {
            get
            {
                var allErrors = new List<Exception>();
                do
                {
                    Exception exeption;
                    if (!Exception.TryPop(out exeption))
                        break;
                    allErrors.Add(exeption);

                } while (true);

                return allErrors;
            }
        }

        private readonly DateTime _lastAccessTime;

        public ConcurrentDictionary<string, string> PropertyCollection { get; set; }

        public Mep MepPattern { get; set; }

        public Guid ExchangeId;
        public Guid ParentExchangeId;

        public Message InMessage = new Message();

        public Message OutMessage = new Message();

        [JsonIgnore]
        public Route Route;

        public Exchange(Route route)
        {
            Route = route;
            ExchangeId = Guid.NewGuid();
            CamelCreatedTimestamp = DateTime.Now;
            _lastAccessTime = DateTime.Now;
            PropertyCollection = new ConcurrentDictionary<string, string>();
        }

        public Exchange CloneExchange(Message inMessage, Message outMessage, Route route)
        {
            return new Exchange(Route)
            {
                InMessage = inMessage,
                OutMessage = outMessage,
                CamelCreatedTimestamp = DateTime.Now,
                ExchangeId = Guid.NewGuid(),
                PropertyCollection = new ConcurrentDictionary<string, string>()
            };
        }

        public DateTime CamelCreatedTimestamp { get; set; }

        public string GetProperty(string propertyName)
        {
            try
            {
                return PropertyCollection[propertyName];
            }
            catch
            {
                return null;
            }
        }

        public void SetProperty(string propertyName, string value)
        {
            try
            {
                PropertyCollection.TryAdd(propertyName, value);
            }
            catch
            {

            }
        }
    }
}
