// <copyright file="KanjiNumerals.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

using System.Text;
using System.Text.RegularExpressions;
using static Aloe.Utils.Wafu.Kansuji.KanjiMap;

namespace Aloe.Utils.Wafu.Kansuji;

/// <summary>
/// 漢数字を扱うユーティリティクラスです。
/// </summary>
public static class KanjiNumerals
{
    // 正規表現：漢数字・単位の連続部分
    private static readonly Regex KanjiNumberRegex = new(
        "[零〇一二三四五六七八九十百千万億兆]+",
        RegexOptions.Compiled
    );

    /// <summary>
    /// 大字を通常漢数字に変換し、かつ数値のみの文字列では逆方向の大字変換も行います。
    /// </summary>
    /// <param name="input">変換対象の文字列。大字、通常漢数字、その他の文字を含むことができます。</param>
    /// <returns>変換後の文字列。大字は通常漢数字に、数値のみの文字列の場合は通常漢数字を大字に変換します。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/>が<see langword="null"/>の場合にスローされます。</exception>
    /// <example>
    /// <code>
    /// var result = KanjiNumerals.Normalize("壱拾参"); // "一十三"を返します
    /// var result2 = KanjiNumerals.Normalize("一二三"); // "壱弐参"を返します（数値のみの場合）
    /// </code>
    /// </example>
    public static string Normalize(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        bool numericOnly = input.Length > 0 && input.All(c => NormalNumericChars.Contains(c));
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (KanjiMap.DaijiToNormalMap.TryGetValue(c, out var normal))
            {
                sb.Append(normal);
            }
            else if (numericOnly && NormalToDaijiMap.TryGetValue(c, out var daiji))
            {
                sb.Append(daiji);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 通常漢数字をすべて大字に変換します。
    /// </summary>
    /// <param name="input">変換対象の文字列。通常漢数字、その他の文字を含むことができます。</param>
    /// <returns>変換後の文字列。すべての通常漢数字が大字に変換されます。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/>が<see langword="null"/>の場合にスローされます。</exception>
    /// <example>
    /// <code>
    /// var result = KanjiNumerals.ConvertToDaiji("一十三"); // "壱拾参"を返します
    /// </code>
    /// </example>
    public static string ConvertToDaiji(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (NormalToDaijiMap.TryGetValue(c, out var daiji))
            {
                sb.Append(daiji);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 漢数字表記（大単位対応）を数値に変換します。
    /// </summary>
    /// <param name="s">変換対象の漢数字表記の文字列。</param>
    /// <returns>変換された数値。</returns>
    private static long ParseJapaneseNumber(string s)
    {
        if (String.IsNullOrEmpty(s))
        {
            return 0;
        }

        // 大単位ごとに再帰的に分割
        foreach (var kvp in LargeUnitMap)
        {
            var unitChar = kvp.Key;
            var unitValue = kvp.Value;
            var idx = s.IndexOf(unitChar);
            if (idx >= 0)
            {
                var left = s.Substring(0, idx);
                var right = s.Substring(idx + 1);

                // 左側が空なら「一」とみなす
                var leftValue = String.IsNullOrEmpty(left)
                    ? 1L
                    : ParseJapaneseNumber(left);

                // 大単位の乗算結果 + 残りを再帰
                return leftValue * unitValue + ParseJapaneseNumber(right);
            }
        }

        // 大単位なし → 小単位のみの解析
        return ParseSmallUnits(s);
    }

    /// <summary>
    /// 漢数字表記（十・百・千のみ）を数値に変換します。
    /// </summary>
    private static long ParseSmallUnits(string s)
    {
        long total = 0;
        long current = 0;
        foreach (var c in s)
        {
            if (KanjiToArabicMap.TryGetValue(c, out var digit))
            {
                current = digit;
            }
            else if (SmallUnitMap.TryGetValue(c, out var unit))
            {
                // 「十」だけなど current==0 の場合は 1 とみなす
                if (current == 0)
                {
                    current = 1;
                }

                total += current * unit;
                current = 0;
            }
            else
            {
                // 未知の文字は無視
            }
        }

        return total + current;
    }

    /// <summary>
    /// 文字列中の漢数字・単位連続部分を位取り漢数字に変換します。
    /// </summary>
    public static string ConvertToPositionalNotation(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return KanjiNumberRegex.Replace(input, m =>
        {
            var numStr = m.Value;
            var value = ParseJapaneseNumber(numStr);
            var digits = value.ToString();

            var sb = new StringBuilder(digits.Length);
            foreach (var ch in digits)
            {
                sb.Append(DigitToKanji(ch));
            }

            return sb.ToString();
        });
    }

    /// <summary>
    /// 指定された文字が漢数字（零、〇、一～九）かどうかを判定します。
    /// </summary>
    /// <param name="c">判定対象の文字。</param>
    /// <returns>漢数字の場合は<see langword="true"/>、それ以外の場合は<see langword="false"/>。</returns>
    private static bool IsNumeric(char c) => c == '零' || c == '〇' || (c >= '一' && c <= '九');

    /// <summary>
    /// 指定された文字が単位（十、百、千、万）かどうかを判定します。
    /// </summary>
    /// <param name="c">判定対象の文字。</param>
    /// <returns>単位の場合は<see langword="true"/>、それ以外の場合は<see langword="false"/>。</returns>
    private static bool IsUnit(char c) => c == '十' || c == '百' || c == '千' || c == '万';

    /// <summary>
    /// アラビア数字を漢数字に変換します。
    /// </summary>
    /// <param name="d">変換対象のアラビア数字文字。</param>
    /// <returns>対応する漢数字。</returns>
    private static char DigitToKanji(char d) => d switch
    {
        '0' => '〇',
        '1' => '一',
        '2' => '二',
        '3' => '三',
        '4' => '四',
        '5' => '五',
        '6' => '六',
        '7' => '七',
        '8' => '八',
        '9' => '九',
        _ => d,
    };

    /// <summary>
    /// 位取り表記から大数単位（千・百・十）を付与した漢数字表記に変換します。
    /// </summary>
    /// <param name="input">変換対象の文字列。位取り形式の漢数字とその他の文字を含むことができます。</param>
    /// <returns>変換後の文字列。数値部分が大数単位付きの漢数字に変換されます。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/>が<see langword="null"/>の場合にスローされます。</exception>
    /// <example>
    /// <code>
    /// var result = KanjiNumerals.ConvertToLargeNumbersNotation("一三"); // "十三"を返します
    /// </code>
    /// </example>
    public static string ConvertToLargeNumbersNotation(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // 数字部／サフィックス分離
        var i = 0;
        var posDigits = new HashSet<char> { '〇', '零', '一', '二', '三', '四', '五', '六', '七', '八', '九' };
        while (i < input.Length && posDigits.Contains(input[i]))
        {
            i++;
        }

        var numPart = input[..i];
        var suffix = input[i..];
        if (numPart.Length == 0)
        {
            return suffix;
        }

        // 数字文字を英数字に
        var sbNum = new StringBuilder(numPart.Length);
        foreach (var c in numPart)
        {
            sbNum.Append(KanjiDigitToChar(c));
        }

        var digits = sbNum.ToString();

        var sb = new StringBuilder();
        var L = digits.Length;
        for (var idx = 0; idx < L; idx++)
        {
            var pos = L - idx;
            var d = digits[idx] - '0';
            if (pos > 1)
            {
                if (d == 0)
                {
                    continue;
                }

                sb.Append(DigitToKanji(digits[idx]));
                sb.Append(pos switch { 4 => '千', 3 => '百', 2 => '十', _ => '\0' });
            }
            else
            {
                if (d > 0 || L == 1)
                {
                    sb.Append(DigitToKanji(digits[idx]));
                }
            }
        }

        sb.Append(suffix);
        return sb.ToString();
    }

    /// <summary>
    /// 漢数字をアラビア数字に変換します。
    /// </summary>
    /// <param name="c">変換対象の漢数字文字。</param>
    /// <returns>対応するアラビア数字文字。</returns>
    private static char KanjiDigitToChar(char c) => c switch
    {
        '零' or '〇' => '0',
        '一' => '1',
        '二' => '2',
        '三' => '3',
        '四' => '4',
        '五' => '5',
        '六' => '6',
        '七' => '7',
        '八' => '8',
        '九' => '9',
        _ => c,
    };

    /// <summary>
    /// 漢数字および大字を 0–9 のアラビア数字文字列に変換します。
    /// </summary>
    /// <param name="input">変換対象の文字列。漢数字、大字、その他の文字を含むことができます。</param>
    /// <returns>変換後の文字列。漢数字と大字がアラビア数字に変換されます。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="input"/>が<see langword="null"/>の場合にスローされます。</exception>
    /// <example>
    /// <code>
    /// var result = KanjiNumerals.ConvertToArabicNumerals("壱拾参"); // "13"を返します
    /// var result2 = KanjiNumerals.ConvertToArabicNumerals("一十三"); // "13"を返します
    /// </code>
    /// </example>
    public static string ConvertToArabicNumerals(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var map = new Dictionary<char, string>
        {
            ['壱'] = "1",
            ['一'] = "1",
            ['十'] = "1",
            ['拾'] = "1",
            ['弐'] = "2",
            ['二'] = "2",
            ['参'] = "3",
            ['三'] = "3",
            ['肆'] = "4",
            ['四'] = "4",
            ['伍'] = "5",
            ['五'] = "5",
            ['陸'] = "6",
            ['六'] = "6",
            ['漆'] = "7",
            ['七'] = "7",
            ['捌'] = "8",
            ['八'] = "8",
            ['玖'] = "9",
            ['九'] = "9",
            ['零'] = "0",
            ['〇'] = "0",
        };

        var sb = new StringBuilder();
        foreach (var c in input)
        {
            if (map.TryGetValue(c, out var digit))
            {
                sb.Append(digit);
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
