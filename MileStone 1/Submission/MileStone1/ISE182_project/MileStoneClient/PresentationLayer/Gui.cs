/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using MileStoneClient.LogicLayer;

namespace MileStoneClient.PresentationLayer
{
    //ChatRoom Console based graphical user interface
    public static class GUI
    {

        //Methods
        /// <summary>
        /// Initiates visual components.
        /// Starts User interactive menu determined in users logged status.
        /// </summary>
        /// <param name="chatRoomStatusMessage">A message from ChatRoom initiation that indicates if there are o registered users.</param>
        public static void Initiate(string chatRoomStatusMessage)
        {
                NewScreen();
                MarkerMessage(1, "\r\nWelcome to TEAM 18 MileStone 1 Chat-Room.\r\n");
                ChatRoom._logger.log(1, "Graphical user interface loaded successfully.");
                if (chatRoomStatusMessage == System.String.Empty)
                {
                    ErrorMessage(1, "No registered users currently.\r\n\r\n");
                }
                else
                {
                    MarkerMessage(2, ChatRoom._registeredUsers.Count + " registered users currently.\r\n\r\n");
                }
                User dummy = ChatRoom._loggedInUser;
                if (dummy == null)
                {
                    if (EntryMenu())
                        Menu();
                }
                else Menu();
        }

        /// <summary>
        /// Creates an interactive user menu for logged users.
        /// </summary>
        public static void Menu()
        {
            char menuChoice = '0';
            bool exit = false;
            while (!exit)
            {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string menu = "*********************************************************************************************************************" + "\r\n" +
                                  "Menu:" + "\r\n" +
                                  "*********************************************************************************************************************"+"\r\n"+
                                  "a. Login/Logout." + "\r\n" +
                                  "b. Retrieve last 10 messages from server." + "\r\n" +
                                  "c. Display last 20 retrieved messages (without retrieving new ones from the server)." + "\r\n" +
                                  "d. Display all retrieved messages (without retrieving new ones from the server)" + "\r\n" +
                                  "e. Write (and send) a new message (max. Length 150 characters)." + "\r\n" +
                                  "f. Delete local message cache (previous message history)." + "\r\n"+
                                  "g. Exit (logout first)." + "\r\n" +
                                  "*********************************************************************************************************************" + "\r\n" +
                                  "Please choose your action: ";
                Console.Write(menu);
                menuChoice = Console.ReadKey().KeyChar;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                switch (menuChoice)
                {
                    case ('A'):
                    case ('a'):
                            NewScreen();
                            if (!ChatRoom._loggedInUser.logged)
                                Login(!ChatRoom._loggedInUser.logged);
                            else Logout();
                         break;
                    case ('B'):
                    case 'b':
                            NewScreen();
                            string checkError = ChatRoom.RetrieveTenMessages();
                            if (!(checkError.Length < 6))
                            {   
                                ErrorMessage(1, "\r\nWhile trying to retrieve messages from server the following error occured:");
                                ErrorMessage(3, "\r\n" + checkError + "\r\n\r\n");
                            }
                            else MarkerMessage(1, "\r\nRetrieved " + checkError + " new messages from server.\r\n\r\n");
                        break;
                    case ('C'):
                    case 'c':
                            {
                                NewScreen();
                                const int messagesToDisplay = 20;
                                int availableMessages = ChatRoom._messages.messages.Count;
                                if (availableMessages >= 20)
                                    MarkerMessage(1, "\r\nDisplaying " + messagesToDisplay + " messages:\r\n\r\n");
                                else MarkerMessage(1, "\r\nDisplaying " + availableMessages + " messages:\r\n\r\n");
                                string messages = DisplayMessages(ChatRoom.DisplayMessages(messagesToDisplay));
                                if (messages == System.String.Empty)
                                    ErrorMessage(1, "No Messages to display at the moment.\r\nTry to retrieve new messages from server.\r\n\r\n");
                            }
                        break;
                    case ('D'):
                    case 'd':
                            {
                                NewScreen();
                                MarkerMessage(1, "\r\nDisplaying " + ChatRoom._messages.messages.Count + " messages:\r\n\r\n");
                                string messages = DisplayMessages(ChatRoom.DisplayAllMessages());
                                if (messages == System.String.Empty)
                                    ErrorMessage(1, "No Messages to display at the moment.\r\nTry to retrieve new messages from server.\r\n\r\n");
                                break;
                            }
                    case ('E'):
                    case 'e':
                            NewScreen();
                            Console.Write("\r\nEnter your message:");
                            WriteAndSend();
                            break;
                    case ('F'):
                    case 'f':
                            NewScreen();
                            ClearMessages();
                            break;

                        case 'g':
                    case ('G'):
                        NewScreen();
                            if (Exit())
                                exit = true;
                            break;
                        default:
                            NewScreen();
                            ErrorMessage(2, "\r\nPlease enter a valid menu selection.\r\n\r\n");
                            break;
                }
            }
                Console.ResetColor();
        }

