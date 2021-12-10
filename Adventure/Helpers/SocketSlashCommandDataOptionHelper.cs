using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Helpers
{
    public static class SocketSlashCommandDataOptionHelper
    {
        public static object? GrabOption(this IReadOnlyCollection<SocketSlashCommandDataOption>? options, string optionName)
        {
            
            return options?.FirstOrDefault(a => a.Name == optionName)?.Value;
        }

    }

    public class CommandOption 
    {
        public CommandOption(SocketSlashCommandDataOption opt)
        {
            Option = opt;
        }

        public SocketSlashCommandDataOption Option;
        public IReadOnlyCollection<SocketSlashCommandDataOption>? Options
        {
            get { return Option?.Options; }
        }
        public SocketSlashCommandDataOption SubOption
        { get { return Options.First(); } }

        public static implicit operator CommandOption(SocketSlashCommandDataOption option)
        {
            return new CommandOption(option);
        }

        public object? this[ string s]
        {
            get
            {
                return Option.Options?.FirstOrDefault(a => a.Name == s)?.Value;
            }
        }

    }
}
