using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ChangeStreamDateApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean dateSet = true;
        Boolean fileSet = false;
        Boolean ready = false;

        string filePath;
        string safeFilePath;
        public MainWindow()
        {
            InitializeComponent();
            DatePicked.DefaultValue = DateTime.Now.AddMinutes(30);
            dateDefaultInfo.Visibility = Visibility.Visible;
        }

        private void DatePicked_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DatePicked.Value < DateTime.Now)
            {
                string box_msg = "This is an invalid choice as it is before the current time";
                string box_title = "Error";
                MessageBox.Show(box_msg, box_title, MessageBoxButton.OK, MessageBoxImage.Error);
                dateSet = false;
            }
            else
            {
                dateSet = true;
            }
            statusCheck();
        }

        private void statusCheck()
        {
            if(dateSet && fileSet)
            {
                ready = true;
                updateButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDFFFFFF"));
                updateButton.Cursor = Cursors.Hand;
                status.Source = new BitmapImage(new Uri(@"green.png", UriKind.Relative));
            } else
            {
                ready = false;
                updateButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#60FFFFFF"));
                updateButton.Cursor = Cursors.No;
                status.Source = new BitmapImage(new Uri(@"red.png", UriKind.Relative));
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ready)
            {
                try
                {
                    DateTime selectedDateTime = (DateTime)DatePicked.Value;
                    string[] arrLine = File.ReadAllLines(filePath);
                    arrLine[18 - 1] = "		var countDownDate = new Date(\"" + selectedDateTime.ToString("MMMM") + " " + selectedDateTime.Day + ", " + selectedDateTime.Year
                        + " " + selectedDateTime.Hour + ":" + selectedDateTime.Minute + ":" + selectedDateTime.Second + "\").getTime();";
                    File.WriteAllLines(filePath, arrLine);

                    string box_msg = "Go Live time successfully updated! Program will now close!";
                    string box_title = "Successful";
                    MessageBox.Show(box_msg, box_title, MessageBoxButton.OK, MessageBoxImage.Information);

                    Environment.Exit(0);
                } catch(Exception Ex)
                {
                    string box_msg = "There was a problem saving the data. Please ensure that you have selected the correct file and try again.";
                    string box_title = "Error";
                    MessageBox.Show(box_msg, box_title, MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Web Pages|*.html" }; ;

            if (ofd.ShowDialog() == true)
            {
                filePath = ofd.FileName;
                safeFilePath = ofd.SafeFileName;

                string line1 = File.ReadLines(filePath).First();

                if (!(line1.Equals("<!--51d4a78f-8943-434a-91f9-dc48cd5c8543-->"))){
                    string box_msg = "This is not the Stream Timer file";
                    string box_title = "Error";
                    MessageBox.Show(box_msg, box_title, MessageBoxButton.OK, MessageBoxImage.Error);

                    filePath = "";
                    safeFilePath = "";

                    path.Content = "No file selected";

                    fileSet = false;
                }
                else
                {
                    path.Content = filePath;
                    fileSet = true;
                }
            }

            statusCheck();
        }
    }
}
