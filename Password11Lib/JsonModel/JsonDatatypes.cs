using System.ComponentModel.DataAnnotations;

namespace Password11Lib.JsonModel;

public class JsonUser
{
    [Key] [StringLength(64)] public string Login { get; set; }
    [StringLength(48)] public string PasswordHash { get; set; }
    public List<JsonTag> Tags { get; set; } = new();
    public List<JsonAccount> Accounts { get; set; } = new();
    public List<JsonField> Fields { get; set; } = new();
}

public class JsonAccount
{
    [Key] public long Id { get; set; }
    public List<long> Tags { get; set; } = new();
    public List<long> Fields { get; set; } = new();
}

public class JsonTag
{
    [Key] public long Id { get; set; }

    [MaxLength(128)] public byte[] DisplayName { get; set; }

    [MaxLength(96)] public byte[] TagColorString { get; set; }
}

public class JsonField
{
    [Key] public long Id { get; set; }

    public bool IsHidden { get; set; }
    public bool Official { get; set; }

    [MaxLength(128)] public byte[] Name { get; set; }

    [MaxLength(1536)] public byte[] Data { get; set; }
}