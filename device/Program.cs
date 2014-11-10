using ICSharpCode.USBlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace device
{
    class Program
    {
       

        static void Main(string[] args)
        {
            foreach (Bus bus in Bus.Busses)
            {
                Console.WriteLine(bus);
                foreach (Descriptor descriptor in bus.Descriptors)
                {
                    Console.WriteLine("\t" + descriptor);
                    try
                    {
                        using (Device device = descriptor.OpenDevice())
                        {
                            Console.WriteLine("\t\t     Product: " + device.Product);
                            Console.WriteLine("\t\tManufacturer: " + device.Manufacturer);
                            Console.WriteLine();
                        }
                    }
                    catch (UsbException e)
                    {
                        Console.WriteLine("Got Exception : " + e);
                    }
                }
            }
            Console.Read();
          
        }
       
    }
    
}
