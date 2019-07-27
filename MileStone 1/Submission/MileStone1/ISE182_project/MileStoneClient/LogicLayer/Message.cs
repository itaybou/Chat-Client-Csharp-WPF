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
using System.Drawing;

namespace MileStoneClient.LogicLayer
{
    [Serializable]
    //A class reprersents a specific message.
    public class Message : IEquatable<Message>
    {
        //Fields
        private string _body; // Message body
        private DateTime _time; // Message date and time
        private User _user;// A user associated with the specific message
        private Guid _guid; //A unique message id

        //Constructors
        public Message (String body) // A local message constructor
        {
            this._user = ChatRoom._loggedInUser;
            this._body = body;
        }

        public Message(String body, DateTime time, Guid guid, User user)  // A retrieved message constructor
        {
            this._body = body;
            this._time = time;
            this._guid = guid;
            this._user = user;
        }

        //Methods	
        /// <summary>
        /// Checks the validity of message content.
        /// </summary>
        /// <param name="msg">The body of the message</param>
        /// <returns>false if message is not valid, true else.</returns>
        public static bool CheckMessageValidity(String msg)
        {
            if (msg.Length > 150 | msg == null | msg == System.String.Empty)
                return false;
            return true;
		}

        /// <summary>
        /// A ToString Method represents a message.
        /// </summary>
        /// <returns>String representing a Message</returns>
        public override string ToString()
        {
            return (this._user.username + " says >> " + this._body + "\r\nDetails :: Sent in: " + this._time.ToString()+" :: Group ID: "+this._user.groupID+" ::\r\n");
        }

        /// <summary>
        /// Checks if to messages are equal by comparing their GUID.
        /// a method of the <code>IEquatable<Message></code> interface.
        /// </summary>
        /// <param name="msg">Message to check equality with</param>
        /// <returns>true if equal, false else</returns>
        public bool Equals(Message msg)
        {
            return this._guid == msg.guid;
        }

        //Getters and Setters
        public User user
        {
            get { return this._user; }
            set { this._user = value; }
        }

        public String body
        {
            get { return this._body; }
        }

        public Guid guid
        {
            get { return this._guid; }
            set { this._guid = value; }
        }

        public DateTime time
        {
            get { return this._time; }
            set { this._time = value; }
        }

    }
}
