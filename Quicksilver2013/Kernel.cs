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
        public static string CurrentDirectory = "legacyfs/";
        public static GDOS.VirtualFileSystem FileSystem;
        Cosmos.Hardware.TextScreen ts = new Cosmos.Hardware.TextScreen();
        Cosmos.Hardware.Mouse mouse = new Cosmos.Hardware.Mouse();
        protected override void BeforeRun()
        {
            Console.WriteLine("");
            #region SMBIOS
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
            #endregion
            #region GLNFS
            bool usableDriveFound = false; // if no GLNFS partition is detected then we need to do something
            while (!usableDriveFound)
            {
                GruntyOS.HAL.ATA.Detect(); // This will detect all ATA devices and add them to the device filesystem
                FileSystem = new GDOS.VirtualFileSystem(); // We need to make sure VirtualFileSystem is not null
                GruntyOS.CurrentUser.Privilages = 0; // This has to be set, 1 = limited 0 = root
                GruntyOS.CurrentUser.Username = "Administrator"; // When using anything under the filenamespace this will be the defualt username

                GruntyOS.HAL.FileSystem.Root = new GruntyOS.HAL.RootFilesystem();
                GruntyOS.HAL.FileSystem.Root.Seperator = '/'; // Use \ instead of /
                for (int i = 0; i < GruntyOS.HAL.Devices.dev.Count; i++)
                {
                    if (GruntyOS.HAL.Devices.dev[i].dev is Cosmos.Hardware.BlockDevice.Partition)
                    {
                        GruntyOS.HAL.GLNFS FS = new GruntyOS.HAL.GLNFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev);

                        usableDriveFound = true;
                        if (GruntyOS.HAL.GLNFS.isGFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev))
                        {
                            Console.WriteLine("Drive detected!");
                        }
                        GDOS.Drive drive = new GDOS.Drive();
                        drive.Filesystem = FS;
                        drive.DeviceFile = GruntyOS.HAL.Devices.dev[i].name;
                        FileSystem.AddDrive(drive);
                    }
                }
                if (!usableDriveFound)
                {
                    new fdisk().Execute(new string[1]);
                    Console.WriteLine("Please reboot (and do not forget to format the partition you detected!)");
                    while (true) ;

                }
            }

            GruntyOS.HAL.FileSystem.Root.Mount("legacyfs/" + FileSystem.DriveLabel, FileSystem);
            #endregion
            Parser.Init();
            mouse.Initialize();
            Console.WriteLine("Quicksilver OS Alpha 1.0.0.17 as of 130128-2030\r\nCopyright (c) 2013");
            mouse.Initialize();
        }
        protected override void Run()
        {
            QuicksilverNEXT.Console.Write(GruntyOS.CurrentUser.Username + "@" + CurrentDirectory + "# ");
            QuicksilverNEXT.Console.Flush();
            commandwpar = Console.ReadLine();
            Parser.Parse(commandwpar);
        }
    }
}
