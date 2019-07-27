/*
 * Authors: Team 18
 * Itay Bouganim, ID: 305278384
 * Sahar Vaya, ID: 205583453
 * Dayan Badalbaev, ID: 209222215
 */
using System;
using System.Diagnostics;
using log4net;
using log4net.Config;

namespace MileStoneClient.PersistentLayer
{
    //Used to capture and record all system events.
    public class EventLogger
    {
        //Fields
        private static readonly ILog _logger = LogManager.GetLogger(typeof(EventLogger)); //Log4net logger
        private StackFrame _callStack; //A callStack to record line number and file to log.

        //Methods
        /// <summary>
        /// Starts callStack and loads logger xml configurations from log4net.config
        /// </summary>
        public void Initiate()
        {
            _callStack = new StackFrame(1, true);
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// A helper method to log events to SystemFiles/Log/EventLog.log with a relevant callStack association.
        /// </summary>
        /// <param name="severity">An integer represents type of logging activity</param>
        /// <param name="toLog">A string to log</param>
        public void log (int severity, String toLog)
        {
            switch (severity)
            {
                case 1: _logger.Debug(toLog + "\r\nOccurred in file: " + callStack.GetFileName() + " | Line: " + callStack.GetFileLineNumber()); //Debug log
                    break;
                case 2: _logger.Info(toLog + "\r\nOccurred in file: " + callStack.GetFileName() + " | Line: " + callStack.GetFileLineNumber()); //Information log
                    break;
                case 3: _logger.Error(toLog + "\r\nOccurred in file: " + callStack.GetFileName() + " | Line: " + callStack.GetFileLineNumber()); //Error log
                    break;
                case 4: _logger.Warn(toLog + "\r\nOccurred in file: " + callStack.GetFileName() + " | Line: " + callStack.GetFileLineNumber()); //Warning log
                    break;
                case 5: _logger.Fatal(toLog + "\r\nOccurred in file: " + callStack.GetFileName() + " | Line: " + callStack.GetFileLineNumber()); //Fatal Error log
                    break;
            }
        }

        //Getters and Setters
        public StackFrame callStack
        {
            get { return _callStack; }
        }
    }
}
