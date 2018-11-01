﻿using Newtonsoft.Json;

namespace Fractum.Entities
{
    public class MessageEditProperties
    {
        internal MessageEditProperties()
        {
        }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("embed")]
        public Embed Embed { get; set; }
    }
}