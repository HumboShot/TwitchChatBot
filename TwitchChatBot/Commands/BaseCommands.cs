using System;
using System.Collections.Generic;
using System.Text;
using TwitchChatBot.Helpers;
using TwitchLib.Client;

namespace TwitchChatBot.Commands
{
    public class BaseCommands
    {
        private TwitchClient _client;
        private Dictionary<string, int> _pogCounter;
        private LogHelper _log;

        public BaseCommands(TwitchClient client, LogHelper log)
        {
            _client = client;
            _pogCounter = new Dictionary<string, int>();
            _log = log;
        }

        public void Ping(string channel)
        {
            _client.SendMessage(channel, "I will pong to your ping");
        }

        public void PogCounter(string channel)
        {
            if(_pogCounter.ContainsKey(channel))
            {
                var pogCount = _pogCounter[channel] + 1;
                if(pogCount % 10 == 0)
                {
                    _client.SendMessage(channel, $"There has been {pogCount} PogChamp in this session");
                }
                _pogCounter[channel] = pogCount;
                _log.Write($"{channel}: {pogCount} in this session");
            }
            else
            {
                _pogCounter.Add(channel, 1);
            }
        }
    }
}
