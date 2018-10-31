﻿using Fractum.WebSocket;

namespace Fractum.Entities
{
    public abstract class PopulatedEntity : DiscordEntity
    {
        protected PopulatedEntity(FractumCache cache)
        {
            Cache = cache;
        }

        internal FractumCache Cache { get; }

        internal FractumSocketClient Client => Cache.Client;
    }
}