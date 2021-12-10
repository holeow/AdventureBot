using Discord.WebSocket;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Helpers
{
    public static class SocketGuildUserHelper
    {
        public static string FinalNick(this SocketGuildUser user)
        {
            return user?.Nickname == null ? user.Username : user.Nickname;
        }
    }
}
