using System;
using System.Collections.Generic;
using System.Text;

namespace HTML_Parser
{
    public class IndexEntry
    {
        public bool Is_th { get; set; }
        public int ChapterNumber { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public bool IsEmptyLine { get; set; } = false;

        public override string ToString()
        {
            string Line1 = "<tr";
            if (Is_th || Index % 2 == 0)
                Line1 += ">";
            else
                Line1 += @" class=""alt"">";

            string Line2, Line3, Line4, Line5;

            if (IsEmptyLine)
            {
                Line2 = Line3 = Line4 = @"<td>&nbsp;</td>";
            }
            else if (Is_th)
            {
                Line2 = string.Format(@"<th><a name=""Chapter{0}""></a>Chapter {0}</th>",
                    ChapterNumber);
                Line3 = @"<th></th>";
                Line4 = string.Format(@"<th><a href=""chapter{0}.html"">{1}</a></th>",
                    ChapterNumber,
                    Name);
            }
            else
            {
                Line2 = string.Format(@"<td class=""indented"">{0}.{1}</td>",
                    ChapterNumber,
                    Index);
                Line3 = @"<td></td>";
                Line4 = string.Format(@"<td><a href=""chapter{0}.html#{0}.{1}"">{2}</a></td>",
                    ChapterNumber,
                    Index,
                    Name);
            }
            Line5 = @"</tr>";


            return string.Format("{0}\n\t{1}\n\t{2}\n\t{3}\n{4}",
                Line1,
                Line2,
                Line3,
                Line4,
                Line5);
        }
    }

    public class Index_Generator
    {
        public List<IndexEntry> Entries = new List<IndexEntry>();

        public Index_Generator(string[] Input)
        {
            int ChapterNo = 0;
            int Index = 0;

            foreach (var x in Input)
            {
                string temp = x;
                if (temp.StartsWith("Chapter"))
                {
                    ++ChapterNo;
                    temp = temp.Substring(temp.IndexOf(": ") + 1);
                    Entries.Add(new IndexEntry
                    {
                        ChapterNumber = ChapterNo,
                        Index = -1,
                        IsEmptyLine = false,
                        Is_th = true,
                        Name = temp
                    });

                    Index = 1;
                    continue;
                }
                else if (temp == "")
                {
                    Entries.Add(new IndexEntry
                    {
                        Index = Index++,
                        IsEmptyLine = true
                    });
                    continue;
                }
                else
                {
                    Entries.Add(new IndexEntry
                    {
                        ChapterNumber = ChapterNo,
                        Index = Index++,
                        IsEmptyLine = false,
                        Is_th = false,
                        Name = temp
                    });
                }
            }

            foreach (var x in Entries)
                Console.WriteLine(x);
        }
    }
}
