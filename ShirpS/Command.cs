using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ShirpS
{
    public class Command
    {
        static public String[] Execute(String cmd)
        {
            String retStr = "";
            String fileName = Guid.NewGuid().ToString();
            StringBuilder sB = new StringBuilder();
            System.Diagnostics.Process p = new System.Diagnostics.Process();

            //Get the exe name to execute
            String[] cmdParams = CommandLineParser.GetArguments(cmd);
            if (cmdParams.Length < 1) throw new Exception("No command given");

            //Get the arguments from the remaining members of the array
            String args = "";
            if (cmdParams.Length > 1) args = String.Format("\"{0}\"", String.Join("\" \"", cmdParams.ToList().GetRange(1, cmdParams.Length - 1)));

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
    }
}
