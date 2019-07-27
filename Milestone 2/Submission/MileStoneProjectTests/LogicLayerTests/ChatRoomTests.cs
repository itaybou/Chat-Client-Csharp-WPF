/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using NUnit.Framework;
using System;
using MileStoneClient.LogicLayer;

namespace MileStoneProjectTests.LogicLayerTests
{
    /// <summary>
    /// Chat Room main class test units.
    /// </summary>
    [TestFixture]
    public class ChatRoomTests
    {
        /// <summary>
        /// Test user exists in system after registration.
        /// Tests the main user registration method.
        /// Expects a message indicating the user registration failed.
        /// </summary>
        [Test]
        public void CheckRegister_UserExists_ReturnException()
        {
            //Arrange
            ChatRoom.Initiate();
            var s = ChatRoom._registeredUsers;

            //Act

            foreach (User u in s)
            {
                String username = u.username;
                String password = u.password;
                string group = u.groupID;

                //Assert
                Assert.AreEqual("A User with the same username already exists. please try another username.", ChatRoom.Register(username, password, group));
            }
        }

        /// <summary>
        /// Test the method that check if a user is already registered in system.
        /// Registers a user and checks existence.
        /// </summary>
        [Test]
        public void CheckUserRegistered_UserRegistered_ReturnUser()
        {
            //Arrange
            ChatRoom.Initiate();
            var s = ChatRoom._registeredUsers;
            User user =new User("test","test","18");
            ChatRoom.Register(user.username, user.password, user.groupID);

            //Act
            User u = ChatRoom.CheckUserRegistered("test", "test");
            //Assert
            Assert.AreEqual(u, user);

        }
    }
}
