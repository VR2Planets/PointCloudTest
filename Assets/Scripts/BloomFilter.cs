using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    public class PointBloomFilter
    {
        private BitArray _bits = new BitArray(65536);

        public bool TryAdd(ulong index)
        {
            uint hash1 = lowbias32((uint)(index & 0xffffffff));
            hash1 += lowbias32((uint)((index >> 32) & 0xffffffff));
            
            uint hash2 = triple32((uint)(index & 0xffffffff));
            hash2 += triple32((uint)((index >> 32) & 0xffffffff));

            int index1 = (int) ((hash1 & 0xffff) ^ (hash1 >> 16 & 0xffff));
            int index2 = (int) ((hash2 & 0xffff) ^ (hash2 >> 16 & 0xffff));
            
            if (_bits[index1] && _bits[index2])
            {
                return false;
            }
            
            _bits[index1] = true;
            _bits[index2] = true;
            
            return true;
        }
        
        private uint lowbias32(uint x)
        {
            x ^= x >> 16;
            x *= 0x7feb352d;
            x ^= x >> 15;
            x *= 0x846ca68b;
            x ^= x >> 16;
            return x;
        }
        
        private uint triple32(uint x)
        {
            x ^= x >> 17;
            x *= 0xed5ad4bb;
            x ^= x >> 11;
            x *= 0xac4c1b51;
            x ^= x >> 15;
            x *= 0x31848bab;
            x ^= x >> 14;
            return x;
        }
    }
}