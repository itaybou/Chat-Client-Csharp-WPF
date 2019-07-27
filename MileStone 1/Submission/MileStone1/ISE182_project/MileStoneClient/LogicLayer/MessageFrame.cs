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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MileStoneClient.CommunicationLayer;
using MileStoneClient.PersistentLayer;

namespace MileStoneClient.LogicLayer
{
    //Represents all of ChatRoom message related actions and integration.
    public class MessageFrame
    {
        //Fields
        public static Queue<Message> _messages; // Holds all current messaages
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
            var messagesToSave = _messages.ToArray().ToList();
            _messages = new Queue<Message>(messagesToSave);
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
            messageList.Sort((msg1, msg2) => msg1.Date.CompareTo(msg2.Date));
            foreach (IMessage msgItem in messageList)
            {
                User dummy = new User(msgItem.UserName, System.String.Empty, msgItem.GroupID);
                Message msg = new Message(msgItem.MessageContent, msgItem.Date, msgItem.Id, dummy);
                if (!(_messages.Contains(msg)))
                {
                    _messages.Enqueue(msg);
                    countMessages++;
                }
                else
                {
                    Message message = _messages.Dequeue();
                    _messages.Enqueue(message);
                }
            }
            var messagesToSave = _messages.ToList();
            try
            {
                _messageHandler.Write(messagesToSave);
            }
            catch (Exception e)
            {
                ChatRoom._logger.log(3, "Failed to write updated messages list located: SystemFiles\\Messages.bin\r\n");
            }
            return countMessages;
        }

        /// <summary>
        /// Gets last <code>int amount</code> messages and creates a string representing all the messages.
        /// </summary>
        /// <param name="amount">Number last messages to add to string</param>
        /// <returns>A string representing last <code>int amount</code> messages.</returns>
        public String DisplayMessages (int amount)
        {
            String toDisplay = System.String.Empty;
            try
            {
                List<Message> sortedMessages = _messages.OrderBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ToList();
                if (sortedMessages.Count >= 20)
                {
                    for (int i = sortedMessages.Count - amount ; i < sortedMessages.Count; i++)
                    {
                        toDisplay += sortedMessages.ElementAt(i).ToString() + "\r\n";
                    }
                }
                else
                {
                    foreach (Message msg in sortedMessages)
                    {
                        toDisplay += msg.ToString() + "\r\n";
                    }
                }
            }
            catch (Exception e)
            {
                return toDisplay;
            }
            return toDisplay;
        }
        /// <summary>
        /// Gets all current messages and creates a string representing them.
        /// </summary>
        /// <returns>A string representing all current messages.</returns>
        public String DisplayAllMessages()
        {
            String toDisplay = System.String.Empty;
            try
            {
                List<Message> sortedMessages = _messages.OrderBy(msg => msg.time.Year).ThenBy(msg => msg.time.Date).ThenBy(msg => msg.time.TimeOfDay).ToList();
                foreach (Message msg in sortedMessages)
                {
                    toDisplay += msg.ToString() + "\r\n";
                }
            }
            catch (Exception e)
            {
                return toDisplay;
            }
            return toDisplay;
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
                return true;
            return false;
        }

        //Getters and Setters
        public Queue<Message> messages
        {
            get { return _messages; }
        }

        public MessageHandler messageHandler
        {
            get { return _messageHandler; }
        }
    }
}
