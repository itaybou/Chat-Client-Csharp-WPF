/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MileStoneClient.LogicLayer;

namespace MileStoneProjectTests.LogicLayerTests
{
    /// <summary>
    /// All message frame class unit tests.
    /// </summary>
    [TestFixture]
    class MessageFrameTests
    {
        /// <summary>
        /// Tests addition of new messages to the message frame messages list. 
        /// Assert messages added are equal to the messages on the messages list.
        /// </summary>
        [Test]
        public void CheckNewMessageSaved_MessageSaved_ReturnsMessage()
        {
            MessageFrame messages2 = new MessageFrame();

            for (int i = 0; i <= 50; i++)
            {
                Guid id = new Guid();
                Message m = new Message("test" + i, DateTime.Now, id, new User("test", "test", "18"));
                messages2.NewUserMessage(m);
                Assert.AreEqual(m, messages2.messages.Peek());
            }
        }

        /// <summary>
        /// Tests a message list sort by username after being shuffled.
        /// Asserts that the messages are sorted as a pre-defined sorted list.
        /// </summary>
        [Test]
        public void CheckSortMessagesByUsername_MessagesSorted_ReturnsSortedList()
        {
            //Arrange
            MessageFrame messages1 = new MessageFrame();
            MessageFrame messages2 = new MessageFrame();

            //Act
            for (int i = 0; i <= 50; i++)
            {
                Guid id = new Guid();
                Message m = new Message("test" + i, DateTime.Now, id, new User("test", "test", "18"));
                messages1.NewUserMessage(m);           
            }
            messages2.messages = messages1.messages;
            List<Message> l2 = messages2.messages.ToList();
            Shuffle(l2);
            var queue = new Queue<Message>(l2);
            messages2.messages = queue;
            messages2.SortingMethods("Username", true);

            //Assert
            Assert.AreEqual(messages1.messages, messages2.messages);
        }

        /// <summary>
        /// Tests a message list sort by group ID then by username and then by timestamp after being shuffled.
        /// Asserts that the messages are sorted as a pre-defined sorted list.
        /// </summary>
        [Test]
        public void CheckSortMessagesByAllCriteria_MessagesSorted_ReturnsSortedList()
        {
            //Arrange
            MessageFrame messages1 = new MessageFrame();
            MessageFrame messages2 = new MessageFrame();

            //Act
            for (int i = 0; i <= 50; i++)
            {
                Guid id = new Guid();
                Message m = new Message("test" + i, DateTime.Now, id, new User("test"+i, "test", i.ToString()));
                messages1.NewUserMessage(m);
            }
            messages2.messages = messages1.messages;
            List<Message> l2 = messages2.messages.ToList();
            Shuffle(l2);
            var queue = new Queue<Message>(l2);
            messages2.messages = queue;
            messages2.SortingMethods("All Criteria", true);

            //Assert
            Assert.AreEqual(messages1.messages, messages2.messages);
        }

        /// <summary>
        /// Tests a message list filtering by username and group ID.
        /// Asserts that the messages are filtered as a pre-defined sorted list.
        /// </summary>
        [Test]
        public void CheckMessageFiltering_MessagesFiltered_ReturnsFilteredList()
        {
            //Arrange
            MessageFrame messages = new MessageFrame();
            String[] filters = { "18", "test" };
            const int EXPECTED_MESSAGES_COUNT = 20;
            int correctOutputCounter = 0;
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rand = new Random();
            String[] sortingMethods = { "Username", "Timestamp", "All Criteria" };
            List<String[]> output;

            //Act
            for (int i = 0; i < EXPECTED_MESSAGES_COUNT; i++)
            {
                Guid id = new Guid();
                Message m = new Message("test", DateTime.Now, id, new User("test", "test", "18"));
                messages.NewUserMessage(m);
            }

            for (int i = 0; i <= 300; i++)
            {
                Guid id = new Guid();
                Message m = new Message("test", DateTime.Now, id, new User("test"+rand.Next(chars.Length), "test", i.ToString()));
                messages.NewUserMessage(m);
            }

            List<Message> messageList = messages.messages.ToList();
            Shuffle(messageList);
            output = messages.DisplayFiltered_Messages(filters, "All Criteria", true);

            foreach(String[] message in output)
            {
                correctOutputCounter = ((message[0] == "test") && (message[3] == "18")) ? correctOutputCounter + 1 : correctOutputCounter;
            }

            //Assert
            Assert.AreEqual(EXPECTED_MESSAGES_COUNT, correctOutputCounter);
        }

        /// <summary>
        /// Assistance method that shuffles a list
        /// </summary>
        /// <typeparam name="T">List data type</typeparam>
        /// <param name="list">A list to shuffle</param>
        public static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            Random rnd = new Random();
            while (n > 1)
            {
                int k = (rnd.Next(0, n) % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
