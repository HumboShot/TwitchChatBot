using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TwitchChatBot.Helpers;
using TwitchChatBot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TwitchChatBot.Commands
{
    public class CustomTextCommands
    {
        private TwitchClient _client;
        private LogHelper _log;

        public List<TextCommand> TextCommands { get; set; }

        public CustomTextCommands(TwitchClient client, Helpers.LogHelper log)
        {
            _client = client;
            _log = log;

            var allCommands = ReadCommandsFromFile();

            if (allCommands != null)
            {
                TextCommands = allCommands;
            }
            else
            {
                // Makes new file ready to save some commands in
                SaveAllCommandsToFile(new List<TextCommand>());
            }
        }

        #region Commands

        public void DoCommand(OnChatCommandReceivedArgs command)
        {
            var chatCommand = TextCommands.FirstOrDefault(x => x.CommandName == command.Command.CommandText.ToLower() & x.ChannelName == command.Command.ChatMessage.Channel);

            // if we don't find a command with that text, then we don't want to send anything
            if (chatCommand != null)
            {
                _client.SendMessage(command.Command.ChatMessage.Channel, chatCommand.CommandText);
            }

        }

        public void MakeNewCommand(string channel, string commandName, string commandText)
        {
            TextCommand existingCommand = null;

            if (TextCommands.FirstOrDefault(x => x.CommandName == commandName) == null)
            {
                existingCommand = TextCommands.FirstOrDefault(x => x.CommandName == commandName && x.ChannelName == channel);
            }

            string fullCommandText = string.Empty;
            //Removes first arg since this is the command name
            if (commandText.Length > commandName.Length + 1)
            {
                int i = commandText.IndexOf(" ") + 1;
                fullCommandText = commandText.Substring(i);
            }

            TextCommand newCommand = new TextCommand(commandName.ToLower(), fullCommandText, channel);

            if (existingCommand != null)
            {
                TextCommands?.Remove(existingCommand);
                TextCommands?.Add(newCommand);
                _log.Write($"{channel}: Command {commandName} already existed, so i overwrote it :)");
            }
            else
            {
                TextCommands?.Add(newCommand);
                _client.SendMessage(channel, $"Added {commandName} as a new command");
            }

            SaveAllCommandsToFile(TextCommands);
        }

        public void ListAllChannelCommands(string channel, char commandIdentifier, string senderUsername, bool hasPower)
        {
            if(hasPower)
            {
                var channelCommandsList = TextCommands.Where(x => x.ChannelName == channel);

                string channelCommands = "Here a list of all custom text commands" + Environment.NewLine;

                foreach (var item in channelCommandsList)
                {
                    channelCommands += $"{commandIdentifier}{item.CommandText} ";
                }

                _client.SendWhisper(senderUsername, channelCommands);
            }
        }

        #endregion

        private void SaveAllCommandsToFile(List<TextCommand> textCommands)
        {
            File.WriteAllText("commands.json", JsonConvert.SerializeObject(textCommands));
        }

        private List<TextCommand> ReadCommandsFromFile()
        {
            if (File.Exists("commands.json"))
                return JsonConvert.DeserializeObject<List<TextCommand>>(File.ReadAllText("commands.json"));

            return null;
        }
    }
}
