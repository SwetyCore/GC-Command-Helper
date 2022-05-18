using Microsoft.Toolkit.Mvvm.ComponentModel;
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
    /// SpawnCMD.xaml 的交互逻辑
    /// </summary>
    public partial class SpawnCMD : Page
    {
        VM vm;

        public SpawnCMD()
        {
            InitializeComponent();
        }





        public class VM:ObservableObject
        {

            private ObservableCollection<GlobalProps.SimpleItem> _monsters;

            public ObservableCollection<GlobalProps.SimpleItem> Monsters
            {
                get { return _monsters; }
                set { SetProperty(ref _monsters, value); }
            }

            public VM()
            {
                
            }

            public string ToCommand()
            {
                var cmd = $"/spawn {Id} {Num} {Level}";

                GlobalProps.SetCMD(cmd);
                return cmd;
            }

            private string id;

            public string Id
            {
                get { return id; }
                set { SetProperty(ref id, value); ToCommand(); }
            }

            private double _num;

            public double Num
            {
                get { return _num; }
                set {SetProperty(ref _num, value);  ToCommand(); }
            }
            private double _level;

            public double Level
            {
                get { return _level; }
                set { SetProperty(ref _level, value); ToCommand(); }
            }

            private int _selectIndex;

            public int selectIndex
            {
                get { return _selectIndex; }
                set { SetProperty(ref _selectIndex, value); ToCommand(); }
            }




        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            vm = new VM();
            vm.Num = 1;
            vm.Level = 1;
            this.DataContext = vm;




            LoadDataAsync();
            
        }

        private void LoadDataAsync()
        {

            vm.Monsters = new ObservableCollection<GlobalProps.SimpleItem>();
            var file = @"Resources\zh-cn\Monster.txt";
            var lines = File.ReadAllLines(file);

            foreach (var line in lines)
            {

                var item = line.Split(':');
                vm.Monsters.Add(new GlobalProps.SimpleItem { Name = item[1], Id = item[0] });
            }

        }
    }
}
