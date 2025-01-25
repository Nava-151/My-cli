using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    internal class HtmlElement
    {
        public bool EqualElement(HtmlElement element, Selector selector)
        {

            if (element == null || selector == null)
                return false;
            if (selector.TagName != string.Empty && selector.TagName != element.Name)
                return false;
            if (selector.Id != string.Empty && selector.Id != element.Id)
                return false;
            if (element.Classes == null)
                return false;
            foreach (string c in selector.Classes)
            {
                if (!element.Classes.Contains(c))
                    return false;
            }
            return true;
        }
        public HashSet<HtmlElement> ElementsFitSelector(HtmlElement element, Selector selector, HashSet<HtmlElement> list)
        {
            IEnumerable<HtmlElement> children = element.Descendants();
            foreach (HtmlElement child in children.Skip(1))
            {
                if (EqualElement(child, selector))
                {

                    if (selector.Child == null)
                    {
                        if (child.Classes != null)
                            list.Add(child);
                    }
                    ElementsFitSelector(child, selector.Child, list);
                }
            }
            return list;
        }



        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);
            while (queue.Count > 0)
            {
                HtmlElement temp = queue.Dequeue();
                yield return temp;
                foreach (HtmlElement element in temp.Children)
                    queue.Enqueue(element);
            }
        }

        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this;
            while (current.Parent != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; } = string.Empty;
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

    }
}
