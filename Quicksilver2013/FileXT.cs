using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Praxis.Emulator
{
    class FileXT
    {
        private static string[] exts = ".txt".Split(' ');
        private static string[] programs = ".".Split(' ');
        public static void file(string path)
        {
            string extn = path.Substring(path.LastIndexOf('.'));
            string programname = "";
            int i = 0;
            foreach (string s in exts)
            {
                if (s == extn) programname = programs[i];
                i++;
            }
        }
    }
}
