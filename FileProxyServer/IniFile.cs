using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class IniFile   // revision 10
    {
        private string path;
        private string exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder returnValue, int size, string filePath);

        public IniFile(string IniPath = null)
        {
            path = new FileInfo(IniPath ?? exe + ".ini").FullName.ToString();
        }

        public string Read(string key, string section = null)
        {
            var returnValue = new StringBuilder(255);
            GetPrivateProfileString(section ?? exe, key, "", returnValue, 255, path);
            return returnValue.ToString();
        }

        public void Write(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? exe, key, value, path);
        }

        public void DeleteKey(string key, string section = null)
        {
            Write(key, null, section ?? exe);
        }

        public void DeleteSection(string section = null)
        {
            Write(null, null, section ?? exe);
        }

        public bool KeyExists(string key, string section = null)
        {
            return Read(key, section).Length > 0;
        }
    }
}
