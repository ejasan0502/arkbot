using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ark_Bot.Handlers
{
    public class RemindHandler
    {
        private const ulong channelID = 887439337661276160;
        private string directory;
        private string save;

        private List<Reminder> reminders;

        public RemindHandler()
        {
            reminders = new List<Reminder>();
            directory = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Ark Bot";
            save = directory + "/reminders.txt";

            Load();
        }

        private bool HasReminder(string name)
        {
            foreach (Reminder r in reminders)
            {
                if (r.name == name)
                {
                    return true;
                }
            }
            return false;
        }
        private int GetIndex(string name)
        {
            for (int i = 0; i < reminders.Count; i++)
            {
                if (reminders[i].name == name)
                    return i;
            }
            return -1;
        }

        private void Save()
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string json = JsonConvert.SerializeObject(reminders);
            File.WriteAllText(save, json);
        }
        private void Load()
        {
            string json = File.ReadAllText(save);
            reminders = JsonConvert.DeserializeObject<List<Reminder>>(json);
        }

        public string AddReminder(string name, string user, TimeSpan time, int repeat)
        {
            if (!HasReminder(name))
            {
                reminders.Add(new Reminder()
                {
                    name = name,
                    user = user,
                    time = DateTime.Now + time,
                    repeat = repeat
                });
                Save();

                return $"Reminder has been set!";
            }

            return $"Reminder already exists!";
        }
        public string AddReminder(string name, string user, DateTime time, int repeat)
        {
            if (!HasReminder(name))
            {
                reminders.Add(new Reminder()
                {
                    name = name,
                    user = user,
                    time = time,
                    repeat = repeat
                });
                Save();

                return $"Reminder has been set!";
            }

            return $"Reminder already exists!";
        }
        public string RemoveReminder(string name)
        {
            if (HasReminder(name))
            {
                for (int i = reminders.Count-1; i >= 0; i--)
                {
                    if (reminders[i].name == name)
                        reminders.RemoveAt(i);
                }
                Save();

                return $"Removed all reminders with given name, '{name}'";
            }
            return $"Unable to find reminder with given name, '{name}'";
        }
        public string UpdateReminder(string name, TimeSpan time)
        {
            if (HasReminder(name))
            {
                int index = GetIndex(name);
                reminders[index].time = DateTime.Now + time;

                return "Reminder has been updated!";
            }
            return "Unable to find reminder!";
        }
        public string UpdateReminder(string name, DateTime time)
        {
            if (HasReminder(name))
            {
                int index = GetIndex(name);
                reminders[index].time = time;

                return "Reminder has been updated!";
            }
            return "Unable to find reminder!";
        }
        public string UpdateReminder(string name, string newName)
        {
            if (HasReminder(name))
            {
                int index = GetIndex(name);
                reminders[index].name = newName;
                return "Reminder has been updated!";
            }
            return "Unable to find reminder!";
        }
        public string UpdateReminders(TimeSpan time)
        {
            foreach (Reminder reminder in reminders)
            {
                reminder.time += time;
            }
            return $"Added {time} to all reminders!";
        }
        public List<Reminder> GetReminders(string user = "")
        {
            if (user != "")
                return reminders.Where(r => r.user == user).OrderBy(x => x.time).ToList();
            else
                return reminders.OrderBy(x => x.time).ToList();
        }
        public void Clear()
        {
            reminders = new List<Reminder>();
            Save();
        }

        public async Task OnUpdate(DiscordSocketClient client)
        {
            IMessageChannel channel = client.GetChannel(channelID) as IMessageChannel;

            for (int i = reminders.Count-1; i >= 0; i--)
            {
                if ((reminders[i].time - DateTime.Now).TotalSeconds <= 0)
                {
                    await channel.SendMessageAsync($"{reminders[i].user}\n[Reminder] {reminders[i].name}");

                    if (reminders[i].repeat < 1)
                    {
                        reminders.RemoveAt(i);
                    } else
                    {
                        reminders[i].time += new TimeSpan(reminders[i].repeat, 0, 0, 0);
                    }
                    Save();
                }
            }
        }

        [System.Serializable]
        public class Reminder
        {
            public string name;
            public string user;
            public DateTime time;
            public int repeat;
        }
    }
}
