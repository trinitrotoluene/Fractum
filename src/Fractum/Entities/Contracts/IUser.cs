﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Fractum.Entities.Contracts
{
    public interface IUser : IDiscordEntity
    {
        string Username { get; }

        short Discrim { get; }

        bool IsBot { get; }

        string GetAvatarUrl();
    }
}