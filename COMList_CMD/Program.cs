using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO;
using System.IO.Ports;

namespace COMList
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                {
                    var portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                    var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                    Console.WriteLine("COM ports found:\n");
                    foreach (string s2 in portList)
                    {
                        Console.WriteLine(s2);
                    }
                }

                Console.Write("\nEnter to try again! ");
                Console.Write("Any other key to exit! ");
                //Console.ReadLine();
                ConsoleKeyInfo info = Console.ReadKey();
                if (info.KeyChar != 0x0d)
                {
                    break;
                }
                Console.WriteLine("\n");
            }
        }
    }
}
