using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace TDL
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// это ваще полный ДУМ
    public partial class MainWindow : Window
    {
        //ну тут тип дальше надо вот
        int i = 0;
        bool tb_change_permission = false;
        public MainWindow()
        {
            InitializeComponent();
            lb_load("iw_save.txt", iw_lb);
            lb_load("ef_save.txt", ef_lb);
            tb_load("en_save.txt", engine_tb);
        }
        //функция сохранения листбокса в тхт жоска "дру"
        private void lb_save(string filename, ListBox lbname)
        {
            using(StreamWriter writer = new StreamWriter(filename))
            {
                foreach (var item in lbname.Items)
                {
                    writer.WriteLine(item.ToString());
                }
            }
        }
        
        private void lb_load(string filename, ListBox lbname)
        {
            if (File.Exists(filename))
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lbname.Items.Add(line);
                    }
                }
            }
        }

        private void tb_save(string filename, TextBox tbname)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(tbname.Text);
            }
        }

        private void tb_load(string filename, TextBox tbname)
        {
            if (File.Exists(filename))
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    tbname.Text = reader.ReadLine();
                }
            }
        }

        private void iw_add_a(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WAD/PK3/ZIP (*.wad *.pk3 *.zip)|*.wad;*.pk3;*.zip|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                iw_lb.Items.Add(ofd.FileName);
            }
            lb_save("iw_save.txt", iw_lb);
            iw_lb.Items.Clear();
            lb_load("iw_save.txt", iw_lb);
        }

        private void iw_rem_a(object sender, RoutedEventArgs e)
        {
            if(iw_lb.SelectedItem != null)
            {
                iw_lb.Items.Remove(iw_lb.SelectedItem);
                lb_save("iw_save.txt", iw_lb);
                iw_lb.Items.Clear();
                lb_load("iw_save.txt", iw_lb);
            }
        }

        private void ef_add_a(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "WAD/PK3/ZIP/PK7/PKZ/P7Z (*.wad *.pk3 *.zip *.pk7 *.pkz *.p7z)|*.wad;*.pk3;*.zip;*pk7;*.pkz;*p7z|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == true)
            {
                ef_lb.Items.Add(ofd.FileName);
            }
            lb_save("ef_save.txt", ef_lb);
            ef_lb.Items.Clear();
            lb_load("ef_save.txt", ef_lb);
        }

        private void ef_rem_a(object sender, RoutedEventArgs e)
        {
            if(ef_lb.SelectedItem != null)
            {
                ef_lb.Items.Remove(ef_lb.SelectedItem);
                lb_save("ef_save.txt", ef_lb);
                ef_lb.Items.Clear();
                lb_load("ef_save.txt", ef_lb);
            }
        }

        private void ef_up_a(object sender, RoutedEventArgs e)
        {
            MoveItem(-1, ef_lb);
            lb_save("ef_save.txt", ef_lb);
        }

        private void MoveItem(int direction, ListBox lbItems)
        {
            // Checking selected item
            if (lbItems.SelectedItem == null || lbItems.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = lbItems.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= lbItems.Items.Count)
                return; // Index out of range - nothing to do

            //var items = lbItems.ItemsSource as ObservableCollection<string>;
            var selectedItem = lbItems.SelectedItem as string;

            // Removing removable element
            lbItems.Items.Remove(selectedItem);
            // Insert it in new position
            lbItems.Items.Insert(newIndex, selectedItem);
            // Restore selection
            lbItems.SelectedIndex = newIndex;
        }

        private void ef_down_a(object sender, RoutedEventArgs e)
        {
            MoveItem(1, ef_lb);
            lb_save("ef_save.txt", ef_lb);
        }

        private void select_engine(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if(ofd.ShowDialog() == true)
            {
                engine_tb.Text = ofd.FileName;
            }
            tb_save("en_save.txt", engine_tb);
        }

        private void engine_change(object sender, TextChangedEventArgs e)
        {
            if(tb_change_permission == true)
            {
                tb_save("en_save.txt", engine_tb);
            }
            else 
            { 
                tb_change_permission = true;
            }
        }

        public void LaunchExecutable(string exePath, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = System.IO.Path.GetDirectoryName(exePath)
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Handle output and error if needed
                Debug.WriteLine(output);
                Debug.WriteLine(error);
            }
        }

        private void run_a(object sender, RoutedEventArgs e)
        {
            string exePath = engine_tb.Text;
            if (iw_lb.SelectedItem != null)
            {
                string args = "-iwad "+iw_lb.SelectedItem.ToString();
                
                foreach (var item in ef_lb.Items)
                {
                    args = args + " -file " + item.ToString();
                }
                if (aa_tb.Text != null)
                {
                    args = args + " " + aa_tb.Text;
                }
                LaunchExecutable(exePath, args);
            }
        }

        private void settings_a(object sender, RoutedEventArgs e)
        {
            settings tdl_sets = new settings();
            tdl_sets.Show();
        }
    }
}
