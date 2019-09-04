using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class TypeTable
    {
        public struct LocPair { public string file; public string nameSpace; }

        public Dictionary<string, List<LocPair>> typeTable { get; set; }
          = new Dictionary<string, List<LocPair>>();

        //----< add loc - file and namespace - for discovered type >-----

        public void Add(string type, LocPair locPair)
        {
            if (typeTable.ContainsKey(type))
            {
                typeTable[type].Add(locPair);
            }
            else
            {
                List<LocPair> lpl = new List<LocPair>();
                lpl.Add(locPair);
                typeTable.Add(type, lpl);
            }
        }
        //----< does table contain this type? >--------------------------

        public bool Contains(string type)
        {
            return typeTable.ContainsKey(type);
        }
        //----< display type table contents >----------------------------

        public void Show()
        {
            foreach (var item in typeTable)
            {
                Console.Write("\n  {0}", item.Key);
                foreach (var elem in typeTable[item.Key])
                {
                    Console.Write("\n         file: {0}\n    namespace: {1}", elem.file, elem.nameSpace);
                }
            }
        }
        //----< how many locs for specified type? >----------------------

        public int ListSize(string type)
        {
            if (!Contains(type))
                return 0;
            return typeTable[type].Count;
        }
        //----< get file where this type is defined >--------------------
        /*
         * - Use of namespaces is currently disabled
         */
        public string getFile(string type, string nameSpace)
        {
            if (!Contains(type))
                return "";
            // switch to commented if, below, when namespace processing is complete
            //if (listSize(type) == 1)

            if (ListSize(type) > 0)
                return typeTable[type][0].file;
            List<LocPair> list = typeTable[type];
            foreach (var item in list)
            {
                if (item.nameSpace == nameSpace)
                    return item.file;
            }
            return "";
        }

        //----< test stub >----------------------------------------------



    static void Main(string[] args)
    {
      Console.Write("\n  Testing TypeTable");
      Console.Write("\n ===================");

      TypeTable tt = new TypeTable();

      LocPair lp;
      lp.file = "file1";
      lp.nameSpace = "namespace1";
      tt.Add("fileA", lp);

      lp.file = "file2";
      lp.nameSpace = "namespace2";
      tt.Add("fileA", lp);

      lp.file = "file3";
      lp.nameSpace = "namespace3";
      tt.Add("fileB", lp);

      tt.Show();
      Console.Write("\n\n");
    }

    }
}
