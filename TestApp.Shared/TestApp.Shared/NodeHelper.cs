using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using RestSharp;
using RestSharp.Deserializers;
using System.Threading.Tasks;

namespace TestApp.Shared
{

    struct ResponseServer
    {
        public long ServerTimeStamp { get; set; }
        public long TimeDifference { get; set; }
    }

    class AnimationStory
    {
        public int AnimationStoryID { get; set; }
        public DateTime StartDateTime { get; set; }
    }

    struct ResponseTime
    {
        public DateTime ServerTime { get; set; }
        public DateTime CurrentTime { get; set; }
        public long ParameterTimeStamp { get; set; }
        public long ServerTimeStamp { get; set; }
        public long CurrentTimeStamp { get; set; }
        public decimal ServerToAppTime { get; set; }
        public decimal MiddleWareMilliSeconds { get; set; }
        public long ResponseMilliSeconds { get; set; }
        public long DifferenceMilliSeconds { get; set; }
        public ResponseServer ResponseServer { get; set; }
    }

    class NodeHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);



        public async Task<ResponseTime> Calculate()
        {
            var bestResponse =  await Task.Run(() => GetBestResponse());

            bestResponse.ServerTime = UnixEpoch.AddMilliseconds(bestResponse.ServerTimeStamp);
            bestResponse.CurrentTime = UnixEpoch.AddMilliseconds(bestResponse.CurrentTimeStamp);

            var serverCalculate = bestResponse.ServerTimeStamp - bestResponse.ParameterTimeStamp;
            var clientCalculate = bestResponse.ServerTimeStamp - bestResponse.CurrentTimeStamp;

            var absServer = Math.Abs(serverCalculate);
            var absClient = Math.Abs(clientCalculate);
            var totalCalculate = absServer + absClient;

            decimal percentServer = (decimal)serverCalculate / totalCalculate;
            decimal percentClient = (decimal)clientCalculate / totalCalculate;

            var goToServerTime = bestResponse.ResponseMilliSeconds * percentServer;
            var goToClientTime = bestResponse.ResponseMilliSeconds * percentClient;

            bestResponse.DifferenceMilliSeconds = Convert.ToInt64(clientCalculate - goToClientTime);

            return bestResponse;
        }

        public ResponseTime GetBestResponse()
        {
            var responsesServerUTC = new List<ResponseTime>();

            for (int i = 0; i <= 3; i++)
            {
                responsesServerUTC.Add(GetServerTimeDifference());
            }

            return responsesServerUTC.OrderBy(s => s.ResponseMilliSeconds).First();

        }

        public ResponseTime GetServerTimeDifference()
        {

            var responseTime = new ResponseTime();
            var client = new RestClient("http://lightus2-timetest.rhcloud.com/");

            var request = new RestRequest("GetCurrentUTC");
            var beforeTimeStamp = Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalMilliseconds);
            IRestResponse response = client.Execute(request);
            responseTime.CurrentTimeStamp = Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalMilliseconds);

            responseTime.ParameterTimeStamp = beforeTimeStamp;
            responseTime.ServerTimeStamp = Convert.ToInt64(response.Content);
            responseTime.ResponseMilliSeconds = responseTime.CurrentTimeStamp - responseTime.ParameterTimeStamp;

            return responseTime;

        }

        public AnimationStory GetAnimationStory()
        {

            var client = new RestClient("http://lightus2-timetest.rhcloud.com/");

            RestRequest request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.Resource = "GetAnimationStartDateTime/{animationID}";
            request.AddUrlSegment("animationID", "1");

            IRestResponse response = client.Execute(request);

            JsonDeserializer deserial = new JsonDeserializer();            
            var animationStory = deserial.Deserialize<AnimationStory>(response);

            return animationStory;

        }
    }
}
