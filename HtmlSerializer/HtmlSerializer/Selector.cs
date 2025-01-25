using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HtmlSerializer
{
    internal class Selector
    {
        public static string ExtractId(string input)
        {
            Regex regex = new Regex(@"\#([a-zA-Z0-9_-]+)");
            Match match = regex.Match(input);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        public static string ExtractTagName(string input)
        {
            Regex regex = new Regex(@"^[a-zA-Z]+");
            Match match = regex.Match(input);
            return match.Success ? match.Value : string.Empty;
        }


        public static bool IsHtmlTag(string input)
        {
            string[] doubles = HtmlHelper.Instance.DoubleTag;
            string[] singles=HtmlHelper.Instance.SingleTag;
            if(input=="")
                return true;
            if(doubles.Contains(input)||singles.Contains(input)) 
                return true;
            return false;
        }
     

        public static List<string> ExtractClasses(string input)
        {
            Regex regex = new Regex(@"\.([a-zA-Z0-9_-]+)");
            List<string> classes = new List<string>();
            MatchCollection matches = regex.Matches(input);
            foreach (Match match in matches)
            {
                classes.Add(match.Groups[1].Value);
            }
            return classes;
        }


        public static Selector CreateQuery(string query)
        {
            List<string> list = Regex.Split(query, @"\s+").ToList();
            Selector root = new Selector();

            string tag= ExtractTagName(list[0]);
            root.TagName = IsHtmlTag(tag)?tag:"";
            root.Id= ExtractId(list[0]);
            root.Classes= ExtractClasses(list[0]);
            Selector current = root;
            foreach (string s in list.Skip(1))
            {
                Selector child = new Selector();
                tag = ExtractTagName(s);
                child.TagName = IsHtmlTag(tag) ? tag : "";
                child.Id = ExtractId(s);
                child.Classes = ExtractClasses(s);
                current.Child= child;
                child.Parent = current;
                current = child;
            }
            return root;
        }
        public string Id { get; set; }=string.Empty;
        public string TagName { get; set; } = "";
        public List<string> Classes { get; set; } 

        
        public Selector Child { get; set; }
        public Selector Parent { get; set; }

    }
}
