using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

}
