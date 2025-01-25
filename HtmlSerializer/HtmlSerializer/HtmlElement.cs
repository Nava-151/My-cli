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
            if (selector.Id != null && element.Id != null)
                Console.WriteLine("id " + selector.Id + "name " + selector.TagName + " element tag name " + element.Name + " element id " + element.Id);
            else
                Console.WriteLine("one of them is null");
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
                if (child.Classes != null)
                    if (child.Classes.Contains("copyR"))
                        Console.WriteLine("hh---------------");

                if (EqualElement(child, selector))
                {

                    if (selector.Child == null)
                    {
                        Console.WriteLine(child.Name + " class ");
                        if (child.Classes != null)
                            Console.WriteLine(child.Classes[0]);
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
        public string InnerHtml { get; set; } = "";
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

    }
}
