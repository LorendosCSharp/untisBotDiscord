using System.Text;
using DSharpPlus.Entities;
using LorendosBotUntis.conf;
using Untis.Net;

namespace LorendosBotUntis.misc;

public class planRoutine
{
    
    static DateTime today = DateTime.Today; // Get today's date
    static DateOnly monday = DateOnly.FromDateTime(today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday));
    static DateOnly friday = monday.AddDays(4); // Add 4 days to Monday to get Friday

    private List<ulong> fetchedHomeworks = new();
    
    public async void getPlan(DiscordChannel discordChannel)
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
            await discordChannel.SendMessageAsync("Bei der Ausführung gab es eine Problem! Probieren sie es noch einmal, sonst melden sie sich bei den Administrator");
            return;
        }
        
        foreach (var item in value)
        {
            if(fetchedHomeworks.Contains(item.Id)) continue;
            fetchedHomeworks.Add(item.Id);
            
            // Append each homework item to the string
            homeworkText.AppendLine($"Fach: {item.Subject.Name}, bis {item.DueDate}");
            homeworkText.AppendLine(item.Text);
            homeworkText.AppendLine("");
        }

        // Discord message setup
        var planMessage = new DiscordMessageBuilder()
            .AddEmbed(new DiscordEmbedBuilder()
                .WithColor(DiscordColor.Azure)
                .WithTitle("Info : Hausaufgaben, Nachrichten")
                .AddField("Hausaufgaben", homeworkText.ToString())); // Add the aggregated homework information to the field

        // Send the message
        await discordChannel.SendMessageAsync(planMessage);
        await untisClient.LogoutAsync();

        await Task.Delay(TimeSpan.FromMinutes(2));
        getPlan(discordChannel);
    }
}