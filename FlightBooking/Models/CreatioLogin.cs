using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlightBooking.Models
{
    public class CreatioLogin
    {
        private readonly string _appUrl;
        private CookieContainer _authCookie;
        private readonly string _authServiceUrl;
        private readonly string _userName;
        private readonly string _userPassword;
        public CreatioLogin(string appUrl, string userName, string userPassword)
        {
            _appUrl = appUrl;
            _authServiceUrl = _appUrl + @"/ServiceModel/AuthService.svc/Login";
            _userName = userName;
            _userPassword = userPassword;
        }
        public CreatioLogin(string appUrl)
        {
            _appUrl = appUrl;

        }
        public HttpWebRequest TryLogin()
        {
            var authData = @"{
                    ""UserName"":""" + _userName + @""",
                    ""UserPassword"":""" + _userPassword + @"""
                }";

            var request = CreateRequest(_authServiceUrl, authData);
            _authCookie = new CookieContainer();
            request.CookieContainer = _authCookie;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseMessage = reader.ReadToEnd();
                        Console.WriteLine(responseMessage);
                        if (responseMessage.Contains("\"Code\":1"))
                        {
                            throw new UnauthorizedAccessException($"Unauthorized {_userName} for {_appUrl}");
                        }
                    }
                    string authName = ".ASPXAUTH";
                    string authCookeValue = response.Cookies[authName].Value;
                    _authCookie.Add(new Uri(_appUrl), new Cookie(authName, authCookeValue));

                }

            }
            return request;
        }
        private void AddCsrfToken(HttpWebRequest request)
        {
            var cookie = request.CookieContainer.GetCookies(new Uri(_appUrl))["BPMCSRF"];
            if (cookie != null)
            {
                request.Headers.Add("BPMCSRF", cookie.Value);
            }
        }
        private HttpWebRequest CreateRequest(string url, string requestData = null)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.KeepAlive = true;
            if (!string.IsNullOrEmpty(requestData))
            {
                using (var requestStream = request.GetRequestStream())
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        writer.Write(requestData);
                    }
                }
            }
            return request;
        }

        public string CallGetMethod(HttpWebRequest request)
        {
            Cookie cookie1 = request.CookieContainer.GetCookies(new Uri(_appUrl))["BPMCSRF"];
            Cookie cookie2 = request.CookieContainer.GetCookies(new Uri(_appUrl))[".ASPXAUTH"];
            Cookie cookie3 = request.CookieContainer.GetCookies(new Uri(_appUrl))["BPMLOADER"];
            Cookie cookie4 = request.CookieContainer.GetCookies(new Uri(_appUrl))["UserName"];

            string getURL = _appUrl + @"/0/rest/IframeApiCall/GetAccessToken";
            var getRequest = CreateRequestGET(getURL);

            getRequest.Headers.Add("BPMCSRF", cookie1.Value);


            getRequest.CookieContainer = new CookieContainer();
            getRequest.CookieContainer.Add(cookie1);
            getRequest.CookieContainer.Add(cookie2);
            getRequest.CookieContainer.Add(cookie3);
            getRequest.CookieContainer.Add(cookie4);


            //AddCsrfToken(request);

            using (var response = (HttpWebResponse)getRequest.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseMessage = reader.ReadToEnd();
                        return responseMessage;
                    }
                }
                return string.Empty;
            }
        }
        private HttpWebRequest CreateRequestGET(string getURL)
        {
            HttpWebRequest getRequest = (HttpWebRequest)WebRequest.Create(getURL);
            getRequest.ContentType = "application/json";
            getRequest.Method = "GET";
            getRequest.KeepAlive = true;
            getRequest.UseDefaultCredentials = true;
            getRequest.PreAuthenticate = true;
            getRequest.Credentials = CredentialCache.DefaultCredentials;

            return getRequest;
        }

        public void CallWebService(HttpWebRequest authRequest, IList<TicketDetailViewModel> data)
        {
            var x = data[0];
            string JourneyDate = string.Format("{0:dd/MM/yyyy}", x.JourneyDate);

            //string JourneyDate = "05-06-2020";

            DateTime dt = DateTime.Parse((data[0].Arrival).ToString());
            string Arrival = dt.ToString("hh:mm tt");

            DateTime dt1 = DateTime.Parse((data[0].Departure).ToString());
            string Depature = dt1.ToString("hh:mm tt");


            string dataString = x.FromCity + "&ToCity=" + x.ToCity + "&DOJ=" + JourneyDate + "&PassangerCount=" + x.PassengerCount +
                                "&TicketId=" + x.Id + "&Price=" + x.Price + "&TotalCost=" + x.TotalFare +
                                "&Arrival=" + Arrival + "&Depature=" + Depature;
            string serviceURL = _appUrl + @"0/rest/UsrPOSTservice/GetName?FromCity=" + dataString;

            Cookie cookie1 = authRequest.CookieContainer.GetCookies(new Uri(_appUrl))["BPMCSRF"];
            Cookie cookie2 = authRequest.CookieContainer.GetCookies(new Uri(_appUrl))[".ASPXAUTH"];
            Cookie cookie3 = authRequest.CookieContainer.GetCookies(new Uri(_appUrl))["BPMLOADER"];
            Cookie cookie4 = authRequest.CookieContainer.GetCookies(new Uri(_appUrl))["UserName"];

            var request1 = CreateRequestPOST(serviceURL);

            request1.Headers.Add("BPMCSRF", cookie1.Value);

            request1.CookieContainer = new CookieContainer();
            request1.CookieContainer.Add(cookie1);
            request1.CookieContainer.Add(cookie2);
            request1.CookieContainer.Add(cookie3);
            request1.CookieContainer.Add(cookie4);


            using (var response = (HttpWebResponse)request1.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseMessage = reader.ReadToEnd();
                        //Console.WriteLine(responseMessage);
                    }
                }
                else
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var responseMessage = reader.ReadToEnd();
                        //Console.WriteLine(responseMessage);
                    }
                }
            }

        }
        private HttpWebRequest CreateRequestPOST(string serviceURL)
        {
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(serviceURL);
            request1.ContentType = "application/json";
            request1.Method = "GET";
            request1.KeepAlive = true;
            request1.UseDefaultCredentials = true;
            request1.PreAuthenticate = true;
            request1.Credentials = CredentialCache.DefaultCredentials;

            return request1;
        }

        public async Task<string> CallWebApi(string username,string password)
        {
            try
            {
                /*HttpClient client = new HttpClient();
                var login = "grant_type=password&username=fg@gmail.com&password=Newuser@1";

                var data = new StringContent(login, Encoding.UTF8, "text/plain");

                var response = await client.PostAsync("http://localhost:60483/token", data);
                ResponseRootObject ResponseRootObject = new ResponseRootObject();
                string responseText = response.Content.ReadAsStringAsync().Result;
                try
                {
                    ResponseRootObject = JsonConvert.DeserializeObject<ResponseRootObject>(responseText);
                }
                catch (Exception ex)
                {
                    throw new HttpException(500, "DeserializeObject Error : " + ex.ToString());
                }
                var accesstoken = ResponseRootObject.access_token;
                return accesstoken;*/
                string baseAddress = "http://localhost:60483";
                using (var client = new HttpClient())
                {
                    var form = new Dictionary<string, string>
                           {
                               {"grant_type", "password"},
                               {"username", "fg@gmail.com"},
                               {"password", "Newuser@1"},
                           };
                    var tokenResponse = await client.PostAsync(baseAddress + "/oauth/token", new FormUrlEncodedContent(form));
                    var token = tokenResponse.Content.ReadAsStringAsync().Result;
                    ResponseRootObject ResponseRootObject = new ResponseRootObject();
                    ResponseRootObject = JsonConvert.DeserializeObject<ResponseRootObject>(token);
                    return ResponseRootObject.access_token;
                    //var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                    /*if (string.IsNullOrEmpty(token.Error))
                    {
                        //Console.WriteLine("Token issued is: {0}", token.AccessToken);
                        return token.AccessToken;
                    }
                    else
                    {
                        //Console.WriteLine("Error : {0}", token.Error);
                        return token.Error;
                    }*/
                    //Console.Read();

                    /*username = "fg@gmail.com";
                    password = "Newuser@1";
                    var grant = "password";
                    var authData = @"{
                        ""grant_type"":""" + grant + @""",
                        ""username"":""" + username + @""",
                        ""password"":""" + password + @"""
                    }";
                    string _appUrl = "http://localhost:60483/token";
                    string serviceURL = _appUrl;

                    var request1 = CreateRequest(serviceURL, authData);



                    using (var response = (HttpWebResponse)request1.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                var responseMessage = reader.ReadToEnd();
                                return responseMessage;
                            }
                        }
                        else
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                var responseMessage = reader.ReadToEnd();
                                return responseMessage;
                            }
                        }
                    } */
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /*public void getToken()
        {
            //Getting a token is rather simple
            //Using a class based variable is probably a good idea since this is only set once but remains constant
            //If this is not desired then the token will need to be carried via other methods
            //Reuse of this value will be necessary among all your calls
            //Use this method first because none of the other calls can work without your access toekn
            RestSharp.Deserializers.JsonDeserializer deserial = new RestSharp.Deserializers.JsonDeserializer();
            var client = new RestClient("https://api.us.onelogin.com/auth/oauth2/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", "client_id:yourClientID, client_secret:yourClientSecret");
            request.AddParameter("application/json", "{\n\"grant_type\":\"client_credentials\"\n}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            RootObject returnData = deserial.Deserialize<RootObject>(response);
            if (returnData.data[0].access_token != null)
            {
                access = returnData.data[0].access_token; //This correctly gets the Access Token. You should return this to a class variable so that all the  other functions can access it easily and you're not constantly passing along the variable through them.

            }
        }*/
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
    }

    public class ResponseRootObject
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string userName { get; set; }
        public string roleType { get; set; }
        public string userId { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
    }
}