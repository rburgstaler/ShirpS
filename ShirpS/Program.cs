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
            
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Console.Write("Validating your permissions\n");

            //String user    = ENV['USER'];

            String[] res = Command.Execute(String.Format("git rev-list {0}..{1}", oldrev, newrev));

            Console.Write(String.Join("\n", res)+"\n");



            for (int x = res.Length-1; x >= 0; x--)
            {
                String[] newres = Command.Execute(String.Format("git cat-file commit {0}", res[x]));

                Console.Write(String.Join("\n", newres)+"\n");
                Console.Write(GitCommitInfo.Parse(newres).author.date.ToString() + "\n");
                Console.Write(GitCommitInfo.Parse(newres).committer.date.ToString() + "\n");
                Console.Write(GitCommitInfo.Parse(newres).message + "\n");
            }


            Console.Write(Environment.CurrentDirectory + "\n");

            Console.Write("\n****\n");

            EnvInfo.OutputArguments();



            EnvInfo.OutputEnvironmentalVariables();
            


            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Environment.Exit(0);
        }
    }
}
