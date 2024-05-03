using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi3Test.src.Storage
{
    public class StaticStorage : Storage
    {
        public List<Account> Accounts = new List<Account>()
            {
                new AccountImpl("Twitter","JumperOnJava","12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Github", "JumperOnJava", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Minecraft", "JavaJumper", "12345678",new List<Tag>(){GAMING}),
                new AccountImpl("Google", "jumpergooog@gmail.com", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Discord", "javajumper", "12345678",new List<Tag>(){GAMING,SOCIAL}),
                new AccountImpl("Twitter","JumperOnJava","12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Github", "JumperOnJava", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Minecraft", "JavaJumper", "12345678",new List<Tag>(){GAMING}),
                new AccountImpl("Google", "jumpergooog@gmail.com", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Discord", "javajumper", "12345678",new List<Tag>(){GAMING,SOCIAL}),
                new AccountImpl("Twitter","JumperOnJava","12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Github", "JumperOnJava", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Minecraft", "JavaJumper", "12345678",new List<Tag>(){GAMING}),
                new AccountImpl("Google", "jumpergooog@gmail.com", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Discord", "javajumper", "12345678",new List<Tag>(){GAMING,SOCIAL}),
                new AccountImpl("Twitter","JumperOnJava","12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Github", "JumperOnJava", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Minecraft", "JavaJumper", "12345678",new List<Tag>(){GAMING}),
                new AccountImpl("Google", "jumpergooog@gmail.com", "12345678",new List<Tag>(){SOCIAL}),
                new AccountImpl("Discord", "javajumper", "12345678",new List<Tag>(){GAMING,SOCIAL}),
            };
        private static Tag GAMING;
        private static Tag SOCIAL;
        static StaticStorage(){
            GAMING = new TagBasic("Gaming");
            SOCIAL = new TagBasic("Social");
            instance = new StaticStorage();
        }
        public static StaticStorage instance;
        public IList<Tag> Tags = new List<Tag>()
        {
            GAMING,SOCIAL
        };
        IList <Tag> Storage.Tags => Tags;

        IList<Account> Storage.Accounts => Accounts;

    }
}
