using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quicksilver.Filesystem.Praxis
{
    class Partition {
        private Cosmos.Hardware.BlockDevice.Partition partition;
        private uint clusterScale = 1;
        public ulong clusterSize = 4096;
        public ulong clusterAmount = 0;
        public Partition(Cosmos.Hardware.BlockDevice.Partition part, clusterSize cluster_size) {
            clusterAmount = (ulong)cluster_size;
            clusterScale = (uint)cluster_size / (uint)part.BlockSize;
            clusterAmount = part.BlockCount / (ulong)cluster_size;
        }
        public void writeCluster(ulong cluster, byte[] content) {
            if ((ulong)content.Length == clusterSize) {
                partition.WriteBlock(cluster, clusterScale, content);
            }
        }
        public void readCluster(ulong cluster, ref byte[] content) {
            if ((ulong)content.Length == clusterSize) {
                partition.ReadBlock(cluster, clusterScale, content);
            }
        }
        public byte[] readCluster(ulong cluster) {
            byte[] content = new byte[clusterSize];
            partition.ReadBlock(cluster, clusterScale, content);
            return content;
        }
    }
    enum clusterSize : ulong {
        c4096 = 4096,
    }
    unsafe class Praxis
    {
        Partition partition;
        /// <summary>
        /// Formats a partition with Praxis
        /// </summary>
        /// <param name="part">Partition to format with Praxis</param>
        public Praxis(Partition part) {
            partition = part;
            byte[] c0 = part.readCluster(0);
            fixed (byte* ptr = c0) {
                UnsignedWriter writer = new UnsignedWriter(ptr);
                writer.Advance(0x0020); //Label, ASCII not implemented, so will advance until then
                writer.Write16(0xF00F); //if 0xF00F then drive is formatted
                writer.Write64(0x0000); //Used clusters
                writer.Write64(0x0000); //Used entries
                writer.Write64(0x646E454878617250); //UTF8 for PraxHEnd
            }
        }
    }
    unsafe class Listing {
        //Each listing has the ulong next cluster in front, followed by the Entries(byte isdirectory(0xF0 file, 0x0F directory), uint hash, ulong sector)
    }
}
