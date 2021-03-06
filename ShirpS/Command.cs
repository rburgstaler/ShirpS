﻿using System;
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

            //if there were no results returned... then just return an empty array
            if (retStr.Length == 0) return new String[0];

            //The command line execuation will include a trailing carriage return / line feed / or combination
            //to signify the end.  We do not want the non-line after to show up as an empty member of the array.
            //We only want the last one replaced though
            if (retStr.EndsWith("\r\n")) retStr = retStr.Remove(retStr.Length-2);
            else if (retStr.EndsWith("\n")) retStr = retStr.Remove(retStr.Length-1);
            return retStr.TrimEnd().Split(new String[] { "\r\n", "\n" }, StringSplitOptions.None);
        }
    }
}
