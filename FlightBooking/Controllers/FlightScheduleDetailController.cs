using FlightBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlightBooking.Controllers
{
    public class FlightScheduleDetailController : ApiController
    {

        [Route("api/FlightScheduleDetail/GetFlightScheduleDetailsById/{id}")]
        public IHttpActionResult GetFlightScheduleDetailsById(int id)
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightschedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightschedule.FlightDetailId  
                               where flightschedule.Id == id
                               select new FlightDetailViewModel
                               {
                                   Id = flightschedule.Id,
                                   FlightName = flight.FlightName,
                                   JourneyDate = flightschedule.JourneyDate,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightschedule.Price,
                                   SeatAvailability = flightschedule.SeatAvailability,
                                   FlightId = flightschedule.Id
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

        [Route("api/FlightScheduleDetail/GetFlightScheduleDetailsByFlightDetailId/{id}")]
        public IHttpActionResult GetFlightScheduleDetailsByFlightDetailId(int id)
        {
            try
            {
                IList<FlightDetailViewModel> flights = null;

                using (var ctx = new BookingFlightEntities())
                {
                    flights = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightschedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightschedule.FlightDetailId
                               where flightDetail.Id == id
                               select new FlightDetailViewModel
                               {
                                   Id = flightschedule.Id,
                                   FlightName = flight.FlightName,
                                   JourneyDate = flightschedule.JourneyDate,
                                   Departure = flightDetail.Departure,
                                   Arrival = flightDetail.Arrival,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightschedule.Price,
                                   SeatAvailability = flightschedule.SeatAvailability,
                                   FlightId = flightschedule.Id
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
        public IHttpActionResult PostFlightScheduleDetail(FlightScheduleDetail flight)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                using (var ctx = new BookingFlightEntities())
                {
                    var flights = new FlightScheduleDetail()
                    {
                        JourneyDate = flight.JourneyDate,
                        Price = flight.Price,
                        SeatAvailability = flight.SeatAvailability,
                        FlightDetailId = flight.FlightDetailId
                    };
                    ctx.FlightScheduleDetails.Add(flights);

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
        [Route("api/FlightScheduleDetail/PutFlightScheduleDetail")]
        public IHttpActionResult PutFlightScheduleDetail(FlightScheduleDetail flight)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                using (var ctx = new BookingFlightEntities())
                {
                    var flighttobeUpdated = ctx.FlightScheduleDetails.Where(x => x.Id == flight.Id).FirstOrDefault<FlightScheduleDetail>();
                    if (flighttobeUpdated != null)
                    {
                        flighttobeUpdated.JourneyDate = flight.JourneyDate;
                        flighttobeUpdated.Price = flight.Price;
                        flighttobeUpdated.SeatAvailability = flight.SeatAvailability;
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
        public IHttpActionResult DeleteFlightScheduleDetail(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Not a valid flight id");

                using (var ctx = new BookingFlightEntities())
                {
                    var flight = ctx.FlightScheduleDetails
                        .Where(s => s.Id == id)
                        .FirstOrDefault();

                    var tickets = from flightdetails in ctx.FlightDetails
                                  join flightschedule in ctx.FlightScheduleDetails
                                  on flightdetails.Id equals flightschedule.FlightDetailId
                                  join ticket in ctx.TicketDetails
                                  on flightschedule.Id equals ticket.FlightScheduleDetailId
                                  where flightschedule.Id == flight.Id
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
