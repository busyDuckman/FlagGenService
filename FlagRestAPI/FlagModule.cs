/*
 * The following code is Copyright 2018 Dr Warren Creemers (busyDuckman)
 * See LICENSE.md for more information.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FlagLib;
using Nancy.Responses;

namespace default_NancyFXT
{
    using Nancy;

    public class FlagModule : NancyModule
    {
        private static FlagGenerator flagGenerator = null;
        private static volatile object flagLock = new object();  
        
        public FlagModule()
        {
            Get("/", args => "Hello from Nancy running on CoreCLR");
            
            Get("/flags/{id}", args =>
            {
//                return "flag_" + args["id"];
                var id = this.Request.Query["id"];
                byte[] dataPNG =  flagPNG(id);

                //var response = new GenericFileResponse();

                MemoryStream ms = new MemoryStream(dataPNG);
                var response = new StreamResponse(() => ms, "image/png");
//                using (MemoryStream ms = new MemoryStream(dataPNG))
//                {
//                    var response = new StreamResponse(() => ms, "image/png");
//                }
                
                return response.AsAttachment("flag.png");
            });
            
            Get("/flag/get", args =>
            {
                var id = this.Request.Query["id"];
                return flagPNG(id);
            });
            
            
        }

        public byte[] flagPNG(int id)
        {
            try
            {

                //return "flag_" + args["id"];
                Flag f = genFlag(id);
                
                Bitmap bmp = f.Render(new Size(640, 480));
                using(MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    return ms.ToArray();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Flag genFlag(long flagId)
        {
            lock (flagLock)
            {
                if (flagGenerator == null)
                {
                    flagGenerator = FlagGenerator.FromFile(@"flags/flags.txt");
                }
            }

            return flagGenerator.Generate((int) flagId);
        }
    }
}