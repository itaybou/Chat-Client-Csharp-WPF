/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using MileStoneClient.LogicLayer;

namespace MileStoneClient.PresentationLayer
{
    //A class that defines global operation and methods for the GUI components.
    //Initiates initial GUI components.
    public class GUI
    {
        static string _currntRegisteredUsersAmount; // A string describing the current registered users amount.
        public static bool _soundStatus = true; // Indicating the state of the sound in the program.
        private static SoundPlayer _player; // A Sound player that plays system sounds.
        private static bool _userClose = true; // Defines if the program was user closed or closed internally.

        /// <summary>
        /// Initiates the GUI components according to the current components state.
        /// </summary>
        /// <param name="chatRoomStatusMessage">A message sent from the initiation of the chat romm logical layer</param>
        public static void Initiate(string chatRoomStatusMessage)
        {
            ChatRoomWindow chatRoom = new ChatRoomWindow(chatRoomStatusMessage);
            if (chatRoom.IsVisible)
              chatRoom.Close();
            if (chatRoomStatusMessage == System.String.Empty)
            {
                _currntRegisteredUsersAmount = "No registered users currently.";
            }
            else
            {
                _currntRegisteredUsersAmount = ChatRoom._registeredUsers.Count + " registered users currently.\r\n\r\n";
            }
            User dummy = ChatRoom._loggedInUser;
            if (dummy == null || dummy.logged == false)
            {
                LoginWindow login = new LoginWindow(chatRoomStatusMessage);
                login.Show();
            }
            else
            {
                chatRoom = new ChatRoomWindow(chatRoomStatusMessage);
                chatRoom.ChatMessages_Block.ScrollToEnd();
                chatRoom.Show();
            }
        }

        /// <summary>
        /// Plays a sound from the sound path given if sound state is on.
        /// </summary>
        /// <param name="soundPath">The path to the sound file location in the resources folder.</param>
        internal static void PlaySound(String soundPath)
        {
            if (_soundStatus)
            {
                _player = new SoundPlayer(soundPath);
                _player.Play();
            }
        }

        /// <summary>
        /// A method that allows to color components in a rich text boxes in the GUI.
        /// </summary>
        /// <param name="box">The Rich text box to color the added lines in.</param>
        /// <param name="flowDoc">A document to add the the rich text box</param>
        /// <param name="text">A string with a text to be written.</param>
        /// <param name="hexColor">A string describing hex coloring value.</param>
        /// <param name="bold">A boolean indicator if the font should be bold or not</param>
        /// <param name="underline">A boolean indicator if the font should be underlined or not</param>
        public static void ColorTextToRichTextBox(RichTextBox box, FlowDocument flowDoc, string text, string hexColor, bool bold, bool underline)
        {
            Paragraph paragraph = new Paragraph();
            if (bold & !underline)
            {
                paragraph.Inlines.Add(new Bold(new Run(text)) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor)) });
            }
            else if (!bold & underline)
            {
                paragraph.Inlines.Add(new Underline(new Run(text)) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor)) });
            }
            else if (bold & underline)
            {
                paragraph.Inlines.Add(new Underline(new Bold(new Run(text))) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor)) });
            }
            else paragraph.Inlines.Add(new Run(text) { Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hexColor)) });

            flowDoc.Blocks.Add(paragraph);
            box.Document = flowDoc;
        }

        /// <summary>
        /// Performs a programmatically initiated click on a certain button. 
        /// </summary>
        /// <param name="btn">The button to be clicked</param>
        public static void PerformClick(Button btn)
        {
            btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        /// <summary>
        /// Checks if input values given is a numeric value
        /// </summary>
        /// <param name="input">A input string to check</param>
        /// <returns>True if value is numeric false otherwise</returns>
        public static bool IsNumeric(string input)
        {
            int number;
            return int.TryParse(input, out number);
        }

        /// <summary>
        /// Performs a programmatically initiated window close.
        /// </summary>
        /// <param name="toClose">A window to close.</param>
        internal static void WindowClose(Window toClose)
        {
            userClose = false;
            toClose.Close();
            userClose = true;
        }

        //Getters and Setters
        public static bool soundStatus
        {
            get { return _soundStatus; }
            set { _soundStatus = value; }
        }

        public static bool userClose
        {
            get { return _userClose; }
            set { _userClose = value; }
        }
    }
}
