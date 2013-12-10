using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Horror
{
    public class Page : GameObject
    {
        public String text;
        public Page(String fileName, Vector2 pos)
            : base(pos)
        {
            width = 32;
            height = 32;

            try
            {
                using (StreamReader readFile = new StreamReader(fileName))
                {
                    text = readFile.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
