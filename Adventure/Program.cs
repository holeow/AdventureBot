using Discord;
using Discord.WebSocket;
using Discord.Net;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;
using Adventure.SlashCommands;
using Adventure.Helpers;

namespace Adventure
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public static DiscordSocketClient Client;

        public async Task MainAsync()
        {
            Client = new DiscordSocketClient();

            Client.Log += Log;
            Client.Ready += Client_Ready;
            Client.SlashCommandExecuted += OnReceiveSlashCommand;

            
            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = FileManager.SecretConfig["token"];

            
            //"TEST_GUILD_ID": "916380934406811738"
            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();


            //FileManager.Config["DevGuildID"] = 916380934406811738.ToString();
            //FileManager.Config.Save();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            ulong devGuildId = ulong.Parse(FileManager.Config["DevGuildID"]);

            await SlashCommandHandler.StartService();
            await SlashCommandHandler.RefreshGuild(devGuildId);
        }

        private async Task OnReceiveSlashCommand(SocketSlashCommand command)
        {

            await SlashCommandHandler.ExecuteSlashCommand(command);
        }


        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

    }
}