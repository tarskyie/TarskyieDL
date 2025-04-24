using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
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
        private const string SaveFilePath = "save_data.json";


        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void SaveData()
        {
            string iwadStr = string.Empty;
            foreach (string theItem in iw_lb.Items)
            {
                iwadStr = string.Join(";", theItem);
            }
            string efStr = string.Empty;
            foreach (string theItem in ef_lb.Items)
            {
                efStr = string.Join(";", theItem);
            }
            var saveData = new SaveClass
            {
                iwad = iwadStr,
                ef = efStr,
                args = aa_tb.Text,
                engine = engine_tb.Text
            };

            string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SaveFilePath, json);
        }

        private void LoadData()
        {
            if (File.Exists(SaveFilePath))
            {
                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonSerializer.Deserialize<SaveClass>(json);

                if (saveData != null)
                {
                    iw_lb.Items.Clear();
                    if (!string.IsNullOrEmpty(saveData.iwad))
                    {
                        foreach (var item in saveData.iwad.Split(';'))
                        {
                            iw_lb.Items.Add(item);
                        }
                    }

                    ef_lb.Items.Clear();
                    if (!string.IsNullOrEmpty(saveData.ef))
                    {
                        foreach (var item in saveData.ef.Split(';'))
                        {
                            ef_lb.Items.Add(item);
                        }
                    }

                    aa_tb.Text = saveData.args;
                    engine_tb.Text = saveData.engine;
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
            SaveData();
        }

        private void iw_rem_a(object sender, RoutedEventArgs e)
        {
            if(iw_lb.SelectedItem != null)
            {
                iw_lb.Items.Remove(iw_lb.SelectedItem);
                SaveData();
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
            SaveData();
        }

        private void ef_rem_a(object sender, RoutedEventArgs e)
        {
            if(ef_lb.SelectedItem != null)
            {
                ef_lb.Items.Remove(ef_lb.SelectedItem);
                SaveData();
            }
        }

        private void ef_up_a(object sender, RoutedEventArgs e)
        {
            MoveItem(-1, ef_lb);
            SaveData();
        }

        private void MoveItem(int direction, ListBox lbItems)
        {
            if (lbItems.SelectedItem == null || lbItems.SelectedIndex < 0)
                return; 

            int newIndex = lbItems.SelectedIndex + direction;

            if (newIndex < 0 || newIndex >= lbItems.Items.Count)
                return; 

            var selectedItem = lbItems.SelectedItem as string;

            lbItems.Items.Remove(selectedItem);
            lbItems.Items.Insert(newIndex, selectedItem);
            lbItems.SelectedIndex = newIndex;
        }

        private void ef_down_a(object sender, RoutedEventArgs e)
        {
            MoveItem(1, ef_lb);
            SaveData();
        }

        private void select_engine(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
            if(ofd.ShowDialog() == true)
            {
                engine_tb.Text = ofd.FileName;
            }
            SaveData();
        }

        private void engine_change(object sender, TextChangedEventArgs e)
        {
            if(tb_change_permission == true)
            {
                SaveData();
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
                string args = "-iwad \"" + iw_lb.SelectedItem.ToString() + "\"";
                
                foreach (var item in ef_lb.Items)
                {
                    args = args + " -file \"" + item.ToString()+"\"";
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
