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
using WDToolbox;
using WDToolbox.Rendering;

namespace FlagLib
{
    public enum CrestPositions { Central, TopLeft, Left };
    public class CrestLayer : FlagLayer, ICloneable
    {
        public CrestLayer() : base()
        {
        }

        protected CrestLayer(CrestLayer other)
        {
            this.Image = (other.Image==null)?null:(Bitmap)other.Image.Clone();
            this.MaxCres = other.MaxCres;
            this.Mirror = other.Mirror;
            this.Flip = other.Flip;
            this.Prob = other.Prob;
        }

        public override void loadImage(string imageFile)
        {
            Image = ((Bitmap)Bitmap.FromFile(imageFile)).GetUnFuckedVersion();
            FlagGenerator.recolorImage(Image, Color.Black.ToArgb(), Color.Blue.ToArgb());
            FlagGenerator.recolorImage(Image, Color.White.ToArgb(), Color.Transparent.ToArgb());
        }

        public override Bitmap RenderImage(LayerSettings ls, Size flagSize)
        {
            //draw crest
            IRenderer g = IRendererFactory.GetPreferredRenderer(flagSize.Width, flagSize.Height);

            int xPos = 0;
            int yPos = 0;
            int newHeight =flagSize.Height /3;
            int newWidth = (int)((newHeight / (double)Image.Height) * Image.Width); //maintain aspect ratio
            double ratio;
            switch (ls.crestPosition)
            {
                case CrestPositions.Central:
                    ratio = ls.nominalSize / (double)Math.Max(Image.Width, Image.Height);
                    newWidth = (int)(Image.Width * ratio);
                    newHeight = (int)(Image.Height * ratio);
                    xPos = (flagSize.Width / 2) - (newWidth / 2);
                    yPos = (flagSize.Height / 2) - (newHeight / 2);
                    break;
                case CrestPositions.TopLeft:
                    xPos = 0;
                    yPos = 0;
                    break;
                case CrestPositions.Left:
                    newHeight = (int)(flagSize.Height * 0.75);
                    newWidth = (int)((newHeight / (double)Image.Height) * Image.Width); //maintain aspect ratio
                    xPos = 0;
                    yPos = (flagSize.Height / 2) - (newHeight / 2);
                    break;
                default:
                    break;
            }

            int blue = Color.Blue.ToArgb();
            Bitmap b = (Bitmap)Image.Clone();
            FlagGenerator.recolorImage(b, blue, ls.Color);
            g.DrawImage(b, xPos, yPos, newWidth, newHeight);
            return g.RenderTargetAsGDIBitmap();
        }


        public override object Clone()
        {
            return new CrestLayer(this);
        }
    }
}
