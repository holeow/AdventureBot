using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Helpers
{
    public static class FileManager
    {
        public static string InternalDirectoryPath
        {
            get
            {
                var dir = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\InternalFiles";
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string DatabaseDirectoryPath
        {
            get
            {
                var dir = InternalDirectoryPath + "\\Database";
                Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static ConfigFile Config = new ConfigFile(InternalDirectoryPath + "\\config.config");
        public static ConfigFile SecretConfig = new ConfigFile(InternalDirectoryPath + "\\secret.config");
        

        public class ConfigFile
        {
            public ConfigFile(string location)
            {
                 this.location = location;
                Open();
            }

            string location;

           public Dictionary<string, string> properties { get; set; }

            public void Open()
            {
                if (File.Exists(location))
                {
                    properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(location));
                }
                else properties = new Dictionary<string, string>();
                
            }

            public void Save()
            {
                var ndic = new Dictionary<string , string>();
                foreach (var key in properties.Keys)
                {
                    if (properties[key] != null)
                    {
                        ndic.Add (key, properties[key]);
                    }
                }  
                File.WriteAllText(location, JsonConvert.SerializeObject(ndic));
            }


            public string this[string s]
            {
                get
                {
                    if (properties.TryGetValue(s, out string value))
                    {
                        return value;
                    }
                    else return null;
                }
                set
                {
                    if (properties.ContainsKey(s))
                    {
                        properties[s] = value;
                    }
                    else properties.Add(s,value);
                }
            }

        }
    }
}