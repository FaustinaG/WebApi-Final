using FlightBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlightBooking.Controllers
{
    public class TicketDetailController : ApiController
    {
        [Authorize]
        public IHttpActionResult PostTicketBooking(TicketDetail ticket)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");
                using (var ctx = new BookingFlightEntities())
                {
                    var tickets = new TicketDetail()
                    {
                        BookingStatus = ticket.BookingStatus,
                        PassengerCount = ticket.PassengerCount,
                        TotalFare = ticket.TotalFare,
                        CancellationFare = ticket.CancellationFare,
                        FlightScheduleDetailId = ticket.FlightScheduleDetailId
                    };
                    ctx.TicketDetails.Add(tickets);

                    ctx.SaveChanges();

                    var flightdetail = ctx.FlightScheduleDetails.Where(x => x.Id == tickets.FlightScheduleDetailId).FirstOrDefault();
                    if (flightdetail != null)
                    {
                        flightdetail.SeatAvailability -= tickets.PassengerCount;
                    }

                    ctx.SaveChanges();
                    IList<TicketDetailViewModel> ticketdetails = null;
                    var t = new TicketDetailViewModel();

                    ticketdetails = (from flight in ctx.Flights
                                     join flightDetail in ctx.FlightDetails
                                     on flight.Id equals flightDetail.FlightId
                                     join flightSchedule in ctx.FlightScheduleDetails
                                     on flightDetail.Id equals flightSchedule.FlightDetailId
                                     where flightSchedule.Id == tickets.FlightScheduleDetailId
                                     select new TicketDetailViewModel
                                     {
                                         FlightName = flight.FlightName,
                                         JourneyDate = flightSchedule.JourneyDate,
                                         FromCity = flightDetail.FromCity,
                                         ToCity = flightDetail.ToCity,
                                         Price = flightSchedule.Price,
                                         PassengerCount = ticket.PassengerCount,
                                         TotalFare = ticket.TotalFare,
                                         BookingStatus = ticket.BookingStatus.ToString(),
                                         Id = tickets.Id,
                                         Departure = flightDetail.Departure,
                                         Arrival = flightDetail.Arrival
                                     }).ToList<TicketDetailViewModel>();

                    CreatioLogin login = new CreatioLogin("http://localhost:86/", "Supervisor", "Supervisor");
                    var loginRequest = login.TryLogin();

                    CreatioLogin logins = new CreatioLogin("http://localhost:86/");
                    logins.CallWebService(loginRequest, ticketdetails);

                    return Json(new { id = tickets.Id });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [Authorize]
        [Route("api/TicketDetail/GetTicketDetail/{userId}")]
        public IHttpActionResult GetTicketDetail(string userId)
        {
            try
            {
                IList<TicketDetailViewModel> tickets = null;

                using (var ctx = new BookingFlightEntities())
                {
                    tickets = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightSchedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightSchedule.FlightDetailId
                               join ticket in ctx.TicketDetails
                               on flightDetail.Id equals ticket.FlightScheduleDetailId
                               join history in ctx.UserTicketHistories
                               on ticket.Id equals history.TicketDetailId
                               where userId.Equals(history.UserId)
                               && ticket.BookingStatus == BookingStatusValues.Confirmed
                               && flightSchedule.JourneyDate >= DateTime.Now
                               select new TicketDetailViewModel
                               {
                                   FlightName = flight.FlightName,
                                   JourneyDate = flightSchedule.JourneyDate,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightSchedule.Price,
                                   PassengerCount = ticket.PassengerCount,
                                   TotalFare = ticket.TotalFare,
                                   BookingStatus = "Confirmed",
                                   Id = ticket.Id
                               }).ToList<TicketDetailViewModel>();
                }

                if (!tickets.Any())
                {
                    return NotFound();
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [Route("api/TicketDetail/GetTicketDetailHistory/{userId}")]
        public IHttpActionResult GetTicketDetailHistory(string userId)
        {
            try
            {
                IList<TicketDetailViewModel> tickets = null;

                using (var ctx = new BookingFlightEntities())
                {
                    tickets = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightSchedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightSchedule.FlightDetailId
                               join ticket in ctx.TicketDetails
                               on flightDetail.Id equals ticket.FlightScheduleDetailId
                               join history in ctx.UserTicketHistories
                               on ticket.Id equals history.TicketDetailId
                               where userId == history.UserId
                               select new TicketDetailViewModel
                               {
                                   FlightName = flight.FlightName,
                                   JourneyDate = flightSchedule.JourneyDate,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightSchedule.Price,
                                   PassengerCount = ticket.PassengerCount,
                                   TotalFare = ticket.TotalFare,
                                   BookingStatus = ticket.BookingStatus.ToString(),
                                   Id = ticket.Id
                               }).Distinct().ToList<TicketDetailViewModel>();
                }

                if (!tickets.Any())
                {
                    return NotFound();
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [Route("api/TicketDetail/GetTicketDetailByTicketId/{ticketid}")]
        public IHttpActionResult GetTicketDetailByTicketId(int ticketid)
        {
            try
            {
                IList<TicketDetailViewModel> tickets = null;

                using (var ctx = new BookingFlightEntities())
                {
                    tickets = (from flight in ctx.Flights
                               join flightDetail in ctx.FlightDetails
                               on flight.Id equals flightDetail.FlightId
                               join flightSchedule in ctx.FlightScheduleDetails
                               on flightDetail.Id equals flightSchedule.FlightDetailId
                               join ticket in ctx.TicketDetails
                               on flightDetail.Id equals ticket.FlightScheduleDetailId
                               join history in ctx.UserTicketHistories
                               on ticket.Id equals history.TicketDetailId
                               where ticketid == ticket.Id
                               && ticket.BookingStatus == BookingStatusValues.Confirmed
                               && flightSchedule.JourneyDate >= DateTime.Now
                               select new TicketDetailViewModel
                               {
                                   FlightName = flight.FlightName,
                                   JourneyDate = flightSchedule.JourneyDate,
                                   FromCity = flightDetail.FromCity,
                                   ToCity = flightDetail.ToCity,
                                   Price = flightSchedule.Price,
                                   PassengerCount = ticket.PassengerCount,
                                   TotalFare = ticket.TotalFare,
                                   BookingStatus = "Confirmed",
                                   Id = ticket.Id
                               }).ToList<TicketDetailViewModel>();
                }

                if (!tickets.Any())
                {
                    return NotFound();
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
        [Route("api/TicketDetail/PutTicketCancellation/{ticketId}")]
        public IHttpActionResult PutTicketCancellation(int ticketId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");

                using (var ctx = new BookingFlightEntities())
                {
                    var ticketToBeCancelled = ctx.TicketDetails.Where(x => x.Id == ticketId).FirstOrDefault<TicketDetail>();

                    if (ticketToBeCancelled != null)
                    {
                        ticketToBeCancelled.BookingStatus = BookingStatusValues.Cancelled;
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

    }
}
