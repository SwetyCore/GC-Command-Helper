using DefaultWidgets.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GC_Command_Helper.API
{
    public static class CommandApi
    {

        public static Request request { get; set; }

        public static string Server { get; set; }

        public static string api { get; set; }

        public static int uid { get; set; }

        public static string token { get; set; }

        public static void EnsuerInit()
        {
            if (request == null)
            {
                request = new Request();

            }
            if (Server == null)
            {
                Server = "127.0.0.1:25565";
                api = $"https://{Server}/opencommand/api";
            }

        }

        public class ReqDT
        {
            public String token = "";
            public String action = "";
            public Object data = null;
        }

        public class RespDT
        {
            public int retcode = 200;
            public String message = "success";
            public Object data;
        }

        public static async Task<RespDT> Ping()
        {
            EnsuerInit();
            var reqdt = new ReqDT
            {
                action = "ping",
                data = null
            };

            var r=await request.PostJson(api, JsonConvert.SerializeObject(reqdt));
            RespDT resp;
            try
            {
                resp = JsonConvert.DeserializeObject<RespDT>(r);
            }
            catch
            {
                resp = new RespDT();
            }
            return resp;

        }

        public static async Task<RespDT> SendCode()
        {
            EnsuerInit();
            var reqdt = new ReqDT
            {
                action = "sendCode",
                data = uid
            };

            var r = await request.PostJson(api, JsonConvert.SerializeObject(reqdt));
            RespDT resp;
            try
            {
                resp = JsonConvert.DeserializeObject<RespDT>(r);

                if (resp.message != "success")
                {
                    MessageBox.Show(resp.message);
                }

                token = resp.data.ToString();
            }
            catch
            {
                resp = new RespDT();
            }
            return resp;

        }

        public static async Task<RespDT> Verify(string code)
        {
            EnsuerInit();
            var reqdt = new ReqDT
            {
                action = "verify",
                data = int.Parse( code),
                token = token
            };

            var r = await request.PostJson(api, JsonConvert.SerializeObject(reqdt));
            RespDT resp;
            try
            {
                resp = JsonConvert.DeserializeObject<RespDT>(r);
            }
            catch
            {
                resp = new RespDT();
            }
            return resp;

        }


        public static async Task<RespDT> Command(string command)
        {
            EnsuerInit();
            var reqdt = new ReqDT
            {
                action = "command",
                data = command.Substring(1),
                token=token
            };

            var r = await request.PostJson(api, JsonConvert.SerializeObject(reqdt));
            RespDT resp;
            try
            {
                resp = JsonConvert.DeserializeObject<RespDT>(r);
            }
            catch
            {
                resp = new RespDT();
            }
            return resp;

        }
    }
}
