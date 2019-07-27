/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Collections.Generic;
using System.Linq;
using MileStoneClient.PersistentLayer;

namespace MileStoneClient.LogicLayer
{

    //Represents the ChatRoom and all of the integrated components.
    public static class ChatRoom
    {
        //Fields
        public static User _loggedInUser { get; set; } // current logged in user
        public static MessageFrame _messages; // A message frame responsible for all message occurences.
        public static EventLogger _logger; // A logger responsible logging all different system events.
        public const String _url = @"http://ise172.ise.bgu.ac.il:80"; // Chat-Room server URL
        public static List<User> _registeredUsers; // A list that holds all current registered users
        private static UserHandler _userHandler; // A handler responsible for all user related managment.
        public static bool _serverCommunicationStatus { get; set; } //Indicates whether the chat room is connected to server

        //Methods
        /// <summary>
        /// Initiates all ChatRoom Components and reads SystemFiles/Users.bin to get Registered users list.
        /// </summary>
        /// <returns>String that represents the status of the initation.</returns>
        public static string Initiate()
        {
            _loggedInUser = null;
            _logger = new EventLogger();
            _logger.Initiate();
            _registeredUsers = new List<User>();
            _userHandler = new UserHandler();
            _serverCommunicationStatus = true;
            try
            {
                _messages = new MessageFrame();
            }
            catch (Exception e)
            {
                _logger.log(3, "Failed to load message frame module");
            }
            try
            {
                _registeredUsers = _userHandler.Read();
               if (_registeredUsers.Count==0)
                {
                    return System.String.Empty;
                }
            }
            catch (Exception e)
            {
                _logger.log(3, "Failed to load registered users list located: SystemFiles\\Users.bin\r\n" + e.Message);
            }
            _logger.log(1, "Chat-Room Client loaded successfully.");
            _logger.log(1, "Successfully loaded registered users list located: SystemFiles\\Users.bin");
            return null;
        }

