﻿using System.Threading.Tasks;
using Fractum.Entities;
using Fractum.Entities.WebSocket;
using Fractum.WebSocket.EventModels;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class ReactionRemoveHook : IEventHook<EventModelBase>
    {
        public Task RunAsync(EventModelBase args, FractumCache cache, GatewaySession session)
        {
            var eventModel = (ReactionRemoveEventModel) args;

            cache.Client.InvokeReactionRemoved(new CachedReaction(eventModel));

            return Task.CompletedTask;
        }
    }
}