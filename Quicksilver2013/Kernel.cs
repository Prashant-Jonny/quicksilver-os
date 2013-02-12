using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using x86 = Cosmos.Assembler.x86;
using SMBIOS = Cosmos.Hardware.SMBIOS;
using Quicksilver2013.Shells;

namespace Quicksilver2013
{
    public class Kernel : Sys.Kernel
    {
        public bool didcommand = true;
        string commandwpar = "";
        //public static string cd = "/";
        //public static GDOS.VirtualFileSystem FileSystem;
        //Praxis.Emulator.VDisk vd = Praxis.Emulator.VDisk.Create(4096);
        //Praxis.Emulator.PartitionTable pt;
        //Praxis.Emulator.Partition part;
        //Praxis.PraxisPartition prax;
        public static string cd = "/";
        public static Shell current;
        public static GDOS.VirtualFileSystem vfs;
        Cosmos.Hardware.TextScreen ts = new Cosmos.Hardware.TextScreen();
        Cosmos.Hardware.Mouse mouse = new Cosmos.Hardware.Mouse();
        protected override void BeforeRun()
        {
            #region GLNFS
            Console.ReadLine();
            vfs = new GDOS.VirtualFileSystem();
            Console.ReadLine();
            GruntyOS.HAL.ATA.Detect(); // This will detect all ATA devices and add them to the device filesystem
            Console.ReadLine();
            GruntyOS.CurrentUser.Privilages = 0; // This has to be set, 1 = limited 0 = root
            GruntyOS.CurrentUser.Username = "Admin"; // When using anything in the class File this will be the default username


            GruntyOS.HAL.FileSystem.Root = new GruntyOS.HAL.RootFilesystem(); // initialize virtual filesystem
            GruntyOS.HAL.FileSystem.Root.Seperator = '/';
            Console.ReadLine();
            bool ispart = false;
            for (int i = 0; i < GruntyOS.HAL.Devices.dev.Count; i++)
            {
                if (GruntyOS.HAL.Devices.dev[i].dev is Cosmos.Hardware.BlockDevice.Partition)
                {
                    GruntyOS.HAL.GLNFS FS = new GruntyOS.HAL.GLNFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev);

                    if (GruntyOS.HAL.GLNFS.isGFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev))
                    {
                        Console.WriteLine("Drive Detected");
                        Console.ReadLine();
                        ispart = true;
                        cd = "/";
                        var d = new GDOS.Drive();
                        d.Filesystem = FS;
                        d.DeviceFile = GruntyOS.HAL.Devices.dev[i].name;
                        vfs.AddDrive(d);
                        GruntyOS.HAL.FileSystem.Root.Mount("/", FS);
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.Write("Filesystem Label: "); new GruntyOS.HAL.GLNFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev).Format(Console.ReadLine());
                        //Cosmos.Core.Global.CPU.Reboot();
                    }
                }
            }
            if (!ispart) new fdisk().Execute(new string[1]);
            #endregion
            vfs.makeDir("/root", "Admin");
            vfs.saveFile(Quicksilver2013.Files.Exes.Example_exe, "/root/example.exe", "Admin");
            //Console.ReadLine();
            Console.WriteLine("Welcome to Quicksilver OS Alpha 1.0.0.31 as of 130211\r\nCopyright (c) 2013");
            Console.Write("Please pick a username: ");
            UserService.user = Console.ReadLine();
            Parser.Init();
            Console.Clear();
            current = new Prompt();
        }
        protected override void Run()
        {
            current.Run();
        }
    }
}
