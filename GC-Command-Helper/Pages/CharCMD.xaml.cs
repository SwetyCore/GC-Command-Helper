using Microsoft.Toolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
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
    /// CharCMD.xaml 的交互逻辑
    /// </summary>
    public partial class CharCMD : Page
    {

        CharCMDVM vm;
        public CharCMD()
        {
            InitializeComponent();
        }



        public class CharCMDVM:ObservableObject
        {
            public CharCMDVM()
            {

            }


            public string ToCommand()
            {
                var cmd = $"/givechar {ID} {Value}";

                GlobalProps.SetCMD(cmd);
                return cmd;
            }

            private string _id;

            public string ID
            {
                get { return _id; }
                set { SetProperty(ref _id, value); ToCommand(); }
            }

            private double _value;

            public double Value
            {
                get { return _value; }
                set { SetProperty(ref _value, value);ToCommand(); }
            }




            private ObservableCollection<GlobalProps.SimpleItem> _items;

            public ObservableCollection<GlobalProps.SimpleItem> Items
            {
                get { return _items; }
                set { SetProperty(ref _items, value); }
            }



        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            vm = new CharCMDVM { Value=1 };

            this.DataContext = vm;

            LoadDataAsync();


        }


        public class Item
        {
            public string Name { get; set; }
            public string ArgName { get; set; }
            public bool Percent { get; set; }
            public string Tip { get; set; }
        }

        public async Task LoadDataAsync()
        {
            var file = @"Resources\zh-cn\Avatar.txt";
            var lines = File.ReadAllLines(file);
            vm.Items = new ObservableCollection<GlobalProps.SimpleItem>();

            foreach (var line in lines)
            {
                var item = line.Split(':');
                vm.Items.Add(new GlobalProps.SimpleItem { Name = item[1], Id = item[0] });
            }




        }
    }
}
