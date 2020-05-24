using System.IO;

namespace HTML_Parser
{
    class Program
    {
        static void Main()
        {
            string dirlocation = @"D:\D - Software and School Folder\Acads\Computer Self Study\C++\www.learncpp.com\Expert C++\HTML";
            string FileName = @"Chapter 1.html";
            string input = File.ReadAllText(Path.Combine(dirlocation, FileName));

            TagsGenerator gen = new TagsGenerator(input);
            gen.RemoveTagStyles();

            File.WriteAllText(Path.Combine(dirlocation, "Chapter1_converted.html"),
               gen.InsertNewLineCharacters());
        }
    }
}