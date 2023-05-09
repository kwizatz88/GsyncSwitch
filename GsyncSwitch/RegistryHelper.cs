using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsyncSwitch
{
    public static class RegistryHelper
    {
        public const string REG_KEY = @"SOFTWARE\GsyncSwitch";
        public static object GetRegistryValue(RegistryKey baseKey, string subKey, string valueName, object defaultValue = null)
        {
            using (var key = baseKey.OpenSubKey(subKey))
            {
                if (key == null)
                {
                    return defaultValue;
                }
                else
                {
                    return key.GetValue(valueName, defaultValue);
                }
            }
        }

        public static void SetRegistryValue(RegistryKey baseKey, string subKey, string valueName, object value)
        {
            using (var key = baseKey.CreateSubKey(subKey))
            {
                key.SetValue(valueName, value);
            }
        }

        public static bool GetBoolValue(RegistryKey baseKey, string subKey, string valueName, bool defaultValue = false)
        {
            var value = GetRegistryValue(baseKey, subKey, valueName, defaultValue);
            if (value != null && int.TryParse(value.ToString(), out int intValue))
            {
                return intValue != 0;
            }
            return defaultValue;
        }

        public static void SetBoolValue(RegistryKey baseKey, string subKey, string valueName, bool value)
        {
            SetRegistryValue(baseKey, subKey, valueName, value ? 1 : 0);
        }

        public static string GetStringValue(RegistryKey baseKey, string subKey, string valueName, string defaultValue = "")
        {
            var value = GetRegistryValue(baseKey, subKey, valueName, defaultValue);
            return value is string stringValue ? stringValue : defaultValue;
        }

        public static void SetStringValue(RegistryKey baseKey, string subKey, string valueName, string value)
        {
            SetRegistryValue(baseKey, subKey, valueName, value);
        }

        public static int GetIntValue(RegistryKey baseKey, string subKey, string valueName, int defaultValue = 0)
        {
            var value = GetRegistryValue(baseKey, subKey, valueName, defaultValue);
            return value is int intValue ? intValue : defaultValue;
        }

        public static void SetIntValue(RegistryKey baseKey, string subKey, string valueName, int value)
        {
            SetRegistryValue(baseKey, subKey, valueName, value);
        }

        public static Dictionary<string, string> GetDictionary(RegistryKey rootKey, string subKey, string valueName)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            using (RegistryKey key = rootKey.OpenSubKey(subKey))
            {
                if (key != null)
                {
                    byte[] bytes = key.GetValue(valueName) as byte[];
                    if (bytes != null)
                    {
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                while (ms.Position < ms.Length)
                                {
                                    string keyName = reader.ReadString();
                                    string keyValue = reader.ReadString();
                                    dict[keyName] = keyValue;
                                }
                            }
                        }
                    }
                }
            }

            return dict;
        }

        public static void SetDictionary(RegistryKey rootKey, string subKey, string valueName, Dictionary<string, string> dict)
        {
            using (RegistryKey key = rootKey.CreateSubKey(subKey))
            {
                if (key != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(ms))
                        {
                            foreach (KeyValuePair<string, string> pair in dict)
                            {
                                writer.Write(pair.Key);
                                writer.Write(pair.Value);
                            }
                        }

                        key.SetValue(valueName, ms.ToArray(), RegistryValueKind.Binary);
                    }
                }
            }
        }

        public static bool RegistryKeyExists(RegistryKey baseKey, string subKey)
        {
            using (var key = baseKey.OpenSubKey(subKey))
            {
                return key != null;
            }
        }

        public static void ClearRegistryValues(RegistryKey baseKey, string subKey)
        {
            /*
            using (var key = baseKey.OpenSubKey(subKey, true))
            {
                if (key != null)
                {
                    foreach (var valueName in key.GetValueNames())
                    {
                        key.DeleteValue(valueName);
                    }
                }
            }*/
            if (RegistryHelper.RegistryKeyExists(baseKey, subKey))
            {
                baseKey.DeleteSubKeyTree(subKey);
            }
        }

    }
}
