using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FlightBooking.Models
{
    public class FlightViewModel
    { 
        public string FlightName { get; set; }
        public int TotalSeats { get; set; }
        public int Id { get; set; }
        public int FlightId { get; set; }
        public int FlightIdTobeCanceled { get; set; }
    }
}