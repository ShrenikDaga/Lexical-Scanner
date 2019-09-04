using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysis
{
    public class Element
    {
        public string type { get; set; }
        public string name { get; set; }
        public string file { get; set; }
        public string nameSpace { get; set; }
        public int beginLine { get; set; }
        public int endLine { get; set; }
        public int beginScopeCount { get; set; }
        public int endScopeCount { get; set; }

        public override string ToString()
        {
            string fileName = System.IO.Path.GetFileName(file);
            StringBuilder temp = new StringBuilder();
            temp.Append("{");
            temp.Append(String.Format("{0,-10}", type)).Append(" : ");
            temp.Append(String.Format("{0,-10}", name)).Append(" : ");
            temp.Append(String.Format("{0,-10}", fileName)).Append(" : ");
            temp.Append(String.Format("{0,-10}", nameSpace)).Append(" : ");
            //temp.Append(String.Format("{0,-5}", beginLine.ToString()));  // line of scope start
            //temp.Append(String.Format("{0,-5}", endLine.ToString()));    // line of scope end
            temp.Append("}");
            return temp.ToString();
        }
    }
}
