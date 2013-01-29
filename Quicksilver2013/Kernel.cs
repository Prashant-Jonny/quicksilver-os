using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using x86 = Cosmos.Assembler.x86;
using SMBIOS = Cosmos.Hardware.SMBIOS;
namespace Quicksilver2013
{
    public class Kernel : Sys.Kernel
    {
        public bool didcommand = true;
        string topline = "";
        string commandwpar = "";
        Cosmos.Hardware.TextScreen ts = new Cosmos.Hardware.TextScreen();
        Cosmos.Hardware.Mouse mouse = new Cosmos.Hardware.Mouse();
        protected override void BeforeRun()
        {
            Console.WriteLine("");
            SMBIOS.SMBIOS smbios = new SMBIOS.SMBIOS();
            //gets smbios
            if (smbios.CheckSMBIOS())
            {
                smbios.GetSMBIOS_Data();
                List<SMBIOS.BaseInfo> bi = smbios.GetHardwareDescriptorByType( SMBIOS.TableTypes.ProcessorInformation );
                if(bi.Count >= 1) {
                    Console.ReadLine();
                    cpuid.pi = (SMBIOS.Table.ProcessorInformation)bi[0];
                }
            }
            Parser.Init();
            mouse.Initialize();
            Console.WriteLine("Quicksilver OS Alpha 1.0.0.8 as of 130119-1749\r\nCopyright (c) 2013");
            mouse.Initialize();
        }
        protected override void Run()
        {
            if(didcommand) {
                QuicksilverNEXT.Console.Write("Administrator# ");
                QuicksilverNEXT.Console.Flush();
                didcommand = false;
                commandwpar = "";
            }
            ConsoleKeyInfo k = Console.ReadKey(true);
            if (k.Key != ConsoleKey.Enter) {
                commandwpar += k.KeyChar.ToString();
            }
            Console.Write(k.KeyChar);
            if(k.Key == ConsoleKey.Enter) {
                Parser.Parse(commandwpar);
                didcommand = true;
            }

        }
    }
}
