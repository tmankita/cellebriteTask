using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace cellebriteTask
{
    public class Extractor
    {
        private Dictionary<string, Contact> keyToConcat;
        private int barrier = 4;
        private byte[] buffer;
        private byte[] nextBuffer;
        private string nextData;
        private bool isNextBufferEmpty;
        private bool gotAllKeys;
        private UTF8Encoding decoder;
        private Dictionary<string, Action<string, string>> valueType_to_handler;
        private Dictionary<int, string> id_to_valueType;


        public Extractor()
        {
            UTF8Encoding decoder = new UTF8Encoding(true);
            buffer = new byte[1024];
            nextBuffer = new byte[1024];
            isNextBufferEmpty = true;
            gotAllKeys = false;
            valueType_to_handler = new Dictionary<string, Action<string, string>>()
            { { "FirstName", insertFirstName } ,
                { "LastName", insertLastName },
                { "Phone", insertPhone },
                { "Time", insertTime }};
            id_to_valueType = new Dictionary<int, string>()
            { {1, "FirstName"} ,
              {2, "LastName"},
              {3, "Phone"},
              {4, "Time"}};
            keyToConcat = new Dictionary<string, Contact>();

        }

        /// <summary>
        /// Extract all the contacs from the file
        /// </summary>
        /// <param name="path">Path to the source file</param>
        /// <returns></returns>
        public List<Contact> extract(string path)
        {
            int typeCount = 5;
            int curr = 1;
            if (File.Exists(path))
            {
                Console.WriteLine("Can't find the data file");
                return new List<Contact>();
            }
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() >= 0 && curr < typeCount)
                {
                     string data = sr.ReadLine();
                     data = data.Substring(barrier);

                     extractUntilEnd(ref data, id_to_valueType[curr]);
                     curr += 1;

                }
            }


            return keyToConcat.Values.ToList(); ;
        }


        private void insertFirstName(string key, string value)
        {
            value = value.Replace('�', ' ');

            if (keyToConcat.ContainsKey(key))
            {
                keyToConcat[key].FirstName = value;
            }
            else
            {
                Contact c = new Contact();
                c.FirstName = value;
                keyToConcat[key] = c ;
            }
        }
        private void insertLastName(string key, string value)
        {
            value = value.Replace('�', ' ');

            if (keyToConcat.ContainsKey(key))
            {
                keyToConcat[key].LastName = value;
            }
            else
            {
                foreach (string k in keyToConcat.Keys)
                {
                    if (key.Contains(k))
                    {
                        keyToConcat[k].LastName = value;
                    }
                }
            }
        }
        private void insertPhone(string key, string value)
        {
            if (keyToConcat.ContainsKey(key))
            {
                keyToConcat[key].Phone = value;
            }
            else
            {
                foreach (string k in keyToConcat.Keys)
                {
                    if (key.Contains(k))
                    {
                        keyToConcat[k].Phone = value;
                    }
                }
            }
        }
        private void insertTime(string key, string value)
        {
            int seconds = Int32.Parse(value);
            DateTime translated = new DateTime(1970, 1, 1).AddSeconds(seconds);
            if (keyToConcat.ContainsKey(key))
            {
                keyToConcat[key].Time = translated;
            }
            else
            {
                foreach (string k in keyToConcat.Keys)
                {
                    if (key.Contains(k))
                    {
                        keyToConcat[k].Time = translated;
                    }
                }
            }
        }

        /// <summary>
        /// Extract from data all the valueType information 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="valueType"></param>
        private void extractUntilEnd(ref string data, string valueType)
        {
            while (data != "")
            {
                Tuple<string, int> t = extractKeyAndValueLength(ref data);
                string key = t.Item1;
                int valueLength = t.Item2;
                if (validateKey(key))
                {
                    string value = extractValue(ref data, valueLength);
                    valueType_to_handler[valueType](key, value);
                }
                else
                {
                    throw new System.ArgumentException("Something went wrong!! got key:" + key, "key");
                }
            }

        }

        private string extractValue(ref string date, int valueLength)
        {
            string value = date.Substring(0, valueLength);
            date = date.Substring(valueLength);
            return value;
        }

        /// <summary>
        /// Extract from data the next candidate key and value length
        /// </summary>
        /// <param name="data">File content</param>
        /// <returns>Candidate Key</returns>
        private Tuple<string,int> extractKeyAndValueLength(ref string data)
        {
            string prefix = data.Substring(0, 4);
            string suffix = data.Substring(4, 5);
            string valueLengthStr = "";
            int valueLength = 0;
            bool startBuildLength = false;

            while (suffix != "")
            {
                
                if (suffix[0] == '0' && !startBuildLength)
                {
                    prefix += suffix[0];
                    suffix = suffix.Substring(1);
                }
                else
                {
                    startBuildLength = true;
                    valueLengthStr += suffix[0];
                    suffix = suffix.Substring(1);
                }
            }

            if (valueLengthStr.Length == 1)
            {
                valueLength = Convert.ToInt32(valueLengthStr, 16);
            }
            else
            {
                valueLength = Convert.ToInt32(valueLengthStr) + 6;
            }
            data = data.Substring(9);

            return new Tuple<string, int>(prefix, valueLength);
        }

        /// <summary>
        /// Validate if the key legal
        /// </summary>
        /// <param name="key">key candidate</param>
        /// <returns>bool</returns>
        private bool validateKey(string key)
        {
            bool res = true;
            foreach (char c in key)
            {
                // charcters need to be Capital letter or digit
                res = res && ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'));
            }
            return res;
        }
    }
}
