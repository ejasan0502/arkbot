using Ark_Bot.Handlers;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ark_Bot.Commands
{
    [Group("reminder")]
    public class Reminder : ModuleBase<SocketCommandContext>
    {
        private readonly RemindHandler remindHandler;

        public Reminder(RemindHandler remindHandler)
        {
            this.remindHandler = remindHandler;
        }

        private void ParseTime(string time, out int day, out int hour, out int minute)
        {
            string[] args = time.Split(':');
            if (args.Length == 3)
            {
                day = int.Parse(args[0]);
                hour = int.Parse(args[1]);
                minute = int.Parse(args[2]);
            }
            else if (args.Length == 2)
            {
                day = 0;
                hour = int.Parse(args[0]);
                minute = int.Parse(args[1]);
            }
            else if (args.Length == 1)
            {
                day = 0;
                hour = 0;
                minute = int.Parse(args[0]);
            }
            else
            {
                day = 0;
                hour = 0;
                minute = 0;
            }
        }
        private async Task ShowReminders(string user)
        {
            string reply = "Here are your reminders:";
            List<RemindHandler.Reminder> reminders = remindHandler.GetReminders(user);

            DateTime today = DateTime.Now;

            if (reminders.Count > 0)
            {
                for (int i = 0; i < reminders.Count; i++)
                {
                    int index = i;

                    DateTime time = reminders[index].time;
                    string timeString = time.ToString("dddd, dd MMMM yyyy") + "";
                    if (time.Month == today.Month)
                    {
                        if (time.Day == today.Day)
                            timeString = time.ToString("hh:mm tt");
                        else
                            timeString += $" at {time.ToString("hh:mm tt")}.";
                    }

                    reply += $"\n{reminders[index].name} at {timeString}";
                }
            }
            else
            {
                reply = "No reminders";
            }

            await ReplyAsync(reply);
        }

        [Command("add")]
        [Summary("/reminder add (dd:hh:mm) (Reminder Name) - Adds reminder for a set amount of time.")]
        public async Task ReminderAsync(string time, [Remainder]string note)
        {
            try
            {
                int day, hour, minute;
                ParseTime(time, out day, out hour, out minute);
                string user = Context.Message.Author.Mention;
                await ReplyAsync(remindHandler.AddReminder(note, user, new TimeSpan(day, hour, minute, 0), 0));
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("show")]
        [Summary("/reminder show - Show all reminders for the user.")]
        public async Task ShowRemindersAsync()
        {
            await ShowReminders(Context.User.Mention);
        }
        [Command("showall")]
        [Summary("/reminder showall - Show all reminders.")]
        public async Task ShowRemindersAllAsync()
        {
            await ShowReminders("");
        }

        [Command("set")]
        [Summary("/reminder set (MM/DD/YYYY) (hh:mm) (AM/PM) (Reminder Name) - Set a reminder at a specific time.")]
        public async Task ReminderSetAsync(string date, string time, string meridiem, [Remainder]string note)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date + " " + time + " " + meridiem);
                string user = Context.Message.Author.Mention;

                await ReplyAsync(remindHandler.AddReminder(note, user, dateTime, 0));
            } catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
        [Command("repeat")]
        [Summary("/reminder repeat (Number of days between) (MM/DD/YYYY) (hh:mm) (AM/PM) (Reminder Name) - Create a repeating reminder at specific time.")]
        public async Task ReminderRepeatAsync(int repeat, string date, string time, string meridiem, [Remainder]string note)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date + " " + time + " " + meridiem);
                string user = Context.Message.Author.Mention;

                await ReplyAsync(remindHandler.AddReminder(note, user, dateTime, repeat));
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("update")]
        [Summary("/reminder update (dd:hh:mm) (Reminder Name) - Updates a reminder after a specified amount of time.")]
        public async Task UpdateReminderAsync(string time, [Remainder]string note)
        {
            try
            {
                int day, hour, minute;
                ParseTime(time, out day, out hour, out minute);
                await ReplyAsync(remindHandler.UpdateReminder(note, new TimeSpan(day, hour, minute, 0)));
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
        [Command("updateall")]
        [Summary("/reminder updateall (dd:hh:mm) - Updates all reminders after a specified amount of time.")]
        public async Task UpdateAllRemindersAsync(string time)
        {
            try
            {
                int day, hour, minute;
                ParseTime(time, out day, out hour, out minute);
                await ReplyAsync(remindHandler.UpdateReminders(new TimeSpan(day, hour, minute, 0)));
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
        [Command("updateset")]
        [Summary("/reminder updateset (MM/DD/YYYY) (hh:mm) (AM/PM) (Reminder Name) - Set a reminder at a specific time.")]
        public async Task UpdateReminderSetAsync(string date, string time, string meridiem, [Remainder] string note)
        {
            try
            {
                DateTime dateTime = DateTime.Parse(date + " " + time + " " + meridiem);
                await ReplyAsync(remindHandler.UpdateReminder(note, dateTime));
            }
            catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }
        [Command("updatename")]
        [Summary("/reminder updatename (Old Reminder Name) to (New Reminder Name) - Change the name of a reminder.")]
        public async Task UpdateNameAsync([Remainder]string text)
        {
            string[] args = text.Split(new string[] { "to" }, StringSplitOptions.None);
            await ReplyAsync(remindHandler.UpdateReminder(args[0].Trim(), args[1].Trim()));
        }
    }
}
