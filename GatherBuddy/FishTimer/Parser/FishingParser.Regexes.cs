using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Dalamud.Game;

namespace GatherBuddy.FishTimer.Parser;

public partial class FishingParser
{
    private readonly struct Regexes
    {
        public Regex  Cast           { get; private init; }
        public string Undiscovered   { get; private init; }
        public Regex  AreaDiscovered { get; private init; }
        public Regex  Mooch          { get; private init; }

        public static Regexes FromLanguage(ClientLanguage lang)
        {
            return lang switch
            {
                ClientLanguage.English  => English.Value,
                ClientLanguage.German   => German.Value,
                ClientLanguage.French   => French.Value,
                ClientLanguage.Japanese => Japanese.Value,
                _                       => TraditionalChinese.Value,
            };
        }

        // @formatter:off


        private static readonly Lazy<Regexes> English = new( () => new Regexes
        {
            Cast           = new Regex(@"(?:You cast your|.*? casts (?:her|his)) line (?:on|in|at) (?<FishingSpot>.+)\.", RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            AreaDiscovered = new Regex(@".*?(on|at) (?<FishingSpot>.+) is added to your fishing log\.",                   RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Mooch          = new Regex(@"line with the fish still hooked.",                                               RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Undiscovered   = "undiscovered fishing hole",
        });

        private static readonly Lazy<Regexes> German = new(() => new Regexes
        {
            Cast           = new Regex(@".*? has?t mit dem Fischen (?<FishingSpotWithArticle>.+) begonnen\.(?<FishingSpot>invalid)?", RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            AreaDiscovered = new Regex(@"Die neue Angelstelle (?<FishingSpot>.*) wurde in deinem Fischer-Notizbuch vermerkt\.",       RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Mooch          = new Regex(@"ha[^\s]+ die Leine mit",                                                                     RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Undiscovered   = "unerforschten Angelplatz",
        });

        private static readonly Lazy<Regexes> French = new(() => new Regexes
        {
            Cast           = new Regex(@".*? commencez? à pêcher\.\s*Point de pêche: (?<FishingSpot>.+)\.",        RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            AreaDiscovered = new Regex(@"Vous notez le banc de poissons “(?<FishingSpot>.+)” dans votre carnet\.", RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Mooch          = new Regex(@"essa[^\s]+ de pêcher au vif avec",                                        RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Undiscovered   = "Zone de pêche inconnue",
        });

        private static readonly Lazy<Regexes> Japanese = new(() => new Regexes
        {
            Cast           = new Regex(@".+\u306f(?<FishingSpot>.+)で釣りを開始した。",               RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            AreaDiscovered = new Regex(@"釣り手帳に新しい釣り場「(?<FishingSpot>.+)」の情報を記録した！", RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Mooch          = new Regex(@"は釣り上げた.+を慎重に投げ込み、泳がせ釣りを試みた。",            RegexOptions.Compiled | RegexOptions.NonBacktracking | RegexOptions.ExplicitCapture),
            Undiscovered   = "未知の釣り場",
        });
        // TW: Traditional Chinese client messages. Cast / AreaDiscovered / Undiscovered verified in game; Mooch unverified.
        private static readonly Lazy<Regexes> TraditionalChinese = new(() => new Regexes
        {
            Cast           = new Regex(@".*?在(?<FishingSpot>.+?)甩出了魚線開始釣魚。",              RegexOptions.Compiled | RegexOptions.ExplicitCapture),
            AreaDiscovered = new Regex(@"將新釣場「(?<FishingSpot>.+)」記錄到了釣魚筆記中",              RegexOptions.Compiled | RegexOptions.ExplicitCapture),
            Mooch          = new Regex(@"以小釣大",                                                RegexOptions.Compiled | RegexOptions.ExplicitCapture),
            Undiscovered   = "在未知釣場甩出了魚線",
        });
        // @formatter:on
    }
}
