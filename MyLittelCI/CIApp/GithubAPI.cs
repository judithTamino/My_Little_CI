using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CIApp
{
    internal class GithubAPI
    {
        private GithubAPIHelper helper = new GithubAPIHelper();

        private RestClient client;
        private RestRequest request;
        private RestResponse response;

        private async Task<JToken> MakePostRequest(string endpoint, string payload)
        {
            JToken content = new JObject();

            client = helper.SetUrl(endpoint);
            request = helper.CreatePostRequest(payload);
            response = await helper.GetResponse(client, request);

            if (response.StatusCode.Equals(HttpStatusCode.Created) || response.StatusCode.Equals(HttpStatusCode.OK))
                content = helper.GetContent(response);

            return content;
        }

        private async Task<JToken> MakeGetRequest(string endpoint)
        {
            JToken content = new JObject();

            client = helper.SetUrl(endpoint);
            request = helper.CreateGetRequest();
            response = await helper.GetResponse(client, request);

            if (response.StatusCode.Equals(HttpStatusCode.OK))
                content = helper.GetContent(response);

            return content;
        }

        // create blob (binary large object) to store the contents of the file.
        // return encoded file sha.
        public async Task<string> CreateBlob(string encodedFile)
        {
            string fileSha = "";
            string endpoint = "git/blobs";
            string payload = JsonConvert.SerializeObject(new { content = encodedFile, encoding = "base64" });

            JToken content = await MakePostRequest(endpoint, payload);
            if (content.HasValues)
                fileSha = content["sha"].ToString();

            return fileSha;
        }

        public async Task<string> GetCurrentCommit()
        {
            string currentCommitSha = "";
            string endpoint = "git/ref/heads/main";

            JToken content = await MakeGetRequest(endpoint);
            if (content.HasValues)
                currentCommitSha = content["object"]["sha"].ToString();

            return currentCommitSha;
        }

        public async Task<string> GetBaseTree(string currentCommitSha)
        {
            string baseTreeSha = "";
            string endpoint = "git/commits/" + currentCommitSha;

            JToken content = await MakeGetRequest(endpoint);
            if (content.HasValues)
                baseTreeSha = content["tree"]["sha"].ToString();

            return baseTreeSha;
        }

        public async Task<string> CreateTree(string baseTreeSha, string fileSha, string githubPath)
        {
            string newTreeSha = "";
            string endpoint = "git/trees";

            var treeData = new { path = githubPath, mode = "100644", type = "blob", sha = fileSha };
            string payload = JsonConvert.SerializeObject(new { tree = new object[] { treeData }, base_tree = baseTreeSha });

            JToken content = await MakePostRequest(endpoint, payload);
            if (content.HasValues)
                newTreeSha = content["sha"].ToString();

            return newTreeSha;
        }

        public async Task<string> AddCommit(string newTreeSha, string currentCommitSha)
        {
            string newCommitSha = "";

            Console.Write("ENTER NEW COMMIT MESSAGE: ");
            string commitMessage = Console.ReadLine();

            string endpoint = "git/commits";
            string payload = JsonConvert.SerializeObject(new { tree = newTreeSha, message = commitMessage, parents = new string[] { currentCommitSha } });

            JToken content = await MakePostRequest(endpoint, payload);
            if (content.HasValues)
                newCommitSha = content["sha"].ToString();

            return newCommitSha;
        }

        public async void UpdateRepository(string newCommitSha)
        {
            string endpoint = "git/refs/heads/main";
            string payload = JsonConvert.SerializeObject(new { sha = newCommitSha });

            JToken content = await MakePostRequest(endpoint, payload);
            if (content.HasValues)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("UPLOAD SUCCESSFULLY");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("UPLOAD UNSUCCESSFULLY");
                Console.ResetColor();
            }
        }

        public async Task<string[]> CreateTagObject(string latestCommitSha)
        {
            string endpoint = "git/tags";
            string tagName = $"v.{Guid.NewGuid()}";
            string[] tag = new string[2];

            // create tag
            JObject payload = new JObject();
            JProperty Jtag = new JProperty("tag", tagName);
            JProperty Jmessage = new JProperty("message", "initial version");
            JProperty Jobject = new JProperty("object", latestCommitSha);
            JProperty Jtype = new JProperty("type", "commit");

            payload.Add(Jtag);
            payload.Add(Jmessage);
            payload.Add(Jobject);
            payload.Add(Jtype);

            JToken content = await MakePostRequest(endpoint, payload.ToString());
            if(content.HasValues)
            {
                tag[0] = content["tag"].ToString();
                tag[1] = content["sha"].ToString();
            }

            return tag;
        }

        public async Task<bool> AppendTagToRepository(string[] tag)
        {
            string endpoint = "git/refs";

            JObject payload = new JObject();
            JProperty Jreference = new JProperty("ref", $"refs/tags/{tag[0]}");
            JProperty Jsha = new JProperty("sha", tag[1]);

            payload.Add(Jreference);
            payload.Add(Jsha);

            JToken content = await MakePostRequest(endpoint, payload.ToString());
            if (content.HasValues)
                return true;
            return false;
        }
    }
}
