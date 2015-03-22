using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Text;
using System.Reflection;

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

    public class ShContext
    {
        public String ScriptFile { get; set; }
        public List<String> Args { get; set; }

    }


    class Program
    {
        //Sample chunk of code for checking for std in
        public static String TryReadStdIn()
        {
            if (Console.In.Peek() >= 0)
            {
                return Console.In.ReadToEnd();
            }
            return null;
        }


        static void Main(string[] args)
        {
            ShContext ctx = new ShContext();

            //First parameter is the path the the bash
            ctx.ScriptFile = Path.Combine(Environment.CurrentDirectory, args[0].Replace('/', '\\'));
            ctx.Args = args.ToList();






            ScriptRunner sr = new ScriptRunner();
            sr.AddReferencedAssembly(typeof(String).GetType());
            //sr.AddReferencedAssembly(typeof(Console).GetType());
            sr.Compile(File.ReadAllText(@"..\Sample.cs"));

            Console.Write("++++++++++Errors+++++++++++\n");


            String[] errs = sr.Errors().Split(new String[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);
            foreach (String er in errs)
            {
                Console.Write(er+"\n");

            }

            

            Object retVal = sr.Execute("SSRScript.SSR.Exec", null, null);

            Console.Write("++++++++++Test+++++++++++\n");
            Console.Write(retVal.ToString()+"\n");







            String refname = args[1];
            String oldrev = args[2];
            String newrev = args[3];

            ListLookup lookup = new ListLookup();
            lookup.LoadFromFile(@"..\ValidUsers.txt");

            List<String> errors = new List<String>();

            Console.Write("*****************************************\n");
            Console.Write("*****************************************\n");
            Console.Write("Validating your Committers and Authors\n");

            String[] res;
            //When pushing a new branch, oldrev will be 0000000000000000000000000000000000000000.  When this
            //occurs we want to find the first revision that is not on any other branches.
            if (oldrev == "0000000000000000000000000000000000000000") res = Command.Execute(String.Format("git rev-list {0} --not --branches", newrev));
            else res = Command.Execute(String.Format("git rev-list {0}..{1}", oldrev, newrev));
            Console.Write(String.Format("{0} commits found\n", res.Length));

            for (int x = res.Length - 1; x >= 0; x--)
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

    public class ScriptRunner
    {
        CSharpCodeProvider codeProvider = new CSharpCodeProvider(new Dictionary<String, String>() { 
            //{ "CompilerVersion", "v3.5" }   //3.5 was causing problems for some reason
        });
        CompilerParameters parameters = new CompilerParameters();
        CompilerResults compResults;
        public bool Compile(String aScript)
        {
            runScript = aScript;
            //Make sure we generate DLL
            parameters.GenerateExecutable = false;

            //Make a dll
            parameters.GenerateInMemory = false;

            parameters.IncludeDebugInformation = true;

            parameters.OutputAssembly = "DaDaDa.dll";

            compResults = codeProvider.CompileAssemblyFromSource(parameters, aScript);
            return compResults.Errors.Count == 0;
        }

        //Add the assembly by giving a type defined in the assembly
        public bool AddReferencedAssembly(Type classType)
        {
            bool retVal = !parameters.ReferencedAssemblies.Contains(classType.Assembly.Location);
            if (retVal) parameters.ReferencedAssemblies.Add(classType.Assembly.Location);
            return retVal;
        }


        String runScript = "";
        public String Errors()
        {
            StringBuilder sBuild = new StringBuilder();
            if (compResults.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in compResults.Errors)
                {
                    sBuild.AppendLine("Error compiling Script:");
                    sBuild.AppendLine(runScript);
                    sBuild.AppendLine("Line number " + CompErr.Line);
                    sBuild.AppendLine("Error Number: " + CompErr.ErrorNumber);
                    sBuild.AppendLine("Error Text: " + CompErr.ErrorText + "';");
                    sBuild.AppendLine();
                    sBuild.AppendLine();
                }
            }
            return sBuild.ToString();
        }

        public Object Execute(String FullMethodName, PropertyAssigns props, params Object[] args)
        {
            Int32 idx = FullMethodName.LastIndexOf(".");
            return InvokeMethodSlow(compResults.CompiledAssembly, FullMethodName.Substring(0, idx), FullMethodName.Substring(idx + 1), args, props);
        }

        public static Object InvokeMethodSlow(string AssemblyName, string ClassName, string MethodName, Object[] args)
        {
            // load the assemly
            Assembly assembly = Assembly.LoadFrom(AssemblyName);
            return InvokeMethodSlow(assembly, ClassName, MethodName, args, null);
        }


        public static Object InvokeMethodSlow(Assembly assembly, string ClassName, string MethodName, Object[] args, PropertyAssigns props)
        {
            Type tp = assembly.GetType(ClassName, true);
            object ClassObject = Activator.CreateInstance(tp);
            if (props != null)
            {
                foreach (KeyValuePair<String, Object> keyVal in props)
                {
                    PropertyInfo pi = tp.GetProperty(keyVal.Key);
                    if (pi != null) pi.SetValue(ClassObject, keyVal.Value, new object[0]);
                }
            }
            return tp.InvokeMember(MethodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, ClassObject, args);
        }
    }

    public class PropertyAssigns : Dictionary<String, Object>
    {

    }    
}
