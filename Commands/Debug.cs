using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ark_Bot.Commands
{
    public class Debug : ModuleBase<SocketCommandContext>
    {
        private CommandService commandService;

        public Debug(CommandService commandService)
        {
            this.commandService = commandService;
        }

        //[Command("")]
        //[Summary("")]
        //public async Task Method()
        //{

        //}

        [Command("help")]
        [Summary("/help - Displays all commands.")]
        public async Task Help()
        {
            List<CommandInfo> commands = commandService.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands)
            {
                string[] args = command.Summary.Split('-');
                embedBuilder.AddField(args[0].Trim(), args[1].Trim());
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}
