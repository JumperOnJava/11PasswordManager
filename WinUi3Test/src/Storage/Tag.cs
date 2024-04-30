using System;
using System.Collections.Generic;
using System.Linq;
using WinUi3Test.src.Util;

namespace WinUi3Test.src.Storage
{
    public interface Tag : Identifiable
    {
        static Tag Any = TagAny.Instance;
        string DisplayName { get; set; }
        bool matches(Taggable tag);
    }
    public class TagAny : Tag
    {
        public static TagAny Instance = new TagAny();
        public long Identifier { get; }

        public string DisplayName { get { return displayName; } set { } }
        private string displayName = "Any";//TODO TRANSLATE

        public bool matches(Taggable tag) => true;
        private TagAny() { Identifier = 0; }
    }
    public class TagBasic : Tag
    {
        public string DisplayName { get; set; }
        public long Identifier { get; private set; }
        public bool matches(Taggable account)
        {
            if (account.Tags.Where(a => a is TagBasic).Where((a) => ((TagBasic)a).Identifier == Identifier).Count() > 0)
                return true;
            return false;
        }
        public TagBasic(string displayName)
        {
            DisplayName = displayName;
            Identifier = Random.Shared.NextInt64();
        }
        public static TagBasic createRandom()
        {
            return new TagBasic("tag" + Random.Shared.Next(100));
        }
    }
}