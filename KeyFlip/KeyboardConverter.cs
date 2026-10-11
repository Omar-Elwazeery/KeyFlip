using System.Text;

namespace KeyFlip;

/// <summary>
/// Converts text between English QWERTY and the Windows "Arabic (101)" layout
/// by KEY POSITION. It never translates or spell-corrects.
/// </summary>
public static class KeyboardConverter
{
    // One row per physical key press: (English character, Arabic character(s) produced by the same key).
    // Arabic text is written as \u escapes so the file survives any editor/encoding.
    // Four keys (b, T, G, B) produce TWO Arabic characters (lam + alef variant).
    private static readonly (char En, string Ar)[] KeyTable =
    {
        ('`', "\u0630"),  // key grave
        ('q', "\u0636"),  // key q
        ('w', "\u0635"),  // key w
        ('e', "\u062B"),  // key e
        ('r', "\u0642"),  // key r
        ('t', "\u0641"),  // key t
        ('y', "\u063A"),  // key y
        ('u', "\u0639"),  // key u
        ('i', "\u0647"),  // key i
        ('o', "\u062E"),  // key o
        ('p', "\u062D"),  // key p
        ('[', "\u062C"),  // key [
        (']', "\u062F"),  // key ]
        ('a', "\u0634"),  // key a
        ('s', "\u0633"),  // key s
        ('d', "\u064A"),  // key d
        ('f', "\u0628"),  // key f
        ('g', "\u0644"),  // key g
        ('h', "\u0627"),  // key h
        ('j', "\u062A"),  // key j
        ('k', "\u0646"),  // key k
        ('l', "\u0645"),  // key l
        (';', "\u0643"),  // key ;
        ('\'', "\u0637"),  // key '
        ('z', "\u0626"),  // key z
        ('x', "\u0621"),  // key x
        ('c', "\u0624"),  // key c
        ('v', "\u0631"),  // key v
        ('b', "\u0644\u0627"),  // key b (lam-alef)
        ('n', "\u0649"),  // key n
        ('m', "\u0629"),  // key m
        (',', "\u0648"),  // key ,
        ('.', "\u0632"),  // key .
        ('/', "\u0638"),  // key /
        ('~', "\u0651"),  // key shift+grave
        ('Q', "\u064E"),  // key Q
        ('W', "\u064B"),  // key W
        ('E', "\u064F"),  // key E
        ('R', "\u064C"),  // key R
        ('T', "\u0644\u0625"),  // key T (lam-alef-hamza-below)
        ('Y', "\u0625"),  // key Y
        ('U', "`"),  // key U
        ('I', "\u00F7"),  // key I
        ('O', "\u00D7"),  // key O
        ('P', "\u061B"),  // key P
        ('{', "<"),  // key shift+[
        ('}', ">"),  // key shift+]
        ('A', "\u0650"),  // key A
        ('S', "\u064D"),  // key S
        ('D', "]"),  // key D
        ('F', "["),  // key F
        ('G', "\u0644\u0623"),  // key G (lam-alef-hamza-above)
        ('H', "\u0623"),  // key H
        ('J', "\u0640"),  // key J
        ('K', "\u060C"),  // key K
        ('L', "/"),  // key L
        ('Z', "~"),  // key Z
        ('X', "\u0652"),  // key X
        ('C', "}"),  // key C
        ('V', "{"),  // key V
        ('B', "\u0644\u0622"),  // key B (lam-alef-madda)
        ('N', "\u0622"),  // key N
        ('M', "\'"),  // key M
        ('<', ","),  // key shift+,
        ('>', "."),  // key shift+.
        ('?', "\u061F"),  // key shift+/
    };

    private static readonly Dictionary<char, string> EnToAr = BuildEnToAr();
    private static readonly Dictionary<char, char> ArToEn = BuildArToEn();

    private static Dictionary<char, string> BuildEnToAr()
    {
        var d = new Dictionary<char, string>();
        foreach (var (en, ar) in KeyTable) d.Add(en, ar);
        return d;
    }

    // Reverse map uses only single-character outputs. A two-character output such as
    // lam+alef is already covered by its two single characters (g then h), because
    // the text "lam alef" is identical whether typed as g,h or as the b key.
    private static Dictionary<char, char> BuildArToEn()
    {
        var d = new Dictionary<char, char>();
        foreach (var (en, ar) in KeyTable)
            if (ar.Length == 1) d.Add(ar[0], en);
        return d;
    }

    public enum Direction { EnglishToArabic, ArabicToEnglish }

    /// <summary>Majority vote: Arabic letters vs ASCII letters. Ties or no letters => EnglishToArabic.</summary>
    public static Direction Detect(string text)
    {
        int arabic = 0, latin = 0;
        foreach (char c in text)
        {
            if (c >= '\u0600' && c <= '\u06FF') arabic++;
            else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) latin++;
        }
        return arabic > latin ? Direction.ArabicToEnglish : Direction.EnglishToArabic;
    }

    public static string Convert(string text, Direction direction)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (direction == Direction.EnglishToArabic)
                sb.Append(EnToAr.TryGetValue(c, out var ar) ? ar : c.ToString());
            else
                sb.Append(ArToEn.TryGetValue(c, out var en) ? en : c);
        }
        return sb.ToString();
    }

    /// <summary>Auto-detects direction, then converts.</summary>
    public static string Convert(string text) => Convert(text, Detect(text));

    // Exposed for tests only.
    public static IReadOnlyList<(char En, string Ar)> Table => KeyTable;
}
