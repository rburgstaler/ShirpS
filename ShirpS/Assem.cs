using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShirpS
{
    public class AssemLoader
    {
        public static void LoadFromFile(String fileName, ShProj prj)
        {
            List<String> cnf = File.ReadAllLines(fileName).ToList();
            //Strip out #Comments
            for (int idx = cnf.Count - 1; idx >= 0; idx--) if (cnf[idx].Trim().StartsWith("#")) cnf.RemoveAt(idx);
            JObject cnfObj = JObject.Parse(String.Join(Environment.NewLine, cnf.ToArray()));

            JArray scr = cnfObj.GetValue("Scripts", StringComparison.CurrentCultureIgnoreCase) as JArray;
            if (scr != null)
            {
                foreach (JToken jt in scr)
                {
                    JObject scriptObj = jt as JObject;
                    if (scriptObj == null) continue;

                    Assem asm = scriptObj.ToObject<Assem>();
                    prj.Add(asm);
                }
            }
        }
    }

    public class AssemBase
    {

    }

    public class Assem: AssemBase
    {
        public Assem()
        {
            FileNames = new List<String>();
            References = new List<String>();
        }
        public List<String> FileNames { get; private set; }
        public List<String> References { get; private set; }
    }

    public class AssemLink : AssemBase 
    {
        public AssemLink()
        {
            FileName = "";
        }

        public String FileName { get; set; }
    }

    public class ShProj: List<AssemBase>
    {

    }
}
