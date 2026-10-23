using System;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace GatherBuddy.Utility;

public readonly struct MultiString(string en, string de, string fr, string jp, string tc)
{
    public MultiString(string en, string de, string fr, string jp)
        : this(en, de, fr, jp, en)
    { }

    public static string ParseSeStringLumina(ReadOnlySeString? luminaString)
        => luminaString?.ExtractText() ?? string.Empty;

    // TW: fall back to Traditional Chinese when the client has no data for a language.
    public readonly string English  = en.Length > 0 ? en : tc;
    public readonly string German   = de.Length > 0 ? de : (en.Length > 0 ? en : tc);
    public readonly string French   = fr.Length > 0 ? fr : (en.Length > 0 ? en : tc);
    public readonly string Japanese = jp.Length > 0 ? jp : (en.Length > 0 ? en : tc);
    public readonly string Chinese  = tc.Length > 0 ? tc : en;

    public string this[ClientLanguage lang]
        => Name(lang);

    public override string ToString()
        => Name(ClientLanguage.English);

    public string ToWholeString()
        => $"{English}|{German}|{French}|{Japanese}|{Chinese}";

    // TW: the Taiwan client may not ship every language; return an empty name instead of throwing.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, ClientLanguage?), bool> MissingSheets = new();

    private static string SafeName<T>(IDataManager gameData, ClientLanguage? lang, uint id, Func<T, ReadOnlySeString> name)
        where T : struct, Lumina.Excel.IExcelRow<T>
    {
        if (MissingSheets.ContainsKey((typeof(T), lang)))
            return string.Empty;

        try
        {
            var row = lang.HasValue
                ? gameData.GetExcelSheet<T>(lang.Value).GetRowOrDefault(id)
                : gameData.GetExcelSheet<T>().GetRowOrDefault(id);
            return row.HasValue ? ParseSeStringLumina(name(row.Value)) : string.Empty;
        }
        catch
        {
            MissingSheets.TryAdd((typeof(T), lang), true);
            return string.Empty;
        }
    }


    public static MultiString FromPlaceName(IDataManager gameData, uint id)
    {
        var en = SafeName<PlaceName>(gameData, ClientLanguage.English,  id, r => r.Name);
        var de = SafeName<PlaceName>(gameData, ClientLanguage.German,   id, r => r.Name);
        var fr = SafeName<PlaceName>(gameData, ClientLanguage.French,   id, r => r.Name);
        var jp = SafeName<PlaceName>(gameData, ClientLanguage.Japanese, id, r => r.Name);
        var tc = SafeName<PlaceName>(gameData, null,                    id, r => r.Name);
        return new MultiString(en, de, fr, jp, tc);
    }

    public static MultiString FromItem(IDataManager gameData, uint id)
    {
        var en = SafeName<Item>(gameData, ClientLanguage.English,  id, r => r.Name);
        var de = SafeName<Item>(gameData, ClientLanguage.German,   id, r => r.Name);
        var fr = SafeName<Item>(gameData, ClientLanguage.French,   id, r => r.Name);
        var jp = SafeName<Item>(gameData, ClientLanguage.Japanese, id, r => r.Name);
        var tc = SafeName<Item>(gameData, null,                    id, r => r.Name);
        return new MultiString(en, de, fr, jp, tc);
    }

    private string Name(ClientLanguage lang)
        => lang switch
        {
            ClientLanguage.English  => English,
            ClientLanguage.German   => German,
            ClientLanguage.Japanese => Japanese,
            ClientLanguage.French   => French,
            _                       => Chinese,
        };

    public static readonly MultiString Empty = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
}
