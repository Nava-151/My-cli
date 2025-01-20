using cli;
using System.CommandLine;
using System.Text;


static void RemoveEmptyLines(string path)
{
    var nonEmptyLines = File.ReadLines(path).Where(line => !string.IsNullOrWhiteSpace(line));
    File.WriteAllLines(path, nonEmptyLines);
}
static void AddFileToBundle(string bundledPath, string currentFile)
{
    File.AppendAllText(bundledPath, File.ReadAllText(currentFile));
}
static void AuthorOrNote(string message, string bundledPath)
{
    File.AppendAllText(bundledPath, "//" + message + "\n");
}

static List<string> SortFiles(List<string> files, bool sortByExtension)
{
    return sortByExtension
        ? files.OrderBy(f => Path.GetExtension(f)).ToList()
        : files.OrderBy(f => Path.GetFileName(f)).ToList();
}

//a recursive function which brings all file
static void GetInsideDirectory(string folderPath, List<string> files, List<string> extentions)
{
    try
    {
        string[] currentDirectoryFiles = Directory.GetFiles(folderPath);
        string[] directories = Directory.GetDirectories(folderPath);

        if (directories.Length == 0 && currentDirectoryFiles.Length == 0)
            return;
        if (currentDirectoryFiles.Length > 0)
        {
            foreach (string file in currentDirectoryFiles)
            {
                string end = System.IO.Path.GetExtension(file);
                if (extentions.Contains(end) && !file.Contains("bin") && !file.Contains("debug"))
                    files.Add(file);
            }
        }
        if (directories.Length > 0)
        {
            foreach (string directory in directories)
            {
                if (directory != "bin" && directory != "Debug")
                    GetInsideDirectory(directory, files, extentions);
            }
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Argument error: {ex.Message}");
    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine($"Directory not found: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        Console.WriteLine($"Access denied: {ex.Message}");
    }
    catch (PathTooLongException ex)
    {
        Console.WriteLine($"Path too long: {ex.Message}");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"I/O error: {ex.Message}");
    }
}
static void PassOnDirectory(string folderPath, string bundlePath, Options options)
{

    List<string> files = new List<string>();
    GetInsideDirectory(folderPath, files, options.Languages);

    if (options.Author != "")
        AuthorOrNote(options.Author, bundlePath);

    if (options.Sort)
    {
        if (options.SortByCode)
            SortFiles(files, true);
        else
            SortFiles(files, false);
    }
  
    foreach (string file in files)
    {
        if (options.RemoveEmptyLines)
        {
            RemoveEmptyLines(file);
        }
        if (options.Note == true)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            AuthorOrNote( currentDirectory + "\n" + "//" + Path.GetFileName(currentDirectory), file);
        }
        try
        {
            AddFileToBundle(bundlePath, file);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while adding file {file}: {ex.Message}");
        }
    }

}
// Array of programming file extensions
Dictionary<string, string> fileExtensions = new Dictionary<string, string>
        {
            { "C", ".c" },
            { "C++", ".cpp" },
            { "C#", ".cs" },
            { "Java", ".java" },
            { "Python", ".py" },
            { "Ruby", ".rb" },
            { "Go", ".go" },
            { "Swift", ".swift" },
            { "HTML", ".html" },
            { "CSS", ".css" },
            { "JavaScript", ".js" },
            { "TypeScript", ".ts" },
            { "PHP", ".php" },
            { "SQL", ".sql" },
            { "Database", ".db" },
            { "JSON", ".json" },
            { "XML", ".xml" },
            { "YAML", ".yaml" },
            { "INI", ".ini" },
            { "TOML", ".toml" },
            { "Shell", ".sh" },
            { "Batch", ".bat" },
            { "PowerShell", ".ps1" }
        };
static void AddAllLanguages(Options options, Dictionary<string, string> fileExtensions)
{
    options.Languages.AddRange(fileExtensions.Values);
}

var rootCommand = new RootCommand("Root command for file bundle cli");
var bundleCommand = new Command("bundle", "bundle code for one file");
rootCommand.AddCommand(bundleCommand);

var bundleOptionRemoveEmptyLine = new Option<bool>("-rel", "remove all the empty lines");
var bundleOptionOutput = new Option<string>("-ot", "export the file in this name");
var bundleOptionNote = new Option<bool>("-n", "add a remark with the file name and his path");
var bundleOptionLanguage = new Option<string[]>("-l", "which kind of file is allowed") { AllowMultipleArgumentsPerToken = true };
var bundleOptionAuthor = new Option<string>("-a", "add a remark with your name");
var bundleOptionSort = new Option<string>("-s", "sort all the files")
{ Arity = ArgumentArity.ZeroOrOne };

