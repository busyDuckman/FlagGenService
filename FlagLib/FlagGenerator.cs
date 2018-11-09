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
    public class FlagGenerator
    {
        public string ConfigFile;
        public List<FlagLayer> Layers;
        public string FilePath { get { return Path.GetDirectoryName(ConfigFile); } }

        public int[] flagCols = new int[] {Color.Red.ToArgb(), 
                Color.Blue.ToArgb(), 
                Color.Green.ToArgb(), 
                Color.Gold.ToArgb(), 
                Color.Brown.ToArgb(), 
                Color.Yellow.ToArgb(), 
                Color.Black.ToArgb(), 
                Color.White.ToArgb()};

        public int[] crestCols = new int[] {
                Color.Black.ToArgb(),
                Color.Gold.ToArgb()
        };

        public int[] crestBackCols = new int[] {
            Color.Red.ToArgb(),
            Color.White.ToArgb(),
            Color.Yellow.ToArgb()
        };
        
        private FlagGenerator()
        {
            Layers = new List<FlagLayer>();
        }

        public static FlagGenerator FromFile(string configFile)
        {
            FlagGenerator gen = new FlagGenerator();
            gen.ConfigFile = configFile;
            var lines = File.ReadAllLines(configFile);
            foreach (string line in lines.Where(L => !L.Trim().StartsWith("%")))
            {
                FlagLayer[] fl = FlagLayer.FromString(line, gen.FilePath);
                gen.Layers.AddRange(fl);
            }

            return gen;
        }

        static int[] layerDistrib = new int[] {1,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,5,5,5};

        public Flag Generate(int seed = 0x7777777)
        {
            try
            {
            Random r = new Random(seed);
            //setup
            int numLayers = layerDistrib.GetRandomItem(r);

            Flag flag = new Flag();
            flag.FieldCol = Color.FromArgb(flagCols.GetRandomItem(r));
            //generate layers
            for (int i = 0; i < numLayers; i++)
            {
                int layer = pickRandomLayer(flag.FlagLayers, r);
                int color = flagCols.GetRandomItem(r);
                LayerSettings ls = new LayerSettings(color);
                flag.FlagLayers.Add(new Tuple<FlagLayer, LayerSettings>(Layers[layer], ls));
            }

            flag.FlagLayers = (from L in flag.FlagLayers 
                               orderby L.Item1.Priority ascending 
                               select L).ToList(); 

            //draw crest
            if(r.Next(100)<50)
            {
                CrestLayer cl = (CrestLayer)Layers.Where(L => L is CrestLayer).ToList().GetRandomItem(r);

                if (cl == null)
                {
                    return null;
                }
                //is ther a graphic that accomidates a crest
                var crestsHolders = (from L in flag.FlagLayers
                                     where (L.Item1.CanHoldCrest) 
                                     select L).ToList();

                if (crestsHolders.Count > 0)
                {
                    //crest in middle
                    int crestSize = crestsHolders.Min(L => L.Item1.MaxCres);
                    var crestHolder = flag.FlagLayers.FirstOrDefault(L => L.Item1.MaxCres == crestSize);
                    //move the crest holder to the end
                    flag.FlagLayers.Remove(crestHolder);
                    flag.FlagLayers.Add(crestHolder);

                    LayerSettings ls = new LayerSettings(crestCols.GetRandomItem(r));
                    ls.nominalSize = crestSize;
                    ls.crestPosition = CrestPositions.Central;
                    if (r.Next(100) < 50)
                    {
                        //paint the back
                        ls.BackColor = crestBackCols.GetRandomItem();
                    }
                    flag.FlagLayers.Add(new Tuple<FlagLayer, LayerSettings>(cl, ls));
                }
                else
                {
                    //crest in top left
                    LayerSettings ls = new LayerSettings(crestCols.GetRandomItem(r));
                    ls.nominalSize = 0;
                    ls.crestPosition = (r.Next(100) < 50) ? CrestPositions.TopLeft : CrestPositions.Left;
                    flag.FlagLayers.Add(new Tuple<FlagLayer, LayerSettings>(cl, ls));
                }
            }

            return flag;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void recolorImage(Bitmap b, int colBefore, int colAfter)
        {
            int[] pixels = b.GetCopyOfIntsARGB32();
            //pixels = (from P in pixels select (P == blue) ? P: P).ToArray();
            for (int p = 0; p < pixels.Length; p++)
            {
                if (pixels[p] == colBefore)
                {
                    pixels[p] = colAfter;
                }
            }

            b.SetPixelsFromIntsARGB32(pixels);
        }

        private int pickRandomLayer(List<Tuple<FlagLayer, LayerSettings>> usedLayers, Random r)
        {
            while (true)
            {
                int pos = r.Next(Layers.Count);
                if (Layers[pos] is CrestLayer)
                {
                    continue; //try again
                }

                //done
                return pos;
            }
        }
    }
}
