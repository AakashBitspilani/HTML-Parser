using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HTML_Parser
{
    public class Tag
    {
        // Index of '<'
        public int LessIndex { get; set; }
        public int SpaceIndex { get; set; }
        /// <summary>
        /// Index of last '>' of current tag. for e.g. index of '>' in <br />, <p> and </p>
        /// </summary>
        public int EndTagIndex { get; set; }
        /// <summary>
        /// Index of '/' character. It's position serves as parameter which type of tag we have
        /// </summary>
        public int SlashIndex { get; set; }

        public TagsGenerator GeneratorReference { get; set; }

        enum TagType
        {
            StartType,          // <tag....
            StartWithStyleType, // <tag ....
            EndType,            // </tag....
            SelfContained       // <tag...../>
        }

        TagType type;

        public string GetTagName()
        {
            string Input = GeneratorReference.Input;
            if (Input[EndTagIndex - 1] == '/') // <_tag_..... />
            {
                SlashIndex = EndTagIndex - 1;
                type = TagType.SelfContained;
                if (SpaceIndex < SlashIndex && SpaceIndex > -1)
                    return Input.Substring(LessIndex + 1, SpaceIndex - LessIndex - 1);
                else
                    return Input.Substring(LessIndex + 1, SlashIndex - LessIndex - 1);
            }
            else if (SpaceIndex < EndTagIndex && SpaceIndex >= 0)   // "<_tag_ ....
            {
                type = TagType.StartWithStyleType;
                return (Input.Substring(LessIndex + 1, SpaceIndex - LessIndex - 1));
            }
            else if (SlashIndex == LessIndex + 1)   // "</_tag_....
            {
                type = TagType.EndType;
                return (Input.Substring(SlashIndex + 1, EndTagIndex - SlashIndex - 1));
            }
            else
            {
                type = TagType.StartType;
                return (Input.Substring(LessIndex + 1, EndTagIndex - LessIndex - 1));
            }
        }

        public override string ToString()
        {
            string name = GetTagName();

            return type switch
            {
                TagType.StartType => string.Format("<{0}>", name),
                TagType.EndType => string.Format("</{0}>", name),
                TagType.StartWithStyleType => string.Format("<{0}>", name),
                TagType.SelfContained => string.Format("<{0}/>", name),
                _ => default,
            };
        }
    }

    public class TagsGenerator
    {
        /// <summary>
        /// List of all different type of tags inn list
        /// </summary>
        public List<string> FoundTags = new List<string>();
        /// <summary>
        /// List of all tags in HTML
        /// </summary>
        public List<Tag> AllTagsList = new List<Tag>();

        public string Input = default;
        public StringBuilder ProcessedString;

        public TagsGenerator(string input)
        {
            Input = input;
            FoundTags.Clear();
            AllTagsList.Clear();
            int EndTagIndex = 0;

            while (EndTagIndex != Input.Length - 1)     // File must end with <.._tagname..>, not any newline character
            {
                int LessIndex = Input.IndexOf('<', EndTagIndex);
                int SpaceIndex = Input.IndexOf(' ', LessIndex);
                EndTagIndex = Input.IndexOf('>', LessIndex);
                int SlashIndex = Input.IndexOf('/', LessIndex);

                AllTagsList.Add(new Tag
                {
                    LessIndex = LessIndex,
                    EndTagIndex = EndTagIndex,
                    SlashIndex = SlashIndex,
                    SpaceIndex = SpaceIndex,
                    GeneratorReference = this
                });

                string Tag = AllTagsList[AllTagsList.Count - 1].GetTagName();

                if (!FoundTags.Contains(Tag))           // Check for Duplicates Duplicates
                    FoundTags.Add(Tag);
            }
        }

        public string RemoveTagStyles()
        {
            List<(string Old, string New)> Lst = new List<(string Old, string New)>();

            for (int i = AllTagsList.Count - 1; i > -1; --i)
            {
                Lst.Add((Input.Substring(AllTagsList[i].LessIndex, AllTagsList[i].EndTagIndex - AllTagsList[i].LessIndex + 1),
                    AllTagsList[i].ToString()));
            }

            ProcessedString = new StringBuilder(Input);

            foreach (var (x, y) in Lst)
                ProcessedString = ProcessedString.Replace(x, y);

            var tmp = new TagsGenerator(ProcessedString.ToString());
            FoundTags = tmp.FoundTags;
            AllTagsList = tmp.AllTagsList;
            Input = tmp.Input;

            return tmp.Input;
        }

        public string InsertNewLineCharacters()
        {
            // Hardcoded here for testing purposes, will be set by user later in api
            string[] NoEnterAfter = new[] { "i", "a", "b", "span", "img" };

            StringBuilder temp = new StringBuilder(Input);

            foreach (var x in FoundTags.Except(NoEnterAfter))
            {
                temp = temp.Replace("</" + x + ">", "</" + x + ">\r\n");
                temp = temp.Replace("<" + x + "", "\r\n<" + x + "");
            }

            return temp.Replace("\r\n\r\n", "\r\n").ToString();
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < AllTagsList.Count; ++i)
                output += string.Format("{0}\n", AllTagsList[i]);

            return output;
        }
    }
}