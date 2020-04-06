using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlightBooking.Controllers
{
    public class UserTicketHistoryController : ApiController
    {
        [Authorize]
        public IHttpActionResult PostUserTicketHistory(UserTicketHistory history)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");

                using (var ctx = new BookingFlightEntities())
                {
                    ctx.UserTicketHistories.Add(new UserTicketHistory()
                    {
                        UserId = history.UserId,
                        TicketDetailId = history.TicketDetailId
                    });

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
