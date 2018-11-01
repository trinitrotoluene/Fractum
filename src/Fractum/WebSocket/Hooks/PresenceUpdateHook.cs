﻿using System.Threading.Tasks;
using Fractum.Entities;
using Fractum.Entities.WebSocket;
using Fractum.WebSocket.EventModels;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class PresenceUpdateHook : IEventHook<EventModelBase>
    {
        public Task RunAsync(EventModelBase args, FractumCache cache, GatewaySession session)
        {
            var eventModel = (PresenceUpdateEventModel) args;

            var newPresence = new CachedPresence(eventModel.User.Id, eventModel.Activity, eventModel.NewStatus);

            cache.AddOrReplace(newPresence);

            cache.Client.InvokeLog(new LogMessage(nameof(PresenceUpdateHook), "Presence Update", LogSeverity.Debug));

            return Task.CompletedTask;
        }
    }
}