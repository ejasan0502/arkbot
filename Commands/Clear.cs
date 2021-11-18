using Ark_Bot.Handlers;
using Discord.Commands;
using System.Threading.Tasks;

namespace Ark_Bot.Commands
{
    [Group("clear")]
    public class Clear : ModuleBase<SocketCommandContext>
    {
        private RemindHandler remindHandler;

        public Clear(RemindHandler remindHandler)
        {
            this.remindHandler = remindHandler;
        }

        [Command("reminder")]
        [Summary("/clear reminder (Reminder Name) - Clears a reminder based on note")]
        public async Task ClearReminderAsync([Remainder]string name)
        {
            await ReplyAsync(remindHandler.RemoveReminder(name));
        }
        [Command("reminders")]
        [Summary("/clear reminders - Clear all reminders")]
        public async Task ClearRemindersAsync()
        {
            remindHandler.Clear();
            await ReplyAsync("Cleared all reminders!");
        }
    }
}
