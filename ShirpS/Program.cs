using System;
using System.Collections;

namespace ShirpS
{

    class Program
    {
        static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.ASCII;
            
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");


            String refname = args[1];
            String oldrev  = args[2];
            String newrev  = args[3];
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
            Console.Write(String.Join("\n", args));

            Console.Write("\n****\n");
            
            IDictionary	environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables) Console.Write("{0} = {1}\n", de.Key, de.Value);

            Console.Write("Validating your permissions\n");
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Environment.Exit(0);
        }
    }
}
