using Ark_Bot.Handlers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ark_Bot
{
    class Program
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        private CommandHandler commandHandler;
        private RemindHandler remindHandler;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .AddSingleton<CommandHandler>()
                .AddSingleton<RemindHandler>()
                .AddSingleton<CloningHandler>()
                .BuildServiceProvider();
            commandHandler = services.GetService<CommandHandler>();
            remindHandler = services.GetService<RemindHandler>();

            client.Log += Log;
            await commandHandler.InstallCommandsAsync();

            var token = File.ReadAllText("C:/Users/ejasa/Desktop/Current Projects/Ark Bot/token.txt");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            while (true)
            {
                await UpdateAsync();
            }
        }

        private async Task UpdateAsync()
        {
            await Task.Delay(1000);
            await remindHandler.OnUpdate(client);
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
