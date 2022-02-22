using FVMServerCore.Core;
using FVMServerCore.DataCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FVMServerCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public static RichTextBox Ctl_Console_Out;
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;    // 单例
            Console_Out.Document.Blocks.Clear();
            Console_Associate.Content = "";
            Ctl_Console_Out = Console_Out;
            List<string> list = new List<string> { "终端", "用户数据", "设置", "发送邮件" };
            Debug.Instance.Log("连接到数据库....");
            FVMData.Init();
            Debug.Instance.Log("连接完毕");
            
            Debug.Instance.Log("初始化界面....");
            for (int i = 0; i < list.Count; i++)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = list[i],
                    Height = 45,
                    Style = (Style)FindResource("FileListBoxItem"),
                    Margin = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    TabIndex = i,
                };
                item.MouseEnter += Item_MouseEnter;
                item.MouseLeave += Item_MouseLeave;
                item.Selected += Item_Selected;
                LeftList.Items.Add(item);
            }
            LeftList.SelectedIndex = 0;
            Debug.Instance.Log("初始化界面完毕");
            Command.Instance.Init(ref Console_Out);
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            RightTabControl.SelectedIndex = ((ListBoxItem)e.Source).TabIndex;
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!((ListBoxItem)sender).IsSelected)
                ((ListBoxItem)sender).BeginStoryboard((Storyboard)this.FindResource("LeftList_MouseLeave"));
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!((ListBoxItem)sender).IsSelected)
                ((ListBoxItem)sender).BeginStoryboard((Storyboard)this.FindResource("LeftList_MouseEnter"));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Console_In_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Debug.Instance.Usr(((TextBox)sender).Text);
                Command.Instance.CommandEnter(Console_Out, ((TextBox)sender).Text);
                ((TextBox)sender).Text = "";
            }
            if (e.Key == Key.Tab)
            {
                if (Console_Associate.Content.ToString() != "")
                {
                    Console_In.Text = Console_Associate.Content.ToString();
                    Console_In.SelectionStart = Console_In.Text.Length;
                    e.Handled = true;
                }
            }
        }

        private void Console_In_TextChanged(object sender, TextChangedEventArgs e)
        {
            string temp = ((TextBox)sender).Text;
            string[] tempList = temp.Split(' ');

            Console_Associate.Content = (temp.IndexOf(' ') == -1 ? "" : temp.Substring(0, temp.LastIndexOf(' ') + 1)) + Command.Instance.GetCommand(tempList.Length > 0 ? tempList[tempList.Length - 1] : "") ?? "";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 获取用户个数
            
        }
    }
}
