using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Praxis;
using Praxis.Emulator;

namespace Praxis.IO
{
    class File
    {
        /// <summary>
        /// Creates a new file at the specified path
        /// </summary>
        /// <param name="path">Format is %partname/folder/path/file ex. system/Users/Create.txt</param>
        /// <param name="content"></param>
        public static void Create(string path, byte[] content)
        {
            string[] paths = path.Split('/');
            PraxisPartition p = PraxisPartitionTable.Get(paths[0]);
            if (p != null) {
                uint sectors = p.nextblock();
                for (int i = 1; i < paths.Length; i++) {
                    if (paths[i] == "" && i != paths.Length - 1) {

                    }
                    if(i == paths.Length - 1) {
                        p.AddEntry((int)sectors, paths[i].GetHashCode(), 0xF0);
                        WriteFile(paths[1], (int)sectors, content, p);
                    }
                }
            }
        }
        private static void WriteFile(string name, Int32 block, byte[] content, PraxisPartition prp)
        {
            int num_of_sectors = 0, tmp_len = content.Length - 1976;
            while (tmp_len > 0) {
                tmp_len -= 2044;
                num_of_sectors++;
            }
            byte[][] blocks = new byte[num_of_sectors + 1][];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = new byte[2048]
            ;
            var ms = new MemBlocks(blocks[0]);
            ms.Write(Encoding.UTF8.GetBytes(name), 0, 64);
            ms.Write(BitConverter.GetBytes(content.Length), 64, 4);
            if(num_of_sectors == 0) ms.Write(BitConverter.GetBytes(0), 68, 4);
            else ms.Write(BitConverter.GetBytes(prp.nextblock()), 68, 4);
            byte[] temp = new byte[1976];
            for (int i = 0; i < Math.Min(1976, content.Length); i++) temp[i] = content[i];
            ms.Write(temp, 72, 1976);
            prp.part.Write(block, blocks[0]);
            prp.inextblock();
            for (int i = 0; i < num_of_sectors; i++)
            {
                ms = new MemBlocks(blocks[i + 1]);
                int old_next = (int)prp.nextblock();
                byte[] tmp = new byte[2044];
                for (int j = 0; j < Math.Min(1976, content.Length); j++) tmp[j] = content[j + (i * 2044)];
                ms.Write(tmp, 4, 2044);
                prp.inextblock();
                if (i == num_of_sectors - 1) ms.Write(BitConverter.GetBytes(0), 0, 4);
                else ms.Write(BitConverter.GetBytes(prp.nextblock()), 0, 4);
                prp.part.Write(old_next, tmp);
            }
            //1976 is the bytes in the first sector. 2044 in the later ones.
        }
        private static byte[] ReadFile(string name, Int32 block, PraxisPartition prp)
        {
            byte[] sec0 = prp.part.Read(block);
            int length = BitConverter.ToInt32(sec0, 64);
            int num_of_sectors = 0, tmp_len = length - 1976;
            while (tmp_len > 0)
            {
                tmp_len -= 2044;
                num_of_sectors++;
            }
            byte[][] blocks = new byte[num_of_sectors + 1][];
            for (int i = 0; i < blocks.Length; i++)
                blocks[i] = new byte[2048]
            ;
        }
        /*public static void EditFile(string path, byte[] content)
        {
            string[] paths = path.Split('/');
            PraxisPartition p = PraxisPartitionTable.Get(paths[0]);
            if (p != null) {
                uint sectors = p.nextblock();
                for (int i = 1; i < paths.Length; i++) {
                    if (paths[i] == "" && i != paths.Length - 1) {

                    }
                    if (i == paths.Length - 1) {
                        p.AddEntry((int)sectors, paths[i].GetHashCode(), 0xF0);
                        WriteFile(paths[1], (int)sectors, content, p);
                    }
                }
            }
        }*/
        public static byte[] Read(string path) {
            string[] paths = path.Split('/');
            PraxisPartition p = PraxisPartitionTable.Get(paths[0]);
            if (p != null) {
                uint sectors = 0;
                for (int i = 1; i < paths.Length; i++) {
                    if (paths[i] == "" && i != paths.Length - 1) {

                    }
                    if (i == paths.Length - 1 && p.doesHaveFile(paths[1].GetHashCode())) {
                        return ReadFile(paths[1], p.sectorOfFile(paths[1].GetHashCode()), p);
                    }
                }
            }
            return null;
        }
        public static byte[] get(string partition, int sector)
        {
            var x = PraxisPartitionTable.Get(partition).part.Read(sector);
            return x;
        }
    }
}
