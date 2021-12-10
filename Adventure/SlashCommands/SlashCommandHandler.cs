using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Net;
using Newtonsoft.Json;
using Discord.Commands;
using System.Reflection;

namespace Adventure.SlashCommands
{
    public static class SlashCommandHandler
    {
        public static async Task StartService()
        {
            var typesWithMyAttribute =
             // Partition on the type list initially.
             from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().AsParallel()
             let attributes = t.GetCustomAttributes(typeof(AdventureSlashCommandAttribute), true)
             where attributes != null && attributes.Length > 0
             select new { Type = t, Attributes = attributes.Cast<AdventureSlashCommandAttribute>() };


            foreach (var item in typesWithMyAttribute)
            {
                var instance = Activator.CreateInstance(item.Type);
                if(instance is AdventureSlashCommand)
                {
                    var i = instance as AdventureSlashCommand;
                    i.CommandProperties = i.CommandBuilder.Build();
                    commands.Add(i.Name, i);
                    
                }
                else
                {
                    Console.WriteLine($"Impossible to setup {instance.GetType().FullName} as AdventureSlashCommand as only classes inheriting the abstract class AdventureSlashCommand are usable with this attribute.");
                }
            }
        }

        public static async Task RefreshGuild(ulong GuildID)
        {
            var guild = Program.Client.GetGuild(GuildID);
            await guild.DeleteApplicationCommandsAsync();
            foreach (var item in commands)
            {
                try
                {
                    
                    await guild.CreateApplicationCommandAsync(item.Value.CommandProperties);
                    
                    
                }
                catch (Exception exception)
                {
                    var json = JsonConvert.SerializeObject(exception, Formatting.Indented);

                    // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                    Console.WriteLine(json);
                }
                
                
            }
            Console.WriteLine($"Guild:\"{guild.Name}\" refreshed !");
        }

        public static Dictionary<string,AdventureSlashCommand> commands = new Dictionary<string, AdventureSlashCommand>();

        public static async Task ExecuteSlashCommand(SocketSlashCommand command)
        {
            if (commands.TryGetValue(command.Data.Name,out var finalCommand))
            {
               await finalCommand.ExecuteSlashCommand(command);
            }
            else
            {
                await command.RespondAsync("The command hasn't been found internally");
            }
        }
    }

    public abstract class AdventureSlashCommand
    {
        public string Name;
        public string Description;

        public SlashCommandBuilder StartBuilder(string name,string description)
        {
            name = name.ToLower();
            Name = name;
            Description = description;
            CommandBuilder = new SlashCommandBuilder().WithName(name).WithDescription(description);

            
            return CommandBuilder;
        }


        public SlashCommandBuilder CommandBuilder { get; set; }

        public ApplicationCommandProperties CommandProperties { get; set; }
        public abstract Task ExecuteSlashCommand(SocketSlashCommand command);

        public abstract Task Help(SocketSlashCommand command);

    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AdventureSlashCommandAttribute : Attribute
    {

    }
}
