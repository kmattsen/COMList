using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Management;
using System.IO.Ports;
using System.Timers;
using System.Windows.Input;


namespace COMList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void UpdateTextCallback();
        public Timer mTimer;

        public MainWindow()
        {
            InitializeComponent();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var commports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                var portList = portnames.Select(n => n + " - " + commports.FirstOrDefault(s => s.Contains(n))).ToList();

                List<CommPort> displayList = new List<CommPort>();
                for(int i=0; i < portList.Count; i++)
                {
                    var commnumber = int.Parse(portList[i].Split('M', ' ')[1]);
                    displayList.Add(new CommPort() { Title = portList[i], CommNumber = commnumber });
                }

#if false
                for (int i = 10; i < 25; i++)
                {
                    string var = "COM" + i.ToString();
                    displayList.Add(new CommPort() { Title = var, CommNumber = i });
                }
#endif
                List<CommPort> sortedDisplayList = displayList.OrderBy(o => o.CommNumber).ToList();

                commsItemsControl.ItemsSource = sortedDisplayList;
            }



        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            List<CommPort> displayList = new List<CommPort>();
            commsItemsControl.ItemsSource = displayList;

            mTimer = new Timer(300);
            mTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            mTimer.Enabled = true;
            mTimer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            mTimer.Stop();
            mTimer = null;
            commsItemsControl.Dispatcher.Invoke(
                new UpdateTextCallback(this.Update)
            );
        }

        private void Update()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var commports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());
                var portList = portnames.Select(n => n + " - " + commports.FirstOrDefault(s => s.Contains(n))).ToList();

                List<CommPort> displayList = new List<CommPort>();

                for (int i = 0; i < portList.Count; i++)
                {
                    var commnumber = int.Parse(portList[i].Split('M', ' ')[1]);
                    displayList.Add(new CommPort() { Title = portList[i], CommNumber = commnumber });

                }

#if false
                for (int i = 5; i < 20; i++)
                {
                    string var = "COM" + i.ToString();
                    displayList.Add(new CommPort() { Title = var, CommNumber = i });
                }
#endif

                List<CommPort> sortedDisplayList = displayList.OrderBy(o => o.CommNumber).ToList();

                commsItemsControl.ItemsSource = sortedDisplayList;
            }
        }

        public void keyDownEventHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Close();
            }
            else if (e.Key == Key.Space)
            {
                ScanButton_Click(null, null);
            }
            else if (e.Key == Key.Escape)
            {
                Close();
            }

        }

        public void keyUpEventHandler(object sender, KeyEventArgs e)
        {
        }

    }

    public class CommPort
    {
        public string Title { get; set; }
        public int CommNumber { get; set; }
    }
}

