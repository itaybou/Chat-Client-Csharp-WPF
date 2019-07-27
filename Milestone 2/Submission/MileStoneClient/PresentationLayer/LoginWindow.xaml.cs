/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MileStoneClient.LogicLayer;

namespace MileStoneClient.PresentationLayer
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        //Fields
        private static ChatRoomWindow _openChatRoomWindow; //Current related instance of a chat room window window.
        public String _currentUsername { get; set; } // Current username entered in the username text box
        private String _currentPassword { get; set; } // Current password entered in the password text box
        private String _groupID; // Current group ID entered in the groupID text box

        //Constructor
        public LoginWindow(String chatRoomStatusMessage)
        {
            InitializeComponent();
            loginRegisterDetails.DataContext = this;
            _openChatRoomWindow = null;
            if (chatRoomStatusMessage == System.String.Empty)
            {
                UserCountText.Text = "No users are registered currently";
            }
            else
            {
                UserCountText.Inlines.Add(new Run(ChatRoom._registeredUsers.Count + "") { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00B75B")) });
                UserCountText.Inlines.Add(new Run(" registered users currently."));
            }
            ChatRoom._logger.log(1, "Login window graphical user interface loaded successfully.");
        }

        //Methods
        /// <summary>
        /// Tries to perform a login by calling chat room login method with the entered user details.
        /// </summary>
        /// <param name="logged">Indicates if a user is already logged or not.</param>
        /// <returns>True if login is successful false otherwise.</returns>
        private bool Login(bool logged)
        {
            if (!logged)
            {
                if (!ChatRoom.Login(_currentUsername, _currentPassword))
                {
                    MessageBox.Show("You are not a registered user.\r\n" +
                        "Check if you entered your username\\password correctly.\r\n" +
                        "Please register or login to continue.\r\n\r\n", "Chat-Room Login", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK);
                    return false;
                }
                else
                {
                    string statusMessage = "Hello Again " + ChatRoom._loggedInUser.username + ", WELCOME-BACK!";
                    _currentUsername = null;
                    _currentPassword = null;
                    if (_openChatRoomWindow == null)
                        OpenChatWindow(statusMessage);
                    else
                    {
                        _openChatRoomWindow.Close();
                        OpenChatWindow(statusMessage);
                    }
                    GUI.WindowClose(this);
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Tries to perform a user register by calling chat room register method with the entered user details.
        /// </summary>
        /// <param name="statusMessage">A passed message of the current registared users state.</param>
        /// <returns>True if registration was successful false otherwise.</returns>
        private bool Register(string statusMessage)
        {
            if (((groupID != String.Empty) && groupID != null) && GUI.IsNumeric(groupID))
            {
                int g_id = int.Parse(groupID);
                if (g_id >= 1 && g_id <= 100)
                {
                    bool output = false;
                    string registerMessage = ChatRoom.Register(_currentUsername, _currentPassword, groupID);
                    if (registerMessage == null)
                    {
                        statusMessage = "SUCCESSFULLY registered, please remember your username and password.\r\nHELLO new user " + ChatRoom._loggedInUser.username + ".";
                        _currentUsername = null;
                        _currentPassword = null;
                        OpenChatWindow(statusMessage);
                        _openChatRoomWindow.UpdateUserCounter();
                        GUI.WindowClose(this);
                        output = true;
                    }
                    else MessageBox.Show(registerMessage, "Chat-Room Registration error", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
                    return output;
                }
                else
                {
                    MessageBox.Show("A legal group ID must be a number between 1-100", "Error Registering", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    GroupInputTextBox.Clear();
                    return false;
                }
            }
            else
            {
                MessageBox.Show("You have to fill the Group ID field with a legal numeric value in order to register.", "Error Registering", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                GroupInputTextBox.Clear();
                return false;
            }
        }

        /// <summary>
        /// Opens a new chat room window and shows it.
        /// </summary>
        /// <param name="statusMessage">A message indicating login\register status to the chat room window.</param>
        private void OpenChatWindow(string statusMessage)
        {
            try
            {
                GUI._soundStatus = true;
                _openChatRoomWindow = new ChatRoomWindow(statusMessage);
                _openChatRoomWindow.Show();
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(5, "Faild to load chat room main window.");
            }
        }

        /// <summary>
        /// Cleares all user detailes entered in the user details text boxes.
        /// </summary>
        /// <param name="clearGroupTextBox">Determines if the group ID text box should be cleared.</param>
        private void Clear_LoginRegister_TextBoxes(bool clearGroupTextBox)
        {
            UsernameInputTextBox.Clear();
            UserPasswordInputTextBox.Clear();
            if(clearGroupTextBox)
              GroupInputTextBox.Clear();
        }

        //Events
        /// <summary>
        /// An event that is fired if the register button was clicked.
        /// Attempts to register user.
        /// </summary>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            string statusMessage = "";
            bool registrationStatus = Register(statusMessage);
        }

        /// <summary>
        /// An event that is fired if the login button was clicked.
        /// Attempts to login user.
        /// </summary>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
            if (ChatRoom._loggedInUser != null)
                Login(ChatRoom._loggedInUser.logged);
            else Login(false);
        }

        /// <summary>
        /// An event that is fired if the exit button was clicked.
        /// Shows the user an exit confirmation message.
        /// </summary>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (GUI.userClose)
            {
                GUI.PlaySound(@"Resources\Sounds\click_sound.wav");
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit before starting?", "Leaving Chat-Room", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }

        /// <summary>
        /// An event that is fired when the group id text box gets focus.
        /// Cleares the group id text box content.
        /// </summary>
        private void GroupInputTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            GroupInputTextBox.Clear();
            GroupInputTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
        }

        /// <summary>
        /// An event that is fired when the group id text box gets unfocused.
        /// Enters the tag value of the group id text box as current text.
        /// </summary>
        private void GroupInputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GroupInputTextBox.Text == String.Empty)
            {
                GroupInputTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8A8A8A"));
                GroupInputTextBox.Text = (String)GroupInputTextBox.Tag;
            }
        }

        /// <summary>
        /// An event that is fired if one of the registration\login selection check boxes is checked.
        /// Changes the display of relevant controls to match the current wanted operation.
        /// </summary>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            String checkedBox = (e.Source as CheckBox).Tag.ToString();
            if (checkedBox == "Registration" && Check_Box_Login.IsChecked == true)
            {
                GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
                Check_Box_Login.IsChecked = false;
                GroupInputTextBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8A8A8A"));
                GroupInputTextBox.Text = (String)GroupInputTextBox.Tag;
                Clear_LoginRegister_TextBoxes(false);
            }
            else if (checkedBox == "Login" && Check_Box_Registration.IsChecked == true)
            {
                GUI.PlaySound(@"Resources\Sounds\tick_sound.wav");
                Check_Box_Registration.IsChecked = false;
                Clear_LoginRegister_TextBoxes(true);
            }
        }

        /// <summary>
        /// An event that is fired if one of the registration\login selection check boxes is unchecked.
        /// Prevents both check boxes to be unchecked.
        /// </summary>
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            String uncheckedBox = (e.Source as CheckBox).Tag.ToString();
            if ((uncheckedBox == "Registration" & Check_Box_Login.IsChecked == false) || uncheckedBox == "Login" & Check_Box_Registration.IsChecked == false)
                (e.Source as CheckBox).IsChecked = true;
        }

        /// <summary>
        /// Defines default window closing operation.
        /// Shows the user am exit confirmation message.
        /// </summary>
        private void Window_Close(object sender, CancelEventArgs e)
        {
            if (GUI.userClose)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to exit before starting?", "Leaving Chat-Room", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Environment.Exit(0);
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Changes the current password to the changed password.
        /// </summary>
        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            _currentPassword = UserPasswordInputTextBox.Password;
        }

        /// <summary>
        /// Event handler to notify certain components if a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Defines a defualt Enter Key press operation.
        /// Presses the main displayed button (Login\Register).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginRegister_EnterPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (LoginButton.IsVisible)
                    GUI.PerformClick(LoginButton);
                else
                    GUI.PerformClick(RegisterButton);
                e.Handled = true;
            }
            else return;
        }

        //Getters and Setters
        public String groupID
        {
            get { return _groupID; }
            set
            {
                _groupID = value;
                NotifyPropertyChanged("groupID");
            }
        }
    }
}

