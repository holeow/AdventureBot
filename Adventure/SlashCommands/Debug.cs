using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Helpers;

namespace Adventure.SlashCommands
{
    [AdventureSlashCommand]
    public class DebugCommand : AdventureSlashCommand
    {
        public DebugCommand()
        {
        /*group*/    
            var ResourceSubCommandGroup = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommandGroup).WithName("resources").WithDescription("Resources involved commands");


            var CreateResource = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("create").WithDescription("Invent a new resource and add it to the database");
            CreateResource.AddOption("name", ApplicationCommandOptionType.String, "Name of the resource", true);
            CreateResource.AddOption("description", ApplicationCommandOptionType.String, "Description of the resource", true);
            CreateResource.AddOption("category", ApplicationCommandOptionType.String, "Category of the resource", true);
            CreateResource.AddOption("level", ApplicationCommandOptionType.Integer, "Level of the resource", true);
            CreateResource.AddOption("tradevalue", ApplicationCommandOptionType.Integer, "Trade value of the resource", true);

            ResourceSubCommandGroup.AddOption(CreateResource);



            var LogAllResources = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("logall").WithDescription("Reply with a message containing all existing resources.");

            ResourceSubCommandGroup.AddOption(LogAllResources);

            var DeleteResource = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("delete").WithDescription("Delete a ressource from the database");
            DeleteResource.AddOption("name", ApplicationCommandOptionType.String, "Name of the resource",true);

            ResourceSubCommandGroup.AddOption(DeleteResource);


            var GiveResource = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("give").WithDescription("Put a resource in player's inventory");
            GiveResource.AddOption("user", ApplicationCommandOptionType.User, "The user that will receive the resource.", true);
            GiveResource.AddOption("resourcename", ApplicationCommandOptionType.String, "The name of the resource to give", true);
            GiveResource.AddOption("amount", ApplicationCommandOptionType.Integer, "The amount of it to give.", true);

            ResourceSubCommandGroup.AddOption(GiveResource);
            /*group*/
            var CurrenciesSubCommandGroup = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommandGroup).WithName("currencies").WithDescription("currencies involved commands");

            var GiveMoney = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("givemoney").WithDescription("Gives money to an user.");
            GiveMoney.AddOption("user", ApplicationCommandOptionType.User,"The user that will receive the money.", true);
            GiveMoney.AddOption("amount", ApplicationCommandOptionType.Integer, "The amount of money the user will receive.", true);

            CurrenciesSubCommandGroup.AddOption(GiveMoney);

