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
    /// Message class unit tests.
    /// </summary>
    [TestFixture]
    class MessageTests
    {
        /// <summary>
        /// Test message validation does not allow illegal inputs.
        /// Illegal inputs checked: Message content exceeds max message length, empty string message.
        /// </summary>
        [Test]
        public void CheckMessageValidation_MessegeValidityCheck_ReturnsFalse()
        {
            //Arrange
            Random rand = new Random();

            //Act
            for (int i = 1; i <= 100; i++)
            {
                int option = rand.Next(2);
                Message test = (option==0) ? new Message(createRandomString(Message.MAX_LENGTH+i)) : new Message(String.Empty);
                
                //Assert
                Assert.IsFalse(Message.CheckMessageValidity(test.body));
            }
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
