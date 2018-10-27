﻿using System.Threading.Tasks;
using Fractum.Entities;
using Fractum.WebSocket.EventModels;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class RoleCreateHook : IEventHook<EventModelBase>
    {
        public Task RunAsync(EventModelBase args, ISocketCache<ISyncedGuild> cache, ISession session)
        {
            var eventArgs = (RoleCreateEventModel) args;

            if (cache.TryGetGuild(eventArgs.GuildId, out var guild))
            {
                guild.AddOrReplace(eventArgs.Role);

                cache.Client.InvokeRoleCreated(guild.Guild, eventArgs.Role);
            }

            return Task.CompletedTask;
        }
    }
}