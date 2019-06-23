using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TwitchChatBot.Models;
using TwitchLib.Client;
using TwitchLib.Client.Events;

namespace TwitchChatBot.Commands
{
    public class CustomTextCommands
    {
        private TwitchClient _client;
        public List<TextCommand> TextCommands { get; set; }

        public CustomTextCommands(TwitchClient client)
        {
            _client = client;

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

        internal void DoCommand(OnChatCommandReceivedArgs command)
        {
            var chatCommand = TextCommands.FirstOrDefault(x => x.CommandName == command.Command.CommandText);

            // if we don't find a command with that text, then we don't want to send anything
            if (chatCommand != null)
            {
                _client.SendMessage(command.Command.ChatMessage.Channel, chatCommand.CommandText);
            }

        }

        public void MakeNewCommand(string commandName, string commandText)
        {
            TextCommand existingCommand = null;

            if (TextCommands.FirstOrDefault(x => x.CommandName == commandName) == null)
            {
                existingCommand = TextCommands.FirstOrDefault(x => x.CommandName == commandName);
            }

            string fullCommandText = string.Empty;
            //Removes first arg since this is the command name
            if (commandText.Length > commandName.Length + 1)
            {
                int i = commandText.IndexOf(" ") + 1;
                fullCommandText = commandText.Substring(i);
            }

            TextCommand newCommand = new TextCommand(commandName, fullCommandText);

            if (existingCommand != null)
            {
                TextCommands?.Remove(existingCommand);
            }

            TextCommands?.Add(newCommand);
        }

        private void SaveAllCommandsToFile(List<TextCommand> textCommands)
        {
            File.WriteAllText("commands.json", JsonConvert.SerializeObject(textCommands));

            TextCommands = new List<TextCommand>();
        }

        private List<TextCommand> ReadCommandsFromFile()
        {
            if (File.Exists("commands.json"))
                return JsonConvert.DeserializeObject<List<TextCommand>>(File.ReadAllText("commands.json"));

            return null;
        }
    }
}
