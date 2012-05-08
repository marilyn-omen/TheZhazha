using System.Threading;
using System.Threading.Tasks;
using SKYPE4COMLib;
using TheZhazha.Events;

namespace TheZhazha.Models
{
    public class SkypeManager
    {
        #region Fields

        private Skype _skype;
        private readonly IMessageProcessor _messageProcessor;
        private readonly CancellationTokenSource _tokenSource;
        private bool _isStartingListening;
        private bool _isListening;

        #endregion

        #region Events

        public ChangedEventHandler<string> StatusChanged;

        #endregion

        public SkypeManager()
        {
            _tokenSource = new CancellationTokenSource();
            _messageProcessor = new MessageProcessor();
            _messageProcessor.MoodChanged += OnMoodChanged;
        }

        private void OnMoodChanged(object sender, MoodChangedEventArgs e)
        {
            if (_isListening)
                _skype.CurrentUserProfile.MoodText = e.Mood;
        }

        private void OnSkypeMessageStatus(ChatMessage pMessage, TChatMessageStatus status)
        {
            if (status == TChatMessageStatus.cmsReceived)
                _messageProcessor.AddMessage(pMessage);
        }

        public void StartListening()
        {
            if (!_isListening && !_isStartingListening)
            {
                _isStartingListening = true;
                TryAttach();
            }
        }

        private void TryAttach()
        {
            _skype = new Skype();

            if (!_skype.Client.IsRunning)
            {
                OnStatusChanged("Starting Skype");
                _skype.Client.Start(false, false);
                Thread.Sleep(10000);
                // TODO: move attach process to background thread
            }

            ((_ISkypeEvents_Event)_skype).AttachmentStatus += OnSkypeManagerAttachmentStatus;
            _skype.Attach(9, false);
        }

        private void RetryAttach()
        {
            ((_ISkypeEvents_Event)_skype).AttachmentStatus -= OnSkypeManagerAttachmentStatus;
            _skype = null;
            TryAttach();
        }

        private void OnSkypeManagerAttachmentStatus(TAttachmentStatus status)
        {
            switch (status)
            {
                case TAttachmentStatus.apiAttachSuccess:
                    _skype.MessageStatus += OnSkypeMessageStatus;
                    _isListening = true;
                    _isStartingListening = false;
                    OnStatusChanged("Connected");
                    break;
                case TAttachmentStatus.apiAttachPendingAuthorization:
                    OnStatusChanged("Waiting for authorization");
                    break;
                case TAttachmentStatus.apiAttachNotAvailable:
                case TAttachmentStatus.apiAttachUnknown:
                    OnStatusChanged("Connection failed, retrying in 10 seconds...");
                    Thread.Sleep(10000);
                    // TODO: move attach process to background thread
                    RetryAttach();
                    break;
                case TAttachmentStatus.apiAttachRefused:
                    OnStatusChanged("Refused by client");
                    break;
            }
        }

        private void OnStatusChanged(string status)
        {
            var handler = StatusChanged;
            if (handler != null)
            {
                handler(this, new ChangedEventArgs<string>(status));
            }
        }

        public void StopListening()
        {
            if (_isListening)
            {
                ((_ISkypeEvents_Event)_skype).AttachmentStatus -= OnSkypeManagerAttachmentStatus;
                _skype.MessageStatus -= OnSkypeMessageStatus;
                // Seems like Skype4Com API doesn't have any Detach() method
                _skype = null;
                _isListening = false;
                _isStartingListening = false;
            }
        }
    }
}
