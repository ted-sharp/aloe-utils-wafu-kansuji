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
    /// <summary>
    /// 通常漢数字（小字: 一・二・三など）を大字（壱・弐・参など）に変換します。
    /// その他の文字はそのまま保持されます。
    /// </summary>
    /// <param name="input">変換対象の文字列。</param>
    /// <returns>小字を大字に変換した文字列。</returns>
    /// <exception cref="ArgumentNullException">input が null の場合にスローされます。</exception>
    public static string ConvertToDaiji(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // 入力長分のスタックバッファを確保
        Span<char> buffer = stackalloc char[input.Length];
        using var vsb = new ValueStringBuilder(buffer);

        foreach (char c in input)
        {
            if (NormalToDaijiMap.TryGetValue(c, out var daiji))
            {
                // 小字を大字に
                vsb.Append(daiji);
            }
            else
            {
                // その他はそのまま
                vsb.Append(c);
            }
        }

        return vsb.ToString();
    }

    /// <summary>
    /// 大字（壱・弐・参など）を通常漢数字（小字）に変換します。
    /// その他の文字はそのまま保持されます。
    /// </summary>
    /// <param name="input">変換対象の文字列。</param>
    /// <returns>大字を小字に変換した文字列。</returns>
    /// <exception cref="ArgumentNullException">input が null の場合にスローされます。</exception>
    public static string ConvertToShoji(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // 入力長分のスタックバッファを確保
        Span<char> buffer = stackalloc char[input.Length];
        using var vsb = new ValueStringBuilder(buffer);

        foreach (char c in input)
        {
            if (DaijiToNormalMap.TryGetValue(c, out var normal))
            {
                // 大字を小字に
                vsb.Append(normal);
            }
            else
            {
                // その他はそのまま
                vsb.Append(c);
            }
        }

        return vsb.ToString();
    }

    /// <summary>
    /// 漢数字表記（大単位対応）を数値に変換します。
    /// </summary>
    /// <param name="s">変換対象の漢数字表記の文字列。</param>
    /// <returns>変換された数値。</returns>
    private static decimal ParseJapaneseNumber(string s)
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
                    ? 1M
                    : ParseJapaneseNumber(left);

                // 大単位の乗算結果 + 残りを再帰
                return (leftValue * unitValue) + ParseJapaneseNumber(right);
            }
        }

        // 大単位なし → 小単位のみの解析
        return ParseSmallUnits(s);
    }

    /// <summary>
    /// 漢数字表記（十・百・千のみ）を数値に変換します。
    /// </summary>
    private static decimal ParseSmallUnits(string s)
    {
        decimal total = 0;
        decimal current = 0;
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

            // digits.Length 文字分のスタックバッファを確保（最大 32 桁程度なら十分）
            int stackSize = Math.Min(digits.Length, 32);
            Span<char> initialBuffer = stackalloc char[stackSize];
            using var vsbInner = new ValueStringBuilder(initialBuffer);

            foreach (var ch in digits)
            {
                vsbInner.Append(DigitToKanji(ch));
            }

            return vsbInner.ToString();
        });
    }

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
    /// <remarks>
    /// <para>
    /// このメソッドは以下のような変換を行います：
    /// </para>
    /// <list type="bullet">
    /// <item><description>位取り表記（例：「一三」）を大数単位付き表記（例：「十三」）に変換します。</description></item>
    /// <item><description>数値部分のみを変換し、それ以外の文字（サフィックス）はそのまま保持します。</description></item>
    /// <item><description>零（〇）は、一の位以外では省略されます。</description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = KanjiNumerals.ConvertToLargeNumbersNotation("一三"); // "十三"を返します
    /// var result2 = KanjiNumerals.ConvertToLargeNumbersNotation("一三円"); // "十三円"を返します
    /// var result3 = KanjiNumerals.ConvertToLargeNumbersNotation("一〇三"); // "百三"を返します
    /// </code>
    /// </example>
    public static string ConvertToLargeNumbersNotation(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // 数字部／サフィックス分離
        var i = 0;
        while (i < input.Length && PositionalDigits.Contains(input[i]))
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

        // 多くても input.Length ～ input.Length*2 文字程度になる想定のため
        Span<char> initialBuffer = stackalloc char[input.Length];
        using var vsb = new ValueStringBuilder(initialBuffer);

        foreach (var c in input)
        {
            if (ArabicNumeralsMap.TryGetValue(c, out var digit))
            {
                // digit は string 型なので、Append(string) を呼ぶ
                vsb.Append(digit);
            }
            else
            {
                vsb.Append(c);
            }
        }

        return vsb.ToString();
    }
}
