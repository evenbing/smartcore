using System;
using System.Collections.Generic;
using System.Text;
using NLog;
namespace SmartCore.Infrastructure
{
   public static class LogManager
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Debug(string message,params object[] args)
        {
            logger.Debug(message, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Info(string message, params object[] args)
        {
            logger.Info(message, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Warn(string message, params object[] args)
        {
            logger.Warn(message, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void Error(Exception exception,string message="")
        {
            logger.Error(exception, message);
        }
    }
}
