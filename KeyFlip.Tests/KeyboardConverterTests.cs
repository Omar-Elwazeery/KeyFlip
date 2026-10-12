using KeyFlip;
using Xunit;

public class KeyboardConverterTests
{
    // "salam" typed on an English keyboard while the Arabic layout was active.
    private const string Salam = "\u0633\u0644\u0627\u0645"; // seen-lam-alef-meem

    [Fact] public void EnglishToArabic_Salam() =>
        Assert.Equal(Salam, KeyboardConverter.Convert("sghl", KeyboardConverter.Direction.EnglishToArabic));

    // Plain lam-alef defaults to the b key, because "sghl" is not an English word.
    [Fact] public void ArabicToEnglish_LamAlefDefaultsToB() =>
        Assert.Equal("sbl", KeyboardConverter.Convert(Salam, KeyboardConverter.Direction.ArabicToEnglish));

    [Fact] public void AutoDetect_PicksDirection()
    {
        Assert.Equal(Salam, KeyboardConverter.Convert("sghl"));
        Assert.Equal("about", KeyboardConverter.Convert(KeyboardConverter.Convert("about")));
    }

    // English typed while the Arabic layout was active must come back exactly.
    [Theory]
    [InlineData("about")]                       // b key
    [InlineData("Good")]                        // G key (lam + alef-hamza-above)
    [InlineData("TABLE")]                       // T and B keys
    [InlineData("night")]                       // g then h
    [InlineData("bought")]                      // b key and g then h in one word
    [InlineData("Though")]
    [InlineData("highlights")]                  // listed word + ending
    [InlineData("doubt")]                       // "dought" is not a word
    [InlineData("I bought a big light bulb.")]
    public void EnglishWords_RoundTrip(string english)
    {
        string typedInArabic = KeyboardConverter.Convert(english, KeyboardConverter.Direction.EnglishToArabic);
        Assert.Equal(english, KeyboardConverter.Convert(typedInArabic, KeyboardConverter.Direction.ArabicToEnglish));
    }

    [Fact] public void Detect_TieOrNoLetters_IsEnglishToArabic()
    {
        Assert.Equal(KeyboardConverter.Direction.EnglishToArabic, KeyboardConverter.Detect("ab" + "سل"));
        Assert.Equal(KeyboardConverter.Direction.EnglishToArabic, KeyboardConverter.Detect("123 !"));
    }

    [Fact] public void Detect_CountsDiacriticsAsArabic() =>
        Assert.Equal(KeyboardConverter.Direction.ArabicToEnglish, KeyboardConverter.Detect("aَُ"));

    [Fact] public void LamAlefKey_ProducesTwoCharacters() =>
        Assert.Equal("\u0644\u0627", KeyboardConverter.Convert("b", KeyboardConverter.Direction.EnglishToArabic));

    [Fact] public void Digits_Spaces_Newlines_Unchanged() =>
        Assert.Equal("123 \r\n 45", KeyboardConverter.Convert("123 \r\n 45", KeyboardConverter.Direction.EnglishToArabic));

    [Fact] public void EmptyString_ReturnsEmpty() =>
        Assert.Equal("", KeyboardConverter.Convert(""));

    [Fact] public void Table_NoDuplicateEnglishKeys() =>
        Assert.Equal(KeyboardConverter.Table.Count, KeyboardConverter.Table.Select(t => t.En).Distinct().Count());

    [Fact] public void Table_NoDuplicateSingleArabicOutputs()
    {
        var singles = KeyboardConverter.Table.Where(t => t.Ar.Length == 1).Select(t => t.Ar).ToList();
        Assert.Equal(singles.Count, singles.Distinct().Count());
    }

    [Fact] public void RoundTrip_EverySingleCharKey()
    {
        foreach (var (en, ar) in KeyboardConverter.Table.Where(t => t.Ar.Length == 1))
        {
            Assert.Equal(ar, KeyboardConverter.Convert(en.ToString(), KeyboardConverter.Direction.EnglishToArabic));
            Assert.Equal(en.ToString(), KeyboardConverter.Convert(ar, KeyboardConverter.Direction.ArabicToEnglish));
        }
    }

    [Fact] public void FullAlphabetRoundTrip_NoMultiCharKeys()
    {
        const string s = "qwertyuiop asdfhjkl zxcvnm";           // no b/g-shifted multi-char keys
        var ar = KeyboardConverter.Convert(s, KeyboardConverter.Direction.EnglishToArabic);
        Assert.Equal(s, KeyboardConverter.Convert(ar, KeyboardConverter.Direction.ArabicToEnglish));
    }
}
