using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.test
{
    public class XmlProcessor : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            try
            {
                var person = new Person {age = 23, name = "deola.ojo"};
                var stream1 = new MemoryStream();
                var ser = new DataContractJsonSerializer(typeof (Person));
                ser.WriteObject(stream1, person);

                stream1.Position = 0;
                var sr = new StreamReader(stream1);
                var json = sr.ReadToEnd();

                exchange.InMessage.Body = json;
            }
            catch
            {
                
            }
        }
    }

    [DataContract]
    internal class Person
    {
        [DataMember]
        internal string name;

        [DataMember]
        internal int age;
    }
}
