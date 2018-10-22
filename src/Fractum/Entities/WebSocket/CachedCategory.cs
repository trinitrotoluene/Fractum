﻿using Fractum.WebSocket.Core;
using Fractum.WebSocket.EventModels;

namespace Fractum.Entities.WebSocket
{
    public sealed class CachedCategory : CachedGuildChannel
    {
        internal CachedCategory(FractumCache cache, ChannelCreateUpdateOrDeleteEventModel model, ulong? guildId = null)
            : base(cache, model, guildId)
        {
        }
    }
}