        /// <summary>
        /// Creates an interactive user menu for un-logged users.
        /// </summary>
        public static bool EntryMenu()
        {
            char menuChoice = '0';
            bool output = false;
            while (!output)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                string menu = "*****************************************" + "\r\n" +
                                "Menu:" + "\r\n" +
                                "*****************************************"+"\r\n"+
                                "a. Registration." + "\r\n" +
                                "b. Login/Logout." + "\r\n" +
                                "c. Exit." + "\r\n" +
                                "*****************************************" + "\r\n" +
                                "Please choose your action: ";
                Console.Write(menu);
                menuChoice = Console.ReadKey().KeyChar;
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();

                switch (menuChoice)
                {
                    case ('A'):
                    case 'a':
                        NewScreen();
                        output = Register();
                        break;
                    case ('B'):
                    case 'b':
                        NewScreen();
                        if (ChatRoom._loggedInUser != null)
                                output = Login(ChatRoom._loggedInUser.logged);
                                else output = Login(false);
                        break;
                    case ('C'):
                    case 'c':
                        ErrorMessage(1, "\r\nPress any key to exit...");
                        menuChoice = Console.ReadKey().KeyChar;
                        Environment.Exit(0);
                        break;
                    default:
                        NewScreen();
                        ErrorMessage(2, "\r\nPlease enter a valid menu selection.\r\n\r\n");
                        break;
                }
            }
            return output;
        }

        /// <summary>
        /// Displays a given string that represents messages.
        /// </summary>
        /// <param name="messages">A string represents messages</param>
        /// <returns>The messages string</returns>
        public static string DisplayMessages(string messages)
        {
            Console.Write(messages);
            return messages;
        }

        /// <summary>
        /// Shows a notification to user about deleting message cache.
        /// Calls ChatRoom message frame to delete current messages if user approved.
        /// </summary>
        private static void ClearMessages()
        {
            ErrorMessage(3, "\r\n\r\n" + ChatRoom._loggedInUser.username + " you are about to delete the message cache, this process is irreversible!\r\nAre you Sure you want to continue? choose Y/N: ");
            char yesNo = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (yesNo)
            {
                case ('y'):
                case 'Y':
                    if (ChatRoom._messages.DeleteMessages())
                    {
                        ErrorMessage(3, "\r\nLocal message cache cleared.\r\n\r\n");
                    }
                    else ErrorMessage(1, "\r\nLocal message cache is already empty.\r\n\r\n");
                    Menu();
                    break;
                case ('n'):
                case 'N':
                    NewScreen();
                    break;
                default:
                    ErrorMessage(1, "\r\nPlease choose a valid answer.\r\n\r\n");
                    break;
            }
        }

        /// <summary>
        /// Shows a notification to user about logging out and exiting the program.
        /// Calls ChatRoom logout method and closes program.
        /// </summary>
        /// <returns>true if exited successfully, false if user canceled request.</returns>
        private static bool Exit()
        {
            ErrorMessage(3, "\r\n\r\n"+ChatRoom._loggedInUser.username+" you are about to leave the Chat-Room.\r\nAre you Sure you want to exit? choose Y/N: ");
            char yesNo = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (yesNo)
            {
                case ('y'):
                case 'Y':
                    ChatRoom._loggedInUser.Logout();
                    ErrorMessage(3, "\r\n\r\nYou have disconnected.\r\n");
                    ErrorMessage(1, "\r\nPress any key to exit...");
                    yesNo = Console.ReadKey().KeyChar;
                    Environment.Exit(0);
                    return true;
                case ('n'):
                case 'N':
                    NewScreen();
                    return false;
                default:
                    ErrorMessage(1, "\r\nPlease choose a valid answer.\r\n\r\n");
                    return false ;
            }
        }

