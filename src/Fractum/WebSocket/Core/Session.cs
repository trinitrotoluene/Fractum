﻿using System;
using Fractum.Contracts;

namespace Fractum.WebSocket.Core
{
    public sealed class Session : ISession
    {
        public DateTimeOffset Duration { get; set; }

        public string SessionId { get; set; }

        public string GatewayUrl { get; set; }

        public int Seq { get; set; }

        public int ReconnectionAttempts { get; set; }

        public bool Resuming { get; set; }

        public bool Invalidated { get; set; }

        public bool WaitingForACK { get; set; }

        public void Invalidate()
        {
            SessionId = default;
            Seq = default;
            ReconnectionAttempts = default;
            Resuming = default;
            Invalidated = default;
            WaitingForACK = default;
        }
    }
}