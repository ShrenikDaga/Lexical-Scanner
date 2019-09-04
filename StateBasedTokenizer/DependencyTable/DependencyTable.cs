using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class DependencyTable
    {
        public Dictionary<string, List<string>> dependencies { get; set; } = new Dictionary<string, List<string>>();

        public void AddParent(string parentFile)
        {
            if (dependencies.ContainsKey(parentFile))
                return;
            List<string> deps = new List<string>();
            dependencies.Add(parentFile, deps);
        }

        public void Add(string parentFile, string childFile)
        {
            if (parentFile == childFile)
                return;
            if (dependencies.ContainsKey(parentFile))
            {
                if (dependencies[parentFile].Contains(childFile))
                    return;
                dependencies[parentFile].Add(childFile);
            }
            else
            {
                List<string> children = new List<string>();
                children.Add(childFile);
                dependencies.Add(parentFile, children);
            }
        }

        public bool Contains(string parentFile)
        {
            return dependencies.ContainsKey(parentFile);
        }

        public void Clear()
        {
            dependencies.Clear();
        }

        public void Show(bool fullyQualified = false)
        {
            foreach (var item in dependencies)
            {
                string file = item.Key;
                if (!fullyQualified)
                    file = System.IO.Path.GetFileName(file);
                Console.Write("\n {0}", file);
                if (item.Value.Count == 0)
                    continue;
                Console.Write("\n   ");
                foreach (var elem in item.Value)
                {
                    string child = elem;
                    if (!fullyQualified)
                        child = System.IO.Path.GetFileName(child);
                    Console.Write("{0} ",child);
                }
            }
        }
    }
}
