using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace WpfApp.Services
{
    public class OllamaService
    {    

        public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, CancellationToken token)
        {
            var client = new HttpClient()
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
            var requestBody = new
            {
                model = "qwen2.5-coder:3b",
                prompt = prompt,
                stream = true
            };
            var httpRequest = new HttpRequestMessage(HttpMethod.Post,
                "http://localhost:11434/api/generate")
            {
                Content = JsonContent.Create(requestBody)
            };

            var response = await client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead,token);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                token.ThrowIfCancellationRequested();
                var line = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

               token.ThrowIfCancellationRequested();
                var chunk = JsonSerializer.Deserialize<OllamaChunk>(line);

                if (chunk?.response != null)
                    yield return chunk.response;
            }
        }
    }

    public class OllamaChunk
    {
        public string response { get; set; }
    }
}
