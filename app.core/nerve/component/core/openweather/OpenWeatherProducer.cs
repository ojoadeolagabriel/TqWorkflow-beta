using System.Net;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.openweather
{
    public class OpenWeatherProducer : DefaultProducer
    {
        public OpenWeatherProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        class PassData
        {
            public Exchange Exchange { get; set; }
            public UriDescriptor UriInfo { get; set; }
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var baseApiUri = endPointDescriptor.ComponentPath;
            var location = endPointDescriptor.GetUriProperty("location", "");
            var resultHeader = endPointDescriptor.GetUriProperty("resultHeader");
            var lat = endPointDescriptor.GetUriProperty("lat");
            var lon = endPointDescriptor.GetUriProperty("lon");

            using (var client = new WebClient())
            {
                if (!string.IsNullOrEmpty(lat))
                    client.QueryString.Add("lat", lat);
                if (!string.IsNullOrEmpty(lon))
                    client.QueryString.Add("lon", lon);

                var finaUrl = string.Format("{0}?q={1}", baseApiUri, location);
                var result = client.DownloadString(finaUrl);

                if (!string.IsNullOrEmpty(resultHeader))
                    exchange.InMessage.SetHeader("resultHeader", result);
                else
                {
                    exchange.InMessage.Body = result;
                }
            }
            return exchange;
        }     
    }
}
