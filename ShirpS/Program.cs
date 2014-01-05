using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ShirpS
{

    public class GitCommitPersonInfo
    {
        public GitCommitPersonInfo()
        {
            name = "";
            email = "";
        }
        public String name { get; set; }
        public String email { get; set; }
        public DateTime date { get; set; }
    }

    public class GitCommitInfo
    {
        public GitCommitInfo()
        {
            tree = "";
            parent = "";
            author = new GitCommitPersonInfo();
            committer = new GitCommitPersonInfo();
            message = new String[0];
        }
        public String tree { get; set; }
        public String parent { get; set; }
        public GitCommitPersonInfo author { get; set; }
        public GitCommitPersonInfo committer { get; set; }
        public String[] message { get; set; }
        public static GitCommitInfo Parse(String[] inStr)
        {
            bool MsgMode = false;
            List<String> Msg = new List<String>();
            //tree - not working YET
            //parent - not working YET

            Regex persreg = new Regex(@"^(.*?) (.*) <(.*@.*)> (\d*) (-?\+?\d*)$", RegexOptions.IgnoreCase);
            GitCommitInfo retVal = new GitCommitInfo();
            for (int x = 0; x < inStr.Length; x++)
            {
                //The very end of the commit payload is the message.  It is seperated from
                //the rest of the payload with an empty string
                if (MsgMode) 
                {
                    Msg.Add(inStr[x]);
                    continue;
                }
                else if (inStr[x] == "")
                {
                    MsgMode = true;
                    continue;
                }

                Match pm = persreg.Match(inStr[x]);
                if (pm.Success)
                {
                    GitCommitPersonInfo curper = (pm.Groups[1].Value.Equals("author")) ? retVal.author : retVal.committer;
                    curper.name = pm.Groups[2].Value;
                    curper.email = pm.Groups[3].Value;

                    DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    curper.date = dt.AddSeconds(Convert.ToDouble(pm.Groups[4].Value)).ToLocalTime();
                    continue;
                }
            }
            return retVal;
        }
    }

    class Program
    {
        

        static private String[] ExecCmd(String cmd)
        {
            String retStr = "";
            String fileName = Guid.NewGuid().ToString();
            StringBuilder sB = new StringBuilder();
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //Get the exe name to execute
            String[] cmdParams=CommandLineParser.GetArguments(cmd);
            if (cmdParams.Length < 1) throw new Exception("No command given");

            //Get the arguments from the remaining members of the array
            String args = "";
            if (cmdParams.Length > 1) args = String.Format("\"{0}\"", String.Join("\" \"", cmdParams.ToList().GetRange(1, cmdParams.Length-1)));

            p.StartInfo.FileName = cmdParams[0];
            p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();

            p.WaitForExit();
            retStr = p.StandardOutput.ReadToEnd();


            //Remove the file when done...
            return retStr.Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
        }


        

        static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.ASCII;
            
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");


            String refname = args[1];
            String oldrev  = args[2];
            String newrev  = args[3];
            //String user    = ENV['USER'];

            String[] res = ExecCmd(String.Format("git rev-list {0}..{1}", oldrev, newrev));

            Console.Write(String.Join("\n", res)+"\n");



            for (int x = res.Length-1; x >= 0; x--)
            {
                String[] newres = ExecCmd(String.Format("git cat-file commit {0}", res[x]));

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
