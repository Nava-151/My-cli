
using HtmlSerializer;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using static System.Net.Mime.MediaTypeNames;

Selector s = Selector.CreateQuery("div#mydiv.class-name .class-name #class-name");
Console.WriteLine();
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
var html = await Load("https://chani-k.co.il/sherlok-game/");

static List<string> DivideToLine(string input)
{
    string noOtherSpaces = Regex.Replace(input, @"[^\S ]+", "");
    string cleanString = Regex.Replace(noOtherSpaces, @" {2,}", " ");
    List<string> htmlLines = new Regex("<(.*?)>").Split(cleanString).Where(s => s.Length > 0).ToList();
    List<string> listWithoutEmptyLines = new List<string>();
    foreach (var line in htmlLines)
        if (line != " ")
            listWithoutEmptyLines.Add(line);
    return listWithoutEmptyLines;
}



List<string> listObject = DivideToLine(html);


HtmlElement root = new HtmlElement();
root = HtmlTreeBuilder.BuildTree(listObject);
string st = "div#copyright .copyR";
HashSet<HtmlElement> h = new HashSet<HtmlElement>();
Selector selector=Selector.CreateQuery(st);
h =root.ElementsFitSelector(root,selector,h); 
Console.WriteLine("--------------------");
h.ToList().ForEach(h =>Console.WriteLine(h.Name));
Console.WriteLine("--------------------");

