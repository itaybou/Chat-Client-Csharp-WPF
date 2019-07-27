/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MileStoneClient.LogicLayer;
using MileStoneClient.PresentationLayer.Controls;

namespace MileStoneClient.PresentationLayer
{
    //A class that defines all main chat window operation and functionality.
    public partial class ChatRoomWindow : Window, INotifyPropertyChanged
    {
        //Fields
        private static LoginWindow _openLoginWindow = null; //Current related instance of a login window.
        private FlowDocument _shownMessagesDocument; // A document containing all current displayed mesasges.
        private Timer _messageLoadTimer; // A Timer that refreshes retrieves message by defined time interval.
        private DispatcherTimer _clearNotificationsTimer; // A Dispatcher Timer that cleares notification text box by defines time interval.
        private static Double _chatBoxScrollbarVerticalOffset; // Current messages text box scroll bar vertical offset.
        private static String _sortMethod; // A String representing the current active sorting method.
        private static bool _ascendingOrder; // Defines wether the messges are displayed in ascending order or descending order.
        private static bool _hasFilters; // Defines wether messages are currently filtered by any parameters.
        private static bool _chatBubbleView { get; set; } // Determines whether the current message view type is chat bubble or textual.
        private static String[] _filterBy; // An Array of message filtering parameters.
        private String _currentUserMessageText { get; set; } //Current end-user text inserted in the user message text box.
        private int _textSizeOffset { get; set; } //A indentation value to the chat message font size.
        private static WindowState _windowState; //Indicated the window size state.
        private static int _dispalyedMessages; //Indicates how many messages are currently showing in the chat box.
        private const int MESSAGES_RETRIEVE_TIME_INTERVAL = 2; //A constant time in seconds to retrieve new messages in.
        private const int NOTIFICATION_DELETE_TIME_INTERVAL = 7; //A constant time in seconds to retrieve new messages in.
        private const int DEFAULT_FONT_SIZE_OFFSET = 6; //A constant time in seconds to retrieve new messages in.

        //Constructor
        public ChatRoomWindow(string statusMessage)
        {
            InitializeComponent();
            this.DataContext = this;
            shownMessagesDocument = new FlowDocument();
            //Initialize default properties values
            _currentUserMessageText = (String)UserMessageTextBox.Tag;
            _dispalyedMessages = 0;
            ascendingOrder = true;
            sortMethod = "Timestamp";
            hasFilters = false;
            hasFilters = false;
            chatBubbleView = true;
            windowState = WindowState.Normal;
            _filterBy = new String[2];
            textSizeOffset = DEFAULT_FONT_SIZE_OFFSET;
            //Check server communication
            ChatRoom.CheckServerCommunication();
            CheckServerCommunication(statusMessage);
            StartMessageTimer();
            //Updates displayed details and messages
            UpdateUserCounter();
            SetScrollbarTo_NewestMessages();
            ChatRoom._logger.log(1, "Chat Room graphical user interface loaded successfully.");
        }

        //Methods
        /// <summary>
        /// Calls the chat room to retrieve messages from server and calls the display messaages method
        /// to display the retrieved messages in chat box.
        /// </summary>
        private void RetrieveMessages()
        {
            if (ChatRoom._loggedInUser != null && ChatRoom._serverCommunicationStatus)
            {
                String retirevalMessage = ChatRoom.RetrieveTenMessages(false);
                if (!(GUI.IsNumeric(retirevalMessage)))
                {
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "While trying to retrieve messages from server the following error occured:\r\n" + retirevalMessage, "#880C25", true, false);
                    StopMessageTimer();
                    ChatRoom.CheckServerCommunication();
                    CheckServerCommunication("");
                }
                else
                {
                    int newMessages = int.Parse(retirevalMessage);
                    if (newMessages >= 0)
                        DisplayMessages(newMessages);
                }
            }
        }

