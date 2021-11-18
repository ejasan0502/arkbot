using Discord.Commands;
using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace Ark_Bot.Commands
{
    public class Raising : ModuleBase<SocketCommandContext>
    {
        private const string wiki = "https://ark.fandom.com/wiki";

        [Command("timeleft")]
        [Summary("/timeleft (Raising Multiplier) (Maturation Percentage) (Dino Name) - Shows time left for raising a dino")]
        public async Task TimeLeft(int multiplier, double maturation, [Remainder]string dino)
        {
            string dinoName = char.ToUpper(dino[0]) + dino.Substring(1).ToLower();

            string url = $"{wiki}/{dinoName}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            string totalMaturation = "";
            string reply = "";

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='info-arkitex info-unit-row']");
            foreach (var node in nodes)
            {
                if (node.InnerText.Contains("Total Maturation Time"))
                {
                    var name = node.SelectSingleNode("div[@class='info-arkitex-left info-X2-40']");
                    var val = node.SelectSingleNode("div[@class='info-arkitex-right info-X2-60']");

                    if (name != null && val != null)
                    {
                        totalMaturation = val.InnerText.Trim();
                    }
                }
            }

            try
            {
                int day = 0, hour = 0, minute = 0, second = 0;
                string[] args = totalMaturation.Split(' ');
                foreach (string arg in args)
                {
                    if (arg.EndsWith("d"))
                        day = int.Parse(arg.Substring(0, arg.Length - 1));
                    if (arg.EndsWith("h"))
                        hour = int.Parse(arg.Substring(0, arg.Length - 1));
                    if (arg.EndsWith("m"))
                        minute = int.Parse(arg.Substring(0, arg.Length - 1));
                    if (arg.EndsWith("s"))
                        second = (int)Math.Round(float.Parse(arg.Substring(0, arg.Length - 1)));
                }

                TimeSpan totalMaturationTIme = new TimeSpan(day, hour, minute, second);

                double totalSeconds = totalMaturationTIme.TotalSeconds;
                double percentLeft = (100.000 - maturation)/100.000;
                double seconds = totalSeconds * (percentLeft/multiplier);

                TimeSpan timeLeft = TimeSpan.FromSeconds(seconds);
                if (timeLeft.Days > 0) reply += $"{timeLeft.Days}d ";
                if (timeLeft.Hours > 0) reply += $"{timeLeft.Hours}h ";
                if (timeLeft.Minutes > 0) reply += $"{timeLeft.Minutes}m ";
                reply += $"{timeLeft.Seconds}s";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
            await ReplyAsync(reply);

        }
    }
}
