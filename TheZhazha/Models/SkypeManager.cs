using System.Threading;
using System.Threading.Tasks;
using SKYPE4COMLib;
using TheZhazha.Events;

namespace TheZhazha.Models
{
    public class SkypeManager
    {
        #region Fields

        private readonly Skype _skype;
        private readonly IMessageProcessor _messageProcessor;
        private bool _isListening;
        private readonly CancellationTokenSource _tokenSource;

        #endregion

        public SkypeManager()
        {
            _skype = new Skype();
            _skype.Attach();

            _tokenSource = new CancellationTokenSource();
            _messageProcessor = new MessageProcessor();
            _messageProcessor.MoodChanged += OnMoodChanged;
        }

        private void OnMoodChanged(object sender, MoodChangedEventArgs e)
        {
            _skype.CurrentUserProfile.MoodText = e.Mood;
        }

        private void OnSkypeMessageStatus(ChatMessage pMessage, TChatMessageStatus status)
        {
            if (status == TChatMessageStatus.cmsReceived)
                _messageProcessor.AddMessage(pMessage);
        }

        public void StartListening()
        {
            if(!_isListening)
            {
                _isListening = true;
                var ct = _tokenSource.Token;
                Task.Factory.StartNew(
                    () =>
                        {
                            _skype.MessageStatus += OnSkypeMessageStatus;
                            while (!ct.IsCancellationRequested)
                            {
                                Thread.Sleep(500);
                            }
                            _skype.MessageStatus -= OnSkypeMessageStatus;
                        },
                    ct);
            }
        }

        public void StopListening()
        {
            _tokenSource.Cancel();
            _isListening = false;
        }
    }
}
