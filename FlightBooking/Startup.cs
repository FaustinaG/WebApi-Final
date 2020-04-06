using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FlightBooking.Startup))]

namespace FlightBooking
{
    public partial class Startup
    {
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
