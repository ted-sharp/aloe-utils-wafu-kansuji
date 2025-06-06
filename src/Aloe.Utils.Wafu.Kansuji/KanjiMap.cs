// <copyright file="KanjiMap.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Aloe.Utils.Wafu.Kansuji;

/// <summary>
/// 漢数字の関連付けを行うクラス
/// 大字（壱、弐など）と通常の漢数字（一、二など）の変換や、
/// 漢数字からアラビア数字への変換に必要なマッピングを提供します。
/// </summary>
internal static class KanjiMap
{
    /// <summary>
    /// 大字から通常の漢数字への変換マップ
    /// 例：壱→一、弐→二
    /// </summary>
    internal static readonly Dictionary<char, char> DaijiToNormalMap = new()
    {
        ['壱'] = '一',
        ['弐'] = '二',
        ['参'] = '三',
        ['肆'] = '四',
        ['伍'] = '五',
        ['陸'] = '六',
        ['漆'] = '七',
        ['捌'] = '八',
        ['玖'] = '九',
        ['拾'] = '十',
        ['佰'] = '百',
        ['阡'] = '千',
        ['萬'] = '万',
    };

    /// <summary>
    /// 通常の漢数字から大字への変換マップ
    /// 例：一→壱、二→弐
    /// </summary>
    internal static readonly Dictionary<char, char> NormalToDaijiMap = new()
    {
        ['一'] = '壱',
        ['二'] = '弐',
        ['三'] = '参',
        ['四'] = '肆',
        ['五'] = '伍',
        ['六'] = '陸',
        ['七'] = '漆',
        ['八'] = '捌',
        ['九'] = '玖',
        ['十'] = '拾',
        ['百'] = '佰',
        ['千'] = '阡',
        ['万'] = '萬',
    };

    /// <summary>
    /// 正規化処理で使用する通常の漢数字のセット
    /// </summary>
    internal static readonly HashSet<char> NormalNumericChars =
    [
        '一',
        '二',
        '三',
        '四',
        '五',
        '六',
        '七',
        '八',
        '九',
        '十',
        '百',
        '千',
        '万',
    ];

    /// <summary>
    /// 漢数字からアラビア数字への変換マップ
    /// 例：一→1、二→2
    /// </summary>
    internal static readonly Dictionary<char, int> KanjiToArabicMap = new()
    {
        ['零'] = 0,
        ['〇'] = 0,
        ['一'] = 1,
        ['二'] = 2,
        ['三'] = 3,
        ['四'] = 4,
        ['五'] = 5,
        ['六'] = 6,
        ['七'] = 7,
        ['八'] = 8,
        ['九'] = 9,
    };

    /// <summary>
    /// 小単位（十・百・千）の数値マップ
    /// </summary>
    internal static readonly Dictionary<char, int> SmallUnitMap = new()
    {
        ['十'] = 10,
        ['百'] = 100,
        ['千'] = 1000,
    };

    /// <summary>
    /// 大単位（万・億）の数値マップ
    /// </summary>
    internal static readonly Dictionary<char, int> LargeUnitMap = new()
    {
        ['万'] = 1_0000,
        ['億'] = 1_0000_0000,
    };

    /// <summary>
    /// 漢数字と単位の連続部分を検出するための正規表現
    /// </summary>
    internal static readonly Regex KanjiNumberRegex = new(
        "[零〇一二三四五六七八九十百千万億兆]+",
        RegexOptions.Compiled
    );

    /// <summary>
    /// 漢数字（大字・通常）からアラビア数字の文字列への変換マップ
    /// 例：壱→"1"、一→"1"
    /// </summary>
    internal static readonly Dictionary<char, string> ArabicNumeralsMap = new()
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

    /// <summary>
    /// 位取り記数法で使用される漢数字のセット
    /// </summary>
    internal static readonly HashSet<char> PositionalDigits = ['〇', '零', '一', '二', '三', '四', '五', '六', '七', '八', '九'];
}
