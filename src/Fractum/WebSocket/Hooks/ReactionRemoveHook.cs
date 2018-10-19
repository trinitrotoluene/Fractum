﻿using System.Threading.Tasks;
using Fractum.Contracts;
using Fractum.Entities;
using Fractum.WebSocket.Core;
using Newtonsoft.Json.Linq;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class ReactionRemoveHook : IEventHook<JToken>
    {
        public Task RunAsync(JToken args, FractumCache cache, ISession session, FractumSocketClient client)
        {
            var reaction = args.ToObject<Reaction>();

            client.InvokeReactionRemoved(reaction);

            return Task.CompletedTask;
        }
    }
}