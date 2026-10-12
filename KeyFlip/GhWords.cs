namespace KeyFlip;

/// <summary>
/// English words spelled with "gh". On the Arabic (101) layout the "b" key and the keys
/// "g" then "h" both type lam-alef, so when converting back to English this list decides
/// when a lam-alef was "gh" rather than the far more common "b".
/// </summary>
internal static class GhWords
{
    // Lower case. Leave out words whose "b" spelling is also a common word (e.g. "ghee" / "bee", "hugh" / "hub").
    private static readonly HashSet<string> Words = new(StringComparer.Ordinal)
    {
        // -igh-
        "high", "highway", "highland", "highlight", "thigh", "sigh", "sight", "eyesight", "insight",
        "foresight", "hindsight", "oversight", "night", "tonight", "midnight", "overnight", "fortnight",
        "nightmare", "nightclub", "nightlife", "nightfall", "knight", "light", "delight", "daylight",
        "sunlight", "moonlight", "spotlight", "flashlight", "headlight", "lighthouse", "lightweight",
        "lightning", "right", "alright", "bright", "copyright", "birthright", "downright", "outright",
        "upright", "righteous", "playwright", "wright", "might", "almighty", "fight", "fright", "flight",
        "plight", "slight", "blight", "tight", "airtight", "eight", "eighteen", "eighty", "eighties",
        "eighth", "height", "weight", "heavyweight", "overweight", "freight", "weigh", "neigh",
        "neighbor", "neighbour", "neighborhood", "neighbourhood", "sleigh", "straight", "straightforward",
        // -ough-
        "though", "although", "thought", "afterthought", "forethought", "through", "throughout",
        "breakthrough", "thorough", "borough", "enough", "rough", "tough", "cough", "dough", "doughnut",
        "trough", "bough", "plough", "slough", "ought", "bought", "brought", "fought", "sought", "nought",
        "wrought", "drought", "hiccough",
        // -augh-
        "laugh", "laughter", "caught", "taught", "daughter", "naughty", "slaughter", "haughty",
        "distraught", "fraught", "draught", "onslaught",
        // other
        "ghost", "ghastly", "aghast", "ghetto", "ghoul", "gherkin", "spaghetti", "afghan", "afghanistan",
        "dinghy", "yoghurt", "sorghum", "ghana", "baghdad", "shanghai", "edinburgh", "pittsburgh",
        "doghouse", "foghorn", "egghead", "bighorn", "longhorn", "longhand", "stronghold",
        "birmingham", "nottingham", "buckingham", "cunningham",
    };

    private static readonly string[] Suffixes =
    {
        "s", "es", "ed", "er", "ers", "est", "ly", "ing", "ings", "en", "ened", "ening", "ens",
        "y", "ier", "iest", "ily", "ful", "fully", "ness", "able", "ably",
    };

    /// <summary>True if the lower-case word is listed, optionally followed by a common ending.</summary>
    public static bool IsKnown(string word)
    {
        if (Words.Contains(word)) return true;
        foreach (string suffix in Suffixes)
            if (word.Length > suffix.Length && word.EndsWith(suffix, StringComparison.Ordinal)
                && Words.Contains(word[..^suffix.Length]))
                return true;
        return false;
    }
}
