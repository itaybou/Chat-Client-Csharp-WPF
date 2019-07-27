/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using MileStoneClient.CommunicationLayer;

namespace MileStoneClient.LogicLayer
{
    [Serializable]
    //A class representing a ChatRoom user.
    public class User : IComparable<User>, IEquatable<User>
    {
        //Fields
        private string _username; // User unique username
        private string _password; // User passwrod
        private bool _logged { get; set; } // Logged in / Logged out user state.
        private string _groupID; // An associated user group number.

        //Constructor
        public User (string username, string password, string g_id)
        {
            this._username = username;
            this._password = password;
            this._groupID = g_id;
            this._logged = true;
        }

        //Methods
        /// <summary>
        /// Checks user message validity and sends the message to the server.
        /// </summary>
        /// <param name="msgBody">The message content</param>
        /// <returns>A string representing error message if server communication or message validaation was not successful</returns>
        public string SendMessage (String msgBody)
        {
            try
            {
                if (Message.CheckMessageValidity(msgBody))
                {
                    Message message = new Message(msgBody);
                    try
                    {
                        IMessage msg = Communication.Instance.Send(ChatRoom._url, _groupID, this.username, message.body);
                        message.guid = msg.Id;
                        message.time = msg.Date.ToLocalTime();
                        message.user = ChatRoom._loggedInUser;
                        ChatRoom._messages.NewUserMessage(message);
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
                else throw new ArgumentException("A valid message can only contain 1-"+Message.MAX_LENGTH+" characters.");
            }
            catch (ArgumentException e)
            {
                ChatRoom._logger.log(3, "User: " + this._username + " Failed to send the message to server");
                return e.Message;
            }
            return null;
        }

        /// <summary>
        /// Checks if the user is logged in, logges user out. 
        /// if the user is not logged in the method is will do nothing.
        /// </summary>
        /// <returns>True if logged out user, false if user is already logged out</returns>
        public bool Logout()
        {
            if (!_logged)
                return false;
            this._logged = false;
            ChatRoom._logger.log(2, "USER: " + this.username + " has logged out from Chat-Room.");
            return true;
        }

        /// <summary>
        /// Compares users by their usernames.
        /// </summary>
        /// <param name="user">A user to compare to</param>
        /// <returns>Positive integer if same, negetive integer if different</returns>
        public int CompareTo(User user)
        {
            if (this._username == user.username)
                return 1;
            else return -1;
        }

        /// <summary>
        /// Checks if a user given as paramter is the same user as this current user.
        /// Users are same if they have same username, password and group ID.
        /// </summary>
        /// <param name="u">A user to check with the current user.</param>
        /// <returns></returns>
        public bool Equals(User u)
        {
            return ((this.username == u.username) && (this.password == u.password) && (this.groupID == u.groupID));
        }

        //Getters and Setters
        public string groupID
        {
            get { return this._groupID; }
        }

        public string username
        {
            get { return this._username; }
        }

        public string password
        {
            get { return this._password; }
        }

        public bool logged
        {
            get { return this._logged; }
        }
    }
}
