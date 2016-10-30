using Homero.Core.Messages.Attachments;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Homero.Core.Services
{
    public class UploaderService : IUploader
    {
        private IConfiguration _config;

        public UploaderService(IConfiguration config)
        {
            _config = config;
        }

        public Task<string> Upload(FileAttachment image)
        {
            throw new NotImplementedException();
        }

        public Task<string> Upload(AudioAttachment image)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Upload(ImageAttachment image)
        {
            using (HttpClient client = new HttpClient())
            {
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new ByteArrayContent(image.Data), "image");

                client.DefaultRequestHeaders.Add("Authorization", "Client-ID " + _config.GetValue<string>("imgur_client_id"));

                HttpResponseMessage response = await client.PostAsync("https://api.imgur.com/3/image", content);
                string jsonSource = await response.Content.ReadAsStringAsync();
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(jsonSource);

                if ((bool)jsonObj.GetValue("success"))
                {
                    return jsonObj.SelectToken("data.link").ToString();
                }
                return null;
            }
        }
    }
}