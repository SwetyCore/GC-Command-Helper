using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWidgetDesktop.Utils
{
    public static class SettingProvider
    {

        private static string cfgPath = @"./Config/cfg.json";

        private static JObject _globalSetting;

        public static JObject GlobalSetting
        {
            get { return _globalSetting;}
            set { _globalSetting = value; }
        }
        private static void Save(string s)
        {
            File.WriteAllText(cfgPath, s);
        }
        public static void Save()
        {
            Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static void Init()
        {
            if (!File.Exists(cfgPath))
            {
                Directory.CreateDirectory(cfgPath.Replace("cfg.json",""));
                Save("");
            }
            try
            {
                GlobalSetting= JObject.Parse( File.ReadAllText(cfgPath));

            }
            catch (Exception ex)
            {
                GlobalSetting = new JObject();
            }
        }
        public static void Set(string id,object obj)
        {
            GlobalSetting[id]= JsonConvert.SerializeObject( obj);
            Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static void SetNoSave(string id, object obj)
        {
            GlobalSetting[id] = JsonConvert.SerializeObject(obj);
            //Save(JsonConvert.SerializeObject(GlobalSetting));
        }
        public static string Get(string id)
        {
            try
            {
                if (GlobalSetting == null)
                {
                    return "";
                }
                return GlobalSetting[id].ToString();

            }
            catch
            {
                return "";
            }
        }

    }
}
