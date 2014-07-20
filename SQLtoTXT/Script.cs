using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLtoTXT
{
    public class Script
    {
        String text;
        String title;
        Regex reg = new Regex(@"/\*\$(\w+)\*/(.+?)/\*\$\*/");
        Dictionary<string, string> prms = new Dictionary<string, string>();

        public Script(FileInfo file)
        {
            using(StreamReader s = new StreamReader(file.OpenRead(), Encoding.Default)) {
                String text = s.ReadToEnd();
                Init(file.Name, text);
            }
        }

        public Script(String title, String text)
        {
            Init(title, text);
        }

        public void Init(String title, String text)
        {
            this.text = text;
            this.title = title;
            foreach(Match m in reg.Matches(text)) {
                prms.Add(m.Groups[1].Value, m.Groups[2].Value);
            }
        }

        public String Title
        {
            get { return title; }
        }

        public Dictionary<string, string> Params
        {
            get
            {
                return prms;
            }

        }

        public String Text(Dictionary<String, String> prms)
        {
            MatchEvaluator evaluator = new MatchEvaluator(new Func<Match, String>( m =>
            {
                if (prms.ContainsKey(m.Groups[1].Value))
                {
                    return prms[m.Groups[1].Value];
                }
                return m.Value;
            }));
            return reg.Replace((String)this.text.Clone(), evaluator);
        }
    }
}
