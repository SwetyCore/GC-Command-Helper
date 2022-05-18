using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GC_Command_Helper.Pages
{
    /// <summary>
    /// CustomCMD.xaml 的交互逻辑
    /// </summary>
    public partial class CustomCMD : Page
    {
        CustomCMDVM vm;
        public CustomCMD()
        {
            InitializeComponent();
        }







        public class item
        {
            public string Desp { get; set; }
            public string cmd { get; set; }
        }

        public class CustomCMDVM:ObservableObject
        {
            public ICommand SetCmdCMD { get; set; }


            public CustomCMDVM()
            {
                SetCmdCMD = new RelayCommand<string>((string cmd) =>
                  {
                      GlobalProps.SetCMD(cmd);
  
                  });
            }


            private ObservableCollection<item> _commands;

            public ObservableCollection<item> Commands
            {
                get { return _commands; }
                set { SetProperty(ref _commands, value); }
            }

        }

        private void LoadDataAsync()
        {
            var file = @"Resources\zh-cn\CustomCommands.txt";
            var lines = File.ReadAllLines(file);

            for (int i = 0; i < lines.Count(); i++)
            {
                vm.Commands.Add(new item { Desp = lines[i], cmd = lines[++i] });
            }

        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            vm = new CustomCMDVM();
            vm.Commands = new ObservableCollection<item>();
            //vm.Commands.Add(new item { Desp = "/giveall", cmd = "我全都要" });


            this.DataContext = vm;

            LoadDataAsync();
        }
    }
}
