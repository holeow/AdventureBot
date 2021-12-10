using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.SlashCommands
{
    [AdventureSlashCommand]
    public class HelpCommand : AdventureSlashCommand
    {
        public HelpCommand()
        {
            var CommandSubCommand = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("command").WithDescription("Get help with a command.");
            CommandSubCommand.AddOption("commandname", ApplicationCommandOptionType.String, "The name of the command", required: true);

            var GetStartedSubCommand = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("getstarted").WithDescription("Get help with the bot.");

            StartBuilder("Help", "Need some help? Use me !").
                AddOption(CommandSubCommand).AddOption(GetStartedSubCommand);

        }

        public override async Task ExecuteSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Options.First().Name)
            {
                case "command": await HelpWithCommand(command); break;
                case "getstarted": await GetStarted(command); break;
            }
        }

        public async Task HelpWithCommand(SocketSlashCommand command)
        {
            var commandName = command.Data.Options.First().Options?.First(a=> a.Name == "commandname").Value as string;

            if (SlashCommandHandler.commands.TryGetValue(commandName.ToLower(), out var finalCommand))
            {
                await finalCommand.Help(command);
            }
            else
            {
                await command.RespondAsync("command not found");
            }
        }

        public async Task GetStarted(SocketSlashCommand command)
        {
            await command.RespondAsync("Not yet Implemented help message.");
        }


        public override async Task Help(SocketSlashCommand command)
        {
            await command.RespondAsync("The Help command. \n" +
                "Usage: /help getstarted\n" +
                "Or: /help command ping");
        }
    }
}
