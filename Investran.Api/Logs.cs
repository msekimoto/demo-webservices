using log4net;
using log4net.Repository.Hierarchy;
using System;

namespace Investran.Api
{
    public static class Logs
    {
        private static ILog Log { get; set; }

        static Logs()
        {
            Log = LogManager.GetLogger(typeof(Logger));
        }

        public static void Error(object msg)
        {
            Log.Error(msg);
        }

        public static void Error(Exception ex)
        {
            Log.Error(ex.Message);
        }

        public static void Info(object msg)
        {
            Log.Info(msg);
        }
    }
}
