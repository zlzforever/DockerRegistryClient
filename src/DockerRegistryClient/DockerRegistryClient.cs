using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DockerRegistryClient
{
    public static class DockerRegistryClient
    {
        private static readonly Dictionary<string, HttpClient> HttpClientDict = new Dictionary<string, HttpClient>();

        public static async Task<List<string>> GetRepositories(Uri registry, string user = null,
            string password = null)
        {
            var client = GetHttpClient(registry);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{registry}v2/_catalog");
            ConfigureAuthorization(httpRequestMessage, user, password);
            var response = await client.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                var repositories = jsonObject.SelectTokens("$.repositories[*]").Select(x => x.ToString()).ToList();
                return repositories;
            }
            else
            {
                return null;
            }
        }

        public static async Task<List<string>> GetTags(Uri registry, string image, string user = null,
            string password = null)
        {
            var client = GetHttpClient(registry);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{registry}v2/{image}/tags/list");
            ConfigureAuthorization(httpRequestMessage, user, password);
            var response = await client.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                var tags = jsonObject.SelectTokens("$.tags[*]").Select(x => x.ToString()).ToList();
                return tags;
            }
            else
            {
                return null;
            }
        }

        public static async Task<bool> DeleteImage(Uri registry, string image, string tag, string user = null,
            string password = null)
        {
            var client = GetHttpClient(registry);
            var requestSha256Message = new HttpRequestMessage(HttpMethod.Get,
                $"{registry}v2/{image}/manifests/{tag}");
            ConfigureAuthorization(requestSha256Message, user, password);
            requestSha256Message.Headers.Accept.TryParseAdd(
                "application/vnd.docker.distribution.manifest.v2+json");
            var response = await client.SendAsync(requestSha256Message);
            if (response.IsSuccessStatusCode)
            {
                var sha256 = response.Headers.ETag.Tag.Replace("\"", "");
                var deleteMessage =
                    new HttpRequestMessage(HttpMethod.Delete, $"{registry}v2/{image}/manifests/{sha256}");
                ConfigureAuthorization(deleteMessage, user, password);

                response = await client.SendAsync(deleteMessage);

                return response.IsSuccessStatusCode;
            }
            else
            {
                return false;
            }
        }

        private static HttpClient GetHttpClient(Uri registry)
        {
            var key = registry.ToString();
            if (!HttpClientDict.ContainsKey(key))
            {
                HttpClientDict.Add(key, new HttpClient());
            }

            return HttpClientDict[key];
        }

        private static void ConfigureAuthorization(HttpRequestMessage httpRequestMessage, string user, string password)
        {
            string token = null;
            if (!string.IsNullOrWhiteSpace(user))
            {
                token = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{user}:{password}"));
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", $"Basic {token}");
            }
        }
    }
}