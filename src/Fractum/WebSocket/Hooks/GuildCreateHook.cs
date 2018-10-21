﻿using System.Threading.Tasks;
using Fractum.Contracts;
using Fractum.Entities;
using Fractum.WebSocket.Core;
using Fractum.WebSocket.EventModels;
using Newtonsoft.Json.Linq;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class GuildCreateHook : IEventHook<EventModelBase>
    {
        public async Task RunAsync(EventModelBase args, FractumCache cache, ISession session, FractumSocketClient client)
        {
            var guild = args.Cast<GuildCreateEventModel>();

            guild.ApplyToCache(cache);

            if (client.RestClient.Config.AlwaysDownloadMembers && guild.MemberCount > client.RestClient.Config.LargeThreshold)
                await client.RequestMembersAsync(guild.Id);

            client.InvokeLog(
                new LogMessage(nameof(GuildCreateHook), $"Guild Available: {guild.Name}", LogSeverity.Info));

            client.InvokeGuildCreated(cache[guild.Id].Guild);
        }
    }
}