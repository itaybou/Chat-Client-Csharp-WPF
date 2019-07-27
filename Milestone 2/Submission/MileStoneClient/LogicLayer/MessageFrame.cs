/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Linq;
using System.Collections.Generic;
using MileStoneClient.CommunicationLayer;
using MileStoneClient.PersistentLayer;

namespace MileStoneClient.LogicLayer
{
    //Represents all of ChatRoom message related actions and integration.
    public class MessageFrame
    {
        //Fields
        public static Queue<Message> _messages;// Holds all current messaages
        private static MessageHandler _messageHandler; // A handler used to manage all local file integration

        //Constructor
        public MessageFrame()
        {
            _messageHandler = new MessageHandler();
            try
            {
                List<Message> messages = _messageHandler.Read();
                if (messages.Count == 0)
                {
                    _messages = new Queue<Message>();
                }
                else
                {
                    var messageQueue = new Queue<Message>(messages);
                    _messages = messageQueue;
                }
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(3, "Failed to load previous message list located: SystemFiles\\Messages.bin\r\n" + e.Message);
            }
        }

        //Methods
        /// <summary>
        /// Initiated when a user send a message, after sending to server.
        /// Enqueues the message to the current message queue.
        /// Writes updated message list to local SystemFiles.
        /// </summary>
        /// <param name="message"> Indicates the message a user sent.</param>
        public void NewUserMessage (Message message)
        {
            _messages.Enqueue(message);
            var messagesToSave = _messages.ToList();
            try
            {
                _messageHandler.Write(messagesToSave);
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(3, "Failed to write updated messages list located: SystemFiles\\Messages.bin\r\n"+e.Message);
            }
        }

        /// <summary>
        /// Retrieves 10 last messages from server.
        /// Checks messages do not exist already and enquques them to local message list.
        /// Writes updated message list to SystemFiles/Messages.bin
        /// </summary>
        /// <returns>Integer represents the number of new  messages retrieved</returns>
        public int RetrieveTenMessages ()
        {
            int countMessages = 0;
            List<IMessage> messageList = Communication.Instance.GetTenMessages(ChatRoom._url);
            var currMessages = _messages.ToList();
            foreach (IMessage msgItem in messageList)
            {
                User dummy = new User(msgItem.UserName, "", msgItem.GroupID);
                Message msg = new Message(msgItem.MessageContent, msgItem.Date, msgItem.Id, dummy);
                if (!(currMessages.Exists(m => m.Equals(msg))))
                {
                    _messages.Enqueue(msg);
                    countMessages++;
                }
            }
            currMessages = _messages.ToList();
            try
            {
                _messageHandler.Write(currMessages);
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(3, "Failed to write updated messages list located: SystemFiles\\Messages.bin\r\n");
            }
            return countMessages;
        }

        /// <summary>
        /// Sorts all the stores messages Queue by the given parameters.
        /// </summary>
        /// <param name="sortMethod">A string representing the soreting method used to sort.</param>
        /// <param name="ascending">An indicator whether the messages should be sorted ascending or descending</param>
        /// <returns></returns>
        public List<Message> SortingMethods(String sortMethod, bool ascending)
        {
            List<Message> output = new List<Message>();
            switch (sortMethod)
            {
                case "Timestamp":
                    output = ascending ? _messages.OrderBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ThenBy(msg => msg.time.Millisecond).ToList() 
                                        : _messages.OrderByDescending(msg => msg.time.Year).ThenByDescending(msg => msg.time.Date).ThenByDescending(msg => msg.time.TimeOfDay).ThenByDescending(msg => msg.time.Millisecond).ToList();
                    break;
                case "Username":
                    output = ascending ? _messages.OrderBy(user => user.user.username).ThenBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ThenBy(msg => msg.time.Millisecond).ToList()
                                        : _messages.OrderByDescending(user => user.user.username).ThenBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ThenByDescending(msg => msg.time.Millisecond).ToList();
                    break;
                case "All Criteria":
                    output = ascending ? _messages.OrderBy(user => int.Parse(user.user.groupID)).ThenBy(user => user.user.username).ThenBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ThenBy(msg => msg.time.Millisecond).ToList()
                                        : _messages.OrderByDescending(user => int.Parse(user.user.groupID)).ThenByDescending(user => user.user.username).ThenByDescending(msg => msg.time.Year).ThenByDescending(msg => msg.time.Date).ThenByDescending(msg => msg.time.TimeOfDay).ThenByDescending(msg => msg.time.Millisecond).ToList();
                    break;
            }
            return output;
        }

