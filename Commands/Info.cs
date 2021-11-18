using Ark_Bot.Data;
using Ark_Bot.Handlers;
using Discord.Commands;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Ark_Bot.Commands
{
    [Group("info")]
    public class Info : ModuleBase<SocketCommandContext>
    {
        private const string wiki = "https://ark.fandom.com/wiki";

        private CloningHandler cloningHandler;
        private List<string> allowedInfo = new List<string>()
        {
            "Diet",
            "Gestation Time",
            "Baby Time",
            "Juvenile Time",
            "Adolescent Time",
            "Total Maturation Time",
            "Incubation Range"
        };

        public Info(CloningHandler cloningHandler)
        {
            this.cloningHandler = cloningHandler;
        }

        private bool IsAllowedInfo(string text)
        {
            foreach (string allow in allowedInfo)
                if (text.Contains(allow))
                    return true;
            return false;
        }

        //[Command("")]
        //[Summary("")]
        //public async Task Method()
        //{

        //}

        [Command]
        [Summary("/info (Dino Name) - Displays important breedding information for a dino.")]
        public async Task BreedingInfo(string dino)
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
                if (node.InnerText.Contains(dinoName) ||
                    IsAllowedInfo(node.InnerText))
                {
                    var name = node.SelectSingleNode("div[@class='info-arkitex-left info-X2-40']");
                    var val = node.SelectSingleNode("div[@class='info-arkitex-right info-X2-60']");

                    if (name != null && val != null)
                    {
                        reply += $"\n\n{name.InnerText.Trim()}\n     {val.InnerText.Trim()}";

                        if (name.InnerText.Trim() == "Total Maturation Time")
                            totalMaturation = val.InnerText.Trim();
                        else if (name.InnerText.Trim() == "Diet")
                        {
                            switch (val.InnerText.Trim())
                            {
                                default:                reply += " (Unknown)"; break;
                                case "Herbivore":       reply += " (Berries)"; break;
                                case "Omnivore":        reply += " (Raw Meat and Berries)"; break;
                                case "Carnivore":       reply += " (Raw Meat)"; break;
                                case "Piscivore":       reply += " (Fish Meat)"; break;
                                case "Minerals":        reply += " (Stone, Clay, Sulfer)"; break;
                                case "Carrion-Feeder":  reply += " (Spoiled Meat)"; break;
                                case "Sanguinivore":    reply += " (Blood)"; break;
                                case "Coprophagic":     reply += " (Feces)"; break;
                                case "Flame Eater":     reply += " (Sulfur)"; break;
                                case "Sweet Tooth":     reply += " (Giant Bee Honey)";  break;
                            }
                        }
                    } else
                    {
                        reply += $"\n";
                        foreach (var n in node.ChildNodes)
                        {
                            if (!string.IsNullOrWhiteSpace(n.InnerText))
                            {
                                reply += $"{n.InnerText.Trim()}";
                            }
                        }
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
                double imprint = (8.00 / totalMaturationTIme.TotalHours) * 100;
                reply += $"\n\nImprinting:\n     {Math.Round(imprint)}% per interaction";
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

            await ReplyAsync(reply);
        }

        [Command("cloning")]
        [Summary("/info cloning (Dino Level) (Dino Name) - Displays cloning cost and time for given dino and level.")]
        public async Task CloningInfo(int level, [Remainder]string dino)
        {
            try
            {
                string dinoName = char.ToUpper(dino[0]) + dino.Substring(1).ToLower();
                List<double> vals = cloningHandler.GetValues(dinoName);

                double baseCost = vals[0];
                double costPerLevel = vals[1];
                double costForLevel = costPerLevel * level;

                double cloneCost = costForLevel + baseCost;
                double cloneTime = (vals[2] + vals[3] * level) / Settings.babyMatureSpeedMultiplier;

                TimeSpan time = TimeSpan.FromSeconds(cloneTime);
                await ReplyAsync($"Lv. {level} {dinoName}\nCloning Cost: {cloneCost} shards ({Math.Round(cloneCost/100)} element)\nCloning Time: {time}");
            } catch (Exception e)
            {
                await ReplyAsync(e.ToString());
            }
        }

        [Command("crafting")]
        [Summary("/info crafting (Amount) (Item Name) - Displays crafting costs for a given item.")]
        public async Task CraftingInfo(int amount, [Remainder] string name)
        {
            // Ensure item name is properly formated
            string itemName;
            string[] args = name.Split(' ');
            if (args.Length > 0)
            {
                itemName = char.ToUpper(args[0][0]) + args[0].ToLower().Substring(1);
                for (int i = 1; i < args.Length; i++)
                {
                    itemName += "_" + char.ToUpper(args[i][0]) + args[i].ToLower().Substring(1);
                }
            }
            else
                itemName = char.ToUpper(name[0]) + name.ToLower().Substring(1);

            string url = $"{wiki}/{itemName}";
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);

            string reply = $"{itemName} Ingredients";
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='info-arkitex info-unit']");
            foreach (var node in nodes)
            {
                if (node.InnerText.Contains("Ingredients"))
                {
                    var ingredients = node.SelectNodes("//div[@style='padding-left:5px']");
                    foreach (var ingredient in ingredients)
                    {
                        string[] args1 = ingredient.InnerText.Split(' ');

                        int count = int.Parse(args1[0]);
                        reply += $"\n{count * amount} {string.Join(" ", args1.Skip(1))}";
                    }
                }
            }

            await ReplyAsync(reply);
        }
    }
}
