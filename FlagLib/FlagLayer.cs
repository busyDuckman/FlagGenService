/*
 * The following code is Copyright 2018 Dr Warren Creemers (busyDuckman)
 * See LICENSE.md for more information.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDToolbox;
using WDToolbox.Rendering;

namespace FlagLib
{
    public class FlagLayer : ICloneable
    {
        public virtual Bitmap Image { get; set; }
        public int MaxCres { get; set; }
        public bool Mirror { get; set; }
        public bool Flip { get; set; }
        public int Prob { get; set; }
        public bool CanHoldCrest { get { return MaxCres > 0; } }
        //public List<FlagLayer> 
        public int Priority { get; set; }

        public int[] OccupiedAreas  { get; set; }

        protected FlagLayer()
        {
        }

        protected FlagLayer(FlagLayer other)
        {
            this.Image = (other.Image==null)?null:(Bitmap)other.Image.Clone();
            this.MaxCres = other.MaxCres;
            this.Mirror = other.Mirror;
            this.Flip = other.Flip;
            this.Prob = other.Prob;
        }


        public static FlagLayer[] FromString(string line, string imagePath)
        {
            

            //%crest	,mirror	,flip	,prob	,res_crst	,file
            //,	,	,100	,350	,big_circle_384.png
            

            string[] tokens = line.Split(",".ToCharArray(), StringSplitOptions.None);
            if (tokens.Length == 7)
            {
                bool isCrest = tokens[0].Trim() == "1";
                FlagLayer fl = isCrest ? new CrestLayer() : new FlagLayer();

                fl.Mirror = tokens[1].Trim() == "1";
                fl.Flip = tokens[2].Trim() == "1";
                fl.Prob = int.Parse(tokens[3].Trim());
                fl.MaxCres = 0;
                if (!string.IsNullOrWhiteSpace(tokens[4]))
                {
                    int newCrestSize = int.Parse(tokens[4]);
                    fl.MaxCres = (fl.MaxCres == 0) ? newCrestSize :
                                                    Math.Min(newCrestSize, fl.MaxCres);
                }
                fl.Priority = int.Parse(tokens[5].Trim());

                string imageFile = Misc.ToSystemPathFormat(Path.Combine(imagePath, tokens[6].Trim()));

                //look for wildcards
                if (imageFile.Any(C => "?*".Contains(C)))
                {
                    //find final dir (becaue the path token may specify a releative directory(
                    string dir = Path.GetDirectoryName(imageFile);
                    string pattern = Misc.ToSystemPathFormat(Path.GetFileName(imageFile));
                    
                  
                    string[] files = Directory.GetFiles(dir, pattern, SearchOption.AllDirectories);
                    List<FlagLayer> layers = new List<FlagLayer>();
                    foreach (string file in files)
                    {
                        FlagLayer fl2 = (FlagLayer)fl.Clone();
                        fl2.loadImage(file);
                        layers.Add(fl2);
                    }

                    return layers.ToArray();
                }
                else
                {
                    fl.loadImage(imageFile);

                    return new FlagLayer[] {fl};
                }
            }

            return null;
        }

        public virtual void loadImage(string imageFile)
        {
            Image = ((Bitmap)Bitmap.FromFile(imageFile)).GetUnFuckedVersion();
            if (Mirror)
            {
                Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
            }
            if (Flip)
            {
                Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }

            OccupiedAreas = getOverview(Image);
        }

        public static int[] getOverview(Bitmap image)
        {
            if (image == null)
            {
                return null;
            }
            using (Bitmap small = new Bitmap(image, 9, 9))
            {
                return small.GetCopyOfIntsARGB32();
            }
        }

        public virtual Bitmap RenderImage(LayerSettings ls, Size flagSize)
        {
            int blue = Color.Blue.ToArgb();
            Bitmap b = (Bitmap)Image.Clone();
            FlagGenerator.recolorImage(b, blue, ls.Color);
            return b;
        }

        public virtual object Clone()
        {
            return new FlagLayer(this);
        }
    }
}
