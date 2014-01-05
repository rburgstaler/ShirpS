using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ShirpS
{

    public class ListLookup
    {
        public List<String> Names = new List<String>();
        public List<String> Emails = new List<String>();
        public void LoadFromFile(String filePath)
        {
            Regex reg = new Regex(@"^(.*) <(.*@.*)>$", RegexOptions.IgnoreCase);
            String[] fl = File.ReadAllLines(filePath);
            foreach (String ln in fl)
            {
                Match m = reg.Match(ln);
                if (!m.Success) continue;
                Names.Add(m.Groups[1].Value);
                Emails.Add(m.Groups[2].Value);
            }
            Names.Sort(Comp);
            Emails.Sort(Comp);
        }
        StringComparer Comp = StringComparer.CurrentCultureIgnoreCase;
        public int FindNameIndex(String name)
        {
            return Names.BinarySearch(name, Comp);
        }
        public int FindEmailIndex(String email)
        {
            return Emails.BinarySearch(email, Comp);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            String refname = args[1];
            String oldrev = args[2];
            String newrev = args[3];

            ListLookup lookup = new ListLookup();
            lookup.LoadFromFile("ValidUsers.txt");

            List<String> errors = new List<String>();
            
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Console.Write("Validating your Committers and Authors\n");

            String[] res = Command.Execute(String.Format("git rev-list {0}..{1}", oldrev, newrev));

            for (int x = res.Length-1; x >= 0; x--)
            {
                String[] newres = Command.Execute(String.Format("git cat-file commit {0}", res[x]));
                GitCommitInfo gi = GitCommitInfo.Parse(newres);

                if (lookup.FindNameIndex(gi.author.name) < 0) errors.Add(String.Format("Invalid author name: {0}", gi.author.name));
                if (lookup.FindEmailIndex(gi.author.email) < 0) errors.Add(String.Format("Invalid author email: {0}", gi.author.email));
                if (lookup.FindNameIndex(gi.committer.name) < 0) errors.Add(String.Format("Invalid committer name: {0}", gi.committer.name));
                if (lookup.FindEmailIndex(gi.committer.email) < 0) errors.Add(String.Format("Invalid committer email: {0}", gi.committer.email));
            }

            if (errors.Count > 0) Console.Write(String.Join("\n", errors) + "\n");
            else Console.Write("Committers and Authors are OK!\n");

            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Environment.Exit(errors.Count);
        }
    }
}
