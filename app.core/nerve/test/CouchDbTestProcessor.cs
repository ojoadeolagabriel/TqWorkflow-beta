using System;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.test
{
    public class CouchDbTestProcessor : ProcessorBase
    {
        public bool IsGood()
        {
            return true;
        }

        public bool IsTransactionValid()
        {
            return DateTime.Now.Second % 3 == 0;
        }

        public class Person
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public override void Process(Exchange exchange)
        {
            //var person = new Person { Password = "xxxxxxxx", Username = "aojo", Id = 1 };
            //exchange.InMessage.Body = person;
            exchange.InMessage.SetHeader("CouchDbDatabase", "dev_paydirect");
        }
    }
}
