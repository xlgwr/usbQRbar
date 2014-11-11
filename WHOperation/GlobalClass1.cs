using System;
using System.Collections.Generic;
using System.Text;

namespace WHOperation
{
    static class GlobalClass1
    {
       private static String cUserID, cSystemID;
        public static string userID
        {
            get { return cUserID; }
            set { cUserID = value; }
        }
        public static string systemID
        {
            get { return cSystemID; }
            set { cSystemID = value; }
        }
    }
}
