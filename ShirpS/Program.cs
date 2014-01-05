using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
    ///D/Dev/ExperimentationCSharp/BashInterperter/BashInterpreter/bin/Debug/sample.sh
}
