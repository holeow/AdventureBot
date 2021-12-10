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

            
            
            var token = FileManager.SecretConfig["token"];

            
            

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();


            

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