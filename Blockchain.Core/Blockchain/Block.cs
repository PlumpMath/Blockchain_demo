using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Core.Blockchain
{
    [Serializable]
    public class Block{
        public string Hash { get; set;}
        public string PrevHash{get; set;}
        [Key]
        public int Index{get; set;}
        public DateTime Timestamp {get; set;}
        public byte[] Data {get; set;}

        public  static string CalculateHash(long index,string previousHash,DateTime timestamp,byte[] data){
            HashAlgorithm algorithm = SHA256.Create();

            IEnumerable<byte> indexBuff = Encoding.UTF8.GetBytes(index.ToString());
            IEnumerable<byte> prevHashBuff = String.IsNullOrEmpty(previousHash) ? new byte[0] : Encoding.UTF8.GetBytes(previousHash);
            IEnumerable<byte> timeStampBuff = Encoding.UTF8.GetBytes(timestamp.ToString());

            IEnumerable<byte> result = indexBuff.Concat(prevHashBuff).Concat(timeStampBuff).Concat(data); 
            return Encoding.UTF8.GetString(algorithm.ComputeHash(result.ToArray())); 
        }

        
    }
}
