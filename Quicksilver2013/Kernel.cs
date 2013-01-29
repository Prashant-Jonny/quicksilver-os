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
            Console.WriteLine("Quicksilver OS Alpha 1.0.0.17 as of 130128-2030\r\nCopyright (c) 2013");
            mouse.Initialize();
        }
        protected override void Run()
        {
            QuicksilverNEXT.Console.Write("Administrator# ");
            QuicksilverNEXT.Console.Flush();
            commandwpar = Console.ReadLine();
            Parser.Parse(commandwpar);
        }
    }
}
