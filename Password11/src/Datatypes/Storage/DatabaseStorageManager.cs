using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Password11.Datatypes;
using Password11.Datatypes.Serializing;
using Password11.Dialogs;
using Password11.src.Util;
using Password11.Util;
using Password11Lib.JsonModel;
using Password11Lib.Util;

namespace Password11.src.Datatypes.Storage;

public class DatabaseStorageManager : StorageManager, LocationDisplayModel
{
    private static readonly HttpClient client = new();

    [JsonRequired] private readonly string host = "http://localhost:5000/";

    [JsonIgnore] private string password;


    public DatabaseStorageManager(string host, string login)
    {
        if (!host.StartsWith("http://") && !host.StartsWith("https://")) host = "http://" + host;
        this.host = host;
        DisplayName = login;
    }

    private Uri api => new(host);

    public DateTime LastAccessTime { get; set; }
    [field: JsonRequired] public string DisplayName { get; }

    public string DisplayPath => $"{DisplayName} at {host.Replace("http://", "").Replace("https://", "")}";


    public async Task<StorageData> GetData()
    {
        var okCheck = await client.GetAsync(api.Endpoint("api/ok"));
        if (okCheck.StatusCode != HttpStatusCode.OK)
            throw new ExceptionDialog.DialogException("Failed to connect to server: ", okCheck.ReasonPhrase);


        var req = await client.GetAsync(api.Endpoint($"api/getdata?login={DisplayName}"));
        var testString = await req.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(testString)) return new StorageData();

        var jsonUser = JsonConvert.DeserializeObject<JsonUser>(testString);
        var storage = new StorageData();

        storage.Tags = jsonUser.Tags.ToList().Select(jtag =>
        {
            var storageTag = new TagBasic();
            storageTag.TagColorsString = jtag.TagColorString.EncodeBase64();
            storageTag.DisplayName = jtag.DisplayName.EncodeBase64();
            storageTag.Identifier = new UniqueId<Tag>(jtag.Id);
            return (Tag)storageTag;
        }).ToList();

        storage.Accounts = new List<Account>();

        foreach (var jsonAccount in jsonUser.Accounts)
        {
            var storageAccount = new AccountImpl();
            storageAccount.Identifier = new UniqueId<Account>(jsonAccount.Id);
            storageAccount.Tags = jsonAccount.Tags.Select(id => new UniqueId<Tag>(id)).ToList();
            storageAccount.Fields = new Dictionary<string, FieldData>();
            foreach (var jsonAccountField in jsonAccount.Fields)
            {
                var jsonField = jsonUser.Fields.First(f => f.Id == jsonAccountField);
                var dataField = new FieldData(jsonField.IsHidden, jsonField.Name.EncodeBase64(),
                    jsonField.Data.EncodeBase64(),
                    jsonField.Official);
                storageAccount.Fields.Add(dataField.Name, dataField);
            }

            storage.Accounts.Add(storageAccount);
        }

