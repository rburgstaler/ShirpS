﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace ShirpS
{
    class Program
    {
        

        static private String ExecCmd(String cmd)
        {
            String retStr = "";
            String fileName = Guid.NewGuid().ToString();
            StringBuilder sB = new StringBuilder();
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            try
            {
                //Git might not be found in the paths desired... we may need to find it
                //String svnPath = @"C:\Program Files\SlikSvn\bin\svn.exe";
                //if (!File.Exists(svnPath)) svnPath = @"C:\Program Files (x86)\SlikSvn\bin\svn.exe";
                //if (!File.Exists(svnPath)) throw new Exception("Cannot find SlickSvn... is it installed?");
                String[] cmdParams=CommandLineParser.GetArguments(cmd);
                if (cmdParams.Length < 1) throw new Exception("No command given");

                p.StartInfo.FileName = cmdParams[0];
                p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                p.StartInfo.Arguments = "";
                String fullFileName = Path.Combine(@"D:\Debug\", fileName);


                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.Start();
                try
                {
                    p.WaitForExit();
                    retStr = p.StandardOutput.ReadToEnd();
                }
                catch (Exception e)
                {
                    retStr = e.Message + " -- " + ((p.StandardOutput == null) ? "" : p.StandardOutput.ReadToEnd()) + " -- " + ((p.StandardError == null) ? "" : p.StandardError.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                retStr = e.Message;
            }
            //Remove the file when done...
            return retStr;
        }

        static void Main(string[] args)
        {
            //Console.OutputEncoding = Encoding.ASCII;
            
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");

            String res = ExecCmd("git add --help");
            Console.Write(res.Replace("\r\n", "\n"));

            Console.Write(Environment.CurrentDirectory + "\n");
            Console.Write(String.Join("\n", args));

            Console.Write("\n****\n");
            
            IDictionary	environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables) Console.Write("{0} = {1}\n", de.Key, de.Value);

            Console.Write("Validating your permissions\n");
            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            //Console.WriteLine("file ASCII");

            //Console.OutputEncoding = Encoding.Unicode;
            //Console.WriteLine("file Unicode");
            //Console.OutputEncoding = Encoding.UTF32;
            //Console.WriteLine("file UTF32");
            //Console.OutputEncoding = Encoding.UTF7;
            //Console.WriteLine("file UTF7");
            //Console.OutputEncoding = Encoding.BigEndianUnicode;
            //Console.WriteLine("file BigEndianUnicode");
            //Console.OutputEncoding = Encoding.UTF8;
            //Console.WriteLine("file UTF8");


           // File.AppendAllText(@"D:\debug\bashdebug.txt", "I'm coming");
           // File.AppendAllLines(@"D:\debug\bashdebug.txt", args);
           // Console.Write("got here");
            //Console.Error.Write("Standard error");
            Environment.Exit(5);
        }
    }
}
