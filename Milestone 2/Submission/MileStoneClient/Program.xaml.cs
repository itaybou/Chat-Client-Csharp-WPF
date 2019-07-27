using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MileStoneClient.LogicLayer;
using MileStoneClient.PresentationLayer;

namespace MileStoneClient
{
    /// <summary>
    /// Initiates program main components.
    /// </summary>
    public partial class Program : Application
    {
        private void Program_Startup(object sender, StartupEventArgs e)
        {
            Launch();
        }

        /// <summary>
        /// Launches Chat-Room logical layer and GUI components.
        /// </summary>
        private static void Launch()
        {
            String chatRoomStatusMessage = ChatRoom.Initiate();
            GUI.Initiate(chatRoomStatusMessage);
        }
    }
}
