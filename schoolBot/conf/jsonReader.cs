using Newtonsoft.Json;

namespace LorendosBotUntis.conf
{
    internal class jsonReader
    {
        public string discordToken { get; set; }
        public string discordPrefix { get; set; }
        public string untisLogin { get; set; }
        public string untisPassword { get; set; }
        public ulong homeworkChannelId { get; private set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("conf.json"))
            {
                string json = await sr.ReadToEndAsync();
                ISAIDNO data = JsonConvert.DeserializeObject<ISAIDNO>(json);
                discordToken = data.token;         // Corrected property name
                discordPrefix = data.prefix;       // Corrected property name
                untisLogin = data.untisLogin;
                untisPassword = data.untisPassword;
                homeworkChannelId = data.homeworkChannelId;
            }
        }
    }
    internal sealed class ISAIDNO()
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string untisLogin { get; set; }
        public string untisPassword { get; set; }
        public ulong homeworkChannelId { get; init; }
    }
}