        /// <summary>
        /// Calls the load messages method with the number of new messages retrieved from server and an indicator if filters are enabled currently.
        /// </summary>
        /// <param name="newMessages">Indicator of how many new messages retrieved from server.</param>
        private void DisplayMessages(int newMessages)
        {
            if (hasFilters == true)
                LoadMessages(newMessages, true);
            else
            {
                foreach (String str in _filterBy)
                {
                    if (str != null)
                        hasFilters = true;
                }
                if (hasFilters)
                    LoadMessages(newMessages ,true);
                else LoadMessages(newMessages, false);
            }
        }
        /// <summary>
        /// Calls the chat room methods to display messages according to the filtering state.
        /// Adds each message retrieved from the chat room display messages methods to the chat box
        /// Updates the current message counter.
        /// </summary>
        /// <param name="newMessges">Amount of new messages retrieved from server.</param>
        /// <param name="filtered">An indicator if the filtering is enables currently.</param>
        internal void LoadMessages(int newMessges, bool filtered)
        {
            if (!CheckChatScrollBarScrolledToEnd()) //Not scrolled to end.
                _chatBoxScrollbarVerticalOffset = ChatMessages_Block.VerticalOffset; //Save current scrolled location.
            List<String[]> chatMessages = new List<String[]>();
            if (filtered)
            {
                chatMessages = ChatRoom.DisplayFiltered_Messages(_filterBy, sortMethod, ascendingOrder);
                if(chatMessages.Count==0)
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "No messages could be found according to the current filtering.\r\nPlease try changing the filtering values or reset the filtering options to view all messages.", "#FF4F6B89", true, false);
            }
            else chatMessages = ChatRoom.DisplayAllMessages(sortMethod, ascendingOrder);
            shownMessagesDocument = new FlowDocument();
            if (!(chatMessages.Count == 0))
            {
                foreach (String[] message in chatMessages)
                {
                    bool userMessage = ((ChatRoom._loggedInUser != null) && (ChatRoom._loggedInUser.username == message[0])) ? true : false;
                    bool playNewMessageSound = (!userMessage && newMessges > 0) ? true : false;
                    AddMessageToChatBox(userMessage, message, new Paragraph()); //Adds a new message to text box (current user or other user messages).
                    if (playNewMessageSound)
                    {
                        GUI.PlaySound(@"Resources\Sounds\message_recieve.wav");
                        playNewMessageSound = !playNewMessageSound;
                    }
                }
                ManageChatBoxScroller();
            }
            else GUI.ColorTextToRichTextBox(ChatMessages_Block, shownMessagesDocument, "No messages currently available.", "#ce8c99", true, false);
            UpdateMessageCounter();
        }

        /// <summary>
        /// Adds a message to a flow document by the correct layout and design (current user message or other user messages).
        /// Designs each message accordingly (Textual or chat bubble style).
        /// </summary>
        /// <param name="userMessage">Indicator if the message to add is a current user message or other user message.</param>
        /// <param name="message">A String Array representing the individual message parts.</param>
        /// <param name="document">The doucument in which we add each paragraph.</param>
        /// <param name="paragraph">A paragrap to be added to the document, each message is a paragraph.</param>
        private void AddMessageToChatBox(bool userMessage, String[] message, Paragraph paragraph)
        {
            TextAlignment alignment = (userMessage) ? TextAlignment.Right: TextAlignment.Left;
            paragraph.TextAlignment = alignment;
            if (chatBubbleView)
            {
                paragraph.Inlines.Add(new ChatBubble(message[0], message[1], message[2], message[3], userMessage, textSizeOffset + 8)); // Display chat bubbles.
            }
            else
            {   //Display textual
                string usernameHexColor = (userMessage) ? "#FF009C4E" : "#FF4E87C2";
                paragraph.Inlines.Add(new Run(" " + message[0] + " says ➣") { Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(usernameHexColor)), Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF")), FontSize = textSizeOffset + 9 });
                paragraph.Inlines.Add(new Run(" " + message[1] + Environment.NewLine) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF080808")), FontSize = textSizeOffset + 9 });
                paragraph.Inlines.Add(new Run("Time: " + message[2] + " ● ") { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF878787")), FontSize = textSizeOffset + 5 });
                paragraph.Inlines.Add(new Run("Group: " + message[3] + " ● " + Environment.NewLine) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF878787")), FontSize = textSizeOffset + 5 });
            }
            shownMessagesDocument.Blocks.Add(paragraph);
            _dispalyedMessages++;
        }

        /// <summary>
        /// Starts a timer that retrieves messages from server in a given constant time interval.
        /// </summary>
        /// <param name="timeInSeconds">A constant time interval to retrieve messages in.</param>
        private void StartMessageTimer()
        {
            _messageLoadTimer = new Timer(
                e => MessageTimer_Tick(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(MESSAGES_RETRIEVE_TIME_INTERVAL));

        }

        /// <summary>
        /// Called every constant time interval by the message timer to retrieve new messages.
        /// </summary>
        private void MessageTimer_Tick()
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    RetrieveMessages();
                });
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(5, "Message refresh timer has failed.");
            }
        }

        /// <summary>
        /// Initiates both program timers, messages retrieval timer and notification deletion timer.
        /// </summary>
        public void Start_Timers()
        {
            StartMessageTimer();
            _clearNotificationsTimer.Start();
        }

        /// <summary>
        /// Updates and restarts the current dispalyed chat box messags.
        /// </summary>
        private void UpdateMessageCounter()
        {
            if (ChatRoom._messages != null)
            {
                MessageCount.Content = "Displaying " +_dispalyedMessages + " messages.";
            }
            _dispalyedMessages = 0;
        }

        /// <summary>
        /// Updates the user counter label
        /// </summary>
        internal void UpdateUserCounter()
        {
            RegisteredUsers.Content = ChatRoom._registeredUsers.Count + " registered users currently.";
        }

        /// <summary>
        /// Checks if the chat box scroll bar is currently scrolled to the end of the chat box or not.
        /// </summary>
        /// <returns>True if scrolled to end, false otherwise.</returns>
        private bool CheckChatScrollBarScrolledToEnd()
        {
            return ChatMessages_Block.VerticalOffset== ChatMessages_Block.ExtentHeight-ChatMessages_Block.ViewportHeight;
        }

        /// <summary>
        /// Manages the chat box scroller behavior.
        /// Keeps scroller to the end of chat box if it is aready there.
        /// Otherwise scrolles to the last saves postition of to scroller.
        /// The goal is to not interupt user scrolling while viewing older messages.
        /// </summary>
        private void ManageChatBoxScroller()
        {
            if (ChatMessages_Block.VerticalOffset != 0) //Not currently on top.
            {
                if (CheckChatScrollBarScrolledToEnd()) //If already scrolled down keep scrolling down.
                    ChatMessages_Block.ScrollToEnd();
                else ChatMessages_Block.ScrollToVerticalOffset(_chatBoxScrollbarVerticalOffset); //Bot scroll down, scroll o the last saved vertical offset.
            }
        }

        /// <summary>
        /// Sets the scroll bar to the end of the chat box hence to the end of the check box.
        /// Saves current scrolled location.
        /// </summary>
        private void SetScrollbarTo_NewestMessages()
        {
            ChatMessages_Block.ScrollToEnd();
            _chatBoxScrollbarVerticalOffset = ChatMessages_Block.ExtentHeight - ChatMessages_Block.ViewportHeight;
            ChatMessages_Block.ScrollToVerticalOffset(_chatBoxScrollbarVerticalOffset);
        }

        /// <summary>
        /// Stops the messages retrieval timer.
        /// </summary>
        private void StopMessageTimer()
        {
            _messageLoadTimer.Change(Timeout.Infinite, Timeout.Infinite); ;
        }

        /// <summary>
        /// Opens a new login window and shows it.
        /// </summary>
        /// <param name="statusMessage">A user status message to dispaly in login window.</param>
        private void OpenLoginWindow(string statusMessage)
        {
            GUI._soundStatus = false;
            _openLoginWindow = new LoginWindow(statusMessage);
            _openLoginWindow.Show();
        }

        /// <summary>
        /// Calls the chat room method to check if currently gor server communication.
        /// Stops messages retrievel timer if no communication and shows relevant controls to try and re-establish connection.
        /// </summary>
        /// <param name="statusMessage"></param>
        public void CheckServerCommunication(string statusMessage)
        {
            if (!ChatRoom._serverCommunicationStatus)
            {
                if (_messageLoadTimer != null)
                    StopMessageTimer();
                Connection_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#880C25"));
                Connection_Status.FontWeight = FontWeights.SemiBold;
                Connection_Status.Content = "Disconnected.";
                if (IsActive)
                    MessageBox.Show(ChatRoom.CheckServerCommunication(), "Chat-Room Server Communication Error", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                ServerCommunicationError_Message();
                Refresh_Button.Visibility = Visibility.Visible;
                Refresh_Label.Visibility = Visibility.Visible;
            }
            else
            {
                Connection_Status.Foreground = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00B75B"));
                Connection_Status.FontWeight = FontWeights.SemiBold;
                Connection_Status.Content = "Connected.";
                Start_Timers();
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), statusMessage, "#FF00B75B", true, false);
            }
        }

        /// <summary>
        /// Adds a error message to notification box indicating no server communication established.
        /// </summary>
        private void ServerCommunicationError_Message()
        {
            FlowDocument flowDoc = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(new Run("Error while trying to communicate with the server.\r\nMost functionality will be compromised.") { Foreground = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#880C25")), FontWeight = FontWeights.Bold });
            paragraph.Inlines.Add(new Run(Environment.NewLine + "Displaying messages from cache only.") { Foreground = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF353535")), FontWeight = FontWeights.Medium });
            flowDoc.Blocks.Add(paragraph);
            NotificationsBox.Document = flowDoc;
        }


        //Events
        /// <summary>
        /// An event that shows confirmation message if logout button was clicked.
        /// </summary>
        private void Logout_Message(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            MessageBoxResult result = MessageBox.Show(ChatRoom._loggedInUser.username + " are you sure you want to disconnect from the chat?", "Chat-Room Discconect", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ChatRoom._loggedInUser.Logout();
                    if (_messageLoadTimer != null)
                        StopMessageTimer();
                    _clearNotificationsTimer.Stop();
                    MessageCount.Content = "";
                    GUI.WindowClose(this);
                    string statusMessage = ChatRoom._registeredUsers.Count + " registered users currently.";
                    if (_openLoginWindow == null)
                        OpenLoginWindow(statusMessage);
                    else
                    {
                        _openLoginWindow.Close();
                        OpenLoginWindow(statusMessage);
                    }
                    MessageBox.Show("You Have disconnected.\r\nBYE " + ChatRoom._loggedInUser.username + " we hope to see you soon.\r\n", "Disconnected");
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy your stay " + ChatRoom._loggedInUser.username + "!", "Chat-Room", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
                    break;
            }
        }

        /// <summary>
        /// An event that is called if a user send message button was clicked.
        /// Sends user message to the chat room.
        /// </summary>
        private void SendMessage_Button(object sender, RoutedEventArgs e)
        {
            User currentUser = ChatRoom._loggedInUser;
            UserMessageTextBox.Focus();
            if (_currentUserMessageText == (String)UserMessageTextBox.Tag && UserMessageTextBox.Foreground.ToString() != "#FF000000")
                _currentUserMessageText = String.Empty;
            String checkError = ChatRoom.SendUserMessage(_currentUserMessageText);
            FlowDocument flowDoc = new FlowDocument();
            if (!(checkError == null))
            {
                GUI.PlaySound(@"Resources\Sounds\error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, flowDoc, "While trying to send message to server the following error occured:", "#880C25", true, false);
                GUI.ColorTextToRichTextBox(NotificationsBox, flowDoc, checkError, "#880C26", true, false);
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\message_send.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, flowDoc, "Message recieved by server.", "#008141", true, false);
                UserMessageTextBox.Clear();
                DisplayMessages(0);
                SetScrollbarTo_NewestMessages();
            }
        }

        /// <summary>
        /// An event showing a confirmation message if exit button was clicked or a user attempted to close the window manually.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Button(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            MessageBoxResult result = MessageBox.Show(ChatRoom._loggedInUser.username + " are you sure you want to exit from the chat?\r\nThis operation will automatically log you out of the Chat-Room.", "Chat-Room Exit", MessageBoxButton.YesNo, MessageBoxImage.Stop, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ChatRoom._loggedInUser.Logout();
                    MessageBox.Show("You Have disconnected.\r\nBYE " + ChatRoom._loggedInUser.username + " we hope to see you soon.\r\n", "Disconnected");
                    Environment.Exit(0);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// An event that set scroll bar to the end of chat box at window loading time.
        /// </summary>
        private void ChatMessages_Block_Loaded(object sender, RoutedEventArgs e)
        {
            ChatMessages_Block.ScrollToEnd();
        }

        /// <summary>
        /// Dynamically updates the currently displayed messages counter label.
        /// </summary>
        private void MessageCounter(object sender, RoutedEventArgs e)
        {
            string messageCount = ChatRoom._messages.messages.Count + "";
            if (!(messageCount == "0"))
            {
                MessageCount.Content = "Displaying " + _dispalyedMessages + " messages.";
            }
            else MessageCount.Content = "";
            _dispalyedMessages = 0;
        }

        /// <summary>
        /// Starts a timer that clears notification box in a constant interval time.
        /// Fired when a new notification shows.
        /// </summary>
        private void Clear_Notifications(object sender, RoutedEventArgs e)
        {
            _clearNotificationsTimer = new DispatcherTimer();
            _clearNotificationsTimer.Interval = TimeSpan.FromSeconds(NOTIFICATION_DELETE_TIME_INTERVAL);
            _clearNotificationsTimer.Tick += ClearNottifications_Tick;
            _clearNotificationsTimer.Start();
        }

        /// <summary>
        /// Defines the deletion operation of the notification box every constant time interval.
        /// Sent by notification box timer.
        /// </summary>
        private void ClearNottifications_Tick(object sender, EventArgs e)
        {
            if (ChatRoom._serverCommunicationStatus)
            {
                var content = new FlowDocument();
                GUI.ColorTextToRichTextBox(NotificationsBox, content, "", "#ffffff", false, false);
            }
            else ServerCommunicationError_Message();
        }

        /// <summary>
        /// Restarts the notification deletion timer every time a new notification shows.
        /// </summary>
        private void NotificationsBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _clearNotificationsTimer.Stop();
            _clearNotificationsTimer.Start();
        }

        /// <summary>
        /// An event fired by clicked the clear messages cache button.
        /// Calls chat room to delete all current message chace saved locally.
        /// </summary>
        private void Clear_MessageCache(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(ChatRoom._loggedInUser.username + " you are about to delete the message cache.\r\nthis process is irreversible!\r\nAre you Sure you want to continue?", "Chat-Room  -  Delete local messages", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    StopMessageTimer();
                    if (ChatRoom._messages.DeleteMessages())
                    {
                        GUI.PlaySound(@"Resources\Sounds\message_trash.wav");
                        MessageBox.Show("Local message cache cleared.");
                        DisplayMessages(0);
                        SetScrollbarTo_NewestMessages();
                        StartMessageTimer();
                    }
                    else MessageBox.Show("Local message cache is already empty.");
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// Notifies user via notification box and sound if he reached the constant amount of char for a legal message.
        /// Called when text is changed in the user message text box.
        /// </summary>
        private void UserMessageTextBox_NotifyMaximumLength(object sender, TextChangedEventArgs e)
        {
            if (UserMessageTextBox.Text.Length == UserMessageTextBox.MaxLength)
            {
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Reached message maximum length.\r\nA message can not exceed " + Message.MAX_LENGTH + " characters.", "#880C25", true, false);
                Console.Beep(2000, 90);
            }
        }

        /// <summary>
        /// Defines the defualt window closing operation.
        /// Shows a confirmation message to the user.
        /// </summary>
        private void Window_Close(object sender, CancelEventArgs e)
        {
            if (GUI.userClose)
            {
                MessageBoxResult result = MessageBox.Show(ChatRoom._loggedInUser.username + " are you sure you want to exit from the chat?\r\nThis operation will automatically log you out of the Chat-Room.", "Chat-Room Exit", MessageBoxButton.YesNo, MessageBoxImage.Stop, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ChatRoom._loggedInUser.Logout();
                        MessageBox.Show("You Have disconnected.\r\nBYE " + ChatRoom._loggedInUser.username + " we hope to see you soon.\r\n", "Disconnected");
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Checks for a window Enter key pressed to fire a send mesage button click.
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(Filter_GroupNumber_TextBox.IsFocused | Filter_User_TextBox.IsFocused))
            {
                if (e.Key == Key.Enter)
                {
                    if (!(UserMessageTextBox.Text.Length == 0))
                    {
                        GUI.PerformClick(SendMessageUserButton);
                        e.Handled = true;
                    }
                    else
                    {
                        GUI.PlaySound(@"Resources\Sounds\error.wav");
                        GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "A valid message can only contain 1 - " + Message.MAX_LENGTH + " characters.\r\nNo message content entered.", "#880C25", true, false);
                    }
                }
                else UserMessageTextBox.Focus();
            }
        }

        /// <summary>
        /// Event that is called if the scroll to bottom button was pressed.
        /// Sets the current scrolling position to the end of the chat box.
        /// </summary>
        private void Scroll_Down(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            ChatMessages_Block.ScrollToEnd();
        }

        /// <summary>
        /// Event that is called if the scroll to top button was pressed.
        /// Sets the current scrolling position to the start of the chat box.
        /// </summary>
        private void Scroll_Top(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            ChatMessages_Block.ScrollToHome();
        }

        /// <summary>
        /// Called when a ascending menus choice for any sorting method is chosen by the user.
        ///Changes the messages sorting the the ascending choice made.
        /// </summary>
        private void SortingAscendingMenu_Click(object sender, RoutedEventArgs e)
        {
            var sortingButton = sender as MenuItem;
            String sortingType = (String)sortingButton.Tag;
            if (sortMethod != sortingType | !ascendingOrder)
            {
                ascendingOrder = true;
                sortMethod = sortingType;
                if (sortMethod != "Timestamp")
                {
                    Default_Sort_Label.Visibility = Visibility.Hidden;
                    GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are sorted by:\r\n" + sortingType + ", Ascending", "#FF03914A", true, false);
                    Sorting_Reset_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF215B8B"));
                }
                else
                {
                    Default_Sort_Label.Visibility = Visibility.Visible;
                    GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are sorted by:\r\n" + sortingType + ", Ascending (Default)", "#FF03914A", true, false);
                    Sorting_Reset_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF252525"));
                }
                DisplayMessages(0);
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are already sorted by:\r\n" + sortingType + ", Ascending", "#FF4F6B89", true, false);
            }
        }

        /// <summary>
        /// Called when a descending menus choice for any sorting method is chosen by the user.
        /// Changes the messages sorting the the desccending choice made.
        /// </summary>
        private void SortingDescendingMenu_Click(object sender, RoutedEventArgs e)
        {
            var sortingButton = sender as MenuItem;
            String sortingType = (String)sortingButton.Tag;
            if (sortMethod != sortingType | ascendingOrder)
            {
                Default_Sort_Label.Visibility = Visibility.Hidden;
                ascendingOrder = false;
                sortMethod = sortingType;
                GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                Sorting_Reset_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF215B8B"));
                DisplayMessages(0);
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are sorted by:\r\n" + sortingType + ", Descending", "#FF03914A", true, false);
            }
            else GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are already sorted by:\r\n" + sortingType + ", Descending", "#FF4F6B89", true, false);
        }

        /// <summary>
        /// An event that is called when the ascending\descending indicator top button is pressed.
        /// Changes the current sorting for the specific sorting method to ascending\descending according to current state.
        /// </summary>
        private void Ascending_Descending_Indicator_Click(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            if (ascendingOrder)
            {
                ascendingOrder = false;
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are sorted by:\r\n" + sortMethod + ", Descending", "#FF03914A", true, false);
            }
            else
            {
                ascendingOrder = true;
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are sorted by:\r\n" + sortMethod + ", Ascending", "#FF03914A", true, false);
            }
            if(sortMethod=="Timestamp")
                Sorting_Reset_Button.Background = (ascendingOrder) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF252525")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF215B8B"));
            DisplayMessages(0);
        }

        /// <summary>
        /// Calls when the soring reset button is pressed.
        /// Resets current sorting to the initial parameters.
        /// </summary>
        private void ResetMessageSort(object sender, RoutedEventArgs e)
        {
            if (sortMethod != "Timestamp" | !ascendingOrder)
            {
                GUI.PlaySound(@"Resources\Sounds\reset_effect.wav");
                Sorting_Reset_Button.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF252525"));
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Reseted sorting options.\r\nDisplaying messages in the default order:\r\nTimestamp, Ascending", "#FF03914A", true, false);
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are already sorted in the default option.\r\nDisplaying messages in the default order:\r\nTimestamp, Ascending", "#FF717171", false, false);
            }
            Default_Sort_Label.Visibility = Visibility.Visible;
            sortMethod = "Timestamp";
            ascendingOrder = true;
            DisplayMessages(0);
        }

        /// <summary>
        /// Called when the clear message button is pressed.
        /// Deletes all writen message content (before sent).
        /// </summary>
        private void UserMessageBox_Clear(object sender, RoutedEventArgs e)
        {
            if (UserMessageTextBox.Text != String.Empty & UserMessageTextBox.Text != (String)UserMessageTextBox.Tag)
            {
                GUI.PlaySound(@"Resources\Sounds\clear_text.wav");
                UserMessageTextBox.Clear();
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Message content cleared.", "#FF717171", false, false);
                UserMessageTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD0D0D0"));
                UserMessageTextBox.Text = (String)UserMessageTextBox.Tag;
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Message content is already empty.", "#FF717171", false, false);
            }
        }

        /// <summary>
        /// Event that is fired when the user message box gets focus.
        /// Deletes the tag value and lets the end-user enter a message.
        /// </summary>
        private void UserMessageBox_Focus(object sender, RoutedEventArgs e)
        {
            if (UserMessageTextBox.Text == (String)UserMessageTextBox.Tag)
                UserMessageTextBox.Clear();
            UserMessageTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
        }

        /// <summary>
        /// Event that is fired when the user message box gets unfocused.
        /// Shows the tag value of the message box.
        /// </summary>
        private void UserMessageBox_Unfocus(object sender, RoutedEventArgs e)
        {
            if (UserMessageTextBox.Text == String.Empty || UserMessageTextBox.Text == (String)UserMessageTextBox.Tag)
            {
                UserMessageTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD0D0D0"));
                UserMessageTextBox.Text = (String)UserMessageTextBox.Tag;
            }
        }

        /// <summary>
        /// Called when the refresh server communication button is pressed.
        /// Calls chat room check server cummunication to try and renew communication with server.
        /// </summary>
        private void RefreshServerCommunication(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            ChatRoom.CheckServerCommunication();
            if (ChatRoom._serverCommunicationStatus)
            {
                GUI.PlaySound(@"Resources\Sounds\reset_effect.wav");
                Refresh_Button.Visibility = Visibility.Hidden;
                Refresh_Label.Visibility = Visibility.Hidden;
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Established server connection successfully!\r\nWelcom back " + ChatRoom._loggedInUser.username + ".", "#FF00B75B", true, false);
                StartMessageTimer();
                Connection_Status.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00B75B"));
                Connection_Status.FontWeight = FontWeights.SemiBold;
                Connection_Status.Content = "Connected.";
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Communication attempt did not succeed.\r\nTry checking your connection again.", "#880C25", true, false);
            }
        }

        /// <summary>
        /// An event called when sound on\off button are pressed.
        /// Changes current program sound state according the the current value.
        /// </summary>
        private void Sound_OnOff(object sender, RoutedEventArgs e)
        {
            if (GUI._soundStatus)
            {
                GUI._soundStatus = false;
                Sound_icon_on.Visibility = Visibility.Hidden;
                Sound_status_on_label.Visibility = Visibility.Hidden;
                Sound_status_off_label.Visibility = Visibility.Visible;
                Sound_icon_off.Visibility = Visibility.Visible;
                SoundStatus_Menu.Header = "Turn Sound On";
                SoundStatus_Menu.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("../Resources/Icons/sound_on.png", UriKind.Relative)) };
            }
            else
            {
                GUI._soundStatus = true;
                Sound_icon_on.Visibility = Visibility.Visible;
                Sound_status_on_label.Visibility = Visibility.Visible;
                Sound_status_off_label.Visibility = Visibility.Hidden;
                Sound_icon_off.Visibility = Visibility.Hidden;
                GUI.PlaySound(@"Resources\Sounds\sound_on.wav");
                SoundStatus_Menu.Header = "Turn Sound Off";
                SoundStatus_Menu.Icon = new System.Windows.Controls.Image { Source = new BitmapImage(new Uri("../Resources/Icons/sound_off.png", UriKind.Relative)) };
            }
        }

        /// <summary>
        /// Detects Enter Key press while the filtering text boxes are focused.
        /// </summary>
        private void Filter_Enter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GUI.PerformClick(Filter_Button);
                e.Handled = true;
            }
            else return;
        }

        /// <summary>
        /// An Event fired when the filtering approval button is pressed.
        /// Changes current displayed messages in chat box according to the filtering values entered.
        /// </summary>
        private void Filter_Button_Click(object sender, RoutedEventArgs e)
        {
            _filterBy = new String[2];
            String usernameInput = Filter_User_TextBox.Text;
            String groupInput = Filter_GroupNumber_TextBox.Text;
            if (groupInput == String.Empty & usernameInput == String.Empty)
            {
                GUI.PlaySound(@"Resources\Sounds\error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Please enter filtering options in order to filter messages.", "#FF717171", false, false);
            }
            if (GUI.IsNumeric(groupInput) | groupInput == String.Empty)
            {
                int group = (groupInput != String.Empty) ? group = int.Parse(groupInput) : -1;
                if (group > 0 | groupInput == String.Empty)
                {
                    if (usernameInput != String.Empty && groupInput != String.Empty)
                    {
                        hasFilters = true;
                        filterByGroup = groupInput;
                        filterByName = usernameInput;
                        Filtering_Reset_Button.Background = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF215B8B"));
                        GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                        GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are currently filtered by:\r\nGroup number: " + filterByGroup + "\r\nUsername: " + filterByName, "#FF03914A", true, false);
                        DisplayMessages(0);
                        SetScrollbarTo_NewestMessages();

                    }
                    else if (usernameInput == String.Empty && groupInput != String.Empty)
                    {
                        hasFilters = true;
                        filterByGroup = groupInput;
                        Filtering_Reset_Button.Background = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF215B8B"));
                        GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                        GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Messages are currently filtered by:\r\nGroup number: " + filterByGroup, "#FF03914A", true, false);
                        DisplayMessages(0);
                        SetScrollbarTo_NewestMessages();
                    }
                    else if (usernameInput == String.Empty && groupInput == String.Empty)
                    {
                        GUI.PerformClick(Filtering_Reset_Button);
                    }
                    else
                    {
                        GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Can not filter by username only.\r\nPlease provide a user group number.", "#880C25", true, false);
                        MessageBox.Show("Please enter one of the following:\r\n- The group number inorder to filter by a certain group.\r\n-The username and group number inorder to filter by a specific user.", "Unsufficient filtering", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                    }
                }
                else MessageBox.Show("A Group number can only be a positive value.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
            else MessageBox.Show("A Group number can only consist digits.", "Invalid input", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        /// <summary>
        /// Calls when the filtering reset button is pressed.
        /// Resets current filtering to the default parameters.
        /// </summary>
        private void Filter_Reset_Button(object sender, RoutedEventArgs e)
        {
            if (hasFilters)
            {
                hasFilters = false;
                _filterBy = new String[2];
                Filtering_Reset_Button.Background = new  SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF252525"));
                GUI.PlaySound(@"Resources\Sounds\reset_effect.wav");
                Filter_User_TextBox.Clear();
                Filter_GroupNumber_TextBox.Clear();
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Reseted all filtering options.\r\nDisplaying all messages.", "#FF03914A", true, false);
                DisplayMessages(0);
                SetScrollbarTo_NewestMessages();
            }
            else
            {
                    GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "No message filtering in activated in order to reset.", "#FF717171", false, false);
            }
        }

        /// <summary>
        /// An Event that is fired when the clear filtering text boxes button is pressed.
        /// Deletes all current written text in filtering text boxes.
        /// </summary>
        private void Clear_Filtering(object sender, RoutedEventArgs e)
        {
            if (Filter_GroupNumber_TextBox.Text != String.Empty | Filter_User_TextBox.Text != String.Empty)
            {
                GUI.PlaySound(@"Resources\Sounds\clear_text.wav");
                Filter_GroupNumber_TextBox.Clear();
                Filter_User_TextBox.Clear();
            }
            else
            {
                GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Filtering options are already empty.", "#FF717171", false, false);
            }
        }

        /// <summary>
        /// An Event that is fired when a window size is chnaged.
        /// Changes the current saved window state if specific values are met(Normal or Maximized).
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (CheckChatScrollBarScrolledToEnd())
                SetScrollbarTo_NewestMessages();
            this.windowState = (this.WindowState == WindowState.Normal) ? WindowState.Normal : (this.WindowState == WindowState.Maximized) ? WindowState.Maximized : WindowState.Minimized;
        }

        /// <summary>
        /// An Event fired when the resize window button is pressed.
        /// Change the current window size state according to the current value.
        /// </summary>
        private void Maximize_Minimize_Window(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\reset_effect.wav");
            this.windowState = (windowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        /// <summary>
        /// An event fires when the plus font size, the minus font sizes are pressed or the font size slider has been moved.
        /// Changes the font size offset value and reloads messages with the current font size (determined by the font size offset).
        /// </summary>
        private void Change_Font_Size(object sender, RoutedEventArgs e)
        {
            var fontButton = (e.Source is MenuItem) ? (e.Source as MenuItem).Tag : (e.Source as Button).Tag;
            if ((String)fontButton == "+")
            {
                if ((Text_Slider.Value < Text_Slider.Maximum))
                {
                    GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
                    Text_Slider.Value = Text_Slider.Value + 1;
                }
                else
                {
                    GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                    Text_Slider.Value = Text_Slider.Value;
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Font is already set to maximum.", "#FF717171", false, false);
                }
            }
            else if ((String)fontButton == "-")
            {
                if ((Text_Slider.Value > Text_Slider.Minimum))
                {
                    GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
                    Text_Slider.Value = Text_Slider.Value - 1;
                }
                else
                {
                    GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                    Text_Slider.Value = Text_Slider.Value;
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Font is already set to minimum.", "#FF717171", false, false);
                }
            }
            else
            {
                if (Text_Slider.Value != DEFAULT_FONT_SIZE_OFFSET)
                {
                    GUI.PlaySound(@"Resources\Sounds\reset_effect.wav");
                    Text_Slider.Value = DEFAULT_FONT_SIZE_OFFSET;
                }
                else
                {
                    GUI.PlaySound(@"Resources\Sounds\small_error.wav");
                    GUI.ColorTextToRichTextBox(NotificationsBox, new FlowDocument(), "Font is already set to default settings value.", "#FF717171", false, false);
                }
            }
        }

        /// <summary>
        /// A Event fired when the emoticon button is pressed.
        /// Open the emoticon context menu to choose an emoticon to add to the message.
        /// </summary>
        private void Open_Emoticon_Context_Menu(object sender, RoutedEventArgs e)
        {
            MenuItem checkMenuChoice = sender as MenuItem;
            if (checkMenuChoice == null)
            {
                Button button = sender as Button;
                button.ContextMenu.Visibility = Visibility.Visible;
                GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
                button.ContextMenu.IsEnabled = true;
                button.ContextMenu.PlacementTarget = (sender as Button);
                button.ContextMenu.IsOpen = true;
            }
            else GUI.PerformClick(Emoticon_Button);
        }

        /// <summary>
        /// A Event that is fired when a user choses an emoticon from the emoticon buttoncontext menu.
        /// Adds the emoticon to the message box.
        /// </summary>
        private void Add_Emoticon_User_Message(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
            UserMessageTextBox.Focus();
            var emoticonClicked = (sender as Button).Tag.ToString();
            if (emoticonClicked != "Close" && emoticonClicked != "CloseExtra")
            {
                String currText = UserMessageTextBox.Text;
                UserMessageTextBox.Text = currText + emoticonClicked;
                UserMessageTextBox.CaretIndex = UserMessageTextBox.Text.Length;
            }
            else
            {
                if(emoticonClicked == "Close")
                    Emoticon_Button.ContextMenu.Visibility = Visibility.Collapsed;
                else Additional_Emoticons_Button.ContextMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// A Event that is fired when a user clicks the change view type icon.
        /// Changes the view from Chat bubble to textual and vice versa.
        /// </summary>
        private void Change_Message_View_Type(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            chatBubbleView = !chatBubbleView;
            DisplayMessages(0);
        }

        /// <summary>
        /// Event handler to notify certain components if a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Getters and Setters
        public FlowDocument shownMessagesDocument
        {
            get { return _shownMessagesDocument; }
            set
            {
                _shownMessagesDocument = value;
                NotifyPropertyChanged("shownMessagesDocument");
            }
        }

        public String currentUserMessageText
        {
            get { return _currentUserMessageText; }
            set
            {
                _currentUserMessageText = value;
                NotifyPropertyChanged("currentUserMessageText");
            }
        }

        public int textSizeOffset
        {
            get { return _textSizeOffset; }
            set
            {
                _textSizeOffset = value;
                NotifyPropertyChanged("textSizeOffset");
                LoadMessages(0, hasFilters);
            }
        }

        public WindowState windowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                NotifyPropertyChanged("windowState");
            }
        }

        public String sortMethod
        {
            get { return _sortMethod; }
            set
            {
                _sortMethod = value;
                NotifyPropertyChanged("sortMethod");
            }
        }

        public bool ascendingOrder
        {
            get { return _ascendingOrder; }
            set
            {
                _ascendingOrder = value;
                NotifyPropertyChanged("ascendingOrder");
            }
        }

        public String filterByGroup
        {
            get { return (_filterBy[0] == null) ? "none" : _filterBy[0]; }
            set
            {
                _filterBy[0] = value;
                NotifyPropertyChanged("filterByGroup");
            }
        }

        public String filterByName
        {
            get { return (_filterBy[1] == null) ? "none" : _filterBy[1]; }
            set
            {
                _filterBy[1] = value;
                NotifyPropertyChanged("filterByName");
            }
        }

        public bool hasFilters
        {
            get { return _hasFilters; }
            set
            {
                _hasFilters = value;
                NotifyPropertyChanged("hasFilters");
            }
        }

        public bool chatBubbleView
        {
            get { return _chatBubbleView; }
            set
            {
                _chatBubbleView = value;
                NotifyPropertyChanged("chatBubbleView");
            }
        }
    }
}
