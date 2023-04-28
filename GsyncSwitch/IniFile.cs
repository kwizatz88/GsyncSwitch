using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace GsyncSwitch
{
    class IniFile
    {
        private string filePath;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileSection(string section,
            byte[] retVal, int size, string filePath);

        public IniFile(string filePath)
        {
            this.filePath = filePath;
        }

        public string Read(string section, string key)
        {
            StringBuilder retVal = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", retVal, 255, filePath);
            return retVal.ToString();
        }
        public string[] GetKeys(string section)
        {
            byte[] buffer = new byte[2048];
            int length = GetPrivateProfileSection(section, buffer, buffer.Length, filePath);
            string keysString = System.Text.Encoding.ASCII.GetString(buffer, 0, length);
            string[] keys = keysString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[0];
            }

            return keys;
        }
    }
}
