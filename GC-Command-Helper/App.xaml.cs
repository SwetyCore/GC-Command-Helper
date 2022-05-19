using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace GC_Command_Helper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private class MojoParam
        {
            public string d = "";
            public string k2 = "";
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            if (e.Args.Length == 1)
            { 
                Uri arg_url;
                var arg = Uri.TryCreate(e.Args[0],UriKind.Absolute, out arg_url);
                GlobalProps.MojoServer = arg_url.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
                MojoParam mp = JsonConvert.DeserializeObject<MojoParam>(GlobalProps.MojoServer);
                API.MojoApi.token = mp.k2;
                API.MojoApi.api = mp.d + "/mojoplus/api";
            }
                
            wnd.Show();
        }
    }


}
