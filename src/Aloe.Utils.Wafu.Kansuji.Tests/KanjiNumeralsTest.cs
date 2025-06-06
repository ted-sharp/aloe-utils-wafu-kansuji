namespace Aloe.Utils.Wafu.Kansuji.Tests;

using Xunit;
using Aloe.Utils.Wafu.Kansuji;

/// <summary>
/// KanjiNumerals クラスのテスト
/// </summary>
public class KanjiNumeralsTest
{
    // === Normalize テスト ===
    [InlineData("", "")]
    [InlineData("壱", "一")]
    [InlineData("弐", "二")]
    [InlineData("参", "三")]
    [InlineData("肆", "四")]
    [InlineData("伍", "五")]
    [InlineData("陸", "六")]
    [InlineData("漆", "七")]
    [InlineData("捌", "八")]
    [InlineData("玖", "九")]
    [InlineData("拾", "十")]
    [InlineData("佰", "百")]
    [InlineData("阡", "千")]
    [InlineData("萬", "万")]
    // 複数文字の連続
    [InlineData("拾壱", "十一")]
    [InlineData("二十三", "弐拾参")]
    // 非数値文字の混在
    [InlineData("二三年四月foo", "二三年四月foo")]  // Normalize は大字→通常漢字のみ行う
    [Theory(DisplayName = "大字が正しく通常の漢数字に変換されること")]
    public void Normalize_ConvertsDaijiToNormalKanji(string input, string expected)
    {
        var actual = KanjiNumerals.Normalize(input);
        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "Normalize に null を渡すと ArgumentNullException を投げること")]
    public void Normalize_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KanjiNumerals.Normalize(null!));
    }

    // === ConvertToDaiji テスト ===
    [InlineData("", "")]
    [InlineData("一", "壱")]
    [InlineData("二", "弐")]
    [InlineData("三", "参")]
    [InlineData("四", "肆")]
    [InlineData("五", "伍")]
    [InlineData("六", "陸")]
    [InlineData("七", "漆")]
    [InlineData("八", "捌")]
    [InlineData("九", "玖")]
    [InlineData("十", "拾")]
    [InlineData("百", "佰")]
    [InlineData("千", "阡")]
    [InlineData("万", "萬")]
    // 複数文字の連続
    [InlineData("二十三", "弐拾参")]
    // 非数値文字の混在
    [InlineData("二三年四月foo", "弐参年肆月foo")]
    [Theory(DisplayName = "通常の漢数字が正しく大字に変換されること")]
    public void ConvertToDaiji_ConvertsNormalKanjiToDaiji(string input, string expected)
    {
        var actual = KanjiNumerals.ConvertToDaiji(input);
        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "ConvertToDaiji に null を渡すと ArgumentNullException を投げること")]
    public void ConvertToDaiji_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KanjiNumerals.ConvertToDaiji(null!));
    }

    // === ConvertToPositionalNotation テスト ===
    [InlineData("", "")]
    [InlineData("一千九百五十七", "一九五七")]
    [InlineData("二千二十三年", "二〇二三年")]
    [InlineData("三百六十五日", "三六五日")]
    // ゼロの扱い
    [InlineData("零年", "〇年")]
    [Theory(DisplayName = "漢数字が正しく位取り表記に変換されること")]
    public void ConvertToPositionalNotation_ConvertsToPositionalNotation(string input, string expected)
    {
        var actual = KanjiNumerals.ConvertToPositionalNotation(input);
        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "ConvertToPositionalNotation に null を渡すと ArgumentNullException を投げること")]
    public void ConvertToPositionalNotation_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KanjiNumerals.ConvertToPositionalNotation(null!));
    }

    // === ConvertToLargeNumbersNotation テスト ===
    [InlineData("", "")]
    [InlineData("一九五七", "一千九百五十七")]
    [InlineData("二〇二三年", "二千二十三年")]
    [InlineData("三六五日", "三百六十五日")]
    // 複数文字の連続
    [InlineData("一二三四", "一千二百三十四")]
    [Theory(DisplayName = "位取り表記が正しく大数表記に変換されること")]
    public void ConvertToLargeNumbersNotation_ConvertsToLargeNumbersNotation(string input, string expected)
    {
        var actual = KanjiNumerals.ConvertToLargeNumbersNotation(input);
        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "ConvertToLargeNumbersNotation に null を渡すと ArgumentNullException を投げること")]
    public void ConvertToLargeNumbersNotation_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KanjiNumerals.ConvertToLargeNumbersNotation(null!));
    }

    // === ConvertToArabicNumerals テスト ===
    [Theory(DisplayName = "漢数字が正しくアラビア数字に変換されること")]
    [InlineData("", "")]
    [InlineData("一", "1")]
    [InlineData("二", "2")]
    [InlineData("三", "3")]
    [InlineData("四", "4")]
    [InlineData("五", "5")]
    [InlineData("六", "6")]
    [InlineData("七", "7")]
    [InlineData("八", "8")]
    [InlineData("九", "9")]
    [InlineData("零", "0")]
    [InlineData("壱", "1")]
    [InlineData("弐", "2")]
    [InlineData("参", "3")]
    [InlineData("肆", "4")]
    [InlineData("伍", "5")]
    [InlineData("陸", "6")]
    [InlineData("漆", "7")]
    [InlineData("捌", "8")]
    [InlineData("玖", "9")]
    // 複数文字の連続
    [InlineData("拾一", "11")]
    // 非数値文字を保持
    [InlineData("参年A", "3年A")]
    public void ConvertToArabicNumerals_ConvertsToArabicNumerals(string input, string expected)
    {
        var actual = KanjiNumerals.ConvertToArabicNumerals(input);
        Assert.Equal(expected, actual);
    }

    [Fact(DisplayName = "ConvertToArabicNumerals に null を渡すと ArgumentNullException を投げること")]
    public void ConvertToArabicNumerals_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => KanjiNumerals.ConvertToArabicNumerals(null!));
    }
}
