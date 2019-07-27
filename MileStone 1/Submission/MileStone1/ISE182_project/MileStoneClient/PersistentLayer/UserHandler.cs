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
    //Used to handle all local users local files 
    public class UserHandler : IHandler<List<User>>
    {
        //Fields
        private FileWriteRead<User> _writeRead; //Used to write and read to files.
        private const string PATH = @"System Files\Users.bin"; // A constant path of the registered users local file.

        //Constructor
        public UserHandler()
        {
            this._writeRead = new FileWriteRead<User>();
        }

        //Methods
        /// <summary>
        /// Write a list of users to local file path.
        /// </summary>
        /// <param name="messages">A list to write to file.</param>
        public void Write(List<User> users)
        {
            _writeRead.WriteToFile(PATH, users);
        }

        /// <summary>
        /// Reads from local file path a list registered users.
        /// </summary>
        /// <returns>A list read from file.</returns>
        public List<User> Read()
        {
            try
            {
                List<User> users =(List<User>) _writeRead.ReadFromFile(PATH);
                if (users == null)
                    return new List<User>();
                else return users;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
