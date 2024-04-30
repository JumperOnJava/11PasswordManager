using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinUi3Test.src.Storage
{
    public class StaticStorage : Storage
    {
        public static StaticStorage instance = new StaticStorage();
        public List<Account> Accounts = new List<Account>()
            {
                new AccountImpl("Twitter","JumperOnJava","12345678"),
                new AccountImpl("Github", "JumperOnJava", "12345678"),
                new AccountImpl("Minecraft", "JavaJumper", "12345678"),
                new AccountImpl("Google", "jumpergooog@gmail.com", "12345678"),
                new AccountImpl("Discord", "javajumper", "12345678"),
            };

        public IList<Tag> Tags = new List<Tag>()
        {
            new TagBasic("Gaming"),
            new TagBasic("Social"),
        };
        IList<Tag> Storage.Tags => Tags;

        IList<Account> Storage.Accounts => Accounts;

    }
}
