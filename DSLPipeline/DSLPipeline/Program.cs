using System;
using System.Collections.Generic;
using DSLPipeline.MetaModel;

namespace DSLPipeline
{
    class Program
    {
        static void Main2(string[] args)
        {
            HashSetExample();
        }

        public static void HashSetExample()
        {
            HashSet<string> hashSet = new HashSet<string>();
            hashSet.Add("string1");
            hashSet.Add("string2");
            hashSet.Add("string1");
            
            PrintHashSet(hashSet);
        }

        public static void PrintHashSet(HashSet<string> set)
        {
            foreach (var str in set)
            {
                Console.WriteLine(str);
            }
        }

        public static void DictionaryExample()
        {
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            
            myDict.Add("1", "value1");
            //myDict.Add("1", "value1");
            myDict["1"] = "value2";

            printDict(myDict);
        }

        static void printDict(Dictionary<string, string> dict)
        {
            foreach (var key in dict.Keys)
            {
                Console.WriteLine(dict[key]);
            }
        }
    }
}