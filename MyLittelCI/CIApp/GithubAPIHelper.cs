using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CIApp
{
    internal class GithubAPIHelper
    {
        private const string API_KEY = "GITHUB TOKEN";
        private const string BASE_URL = "https://api.github.com/repos/";
        private const string URL_PARAMS = "judithTamino/Demo_Project/";

        RestRequest request;

        public RestClient SetUrl(string endpoint)
        {
            string url = BASE_URL + URL_PARAMS + endpoint;
            RestClient client = new RestClient(url);
            return client;
        }

        public RestRequest CreatePostRequest(string payload)
        {
            request = new RestRequest();
            request.Method = Method.Post;
            request.AddHeader("Authorization", API_KEY);
            //request.AddHeader("Accept", "application/vnd.github.v3+json");
            request.AddParameter("application/json", payload, ParameterType.RequestBody);

            return request;
        }

        public RestRequest CreateGetRequest()
        {
            request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", API_KEY);

            return request;
        }

        public async Task<RestResponse> GetResponse(RestClient client, RestRequest request)
        {
            RestResponse response = await client.ExecuteAsync(request);
            return response;
        }

        public JToken GetContent(RestResponse response)
        {
            var content = response.Content;

            if (content[0].Equals('['))
                return JArray.Parse(content);

            return JObject.Parse(content);
        }
    }
}
