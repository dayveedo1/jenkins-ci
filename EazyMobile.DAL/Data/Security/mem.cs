﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EazyMobile.DAL.Data.Security
{
    public class mem
    {
        public static void _set(ref byte[] data, int first, byte val, int count)
        {
            for (int nI = 0; nI < count; nI++)
            {
                data[nI + first] = val;
            }
        }

        public static void _cpy(ref byte[] dest, int dest_first, byte[] srce, int srce_first, int count)
        {
            for (int nI = 0; nI < count; nI++)
            {
                dest[dest_first + nI] = srce[nI + srce_first];
            }
        }
    }

}
