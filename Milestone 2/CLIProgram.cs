/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileStoneClient.PresentationLayer;
using MileStoneClient.LogicLayer;

namespace MileStoneClient
{
    class Program
    {
        public static void Main (String[] args)
        {
            Launch();
        }

        /// <summary>
        /// Initating the ChatRoom and GUI and start running the program.
        /// </summary>
        private static void Launch()
        {
            String chatRoomStatusMessage = ChatRoom.Initiate();
            GUI.Initiate(chatRoomStatusMessage);
        }
    }
}
