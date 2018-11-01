﻿using System.Linq;
using System.Threading.Tasks;
using Fractum.Entities;
using Fractum.Entities.WebSocket;
using Fractum.WebSocket.EventModels;

namespace Fractum.WebSocket.Hooks
{
    internal sealed class MessageDeleteHook : IEventHook<EventModelBase>
    {
        public Task RunAsync(EventModelBase args, FractumCache cache, GatewaySession session)
        {
            var eventModel = (MessageDeleteEventModel) args;

            if (cache.TryGetGuild(eventModel.ChannelId, out var guild, SearchType.Channel)
            && guild.TryGet(eventModel.ChannelId, out CircularBuffer<CachedMessage> messages))
            {
                var message = messages.FirstOrDefault(x => x.Id == eventModel.Id);

                if (message != null)
                {
                    cache.Client.InvokeMessageDeleted(new Cacheable<CachedMessage>(message));
                    messages.Remove(message);
                }
            }
            else if (cache.DmChannels.FirstOrDefault(x => x.Id == eventModel.ChannelId) is CachedDMChannel dmChannel 
                && dmChannel.MessageBuffer is CircularBuffer<CachedMessage> dmMessages)
            {
                var message = dmMessages.FirstOrDefault(x => x.Id == eventModel.Id);

                if (message != null)
                {
                    cache.Client.InvokeMessageDeleted(new Cacheable<CachedMessage>(message));
                    dmMessages.Remove(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}