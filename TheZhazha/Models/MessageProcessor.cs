using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SKYPE4COMLib;
using Shock.MarkovTextGenerator;
using TheZhazha.Data;
using TheZhazha.Events;
using TheZhazha.Utils;
using Shock.Logger;

namespace TheZhazha.Models
{
    public class MessageProcessor : IMessageProcessor
    {
        #region Fields

        private readonly Queue<ChatMessage> _messages;
        private readonly CancellationTokenSource _tokenSource;
        private readonly List<QuoteGenerator> _quoteGenerators;

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
                ).LogExceptions();
        }

        #endregion

        #region Public methods



        #endregion

        #region Private methods

        private void Process(ChatMessage message)
        {
            StatsManager.Instance.ProcessMessage(message);

            if (DateTime.Now.Subtract(message.Timestamp) > TimeSpan.FromMinutes(1))
                return;

            if(message.Body.StartsWith("@")
                || message.Body.TrimStart().StartsWith("@"))
            {
                // command
                ProcessCommand(message);
            }
            else if (message.Body.Equals(Quot.Suffix1) || message.Body.Equals(Quot.Suffix2))
            {
                Send(message.Chat, Storage.GetTodayQuots(message.ChatName));
            }
            else if(Quot.IsQuotString(message.Body))
            {
                // quote
                ProcessQuot(message);
            }
            else if (Settings.Instance.Get(message.ChatName).IsBabkaEnabled
                && Babka.Match(message.Body))
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
                    if (Settings.Instance.Get(message.ChatName).IsSaveLogEnabled)
                    {
                        AccumulateDictionary(message);
                    }
                    Respond(message);
                    break;
            }
        }

        private void AccumulateDictionary(IChatMessage message)
        {
            Generator.FeedDictionary(SkypeUtils.GetFsSafeName(message.ChatName), SkypeUtils.RemoveSmilies(message.Body));
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
            var s = message.Body.Trim().ToLowerInvariant();
            if (!s.StartsWith("@") || s.Length <= 1)
                return;

            s = s.TrimStart(new[] { '@' });
            var parts = s.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts == null || parts.Length == 0)
                return;

            var cmd = parts[0];
            var args = new string[parts.Length - 1];
            for (var i = 0; i < parts.Length - 1; i++)
            {
                args[i] = parts[i + 1];
            }

            IChatMember member;

            switch (cmd)
            {
                //case Commands.GameNews:
                //GameNews.Load(response => Send(message.Chat, response));
                //break;
                case Commands.Help:
                    Send(message.Chat, Commands.ToString());
                    break;
                case Commands.Siske:
                    Siske.Load(response => Send(message.Chat, response));
                    break;
                case Commands.Disable:
                    if (!SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                        break;
                    if (args.Length > 0)
                    {
                        switch (args[0])
                        {
                            case "vbros":
                                if (Settings.Instance.Get(message.ChatName).IsVbrosEnabled)
                                {
                                    Send(message.Chat, "XYNTA MODE OFF");
                                    Settings.Instance.Get(message.ChatName).IsVbrosEnabled = false;
                                }
                                break;
                            case "babka":
                                if (Settings.Instance.Get(message.ChatName).IsBabkaEnabled)
                                {
                                    Send(message.Chat, "BABKA IS OFFLINE");
                                    Settings.Instance.Get(message.ChatName).IsBabkaEnabled = false;
                                }
                                break;
                        }
                    }
                    if (Settings.Instance.Get(message.ChatName).IsReplyEnabled)
                    {
                        Send(message.Chat, "Все, молчу, молчу! :x");
                        Settings.Instance.Get(message.ChatName).IsReplyEnabled = false;
                    }
                    break;
                case Commands.Enable:
                    if (!SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                        break;
                    if (args.Length > 0)
                    {
                        switch (args[0])
                        {
                            case "vbros":
                                if (!Settings.Instance.Get(message.ChatName).IsVbrosEnabled)
                                {
                                    Send(message.Chat, "Буллшит генератор включён!");
                                    Settings.Instance.Get(message.ChatName).IsVbrosEnabled = true;
                                }
                                break;
                            case "babka":
                                if (!Settings.Instance.Get(message.ChatName).IsBabkaEnabled)
                                {
                                    Send(message.Chat, "Бабка следит за тобой, юзернейм.");
                                    Settings.Instance.Get(message.ChatName).IsBabkaEnabled = true;
                                }
                                break;
                        }
                    }
                    else if (!Settings.Instance.Get(message.ChatName).IsReplyEnabled)
                    {
                        Send(message.Chat, "Спасибо, солнышко, я так соскучилась :*");
                        Settings.Instance.Get(message.ChatName).IsReplyEnabled = true;
                    }
                    break;
                case Commands.Admin:
                    if (args.Length == 2 && SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                    {
                        switch (args[0])
                        {
                            case Commands.Type.Add:
                                if (!Storage.IsUserAdmin(args[1], message.ChatName))
                                {
                                    Storage.AddAdmin(args[1], message.ChatName);
                                    Send(message.Chat, string.Format("Пользователь {0} добавлен в администраторы.", args[1]));
                                }
                                else
                                {
                                    Send(message.Chat, string.Format("Пользователь {0} уже имеет права администратора.", args[1]));
                                }
                                break;
                            case Commands.Type.Remove:
                                if (Storage.IsUserAdmin(args[1], message.ChatName))
                                {
                                    Storage.RemoveAdmin(args[1], message.ChatName);
                                    Send(message.Chat, string.Format("{0} мне больше не указ, какая досада! (rofl)", args[1]));
                                }
                                else
                                {
                                    Send(message.Chat, string.Format("Администратор с логином {0} не найден.", args[1]));
                                }
                                break;
                        }
                    }
                    break;
                case Commands.Admins:
                    Send(message.Chat, Storage.GetAdmins(message.ChatName));
                    break;
                case Commands.Mute:
                    if (args.Length == 2 && SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                    {
                        member = SkypeUtils.GetChatMemberByHandler(message.Chat, args[1]);
                        if (member != null)
                        {
                            switch (args[0])
                            {
                                case Commands.Type.On:
                                    member.Role = TChatMemberRole.chatMemberRoleListener;
                                    Send(message.Chat, string.Format("Заткнись, {0}!", args[1]));
                                    break;
                                case Commands.Type.Off:
                                    member.Role = TChatMemberRole.chatMemberRoleUser;
                                    Send(message.Chat, string.Format("{0} может говорить.", args[1]));
                                    break;
                            }
                        }
                        else
                        {
                            Send(message.Chat, "Пользователь с таким логином в чате не найден.");
                        }
                    }
                    break;
                case Commands.Promote:
                    if (args.Length == 1 && SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                    {
                        member = SkypeUtils.GetChatMemberByHandler(message.Chat, args[0]);
                        if (member != null)
                        {
                            member.Role = TChatMemberRole.chatMemberRoleMaster;
                        }
                        else
                        {
                            Send(message.Chat, "Пользователь с таким логином в чате не найден.");
                        }
                    }
                    break;
                case Commands.Demote:
                    if (args.Length == 1 && SkypeUtils.IsUserAdmin(message.Sender.Handle, message.ChatName))
                    {
                        member = SkypeUtils.GetChatMemberByHandler(message.Chat, args[0]);
                        if (member != null)
                        {
                            member.Role = TChatMemberRole.chatMemberRoleUser;
                        }
                        else
                        {
                            Send(message.Chat, "Пользователь с таким логином в чате не найден.");
                        }
                    }
                    break;
                case Commands.Diablo3:
                    Send(message.Chat, Diablo3.GetTimeLeftStr());
                    break;
                case Commands.Statistic:
                    Send(message.Chat, StatsManager.Instance.GetStatistic(message.ChatName));
                    break;
                case Commands.Generate:
                    Send(message.Chat, SkypeUtils.ReturnSmilies(Generator.Generate(SkypeUtils.GetFsSafeName(message.ChatName))));
                    break;
                default:
                    if (Settings.Instance.Get(message.ChatName).IsVbrosEnabled)
                    {
                        Send(message.Chat, SkypeUtils.ReturnSmilies(Generator.Generate(SkypeUtils.GetFsSafeName(message.ChatName))));
                    }
                    break;
            }
        }

        private void Respond(IChatMessage message)
        {
            if (message == null || message.Body.Length == 0)
                return;

            string response = null;
            if(Settings.Instance.Get(message.ChatName).IsReplyEnabled
                && Zhazha.Rnd.NextDouble() < 0.001)
            {
                var name = message.FromDisplayName;
                if(string.IsNullOrEmpty(name))
                {
                    name = message.FromHandle;
                }
                response = string.Format("/topic {0} (c) {1}", message.Body, name);
            }
            else if (Settings.Instance.Get(message.ChatName).IsVbrosEnabled
                && Zhazha.Rnd.NextDouble() < 0.04)
            {
                response = SkypeUtils.ReturnSmilies(Generator.Generate(SkypeUtils.GetFsSafeName(message.ChatName)));
            }
            else if(Settings.Instance.Get(message.ChatName).IsReplyEnabled)
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
