using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SignedHttpRequests
{
    class Program
    {
        static async Task Main(string[] args)
        {

            const string Secret = "abc123";
            const string Message = "Hello World";
            const string TargetUrl = "https://neilramsbottom.com";

            var http = new HttpClient(new AppendSha1HashHeaderHandler(Secret, new HttpClientHandler()));

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(TargetUrl))
            {
                Content = new StringContent(Message)
            };

            var response = await http.SendAsync(request);

            Console.WriteLine((int)response.StatusCode);
        }
    }
}
