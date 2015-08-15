using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.test
{
    public class CouchDbTestProcessor : ProcessorBase
    {
        public bool IsGood()
        {
            return true;
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
