using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FlightBooking.Models
{
    public class TicketDetailViewModel
    {
        [JsonProperty(PropertyName = "Flight Name")]
        public string FlightName { get; set; }
        [JsonProperty(PropertyName = "Passenger Count")]
        public int PassengerCount { get; set; }
        [JsonProperty(PropertyName = "Total Fare")]
        public decimal TotalFare { get; set; }
        [JsonProperty(PropertyName = "Journey Date")]
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime JourneyDate { get; set; }
        [JsonProperty(PropertyName = "From City")]
        public string FromCity { get; set; }
        [JsonProperty(PropertyName = "Booking Status")]
        public string BookingStatus { get; set; }
        [JsonProperty(PropertyName = "To City")]
        public string ToCity { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
        [JsonConverter(typeof(OnlyTimeConverter))]
        public DateTime Departure { get; set; }
        [JsonConverter(typeof(OnlyTimeConverter))]
        public DateTime Arrival { get; set; }

        [ForeignKey("FlightDetailId")]
        public FlightDetailViewModel FlightDetail { get; set; }
    }
}