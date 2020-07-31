using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FlightBooking.Models
{
    public class FlightDetailViewModel
    {
        [JsonProperty(PropertyName = "Flight Name")]
        public string FlightName { get; set; }
        [JsonConverter(typeof(OnlyTimeConverter))]
        public DateTime Departure { get; set; }
        [JsonConverter(typeof(OnlyTimeConverter))]
        public DateTime Arrival { get; set; }
        [JsonProperty(PropertyName = "Journey Date")]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime JourneyDate { get; set; }
        [JsonProperty(PropertyName = "From City")]
        public string FromCity { get; set; }
        [JsonProperty(PropertyName = "To City")]
        public string ToCity { get; set; }
        public decimal Price { get; set; }
        [JsonProperty(PropertyName = "Seat Availability")]
        public int SeatAvailability { get; set; }
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int FlightIdTobeCanceled { get; set; }
        public string token { get; set; }
    }

    public class OnlyDateConverter : IsoDateTimeConverter
    {
        public OnlyDateConverter()
        {
            DateTimeFormat = "MM-dd-yyyy";
        }
    }

    public class OnlyTimeConverter : IsoDateTimeConverter
    {
        public OnlyTimeConverter()
        {
            DateTimeFormat = "hh:mm tt";
        }
    }
}