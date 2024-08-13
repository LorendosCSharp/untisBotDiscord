using Untis.Net;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using LorendosBotUntis.conf;
using LorendosBotUntis.discordCommands.prefixBasic;
namespace LorendosBotUntis
{
    public sealed class Program
    {
        #region setup
        //Clients
        public static DiscordClient discordClient { get; set; }
        public static CommandsNextExtension commands { get; set; }


        #endregion

        static async Task Main(string[] args)
        {
            //Init secrets 
            var jsonReader = new jsonReader();
            await jsonReader.ReadJSON();
            //discordBot Setup (Config)
            var discordConf = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.discordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            discordClient = new DiscordClient(discordConf);

            discordClient.Ready += discordClientReady;

            var commandConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.discordPrefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            commands = discordClient.UseCommandsNext(commandConfig);
            commands.RegisterCommands<prefixCommands>();

            await discordClient.ConnectAsync();
            // secures bots life , you don't want to kill him , are you?
            await Task.Delay(-1);
        }

        private static Task discordClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}