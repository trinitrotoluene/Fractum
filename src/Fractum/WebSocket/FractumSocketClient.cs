﻿using System;
using System.Threading.Tasks;
using Fractum.Entities;
using Fractum.Entities.Extensions;
using Fractum.Rest;
using Fractum.WebSocket.Entities;
using Fractum.WebSocket.Hooks;
using Fractum.WebSocket.Pipelines;

namespace Fractum.WebSocket
{
    public sealed class FractumSocketClient
    {
        private IPipeline<Payload> _pipeline;
        internal FractumCache Cache;
        internal FractumSocketConfig Config;
        internal FractumRestClient RestClient;
        internal ISession Session;
        internal SocketWrapper Socket;

        public FractumSocketClient(FractumSocketConfig config)
        {
            Config = config;
            Cache = new FractumCache(this);
            Session = new Session();
            RestClient = new FractumRestClient(Config);
        }

        private void UseDefaultPipeline()
        {
            var connectionStage = new ConnectionStage(this);

            var eventStage = new EventStage(this)
                .RegisterHook("READY", new ReadyHook())

                .RegisterHook("PRESENCE_UPDATE", new PresenceUpdateHook())

                .RegisterHook("GUILD_CREATE", new GuildCreateHook())
                .RegisterHook("GUILD_UPDATE", new GuildUpdateHook())
                .RegisterHook("GUILD_MEMBER_UPDATE", new PresenceUpdateHook())
                .RegisterHook("GUILD_MEMBERS_CHUNK", new GuildMembersChunkHook())

                .RegisterHook("CHANNEL_CREATE", new ChannelCreateHook())
                .RegisterHook("CHANNEL_UPDATE", new ChannelUpdateHook())
                .RegisterHook("CHANNEL_DELETE", new ChannelDeleteHook())
                .RegisterHook("CHANNEL_PINS_UPDATE", new ChannelPinsUpdateHook())

                .RegisterHook("MESSAGE_CREATE", new MessageReceivedHook())
                .RegisterHook("MESSAGE_CREATE", new TempCommandsHook());

            _pipeline = new PayloadPipeline()
                .AddStage(connectionStage)
                .AddStage(eventStage);
        }

        public async Task InitialiseAsync()
        {
            var gatewayInfo = await RestClient.GetSocketUrlAsync();
            if (gatewayInfo.SessionStartLimit["remaining"] <= 0)
                throw new InvalidOperationException("No new sessions can be started.");

            Socket = new SocketWrapper(new Uri(gatewayInfo.Url + Consts.GATEWAY_PARAMS));

            if (_pipeline is null)
                UseDefaultPipeline();

            Socket.PayloadReceived += async payload =>
            {
                var logMessage = await _pipeline.CompleteAsync(payload);
                if (logMessage != null)
                    InvokeLog(logMessage);
            };
        }

        public Task<GatewayBotResponse> GetSocketUrlAsync()
            => RestClient.GetSocketUrlAsync();

        public Task UpdatePresenceAsync(string name, ActivityType type = ActivityType.Playing,
            Status status = Status.Online)
        {
            var payload = new
            {
                op = OpCode.StatusUpdate,
                d = new Presence
                {
                    Activity = new Activity
                    {
                        Name = name,
                        Type = type
                    },
                    Status = status
                }
            }.Serialize();
            return Socket.SendMessageAsync(payload);
        }

        public Task RequestMembersAsync(ulong guildId, string queryString = null, int limit = 0)
        {
            var memberRequest = new SendPayload
            {
                op = OpCode.RequestGuildMembers,
                d = new
                {
                    guild_id = guildId,
                    query = queryString ?? string.Empty,
                    limit
                }
            }.Serialize();

            return Socket.SendMessageAsync(memberRequest);
        }

        public Task ConnectAsync()
            => Socket.ConnectAsync();

        internal void InvokeLog(LogMessage msg)
            => Log?.Invoke(msg);

        internal void InvokeMessagePinned(TextChannel channel)
            => MessagePinned?.Invoke(channel);

        public event Func<LogMessage, Task> Log;
        public event Func<TextChannel, Task> MessagePinned;
    }
}