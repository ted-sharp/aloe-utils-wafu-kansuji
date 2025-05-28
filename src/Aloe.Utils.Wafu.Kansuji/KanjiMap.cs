// <copyright file="KanjiMap.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Aloe.Utils.Wafu.Kansuji;

/// <summary>
/// 漢数字の関連付け
/// </summary>
internal static class KanjiMap
{
    // 大字→通常漢数字マップ
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

    // 通常漢数字→大字マップ
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

    // Normalize 用に判定する通常漢数字セット
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

    // 小単位マップ（十・百・千）
    internal static readonly Dictionary<char, int> SmallUnitMap = new()
    {
        ['十'] = 10,
        ['百'] = 100,
        ['千'] = 1000,
    };

    // 大単位マップ（万・億・兆）
    internal static readonly Dictionary<char, int> LargeUnitMap = new()
    {
        ['万'] = 1_0000,
        ['億'] = 1_0000_0000,
    };
}
