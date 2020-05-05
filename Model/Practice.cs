using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TabletArtco
{
    public class MultiMap<V>
    {
        private Dictionary<string, List<V>> _dictionary = new Dictionary<string, List<V>>();

        public void Add(string key, V value)
        {
            // Add a key.
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                // Get all keys.
                return this._dictionary.Keys;
            }
        }

        public List<V> this[string key]
        {
            get
            {
                // Get list at a key.
                if (!this._dictionary.TryGetValue(key, out List<V> list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

    class Practice
    {

        public static Stream stream { get; set; }

        public string level { get; set; }
        public string intro { get; set; }
        public int explainId{ get; set; }
        public int practiceId { get; set; }
        public List<string> solutionList { get; set; } = new List<string>();
        public List<Point> pointsList { get; set; } = new List<Point>();

        public static MultiMap<string> ReadXMLWithLevel(string Level)
        {
            MultiMap<string> multiMap = new MultiMap<string>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(DBManager.LoadPractise());
            XmlNodeList nodes = xdoc.SelectNodes("/Practice/" + Level + "/Intro");
            foreach (XmlNode node in nodes)
                multiMap.Add(node.Name, node.InnerText);

            nodes = xdoc.SelectNodes("/Practice/" + Level + "/Solution");
            foreach (XmlNode node in nodes)
                multiMap.Add(node.Name, node.InnerText);

            nodes = xdoc.SelectNodes("/Practice/" + Level + "/Points");
            foreach (XmlNode node in nodes)
                multiMap.Add(node.Name, node.InnerText);

            return multiMap;
        }

        //"<Practice><Level1><Intro>backgrounds/Practice_Explain/Practice_Explain_1.mp4</Intro><Solution>EventS…"

        public static Practice createPracticeMode(string level)
        {
            MultiMap<string> datas = ReadXMLWithLevel("Level" + level);

            int[] explains = {
                Resource.Raw.explain_11,Resource.Raw.explain_12,Resource.Raw.explain_13,Resource.Raw.explain_14,Resource.Raw.explain_15,
                Resource.Raw.explain_16,Resource.Raw.explain_17,Resource.Raw.explain_18,Resource.Raw.explain_19,Resource.Raw.explain_20
            };

            int[] practices = {
                Resource.Raw.Practice_11,Resource.Raw.Practice_12,Resource.Raw.Practice_13,Resource.Raw.Practice_14,Resource.Raw.Practice_15,
                Resource.Raw.Practice_16,Resource.Raw.Practice_17,Resource.Raw.Practice_18,Resource.Raw.Practice_19,Resource.Raw.Practice_20
            };

            int index = int.Parse(level);
            if (index<1 || index>10) {
                return null;
            }

            Practice practice = new Practice();

            practice.explainId = explains[index - 1];
            practice.practiceId = practices[index - 1];

            string[] solution = datas["Solution"][0].Split(',');
            for (int i = 0; i<solution.Length; i++)
                practice.solutionList.Add(solution[i]);

            string[] map = datas["Points"][0].Split(',');
            for (int i = 0; i<map.Length; i += 2)
                practice.pointsList.Add(new Point(int.Parse(map[i]), int.Parse(map[i + 1])));

            return practice;
        }
    }
}