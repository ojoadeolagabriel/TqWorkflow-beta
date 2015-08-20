using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.workflow.dto;

namespace app.core.nerve.test
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
