using Unity.Mathematics;

namespace DefaultNamespace
{
    public static class MortonCode
    {
        public static int3 MortonCodeToXYZ(ulong index)
        {
            ulong morton = index;
            int x = 0, y = 0, z = 0;

            for (int i = 0; i < 21; i++)
            {
                x += (int) ((0x1 & morton) << i);
                y += (int) (((0x2 & morton) >> 1) << i);
                z += (int) (((0x4 & morton) >> 2) << i);

                morton = (morton >> 3);
            }

            return new int3(x, y, z);
        }

        public static ulong XYZToMortonCode(uint3 index)
        {
            ulong mortonC = 0;
            for (int i = 0; i < 21; ++i)
            {
                mortonC |= (index.x & (0x1UL << i)) << (2 * i)
                           | (index.y & (0x1UL << i)) << (2 * i + 1)
                           | (index.z & (0x1UL << i)) << (2 * i + 2);
            }

            return mortonC;
        }
    }
}