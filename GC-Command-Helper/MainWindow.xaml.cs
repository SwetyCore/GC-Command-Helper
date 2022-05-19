using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;

namespace GC_Command_Helper
{



    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(Keys vKey);
        [DllImport("user32.dll")]
        public static extern bool GetKeyState(Keys vKey);
        private const int GWL_HWNDPARENT = -8;
        [DllImport("user32.dll", EntryPoint = "SetWindowLongA")]
        private static extern int SetWindowLong(int hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "FindWindowExA")]
        private static extern int FindWindowEx(int hWndParent, int hWndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", EntryPoint = "FindWindowA")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        //public class WindowWrapper : System.Windows.Forms.IWin32Window
        //{
        //    public WindowWrapper(IntPtr handle)
        //    {
        //        _hwnd = handle;
        //    }

        //    public IntPtr Handle
        //    {
        //        get { return _hwnd; }
        //    }

        //    private IntPtr _hwnd;


        //}



        private MainVM vm;
        private int ysHandle = 0x00050DC0;

        public MainWindow()
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            InitializeComponent();

            GlobalProps.SetCMD = SetCmd;



            CheckKeyAsync();
        }

        public async Task CheckKeyAsync()
        {
            while (true)
            {
                var r = GetAsyncKeyState(Keys.T);
                int focus =  GetForegroundWindow();
                //Console.WriteLine(focus);
                

                if (r)
                {
                    if (focus == ysHandle)
                    {

                        vm.IsShow = true;
                        await Task.Delay(1000);
                    }




                }
                else
                {
                    await Task.Delay(100);
                }
            }
        }

        public void SetCmd(string cmd)
        {
            vm.Command = cmd;
        }

        static void RegisterMyProtocol()  //myAppPath = full path to your application
        {
            var myAppPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // RegistryKey key = Registry.ClassesRoot.OpenSubKey("gccomh");  //open myApp protocol's subkey
            var KeyTest = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
            RegistryKey key = KeyTest.CreateSubKey("gccomh");
            key.SetValue(string.Empty, "URL: GC-Command-Helper Protocol");
            key.SetValue("URL Protocol", string.Empty);

            key = key.CreateSubKey(@"shell\open\command");
            key.SetValue(string.Empty, myAppPath + " " + "%1");
                //%1 represents the argument - this tells windows to open this program with an argument / parameter
            key.Close();
        }



        private static int FindYS()
        {
            return FindWindow("UnityWndClass", null);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegisterMyProtocol();
                
                ysHandle = FindYS();

                if (ysHandle == 0)
                {
                    throw new Exception();
                }

                //SetParent(new WindowInteropHelper(this).Handle, );
                var myhandle = new WindowInteropHelper(this).Handle;

                SetWindowLong((int)myhandle, (-20), 0x80);

                SetWindowLong((int)myhandle, GWL_HWNDPARENT,ysHandle );
                SetWindowPos(myhandle, new IntPtr(0), 0, 0, 0, 0, 1 | 2);


            }
            catch (Exception ex)
            {
                MessageBox.Show("未找到原神进程，程序将以普通窗口方式运行！");
                //Topmost = true;
            }



            vm = new MainVM(this);
            this.DataContext = vm;
        }

        public async Task CheckServer()
        {

            API.CommandApi.RespDT r = await API.CommandApi.Ping();

            if (r.message == "success")
            {
                vm.CheckServer = true;
            }
            else
            {
                vm.CheckServer = false;
            }
        }


        public class MainVM : ObservableObject
        {
            private bool checkserver;

            public ICommand showCMD { get; private set; }
            public ICommand SendCodeCMD { get; private set; }
            public ICommand ConnectCMD { get; private set; }
            public ICommand SendCmdCMD { get; private set; }

            public MainWindow wd;
            public MainVM(MainWindow wd)
            {
                IsShow = false;
                IsSupport = false;
                IP = "127.0.0.1:25565";
                UID = 10003;
                this.wd = wd;

                showCMD = new RelayCommand(() =>
                  {



                      this.IsShow = true;


                  });

                SendCodeCMD = new AsyncRelayCommand(SendCodeAsync);

                ConnectCMD = new AsyncRelayCommand(async () => 
                {
                    var r = await API.CommandApi.Verify(this.Code);
                    if (r.message == "success")
                    {
                        this.Statue = true;
                    }
                    else
                    {
                        MessageBox.Show(r.message);
                    }
                });

                SendCmdCMD = new AsyncRelayCommand(SendCmdAsync);

                if (GlobalProps.MojoServer != null && GlobalProps.MojoServer != "")
                {
                    this.Statue = true;
                }
            }

            public async Task SendCmdAsync()
            {
                
                if (GlobalProps.MojoServer != null && GlobalProps.MojoServer != "")
                {
                    var r = await API.MojoApi.Command(Command);
                    ShowMsg(r.payload);
                } else
                {
                    var r = await API.CommandApi.Command(Command);
                    ShowMsg(r.data);
                }


                if (AutoClose)
                {
                    IsShow = false;

                }
                //MessageBox.Show(r.message);
            }

            public async Task ShowMsg(object msg)
            {
                this.Msg = msg.ToString();
                await Task.Delay(2000);

                this.Msg = "";
            }


            private string _msg;

            public string Msg
            {
                get { return _msg; }
                set { SetProperty(ref _msg, value); }
            }


            public async Task SendCodeAsync()
            {
                {
                    API.CommandApi.uid = UID;

                    var r = await API.CommandApi.SendCode();

                    if (r.message == "success")
                    {
                        IsSupport = true;
                    }
                    else
                    {
                        MessageBox.Show(r.message);
                    }

                }
            }


            private bool autoClose;

            public bool AutoClose
            {
                get { return autoClose; }
                set { SetProperty(ref autoClose, value); }
            }



            private string command;

            public string Command
            {
                get { return command; }
                set { SetProperty(ref command, value); }
            }

            public bool CheckServer
            {
                get { return checkserver; }
                set { SetProperty(ref checkserver, value); }
            }

            private bool isSupport;

            public bool IsSupport
            {
                get { return isSupport; }
                set { SetProperty(ref isSupport, value); }
            }

            private bool isShow;

            public bool IsShow
            {
                get { return isShow; }
                set { 
                    SetProperty(ref isShow, value);

                    if (wd == null)
                    {
                        return;
                    }
                    if (value)
                    {
                        wd.Background = new SolidColorBrush(Color.FromArgb(100, 250, 250, 250));
                        wd.Activate();
                        if (Statue)
                        {
                            wd.cmdtb.Focus();
                        }
                    }
                    else
                    {
                        wd.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                        // 主动失去焦点或者让ys获得焦点
                    }

                }
            }

            private bool statue;

            public bool Statue
            {
                get { return statue; }
                set { SetProperty(ref statue, value); }
            }

            private string ip;

            public string IP
            {
                get { return ip; }
                set { SetProperty(ref ip, value); }
            }

            private string code;

            public string Code
            {
                get { return code; }
                set { SetProperty(ref code, value); }
            }

            private int uid;

            public int UID
            {
                get { return uid; }
                set { SetProperty(ref uid, value); }
            }

        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                vm.IsShow = false;
                this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            }
        }

        private void cmdtb_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                vm.SendCmdAsync();

            }
        }
    }
}