            var GiveReputation = new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.SubCommand).WithName("givereputation").WithDescription("Gives reputation to an user.");
            GiveReputation.AddOption("user", ApplicationCommandOptionType.User, "The user that will receive the reputation.", true);
            GiveReputation.AddOption("amount", ApplicationCommandOptionType.Integer, "The amount of reputation the user will receive.", true);

            CurrenciesSubCommandGroup.AddOption(GiveReputation);
            /*end*/
            var builder = StartBuilder("Debug", "Development tools");
            builder.AddOption(ResourceSubCommandGroup);
            builder.AddOption(CurrenciesSubCommandGroup);

        }

        public override async Task ExecuteSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Options.First().Name)
            {
                case "resources": await ResourcesGroup(command); break;
                case "currencies": await CurrenciesGroup(command); break;
            }
        }

        public async Task CurrenciesGroup(SocketSlashCommand command)
        {
            switch (command.Data.Options.First().Options.First().Name)
            {
                case "givemoney": await GiveCurrency(command,true); break;
                case "givereputation": await GiveCurrency(command,false); break;
                
            }
        }

        public async Task GiveCurrency(SocketSlashCommand command, bool IsMoney)
        {
            CommandOption options = command.Data.Options.First().Options.First();
            var user = options["user"] as SocketGuildUser;
            var amount = (long)options["amount"];
            using (var db = new UserContext())
            {
                var dbUser = await db.GrabUserAsync(user.Id);
                if (IsMoney) dbUser.Money += amount;
                else dbUser.Reputation += amount;
                await db.SaveChangesAsync();
                await command.RespondAsync($"User {user.FinalNick()} won {amount} {(IsMoney ? "coins" : "reputation points")} and now has {(IsMoney ? dbUser.Money : dbUser.Reputation)}.");
                
            }
        }


        public async Task ResourcesGroup(SocketSlashCommand command)
        {
            switch (command.Data.Options.First().Options.First().Name)
            {
                case "create": await CreateResource(command); break;
                case "logall": await LogAllResources(command); break;
                case "delete": await DeleteResource(command); break;
                case "give": await GiveResource(command);break;
            }
        }
        public async Task GiveResource(SocketSlashCommand command)
        {
            CommandOption options = command.Data.Options.First().Options.First();

            string resourcename = options["resourcename"] as string;
            SocketGuildUser user = options["user"] as SocketGuildUser;
            long amount = (long)options["amount"];
            
            using (var db = new UserContext())
            {
                var resource = db.Resources.Where(a => a.Name == resourcename).FirstOrDefault();
                
                if (resource != null)
                {
                    var dbUser = await db.GrabUserAsync(user.Id);

                    if(dbUser.UserResources == null)
                    {
                        dbUser.UserResources = new List<UserResource>();
                        UserResource ress = new UserResource() { Resource = resource, Amount = amount };
                        dbUser.UserResources.Add(ress);
                    }
                    else
                    {
                        var ress = dbUser.UserResources.FirstOrDefault(a => a.Resource.Name == resourcename);
                        if (ress!=null)
                        {
                            ress.Amount += amount;
                        }
                        else
                        {
                            ress = new UserResource() { Resource = resource, Amount = amount };
                            dbUser.UserResources.Add(ress);
                        }
                    }
                        

                    
                    await db.SaveChangesAsync();
                    await command.RespondAsync($"{resourcename} given to {user.FinalNick()}");
                }
                else await command.RespondAsync($"Couldn't find {resourcename} in the database");

            }
        }
        public async Task DeleteResource(SocketSlashCommand command)
        {
            var options = command.Data.Options.First().Options.First().Options;

            string name = options.First(a => a.Name == "name").Value as string;
            using (var db = new UserContext())
            {
                var resource = db.Resources.Where(a => a.Name == name).FirstOrDefault();
                if (resource!=null)
                {
                    db.Resources.Remove(resource);
                    await command.RespondAsync($"{name} got removed from the database.");
                    await db.SaveChangesAsync();
                }
                else await command.RespondAsync($"Couldn't find {name} in the database");
                
            }
        }

        public async Task LogAllResources(SocketSlashCommand command)
        {
            using (var db = new UserContext())
            {
                var resources = string.Join("\n", db.Resources.Select(a => $"{a.Name} (lvl:{a.Level}) [{a.Category.CategoryName}] value: {a.TradeValue}"));
                await command.RespondAsync(resources);
            }
        }

        public async Task CreateResource(SocketSlashCommand command)
        {
            var options = command.Data.Options.First().Options.First().Options;

            string name = options.First(a=> a.Name == "name").Value as string;
            string description = options.First(a=> a.Name == "description").Value as string;
            string category = options.First(a => a.Name == "category").Value as string;
            int? level = options.First(a => a.Name == "level").Value as int?;
            int? tradevalue = options.First(a => a.Name == "tradevalue").Value as int?;

            using (var db = new UserContext())
            {
                
                ResourceCategory foundCategory = db.ResourceCategories.Where(a => a.CategoryName == category.ToLower()).FirstOrDefault();
                if (foundCategory == null)
                {
                    foundCategory = new ResourceCategory() { CategoryName = category.ToLower() };
                    db.ResourceCategories.Add(foundCategory);
                }
                

                
                Resource resource = db.Resources.Where(a => a.Name == name).FirstOrDefault();
                if (resource != null)
                {
                    await command.RespondAsync($"A resource with that name already exists ({name})\n" +
                        $"Modifying existing resource...");
                    
                    resource.Description = description;
                    resource.Category = foundCategory;
                    resource.Level = level.Value;
                    resource.TradeValue = tradevalue.Value;
                    await db.SaveChangesAsync();
                }
                else
                {
                    resource = new Resource() { Name = name, Description = description, Level = level != null ? level.Value : 0, TradeValue = tradevalue != null ? tradevalue.Value : 0, Category = foundCategory };
                    db.Resources.Add(resource);
                    await command.RespondAsync($"New resource created and added to database: {name}");
                    await db.SaveChangesAsync();
                }
                

                
                

            }
            
        }


        public override async Task Help(SocketSlashCommand command)
        {
            await command.RespondAsync("The Help command. \n" +
                "Usage: /help getstarted\n" +
                "Or: /help command ping");
        }
    }
}
