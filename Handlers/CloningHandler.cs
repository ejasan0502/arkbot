using System;
using System.Collections.Generic;
using System.IO;

namespace Ark_Bot.Handlers
{
    public class CloningHandler
    {
        private const string path = "C:/Users/ejasa/Desktop/Current Projects/Ark Bot/cloning.txt";

        private Dictionary<string, List<double>> dinos;

        public CloningHandler()
        {
            dinos = new Dictionary<string, List<double>>();

            string data = File.ReadAllText(path);
            string[] args = data.Split('\n');
            foreach (string arg in args)
            {
                string[] args2 = arg.Split(':');
                string dino = args2[0].Trim('\"', ' ');

                string stats = args2[1].Replace("[", "").Replace("]","").Replace(" ","");
                string[] args3 = stats.Split(',');
                List<double> vals = new List<double>();
                for (int i = 0; i < args3.Length; i++)
                    vals.Add(double.Parse(args3[i]));

                dinos.Add(dino, vals);
            }
        }

        public List<double> GetValues(string dino)
        {
            if (dinos.ContainsKey(dino))
                return dinos[dino];
            return null;
        }
    }
}
