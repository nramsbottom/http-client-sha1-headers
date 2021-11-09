using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignedHttpRequests
{
    class AppendSha1HashHeaderHandler : DelegatingHandler
    {
        private readonly string _secret;

        public AppendSha1HashHeaderHandler(string secret, HttpMessageHandler next) : base(next)
        {
            _secret = secret;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Post && request.Method != HttpMethod.Put)
                return await base.SendAsync(request, cancellationToken);

            var body = await request.Content.ReadAsStringAsync(cancellationToken);
            var hash = Hash(_secret + body);

            request.Headers.Add("X-Signature", hash);

            return await base.SendAsync(request, cancellationToken);
        }

        static string Hash(string input)
        {
            using SHA1Managed sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