        return storage;
    }

    public async Task SetData(StorageData value)
    {
        var storage = value.CloneRef();
        var jsonUser = new JsonUser();
        var tags = new List<JsonTag>();

        foreach (var storageTag in storage.Tags)
        {
            var jsonTag = new JsonTag
            {
                DisplayName = storageTag.DisplayName.DecodeBase64(),
                TagColorString = storageTag.TagColorsString.DecodeBase64(),
                Id = storageTag.Identifier.id
            };
            tags.Add(jsonTag);
        }

        jsonUser.Tags = tags;

        var fields = new List<JsonField>();
        foreach (var storageAccount in storage.Accounts)
        foreach (var fieldPair in storageAccount.Fields)
        {
            var jsonField = new JsonField();
            var dataField = fieldPair.Value;
            jsonField.Id = dataField.Identifier.id;
            jsonField.Name = dataField.Name.DecodeBase64();
            jsonField.Data = dataField.Data.DecodeBase64();
            jsonField.IsHidden = dataField.IsHidden;
            jsonField.Official = dataField.Official;
            fields.Add(jsonField);
        }

        jsonUser.Fields = fields;

        var accounts = new List<JsonAccount>();

        foreach (var dataAccount in storage.Accounts)
        {
            var jsonAccount = new JsonAccount();
            jsonAccount.Fields = dataAccount.Fields.Select(pair => pair.Value.Identifier.id).ToList();
            jsonAccount.Tags = dataAccount.Tags.Select(identifier => identifier.id).ToList();
            jsonAccount.Id = dataAccount.Identifier.id;
            accounts.Add(jsonAccount);
        }

        jsonUser.Accounts = accounts;

        jsonUser.Login = DisplayName;
        jsonUser.PasswordHash = password;

        var json = JsonConvert.SerializeObject(jsonUser);
        var endpoint = api.Endpoint("api/setdata");
        var req = await client.PutAsync(endpoint, new StringContent(json, Encoding.UTF8, "application/json"));

        if (req.StatusCode != HttpStatusCode.OK) throw new Exception(req.ReasonPhrase);
    }

    [JsonIgnore] public LocationDisplayModel DisplayInfo => this;

    public async Task<bool> SetupManagerInGui(Page parent)
    {
        if (password == null)
        {
            var r = await PasswordDialog.AskPassword(parent, false, "Enter account password").GetResult();
            if (!r.Item1)
            {
                password = null;
                return false;
            }

            password = r.Item2;
        }

        Task<HttpResponseMessage> requestTask;
        HttpResponseMessage result;
        var dialog = new DialogBuilder(parent).Title("Loading...").Content("Connecting to " + DisplayPath)
            .SecondaryButtonText("Cancel").Build();
        try
        {
            dialog.ShowAsync();
            requestTask = client.GetAsync(api.Endpoint($"api/checklogin?login={DisplayName}&password={password}"));
            await requestTask;
            result = requestTask.Result;
            dialog.Hide();
        }
        catch (Exception e)
        {
            dialog.Hide();
            await ExceptionDialog.ShowException(parent, e);
            return false;
        }

        if (result.StatusCode == HttpStatusCode.Unauthorized)
        {
            await ExceptionDialog.ShowException(parent,
                new ExceptionDialog.DialogException("Error", "Wrong login or password"));
            return false;
        }

        LastAccessTime = DateTime.Now;
        return true;
    }

    public void ResetOnFail()
    {
        password = null;
    }

    public bool IsValid()
    {
        return true;
    }

    public static async Task<DatabaseStorageManager> OpenWithConnectionCheck(string host, string login, string password)
    {
        var manager = new DatabaseStorageManager(host, login);
        manager.password = password;

        var requestTask = client.GetAsync(manager.api.Endpoint($"api/checklogin?login={login}&password={password}"));
        requestTask.Wait();
        if (requestTask.IsFaulted) throw requestTask.Exception;
        var result = requestTask.Result;
        if (result.StatusCode == HttpStatusCode.NotFound)
            throw new ExceptionDialog.DialogException("Wrong server link", "");
        if (result.StatusCode == HttpStatusCode.OK)
        {
            manager.LastAccessTime = DateTime.Now;
            return manager;
        }

        throw new ExceptionDialog.DialogException("Wrong login or password", "");
    }

    public static async Task<DatabaseStorageManager> RegisterWithConnectionCheck(string host, string login,
        string password)
    {
        var manager = new DatabaseStorageManager(host, login);
        manager.password = password;

        var requestTask = client.PutAsync(manager.api.Endpoint("api/register"), new StringContent(JsonTools.Serialize(
            new
            {
                login,
                password
            }), Encoding.UTF8, "application/json"));
        requestTask.Wait();
        if (requestTask.IsFaulted) throw requestTask.Exception;
        var result = requestTask.Result;
        if (result.StatusCode == HttpStatusCode.NotFound)
            throw new ExceptionDialog.DialogException("Wrong server link", "");
        if (result.StatusCode == HttpStatusCode.Conflict)
            throw new ExceptionDialog.DialogException("Account already exists", "");
        if (result.StatusCode != HttpStatusCode.OK)
            throw new ExceptionDialog.DialogException($"Error {result.StatusCode}", result.ReasonPhrase);
        return manager;
    }
}