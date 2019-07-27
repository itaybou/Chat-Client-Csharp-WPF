using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MileStoneClient.PresentationLayer.Controls
{
    public partial class ChatBubble : UserControl
    {
        private String _username;
        private String _messageBody;
        private String _timeAndGroup;
        private bool _userMessage;
        private int _fontSize;
        private int _chatBubbleSize;
        private ChatRoomWindow _instance;

        public ChatBubble(String username, String messageBody, String time, String groupID, bool userMessage, int fontSize)
        {
            InitializeComponent();
            this.DataContext = this;
            this.username = username+ " says ➣";
            this.messageBody = messageBody;
            this.timeAndGroup = "Time: " + time + " ● Group: " + groupID + " ●";
            this.userMessage = userMessage;
            this.fontSize = fontSize;
            this.chatBubbleSize = fontSize * 35;
            this._instance = (ChatRoomWindow)Parent;
        }

        /// <summary>
        /// Event handler to notify certain components if a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public String username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyPropertyChanged("username");
            }
        }

        public String messageBody
        {
            get { return _messageBody; }
            set
            {
                _messageBody = value;
                NotifyPropertyChanged("messageBody");
            }
        }

        public String timeAndGroup
        {
            get { return _timeAndGroup; }
            set
            {
                _timeAndGroup = value;
                NotifyPropertyChanged("timeAndGroup");
            }
        }

        public bool userMessage
        {
            get { return _userMessage; }
            set
            {
                _userMessage = value;
                NotifyPropertyChanged("userMessage");
            }
        }

        public int fontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                NotifyPropertyChanged("fontSize");
            }
        }

        public int chatBubbleSize
        {
            get { return _chatBubbleSize; }
            set
            {
                _chatBubbleSize = value;
                NotifyPropertyChanged("chatBubbleSize");
            }
        }

        public ChatRoomWindow ChatRoomInstance
        {
            get { return _instance; }
            set
            {
                _instance = value;
                NotifyPropertyChanged("ChatRoomInstance");
            }
        }
    }
}
