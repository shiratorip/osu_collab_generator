using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace osuCollabGenerator
{
    public static class MainWindowHelpers
    {
            
        public static async Task<UserCompact[]?> SearchUsers(string url)
        {

            using var client = new HttpClient();

            HttpResponseMessage responseMessage = await client.GetAsync(url);
            if (responseMessage.IsSuccessStatusCode)
            {
                string response = await responseMessage.Content.ReadAsStringAsync();
                try
                {
                    UserCompact[]? users = JsonConvert.DeserializeObject<UserCompact[]>(response);
                    return users;
                }
                catch (JsonSerializationException ex)
                {
                    Trace.WriteLine("=======================");
                }
                return null;

            }
            else
            {
                return null;
            }


        }

        public static async Task<Uri?> UploadImage(byte[] imageBytes)
        {
            using var client = new HttpClient();

            HttpContent content = new MultipartFormDataContent
            {
                new ByteArrayContent(imageBytes)
            };
            HttpResponseMessage responseMessage = await client.PostAsync("https://osu-collab-generator-api.shuttleapp.rs/image_upload", content);
            if (responseMessage.IsSuccessStatusCode)
            {
                string link = await responseMessage.Content.ReadAsStringAsync();
                return new Uri(link);
            }
            return null;
        }
    }
}