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
    /// All User class unit tests.
    /// </summary>
    [TestFixture]
    class UserTests
    {
        /// <summary>
        /// Test user tries to send an illegal user message.
        /// Check user send message method.
        /// </summary>
        [Test]
        public void CheckSendMessage_MessegeValidityCheck_ReturnsException()
        {
            //Arrange
            User u = new User("test", "test", "18");
            //Act
            for (int i = 0; i <= 30; i++)
            {
                string s = createRandomString(Message.MAX_LENGTH+1) + createRandomString(i);
            //Assert
                Assert.AreEqual("A valid message can only contain 1-150 characters.", u.SendMessage(s));
            }
        }

        /// <summary>
        /// Tests user is logged automatically after registration.
        /// </summary>
        [Test]
        public void Checklogged_loggedAfterRegister_Returntrue()
        {
            //Arrange
            ChatRoom.Initiate();
            User user = new User("test", "test", "18");
            //Act
            ChatRoom.Register(user.username, user.password, user.groupID);
            //Assert
            Assert.AreEqual(true, user.logged);

        }

        /// <summary>
        /// Assistance method that creates A random string by length.
        /// </summary>
        /// <param name="Length">Length of random string to generate.</param>
        /// <returns>A random String in the length given as paramter.</returns>
        public static string createRandomString(int Length)
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ123456789!@#$%^&*()|}{?><";
            Random randNum = new Random();
            char[] chars = new char[Length];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < Length; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

    }
}
