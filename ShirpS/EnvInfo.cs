using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ShirpS
{
    //Basic utility methods that can be used for debugging (to know what is available)
    public class EnvInfo
    {
        //Output the command line arguments passed in
        public static void OutputArguments()
        {
            String[] args = Environment.GetCommandLineArgs();
            for (int argidx = 0; argidx < args.Length; argidx++)
            {
                Console.Write(String.Format("Argument[{0}]: {1}\n", argidx, args[argidx]));
            }
        }

        //Output environmental variables to know what is available
        public static void OutputEnvironmentalVariables()
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables) Console.Write("{0} = {1}\n", de.Key, de.Value);

        }
    }
}
