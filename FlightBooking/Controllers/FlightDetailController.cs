using FlightBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace FlightBooking.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FlightDetailController : ApiController
    {
    
        public IHttpActionResult GetFlights()
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                CreatioLogin login = new CreatioLogin("http://localhost:86/", "Supervisor", "Supervisor");
                var loginrequest = login.TryLogin();
                CreatioLogin logins = new CreatioLogin("http://localhost:86/");
                //var token = logins.CallGetMethod(loginrequest);
                string token = logins.CallWebApi("fg@gmail.com", "Newuser@1").ToString();

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightSchedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightSchedule.FlightDetailId
                               select new FlightDetailViewModel
                               {
                                   Id = flightSchedule.Id,
                                   FlightName = flight.FlightName,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   JourneyDate = flightSchedule.JourneyDate,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightSchedule.Price,
                                   SeatAvailability = flightSchedule.SeatAvailability,
                                   token = token
                               }).ToList<FlightDetailViewModel>();
                }

                //CreatioLogin login = new CreatioLogin("http://localhost:86/", "Supervisor", "Supervisor");
               // login.TryLogin();

                if (!flights.Any())
                {
                    return NotFound();
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [Route("api/FlightDetail/GetToken")]
        public IHttpActionResult GetToken(string Token)
        {
            try
            {
                var session = HttpContext.Current.Session;
                session["accessToken"] = Token;
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("api/FlightDetail/GetFlightsById/{id}")]
        public IHttpActionResult GetFlightsById(int id)
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightSchedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightSchedule.FlightDetailId
                               where flightSchedule.Id == id
                               select new FlightDetailViewModel
                               {
                                   Id = flightSchedule.Id,
                                   FlightName = flight.FlightName,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   JourneyDate = flightSchedule.JourneyDate,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightSchedule.Price,
                                   SeatAvailability = flightSchedule.SeatAvailability,
                               }).ToList<FlightDetailViewModel>();
                }

                if (!flights.Any())
                {
                    return NotFound();
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("api/FlightDetail/GetFlightDetailsByFlightId/{id}")]
        public IHttpActionResult GetFlightDetailsByFlightId(int id)
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               //join flightSchedule in ctx.FlightScheduleDetails
                               //on flightDetail.Id equals flightSchedule.FlightDetailId
                               where flightDetail.FlightId == id
                               select new FlightDetailViewModel
                               {
                                   Id = flightDetail.Id,
                                   FlightName = flight.FlightName,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   FlightId = flightDetail.Id,
                                   FlightIdTobeCanceled = flightDetail.Id
                               }).Distinct().ToList<FlightDetailViewModel>();
                }

                if (!flights.Any())
                {
                    return NotFound();
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Route("api/FlightDetail/GetFlightDetailsById/{id}")]
        public IHttpActionResult GetFlightDetailsById(int id)
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               where flightDetail.Id == id
                               select new FlightDetailViewModel
                               {
                                   Id = flightDetail.Id,
                                   FlightName = flight.FlightName,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   FlightId = flightDetail.Id
                               }).ToList<FlightDetailViewModel>();
                }

                if (!flights.Any())
                {
                    return NotFound();
                }

                return Ok(flights);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostFlightDetail(FlightDetail flight)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                using (var ctx = new BookingFlightEntities())
                {
                    var flights = new FlightDetail()
                    {
                        FromCity = flight.FromCity,
                        ToCity = flight.ToCity,
                        Departure = flight.Departure,
                        Arrival = flight.Arrival,
                        FlightId = flight.FlightId
                    };
                    ctx.FlightDetails.Add(flights);

                    ctx.SaveChanges();
                    return Json(new { id = flights.Id });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("api/FlightDetail/PutFlightdetail")]
        public IHttpActionResult PutFlightdetail(FlightDetail flight)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                using (var ctx = new BookingFlightEntities())
                {
                    var flighttobeUpdated = ctx.FlightDetails.Where(x => x.Id == flight.Id).FirstOrDefault<FlightDetail>();
                    if (flighttobeUpdated != null)
                    {
                        flighttobeUpdated.FromCity = flight.FromCity;
                        flighttobeUpdated.ToCity = flight.ToCity;
                        flighttobeUpdated.Departure = flight.Departure;
                        flighttobeUpdated.Arrival = flight.Arrival;
                    }

                    ctx.SaveChanges();

                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteFlightDetail(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Not a valid flight id");

                using (var ctx = new BookingFlightEntities())
                {
                    var flight = ctx.FlightDetails
                        .Where(s => s.Id == id)
                        .FirstOrDefault();

                    var tickets = from flightdetails in ctx.FlightDetails
                                  join flightschedule in ctx.FlightScheduleDetails
                                  on flightdetails.Id equals flightschedule.FlightDetailId
                                  join ticket in ctx.TicketDetails
                                  on flightschedule.Id equals ticket.FlightScheduleDetailId
                                  where flightdetails.Id == flight.Id
                                  && ticket.BookingStatus == BookingStatusValues.Confirmed
                                  && flightschedule.JourneyDate >= DateTime.Now
                                  select ticket;
                    if (tickets.Any())
                    {
                        throw new Exception("Tickets were booked for the flight");
                    }

                    ctx.Entry(flight).State = System.Data.Entity.EntityState.Deleted;
                    ctx.SaveChanges();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)

                {

                    ReasonPhrase = ex.Message

                };

                throw new HttpResponseException(resp);
            }
        }
    }
}
