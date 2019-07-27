/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Collections.Generic;
using MileStoneClient.LogicLayer;

namespace MileStoneClient.PersistentLayer
{
    //Used to handle all local messages local files 
    public class MessageHandler : IHandler<List<Message>>
    {
        //Fields
        private FileWriteRead<Message> _writeRead; //Used to write and read to files.
        private const string PATH = @"System Files\Messages.bin"; // A constant path of the messages local file.

        //Constructor
        public MessageHandler()
        {
            this._writeRead = new FileWriteRead<Message>();
        }

        //Methods
        /// <summary>
        /// Write a list of messages to local file path.
        /// </summary>
        /// <param name="messages">A list to write to file.</param>
        public void Write(List<Message> messages)
        {
            _writeRead.WriteToFile(PATH, messages);
        }

        /// <summary>
        /// Reads from local file path a list of messages.
        /// </summary>
        /// <returns>A list read from file.</returns>
        public List<Message> Read()
        {
            try
            {
                List<Message> messages = _writeRead.ReadFromFile(PATH);
                if (messages == null)
                {
                   return new List<Message>();
                }
                else return messages;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes local messages file.
        /// </summary>
        /// <returns>True if was existed and  got deleted, false otherwise.</returns>
        public bool Delete()
        {
          bool deleted = _writeRead.DeleteFile(PATH);
            return deleted;
        }
    }
}
