﻿using System;
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
        public static GruntyOS.HAL.GLNFS fs;
        Cosmos.Hardware.TextScreen ts = new Cosmos.Hardware.TextScreen();
        Cosmos.Hardware.Mouse mouse = new Cosmos.Hardware.Mouse();
        protected override void BeforeRun()
        {
            #region GLNFS
            GruntyOS.HAL.ATA.Detect(); // This will detect all ATA devices and add them to the device filesystem
            GruntyOS.CurrentUser.Privilages = 0; // This has to be set, 1 = limited 0 = root
            GruntyOS.CurrentUser.Username = "Admin"; // When using anything in the class File this will be the default username


            GruntyOS.HAL.FileSystem.Root = new GruntyOS.HAL.RootFilesystem(); // initialize virtual filesystem
            GruntyOS.HAL.FileSystem.Root.Seperator = '/';
            bool ispart = false;
            for (int i = 0; i < GruntyOS.HAL.Devices.dev.Count; i++)
            {
                if (GruntyOS.HAL.Devices.dev[i].dev is Cosmos.Hardware.BlockDevice.Partition)
                {
                    GruntyOS.HAL.GLNFS FS = new GruntyOS.HAL.GLNFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev);

                    if (GruntyOS.HAL.GLNFS.isGFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev))
                    {
                        Console.WriteLine("Drive Detected");
                        ispart = true;
                        cd = "/";
                        fs = FS;
                        GruntyOS.HAL.FileSystem.Root.Mount("/", FS);

                    }
                    else
                    {
                        Console.Write("Filesystem Label: "); new GruntyOS.HAL.GLNFS((Cosmos.Hardware.BlockDevice.Partition)GruntyOS.HAL.Devices.dev[i].dev).Format(Console.ReadLine());
                        Cosmos.Core.Global.CPU.Reboot();
                    }
                }
            }
            if (!ispart) new fdisk().Execute(new string[1]);
            #endregion
            fs.makeDir("/root", "Admin");
            Console.WriteLine("Welcome to Quicksilver OS Alpha 1.0.0.31 as of 130211\r\nCopyright (c) 2013");
            byte[] pswd = fs.readFile("/root/password.sys");
            if (fs.ListFiles("/root").Contains("users.sys"))
            {
                String s = GruntyOS.IO.File.Open("/root/users.sys");
                string[] parts = s.Split(':');
                Console.Write("Please enter your password, " + parts[0] + ": ");
                string atpw = Console.ReadLine();
                if (atpw == ASCII.GetString(Quicksilver2013.Obfuscation.RandomObfuscator.decrypt(pswd, int.Parse(atpw))))
                {
                    UserService.user = parts[0];
                    cd = parts[2];
                }
            else
            {
                Console.Write("Please pick a username and Password\r\nUsername: ");
                UserService.user = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                GruntyOS.IO.File.Save("/root/users.sys", UserService.user + ":/:/home/" + UserService.user + ":ROOT");
                Console.WriteLine("Account " + UserService.user + " has been created. Press a key to continue.");
                Console.Read();
            }
            Parser.Init();
            Console.Clear();
            current = new Prompt();
        }
        protected override void Run()
        {
            current.Run();
        }
    }
    static class QArray
    {
        public static bool Contains(this string[] x, string s)
        {
            foreach (string var0 in x) if (s == var0) return true;
            return false;
        }
    }
}
