﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlTreeBuilder
    {
        static string[] singleTag = HtmlHelper.Instance.SingleTag;
        static string[] doubleTag = HtmlHelper.Instance.DoubleTag;
        public static HtmlElement BuildTree(List<string> htmlLines)
        {
            HtmlElement root = new HtmlElement();
            root.Name = GetFirstWord(htmlLines[1]);
            root.Attributes = GetAttributes(htmlLines[1]);
            root.Id = root.Attributes.GetValueOrDefault("id");
            root.Classes = root.Attributes.GetValueOrDefault("class") != null ? Regex.Split(root.Attributes.GetValueOrDefault("class"), @"\s+").ToList() : new List<string>();
            root.Children = new List<HtmlElement>();
            HtmlElement current = root;

            foreach (string line in htmlLines.Skip(2))
            {
                bool isSingle = singleTag.Contains(GetFirstWord(line));
                bool isdouble = doubleTag.Contains(GetFirstWord(line));

                if (line == "/html")
                    return root;
                if (isSingle || isdouble) 
                {
                    HtmlElement child = new HtmlElement();
                    child.Name = GetFirstWord(line);
                    child.Attributes = GetAttributes(line);
                    child.Id = child.Attributes.GetValueOrDefault("id");
                    child.Classes = child.Attributes.GetValueOrDefault("class") != null ? Regex.Split(child.Attributes.GetValueOrDefault("class"), @"\s+").ToList() :new List<string>();
                    child.Children = new List<HtmlElement>();
                    child.Parent = current;
                    current = child;
                }
                if (line.StartsWith('/') || isSingle)
                {
                    current.Parent.Children.Add(current);
                    current = current.Parent;
                }
                else if (!isdouble)
                {
                    current.InnerHtml += line;
                }
            }
            return root;
        }
        public static string GetFirstWord(string text)
        {
            string pattern = @"^\s*(\S+)";
            Match match = Regex.Match(text, pattern);
            if (!match.Success)
                return null;
            return match.Groups[1].Value;
        }
        private static Dictionary<string, string> GetAttributes(string htmlTag)
        {
            string pattern = @"(\w+)=[""'](.*?)[""']";
            MatchCollection matches = Regex.Matches(htmlTag, pattern);

            Dictionary<string, string> attributes = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                string attributeName = match.Groups[1].Value;
                string attributeValue = match.Groups[2].Value;
                attributes[attributeName] = attributeValue;
            }
            return attributes;
        }
    }
}

