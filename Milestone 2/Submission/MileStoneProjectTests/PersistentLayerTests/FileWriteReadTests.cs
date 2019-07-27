/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using MileStoneClient.LogicLayer;
using System.IO;

namespace MileStoneProjectTests.PersistentLayerTests
{
    /// <summary>
    /// All File Write and Read class unit tests.
    /// </summary>
    [TestFixture]
    class FileWriteReadTests
    {
        private string PATH; // Path to the tested file read and written to.

        public FileWriteReadTests()
        {
            string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            PATH = dir + @"\Test Files\Test.bin"; //File location path.
        }

        /// <summary>
        /// Tests File writes and reads correctly from file.
        /// </summary>
        [Test]
        public void CheckFileWriteRead_WriteRead_ReturnTrue()
        {

            List<User> l = new List<User>();
            for (int i = 0; i <= 50; i++)
            {

                User u = new User("test" + i, "test" + i, i.ToString());
                l.Add(u);
            }

            FileWriteRead<User> f = new FileWriteRead<User>();
            f.WriteToFile(PATH, l);
            List<User> l2 = f.ReadFromFile(PATH);
            Assert.AreEqual(l,l2);

            }

        /// <summary>
        /// Tests file deletion.
        /// </summary>
        [Test]
        public void CheckFileWriteRead_Delete_ReturnTrue()
        {
            CheckFileWriteRead_WriteRead_ReturnTrue();

            FileWriteRead<User> f = new FileWriteRead<User>();
            bool ans = f.DeleteFile(PATH);
            Assert.IsTrue(ans);
        }

    }
}
   