        /// <summary>
        /// Shows a notification to user about logging out.
        /// Calls ChatRoom logout method.
        /// </summary>
        /// <returns>false if user disconnected, true otherwise</returns>
        private static bool Logout()
        {
            ErrorMessage(1, "\r\n" + ChatRoom._loggedInUser.username + " You are already logged in.\r\nDo you want to logout? choose Y/N: ");
            char yesNo = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (yesNo)
            {
                case ('y'):
                case 'Y':
                    NewScreen();
                    ChatRoom._loggedInUser.Logout();
                    MarkerMessage(1, "\r\nBYE " + ChatRoom._loggedInUser.username + " we hope to see you soon.\r\n");
                    ErrorMessage(3, "\r\nYou have disconnected.\r\n\r\n");
                    EntryMenu();
                    break;
                case ('n'):
                case 'N':
                    Console.WriteLine();
                    NewScreen();
                    return true;
                default:
                    ErrorMessage(1, "\r\nPlease choose a valid answer.\r\n\r\n");
                    break;
            }
            return false;
        }

        /// <summary>
        /// Displays and asks user for detaild in order to login.
        /// </summary>
        /// <param name="logged">A logged user status</param>
        /// <returns>return false if logging in failed, true otherwise.</returns>
        private static bool Login(bool logged)
        {
            if (!logged)
            {
                Console.Write("\r\nWELCOME BACK - user, please enter login deatils: ");
                MarkerMessage(3, "\r\nUSERNAME: ");
                String username = Console.ReadLine();
                MarkerMessage(3, "PASSWORD: ");
                String password = Console.ReadLine();
                if (!ChatRoom.Login(username, password))
                {
                    NewScreen();
                    ErrorMessage(2, "\r\nYou are not a registered user.\r\n" +
                        "Check if you entered your username\\password correctly.\r\n" +
                        "Please register or login to continue.\r\n\r\n");
                    return false;
                }
                else
                {
                    NewScreen();
                    MarkerMessage(1, "\r\nHello Again " + ChatRoom._loggedInUser.username + ", WELCOME-BACK!\r\n\r\n");
                    return true;
                }
            }
            else return Logout();
        }

        /// <summary>
        /// Displays and asks user for detaild in order to register.
        /// </summary>
        /// <returns>returns true if registration was successful, false otherwise.</returns>
        private static bool Register ()
        {
            bool output = false;
            Console.Write("\r\nWELCOME - Please enter registration deatils: ");
            MarkerMessage(3, "\r\nUSERNAME: ");
            String username = Console.ReadLine();
            MarkerMessage(3, "PASSWORD: ");
            String password = Console.ReadLine();
            string registerMessage = ChatRoom.Register(username, password, ChatRoom.GROUP_NUM);
            NewScreen();
            if (registerMessage == null)
            {
                MarkerMessage(1, "\r\nSUCCESSFULLY registered, please remember your username and password.\r\nHELLO new user " + ChatRoom._loggedInUser.username + ".\r\n\r\n");
                output = true;
            }
            else ErrorMessage(2, "\r\n" + registerMessage + "\r\n\r\n");
            MarkerMessage(2, "Choose your next action:\r\n");
            return output;
        }

        /// <summary>
        /// Interacts with user in order to send a message.
        /// </summary>
        private static void WriteAndSend()
        {
            User currentUser = ChatRoom._loggedInUser;
            MarkerMessage(1,"\r\n"+currentUser.username+ " says >> ");
            String message = Console.ReadLine();
            String checkError = ChatRoom.SendUserMessage(message);
            if (!(checkError == null))
            {
                ErrorMessage(1, "\r\nWhile trying to send message to server the following error occured:");
                ErrorMessage(3, "\r\n" + checkError + "\r\n\r\n");
            }
            else MarkerMessage(1, "Message reecieved by server.\r\n\r\n");
        }

        /// <summary>
        /// Helper method to start a new screen with the relevant title.
        /// </summary>
        private static void NewScreen()
        {
            Console.Clear();
            MarkerMessage(3, "========================================== MileStone 1 -CHAT-ROOM- GROUP:18 ==========================================\r\n");
        }

        /// <summary>
        /// Colors text in marker notifications.
        /// </summary>
        /// <param name="markedColor">An integer represents a certain notification color.</param>
        /// <param name="str">A string to color.</param>
        private static void MarkerMessage(int markedColor, String str)
        {
            switch (markedColor)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(str);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Colors text in error notifications.
        /// </summary>
        /// <param name="severity">An integer represents a certain notification color.</param>
        /// <param name="err">A string to color.</param>
        private static void ErrorMessage(int severity, String err)
        {
            switch (severity)
            {
                case 1: Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 2: Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 3:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write(err);
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
    }

