using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitchChatBot.Commands;
using TwitchChatBot.Helpers;
using TwitchChatBot.Models;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace TwitchChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            Console.ReadLine();
        }
    }

    public class Bot
    {
        private const string SETTINGS_LOCATION = "settings.json";

        private LiveStreamMonitorService Monitor;
        private TwitchAPI API;
        private TwitchClient Client;
        private LogHelper Log;
        private Settings Settings;

        #region commands
        private CustomTextCommands customTextCommands;
        private HeistCommands heistCommands;
        #endregion
        public Bot()
        {
            Console.WriteLine("Starting Bot");
            Task.Run(() => ConfidLiveMonitorAsync());
        }

        private async void ConfidLiveMonitorAsync()
        {
            API = new TwitchAPI();
            Client = new TwitchClient();
            Log = new LogHelper();

            FindSettings();

            API.Settings.ClientId = Settings.ClientId;
            API.Settings.AccessToken = Settings.AccessToken;

            // Monitor stuff
            Monitor = new LiveStreamMonitorService(API);
            Monitor.SetChannelsByName(Settings.MonitoredChannels);

            Monitor.OnStreamOnline += Monitor_OnStreamOnline;
            Monitor.OnStreamOffline += Monitor_OnStreamOffline;
            Monitor.OnStreamUpdate += Monitor_OnStreamUpdate;
            Monitor.OnServiceStarted += Monitor_OnServiceStarted;
            Monitor.OnChannelsSet += Monitor_OnChannelSet;

            Monitor.Start();

            // Client Stuff
            ConnectionCredentials credentials = new ConnectionCredentials(Settings.BotConnectionDetails.UserName, Settings.BotConnectionDetails.OAuth);

            Client.Initialize(credentials, Settings.MonitoredChannels.FirstOrDefault());
            Client.AddChatCommandIdentifier('!');

            Client.OnLog += Client_OnLog;
            Client.OnJoinedChannel += Client_OnJoinedChannel;
            Client.OnMessageReceived += Client_OnMessageReceived;
            Client.OnConnected += Client_OnConnected;
            Client.OnNewSubscriber += Client_OnNewSubscriber;
            Client.OnReSubscriber += Client_OnResubscriber;
            Client.OnGiftedSubscription += Client_GiftedSubscription;
            Client.OnChatCommandReceived += Client_OnChatCommandReceived;

            Client.Connect();

            Log.SetBotUsername(Client.TwitchUsername);

            SetupCommands();

            await Task.Delay(-1);
        }

        private void FindSettings()
        {
            if (File.Exists(SETTINGS_LOCATION))
            {
                Settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SETTINGS_LOCATION));
            }
            else
            {
                //Closes the program, if not settings were found, also makes you a settings file
                File.WriteAllText(SETTINGS_LOCATION, JsonConvert.SerializeObject(new Settings()));
                Log.Write("There was no settings file found, i made one you can use :)");
                Environment.Exit(0);
            }

        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs command)
        {
            Log.WriteCommand(command);
            switch (command.Command.CommandText)
            {
                case "heist":
                    break;
                case "newCommand":
                    customTextCommands.MakeNewCommand(command.Command.ArgumentsAsList[0], command.Command.ArgumentsAsString);
                    break;
                default:
                    customTextCommands.DoCommand(command);
                    break;
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Log.Write($"{e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Log.Write($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Log.Write($"Message from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {

        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {

        }
        private void Client_OnResubscriber(object sender, OnReSubscriberArgs e)
        {

        }

        private void Client_GiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {

        }

        private void Monitor_OnChannelSet(object sender, OnChannelsSetArgs e)
        {

        }

        private void Monitor_OnServiceStarted(object sender, OnServiceStartedArgs e)
        {

        }

        private void Monitor_OnStreamUpdate(object sender, OnStreamUpdateArgs e)
        {
            Log.Write($"Stream updated to {e.Stream.Title}");
        }

        private void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
        {
            Log.Write($"{e.Channel} stream went offline. The stream was live for {DateHelper.TimeDiffFormatted(e.Stream.StartedAt, DateTime.Now)}");
        }

        private void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            Log.Write($"{e.Channel} is now live with the title {e.Stream.Title}");
        }

        private void SetupCommands()
        {
            customTextCommands = new CustomTextCommands(Client);
            heistCommands = new HeistCommands();
        }
    }
}