        /// <summary>
        /// Gets all current messages and creates a list of String arrays indicating the seperated message parts.
        /// Adds only the messages corresponding to the filters given.
        /// </summary>
        /// <param name="sortMethod">A string representing the sorting method to be used to sort.</param>
        /// <param name="ascending">An indicator whether the messages should be sorted ascending or descending</param>
        /// <param name="filterBy">An Array of strings indicating the parmeters in which the messages should be filtered.</param>
        /// <returns>A string representing all current messages.</returns>
        public List<String[]> DisplayFiltered_Messages(String[] filterBy, String sortMethod, bool ascending)
        {
            List<String[]> allMessagesDetails = new List<String[]>();
            try
            {
                List<Message> sortedMessages = SortingMethods(sortMethod, ascending);
                foreach (Message msg in sortedMessages)
                {
                    String[] messageDetails = new string[4];
                    messageDetails[0] = msg.user.username;
                    messageDetails[1] = msg.body;
                    messageDetails[2] = msg.time.ToString();
                    messageDetails[3] = msg.user.groupID;
                    if ((filterBy[1] == null | filterBy[1] == String.Empty) && messageDetails[3] == filterBy[0])
                    {
                        allMessagesDetails.Add(messageDetails);
                    }
                    else if (messageDetails[3] != null && (messageDetails[3] == filterBy[0] & messageDetails[0] == filterBy[1]))
                    {
                        allMessagesDetails.Add(messageDetails);
                    }
                }
            }
            catch (Exception e)
            {
                return new List<String[]>(); ;
            }
            return allMessagesDetails; ;
        }
        /// <summary>
        /// Gets all current messages and creates a list of String arrays indicating the seperated message parts.
        /// </summary>
        /// <param name="sortMethod">A string representing the sorting method to be used to sort.</param>
        /// <param name="ascending">An indicator whether the messages should be sorted ascending or descending</param>
        /// <returns>A string representing all current messages.</returns>
        public List<String[]> DisplayAllMessages(String sortMethod, bool ascending)
        {
            List<String[]> allMessagesDetails = new List<String[]>();
            try
            {
                List<Message> sortedMessages = SortingMethods(sortMethod, ascending);
                foreach (Message msg in sortedMessages)
                {
                    String[] messageDetails = new string[4];
                    messageDetails[0] = msg.user.username;
                    messageDetails[1] = msg.body;
                    messageDetails[2] = msg.time.ToString();
                    messageDetails[3] = msg.user.groupID;
                    allMessagesDetails.Add(messageDetails);
                }
            }
            catch (Exception e)
            {
                return new List<String[]>(); ;
            }
            return allMessagesDetails; ;
        }

        /// <summary>
        /// Resets local message list.
        /// Deletes and recreates SystemFiles/Messages.bin
        /// </summary>
        /// <returns>true id deleted, false otherwise</returns>
        public bool DeleteMessages()
        {
            _messages = new Queue<Message>();
            if (_messageHandler.Delete())
            {
                ChatRoom._logger.log(2, "Message cahce cleared successfully.");
                return true;
            }
            return false;
        }

        //Getters and Setters
        public Queue<Message> messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public MessageHandler messageHandler
        {
            get { return _messageHandler; }
        }
    }
}
