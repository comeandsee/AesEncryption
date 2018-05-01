using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSKprojekt1
{
    public class EncryptionObject
    {
        public string encryptedSessionKey { get; set; }
        //public byte[] sessionKey { get; set; }
        public string ivString { get; set; }
        public int blockSize { get; set; }
    }
}
