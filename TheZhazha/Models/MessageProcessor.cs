using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SKYPE4COMLib;
using TheZhazha.Data;
using TheZhazha.Events;
using TheZhazha.Utils;

namespace TheZhazha.Models
{
    public class MessageProcessor : IMessageProcessor
    {
        #region Fields

        private readonly Queue<ChatMessage> _messages;
        private readonly CancellationTokenSource _tokenSource;
        private readonly List<QuoteGenerator> _quoteGenerators;
        private readonly TextGenerator _textGenerator;

        #endregion

        #region Events

        #region MoodChanged

        public event MoodChangedEventHandler MoodChanged;

        public void OnMoodChanged(MoodChangedEventArgs e)
        {
            var handler = MoodChanged;
            if (handler != null) handler(this, e);
        }

        #endregion

        #endregion

        #region Constructors

        public MessageProcessor()
        {
            _quoteGenerators =
                QuotesLoader.GetGeneratorsList(
                    System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            _textGenerator = new TextGenerator();

            _messages = new Queue<ChatMessage>();
            _tokenSource = new CancellationTokenSource();

            var ct = _tokenSource.Token;
            Task.Factory.StartNew(
                () =>
                    {
                        while (true)
                        {
                            Thread.Sleep(500);
                            if (ct.IsCancellationRequested)
                                break;
                            if (_messages.Count == 0)
                                continue;
                            Process(_messages.Dequeue());
                        }
                    },
                ct
                );
        }

        #endregion

        #region Public methods



        #endregion

        #region Private methods

        private void Process(ChatMessage message)
        {
            if (DateTime.Now.Subtract(message.Timestamp) > TimeSpan.FromMinutes(5))
                return;

            if(message.Body.StartsWith("@"))
            {
                // command
                ProcessCommand(message);
            }
            else if (message.Body.Equals(Quot.Suffix1) || message.Body.Equals(Quot.Suffix2))
            {
                //Send(message.Chat, Storage.GetTodayQuots(message.ChatName));
                Send(message.Chat, "Temporary disabled :)");
            }
            else if(Quot.IsQuotString(message.Body))
            {
                // quote
                // ProcessQuot(message);
                Send(message.Chat, "Temporary disabled :)");
            }
            else if (Babka.Match(message.Body))
            {
                string response;
                var kick = Babka.ProcessMessage(message, out response);
                if(!string.IsNullOrEmpty(response))
                    Send(message.Chat, response.Replace("{name}", SkypeUtils.GetUserName(message.Sender)));
                if(kick)
                    message.Chat.Kick(message.Sender.Handle);
            }
            else
            {
                // common message
                ProcessMessage(message);
            }
        }

        private void ProcessMessage(ChatMessage message)
        {
            switch (message.Type)
            {
                case TChatMessageType.cmeLeft:
                    if (message.LeaveReason == TChatLeaveReason.leaUnsubscribe
                        && message.Chat.Members.Count > 1) // many members
                    {
                        var users = new UserCollection { message.Sender };
                        message.Chat.AddMembers(users);
                        message.Chat.SendMessage("Стоять крутить фонарики!");
                    }
                    break;
                case TChatMessageType.cmeAddedMembers:
                    foreach (IUser user in message.Users)
                    {
                        if(Storage.IsUserBanned(user.Handle, message.ChatName))
                        {
                            message.Chat.Kick(user.Handle);
                            Send(message.Chat, "Вакуумные головы!");
                        }
                    }
                    break;
                case TChatMessageType.cmeSaid:
                case TChatMessageType.cmeEmoted:
                    Respond(message);
                    break;
            }
        }

        private void ProcessQuot(ChatMessage message)
        {
            var quotContent = Quot.GetQuotContent(message.Body);
            if(!string.IsNullOrEmpty(quotContent))
            {
                var quot = new Quot(quotContent, message.Sender.Handle);
                if(Storage.CanAddQuot(quot, message.ChatName))
                {
                    Storage.AddQuot(quot, message.ChatName);
                    string mood;
                    if (quotContent.Length < 190)
                    {
                        mood = quotContent;
                    }
                    else
                    {
                        mood = quotContent.Substring(0, 190) + "…";
                    }
                    OnMoodChanged(new MoodChangedEventArgs(mood));
                    Send(message.Chat, Storage.GetTodayQuots(message.ChatName));
                }
                else
                {
                    Send(
                        message.Chat,
                        string.Format(
                            "Вы недавно добавляли цитату, {0}, отдохните.", SkypeUtils.GetUserName(message.Sender)));
                }
            }
        }

        private void ProcessCommand(ChatMessage message)
        {
            if (!message.Body.StartsWith("@") || message.Body.Length <= 1)
                return;

            var cmd = message.Body.Substring(1).Trim().ToLowerInvariant();
            switch (cmd)
            {
                case Commands.CmdGameNews:
                    GameNews.Load(response => Send(message.Chat, response));
                    break;
                case Commands.CmdSiske:
                    Siske.Load(response => Send(message.Chat, response));
                    break;
                case Commands.CmdGenerate:
                    if (_textGenerator != null
                        && _textGenerator.IsReady)
                    {
                        Send(message.Chat, _textGenerator.GenerateDeep());
                    }
                    break;
                default:
                    Send(message.Chat, "Неизвестная команда.");
                    break;
            }
        }

        private void Respond(IChatMessage message)
        {
            if (message == null || message.Body.Length == 0)
                return;

            string response = null;
            if(Zhazha.Rnd.NextDouble() < 0.001)
            {
                var name = message.FromDisplayName;
                if(string.IsNullOrEmpty(name))
                {
                    name = message.FromHandle;
                }
                response = string.Format("/topic {0} (c) {1}", message.Body, name);
            }
            else if (Zhazha.Rnd.NextDouble() < 0.04
                && _textGenerator != null
                && _textGenerator.IsReady)
            {
                response = _textGenerator.GenerateDeep();
            }
            else
            {
                foreach (var quoteGenerator in _quoteGenerators)
                {
                    if(quoteGenerator.AlwaysRespond || quoteGenerator.Matches(message.Body))
                    {
                        response = quoteGenerator.GetQuote(message.Sender);
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(response))
                SendDelayed(message.Chat, response);
        }

        private void Send(IChat chat, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            chat.SendMessage(message);
        }

        private void SendDelayed(IChat chat, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            Thread.Sleep(2000 + Zhazha.Rnd.Next(4000));
            Send(chat, message);
        }

        #endregion

        #region Implementation of IMessageProcessor

        public void AddMessage(ChatMessage message)
        {
            _messages.Enqueue(message);
        }

        #endregion
    }
}
