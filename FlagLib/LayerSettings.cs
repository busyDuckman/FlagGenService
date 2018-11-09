/*
 * The following code is Copyright 2018 Dr Warren Creemers (busyDuckman)
 * See LICENSE.md for more information.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlagLib
{
    public class LayerSettings
    {
        public int Color;
        public int BackColor;
        public bool outline;

        public int nominalSize;
        public CrestPositions crestPosition;

        public LayerSettings(int color)
        {
            Color = color;
        }
    }
}
