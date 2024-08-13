using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LorendosBotUntis.conf;
using System.Security.Cryptography;
using System.Text;
using Untis.Net;


namespace LorendosBotUntis.discordCommands.prefixBasic
{
    public class prefixCommands : BaseCommandModule
    {
        //Units Client
        public static UntisClient untisClient { get; set; }
        //Date shenanigans 
        static DateTime today = DateTime.Today; // Get today's date
        static DateOnly monday = DateOnly.FromDateTime(today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday));
        static DateOnly friday = monday.AddDays(4); // Add 4 days to Monday to get Friday

        [Command("kys")]
        public async Task shutOffDiscordBot(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("I'm starting to mog you");
            System.Environment.Exit(0);
        }
        [Command("reload")]
        public async Task reload(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("reloading");
        }


        [Command("Info")]
        public async Task plan(CommandContext ctx)
        {
            // Init secrets
            var jsonReader = new jsonReader();
            await jsonReader.ReadJSON();

            // Untis Setup

            var untisClient = new UntisClient("Andreas-Gordon-Schule", "hektor.webuntis.com");
            await untisClient.LoginAsync(jsonReader.untisLogin, jsonReader.untisPassword);

            // Getting Homework
            var homeworks = await untisClient.GetHomeworksAsync(monday, friday);
            var value = homeworks.GetValueOrThrow();

            // Aggregate homework information
            var homeworkText = new StringBuilder();

            if (homeworks.TryGetError(out Exception error))
            {
                await ctx.Channel.SendMessageAsync("Bei der Ausführung gab es eine Problem! Probieren sie es noch einmal, sonst melden sie sich bei den Administrator");
                return;
            }
            else
            {
                foreach (var item in value)
                {
                    // Append each homework item to the string
                    homeworkText.AppendLine($"Fach: {item.Subject.Name}, bis {item.DueDate}");
                    homeworkText.AppendLine(item.Text);
                    homeworkText.AppendLine("");
                }
            }

            // Discord message setup
            var planMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                    .WithColor(DiscordColor.Azure)
                    .WithTitle("Info : Hausaufgaben, Nachrichten")
                    .AddField("Hausaufgaben", homeworkText.ToString())); // Add the aggregated homework information to the field

            // Send the message
            await ctx.Channel.SendMessageAsync(planMessage);
            await untisClient.LogoutAsync();
        }

    }
}
