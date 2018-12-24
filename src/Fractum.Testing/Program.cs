﻿using Fractum;
using Fractum.WebSocket;
using Fractum.WebSocket;
using Fractum.WebSocket.EventModels;
using Qmmands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Fractum.Testing
{
    public sealed class Program
    {
        private FractumSocketClient _client;

        private CommandService _commands;

        static Task Main(string[] args)
            => new Program().RunAsync();

        public async Task RunAsync()
        {
            _client = new FractumSocketClient(new FractumConfig()
            {
                Token = Environment.GetEnvironmentVariable("fractum_token"),
                LargeThreshold = 250,
                MessageCacheLength = 100,
                AlwaysDownloadMembers = false
            });

            _commands = new CommandService(new CommandServiceConfiguration()
            {
                CaseSensitive = true,
                DefaultRunMode = RunMode.Sequential
            });

            _client.MessageCreated += HandleMessageCreated;

            _client.Ready += () => _client.UpdatePresenceAsync("Benchmarking uptime!", ActivityType.Playing, Status.Online);

            _client.RegisterDefaultLogHandler(LogSeverity.Verbose);

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());

            await _client.PrepareForSessionAsync();

            _client.GetEventStage().RegisterCallback(Dispatch.GUILD_MEMBERS_CHUNK, (model, cache, session) =>
            {
                Console.WriteLine(session.SessionId);
                return Task.CompletedTask;
            });

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleMessageCreated(CachedMessage message)
        {
            if (!message.IsUserMessage || message.Author.IsBot)
                return;

            if (CommandUtilities.HasPrefix(message.Content, '>', false, out var commandString))
            {
                var result = await _commands.ExecuteAsync(commandString, new CommandContext(_client, message));
            }
        }
    }
}