        /// <summary>
        /// Validates user registration and logs user to the system.
        /// </summary>
        /// <param name="username">User username</param>
        /// <param name="password">User password</param>
        /// <returns>True if login successful, otherwise returns false</returns>
        public static bool Login(String username, String password)
        {
            User user = CheckUserRegistered(username, password);
            if (user != null)
            {
                _loggedInUser = user;
                _logger.log(2, "USER: " + _loggedInUser.username + " has logged in to Chat-Room.");
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Registers a new user.
        /// Checks if user with the same registration deatils already exist beforehand..
        /// If user successfully registers writes updated Users list to SystemFiles/Users.bin.
        /// </summary>
        /// <param name="username">User password</param>
        /// <param name="password">User password</param>
        /// <param name="groupID">User associated group number</param>
        /// <returns>A string representing an error during registration proccess.</returns>
        public static string Register(String username, String password, string groupID)
        {
            string checkResult = CheckUserDetails(username, password);
            if (checkResult == null)
            {
                try
                {
                    User user = new User(username, password, groupID);
                    if (_registeredUsers != null)
                    {
                        if (isRegisteredUsername(user.username))
                            throw new ArgumentException("A User with the same username already exists. please try another username.");
                    }
                    _loggedInUser = user;
                    _registeredUsers.Add(_loggedInUser);
                    try
                    {
                        _userHandler.Write(_registeredUsers);
                    }
                    catch(Exception e)
                    {
                        _logger.log(3,"there was a problem with the writing of the user in the file error:"+e.Message);
                    }
                    
                    _logger.log(2, "USER: " + _loggedInUser.username + " has successfully registered. Current user count: " + _registeredUsers.Count);
                }
                catch (ArgumentException e)
                {
                    return e.Message;
                }
            }
            return checkResult;
        }

        /// <summary>
        /// Check input username is already taken by other user.
        /// </summary>
        /// <param name="username">A username to check if already exists.</param>
        /// <returns></returns>
        private static bool isRegisteredUsername(string username)
        {
            User user = new User(username, "dummy", "0");
            foreach (User u in _registeredUsers)
            {
                if (user.CompareTo(u) > 0)
                {
                    return true;
                }
            }
            return false;
        }
        ///******************************************************************************************************************************

        /// <summary>
        /// Checks user is registered in the Users list.
        /// </summary>
        /// <param name="username">Checked user username</param>
        /// <param name="password">Checked user password</param>
        /// <returns>A registered user if user is registered, else null</returns>
        public static User CheckUserRegistered(String username, String password)
        {
            var userSearch = (from u in _registeredUsers where u.username == username && u.password == password select u).SingleOrDefault();
            User user = userSearch;
            if (!(user == null))
                return user;
            return null;
        }

        /// <summary>
        /// Checks user details are valid.
        /// </summary>
        /// <param name="username">Username to check validation</param>
        /// <param name="password">Password to check validation</param>
        /// <returns>Error message if user deatils are not vaild, else null</returns>
        private static string CheckUserDetails(String username, String password)
        {
            try
            {
                if (username.Length > 15 | username.Length < 2)
                {
                    throw new ArgumentException("A valid username needs to be 2-15 characters only.");
                }
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }
            int countSpaces = 0;
            foreach (char c in username)
            {
                try
                {
                    if (!Char.IsLetterOrDigit(c) && !(c == ' '))
                    {
                        throw new ArgumentException("A username can contain only english ABC, digits and spaces.");
                    }
                    else if ((c == ' '))
                    {
                        countSpaces++;
                        if(countSpaces==username.Length)
                        {
                            throw new ArgumentException("A username can not be blank spaces.");
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    return e.Message;
                }
            }
            try
            {
                if (password.Length > 15 | password.Length < 4 | password.Contains(" "))
                    throw new ArgumentException("A valid password needs to be 4-15 characters only and no spaces.");
            }
            catch (ArgumentException e)
            {
                return e.Message;
            }
            return null;
        }

        /// <summary>
        /// Calls the current user and sends a message.
        /// </summary>
        /// <param name="msgBody">Message body to be sent.</param>
        /// <returns>A string representing an error durring the sending proccess</returns>
        public static String SendUserMessage(String msgBody)
        {
            string output;
            try
            {
                output = _loggedInUser.SendMessage(msgBody);
            }
            catch (Exception e)
            {
                _logger.log(3, "The following error occurred while USER: " + _loggedInUser + " was trying to send to server: " + e.Message);
                return e.Message;
            }
            _logger.log(2, "USER: " + _loggedInUser.username + " sent the following message: " + msgBody);
            RetrieveTenMessages(false);
            return output;
        }

        /// <summary>
        /// Calls message frame and retrieves last 10 messages from server.
        /// </summary>
        /// <param name="checkRetrieval">Indicates whteher the method was used in order to check connection ore for normal use.</param>
        /// <returns>A string indicating the number of messages rettrieved.</returns>
        /// <returns>A string with the number of messages retrieved if retrieving was successful or an error message else.</returns>
        public static string RetrieveTenMessages(bool checkRetrieval)
        {
            int messagesRetrieved;
            try
            {
                messagesRetrieved = (char)_messages.RetrieveTenMessages();
                if(!checkRetrieval && messagesRetrieved != 0)
                  _logger.log(2, "USER: " + _loggedInUser.username + " retrieved " + messagesRetrieved+"" + " messages successfully from server.");
            }
            catch (Exception e)
            {
                _logger.log(3, "The following error occurred while trying to retrieve from server: " + e.Message);
                return e.Message;
            }
            return messagesRetrieved+String.Empty;
        }

        /// <summary>
        /// Calls message frame and gets a string representing all existing messages by the curreny sorrting method and direction.
        /// </summary>
        /// <param name="sortMethod">A string representing the current message sorting method.</param>
        /// <param name="ascending">AIndicated whether the order is ascending or descending.</param>
        /// <returns>A string representing messges</returns>
        public static List<String[]> DisplayAllMessages(String sortMethod, bool ascending)
        {
            return _messages.DisplayAllMessages(sortMethod, ascending);
        }

        /// <summary>
        /// Calls message frame and gets a string representing all mesges corresponding the filters, sorting method and sorting direction given.
        /// </summary>
        /// <param name="sortMethod">A string representing the current message sorting method.</param>
        /// <param name="ascending">AIndicated whether the order is ascending or descending.</param>
        /// <returns>A string representing messges</returns>
        public static List<String[]> DisplayFiltered_Messages(String[] filterBy, String sortMethod, bool ascending)
        {
            return _messages.DisplayFiltered_Messages(filterBy, sortMethod, ascending);
        }

        /// <summary>
        /// Checks the connection to the server by trying to retrive messages.
        /// </summary>
        /// <returns>A String representing an error in the connection or an empty string if connection succeeded.</returns>
        public static string CheckServerCommunication()
        {
            try
            {
                int temp = int.Parse(RetrieveTenMessages(true));
                _serverCommunicationStatus = true;
                _logger.log(1, "Chat Room connected to server successfully.");
                return String.Empty;
            }
            catch
            {
                _serverCommunicationStatus = false;
                _logger.log(5, "Error communicating with the server.");
                return "Error communicating with the server.\r\nPlease make sure you are properly connected.";
            }
        }
                
    }
}