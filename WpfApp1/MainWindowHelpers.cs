using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OCG
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
    }
}