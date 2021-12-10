using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.SlashCommands
{
    [AdventureSlashCommand]
    public class Ping : AdventureSlashCommand
    {
        public Ping()
        {
            StartBuilder("Ping", "Simple Ping command to check if bot is working.");
        }

        public override async Task ExecuteSlashCommand(SocketSlashCommand command)
        {
          await  command.RespondAsync("Pong");
        }

        public override async Task Help(SocketSlashCommand command)
        {
            await command.RespondAsync("Checks if the bot is connected and working. If it is, the bot will respond with \"Pong\". \n" +
                "Usage: /ping");
        }
    }
}
