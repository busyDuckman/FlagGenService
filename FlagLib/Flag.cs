/*
 * The following code is Copyright 2018 Dr Warren Creemers (busyDuckman)
 * See LICENSE.md for more information.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDToolbox.Rendering;

namespace FlagLib
{
    public class Flag
    {
        public List<Tuple<FlagLayer, LayerSettings>> FlagLayers { get; set; }
        public Color FieldCol { get; set; }

        public Flag()
        {
            FlagLayers = new List<Tuple<FlagLayer, LayerSettings>>();
        }

        public Bitmap Render(Size flagSize)
        {
            IRenderer g = IRendererFactory.GetPreferredRenderer(flagSize.Width, flagSize.Height);
            g.SetHighQuality(true);
            //draw field
            g.FillRectangle(FieldCol, 0, 0, flagSize.Width, flagSize.Height);

            //draw layers
            foreach (var layerInfo in FlagLayers)
            {
                //render image
                Bitmap b = layerInfo.Item1.RenderImage(layerInfo.Item2, flagSize);
                g.DrawImage(b, 0, 0);
            }

            return g.RenderTargetAsGDIBitmap();
        }
    }
}