bundleOptionLanguage.IsRequired = true;
bundleOptionRemoveEmptyLine.AddAlias("-rel");
bundleOptionOutput.AddAlias("-ot");
bundleOptionAuthor.AddAlias("-a");
bundleOptionLanguage.AddAlias("-l");
bundleOptionNote.AddAlias("-n");
bundleOptionSort.AddAlias("-s");

bundleCommand.AddOption(bundleOptionOutput);
bundleCommand.AddOption(bundleOptionNote);
bundleCommand.AddOption(bundleOptionLanguage);
bundleCommand.AddOption(bundleOptionAuthor);
bundleCommand.AddOption(bundleOptionSort);
bundleCommand.AddOption(bundleOptionRemoveEmptyLine);
bundleCommand.SetHandler((output, note, author, languages, sort, rel) =>
{
    Options options = new Options();
    if (output != null)
        options.Output = output;

    if (note == true)
        options.Note = true;

    if (author != null && author.Length > 0)
        options.Author = "//" + author;

    if (languages != null)
    {
        foreach (string language in languages)
        {
            if (language == "all")
            {
                AddAllLanguages(options, fileExtensions);
            }
            else if (fileExtensions.ContainsKey(language))
            {
                options.Languages.Add(fileExtensions.GetValueOrDefault(language));
                Console.WriteLine(options.Languages.First());
            }
            else
            {
                Console.WriteLine("invalid language");
                return;
            }
        }
    }
    if (rel == true)
        options.RemoveEmptyLines = true;

    if (sort != null)
    {
        if (sort == "abOrder")
        {
            options.Sort = true;
        }
        else if (sort == "sort-by-extentions")
        {
            options.SortByCode = true;
        }
        else
        {
            Console.WriteLine("a mistake in sort option");
            return;
        }
    }
    try
    {
        File.Create(output).Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
    }
    PassOnDirectory(Environment.CurrentDirectory, output, options);
}, bundleOptionOutput, bundleOptionNote, bundleOptionAuthor, bundleOptionLanguage, bundleOptionSort, bundleOptionRemoveEmptyLine);

var rspCommand = new Command("create-rsp", "create a response file ");
rootCommand.AddCommand(rspCommand);

rspCommand.SetHandler(() =>
{
    StringBuilder request = new StringBuilder("bundle ");
    string path = "", temp = "";
    string path2 = "";
    request.Append("-" + bundleOptionOutput.Name);
    Console.WriteLine("Do you want to enter path? (y/n)");
    char res = Console.ReadKey().KeyChar;
    Console.WriteLine();

    if (res == 'y')
    {
        Console.WriteLine("Enter your path including file name:");
        path = Console.ReadLine();
        path2 += path + ".rsp";
        path += ".txt";
    }
    else if (res == 'n')
    {
        Console.WriteLine("Enter file name:");
        temp = Console.ReadLine();
        path2 = Path.Combine(Environment.CurrentDirectory, temp + ".rsp");
        path = Path.Combine(Environment.CurrentDirectory, temp + ".txt");
    }

    request.Append(" " + path);
    Console.WriteLine("Do you want to remove empty lines? (y/n)");
    res = Console.ReadKey().KeyChar;
    Console.WriteLine();
    if (res == 'y') request.Append(" -" + bundleOptionRemoveEmptyLine.Name);

    Console.WriteLine("Do you want to make a remark with path? (y/n)");
    res = Console.ReadKey().KeyChar;
    Console.WriteLine();
    if (res == 'y') request.Append(" -" + bundleOptionNote.Name);

    Console.WriteLine("Do you want to make a remark with author? (y/n)");
    res = Console.ReadKey().KeyChar;
    Console.WriteLine();
    if (res == 'y')
    {
        Console.WriteLine("Enter your name: ");
        string name = Console.ReadLine();
        request.Append(" -" + bundleOptionAuthor.Name + " " + name);
    }

    Console.WriteLine("Enter all languages you want to bundle to end press -1: ");
    request.Append(" -" + bundleOptionLanguage.Name);
    string lan = Console.ReadLine();
    while (lan != "-1")
    {
        request.Append(" " + lan);
        Console.WriteLine("Enter additional language: ");
        lan = Console.ReadLine();
    }

    Console.WriteLine("Do you want to sort the files? (y/n)");
    res = Console.ReadKey().KeyChar;
    Console.WriteLine();
    if (res == 'y')
    {
        request.Append(" -" + bundleOptionSort.Name);
        Console.WriteLine("Do you want to order it according to the code type? (y/n)");
        res = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (res == 'y')
            request.Append(" sort-by-extentions");
        else
            request.Append(" abOrder");
    }
    try
    {
        using (StreamWriter writer = new StreamWriter(path2))
        {
            writer.WriteLine(request.ToString());
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing to file: {ex.Message}");
    }
});

rootCommand.InvokeAsync(args);
