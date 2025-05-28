# Aloe.Utils.Wafu.Kansuji

[![NuGet Version](https://img.shields.io/nuget/v/Aloe.Utils.Wafu.Kansuji.svg)](https://www.nuget.org/packages/Aloe.Utils.Wafu.Kansuji)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Aloe.Utils.Wafu.Kansuji.svg)](https://www.nuget.org/packages/Aloe.Utils.Wafu.Kansuji)
[![License](https://img.shields.io/github/license/ted-sharp/aloe-utils-wafu-kansuji.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)

`Aloe.Utils.Wafu.Kansuji` は、日本語の漢数字と大字を扱うためのユーティリティライブラリです。

## 主な機能

* 漢数字(一二三)、大字(壱弐参)、位置取り記法(一九五七)、大数(十千万億)、アラビア数字(123)への変換
* 非数値文字は変換されず、そのまま保持

## 対応環境

* .NET 9 以降

## インストール

NuGet パッケージマネージャーからインストール：

```cmd
Install-Package Aloe.Utils.Wafu.Kansuji
```

または、.NET CLI で：

```cmd
dotnet add package Aloe.Utils.Wafu.Kansuji
```

## 使用例

```csharp
using Aloe.Utils.Wafu.Kansuji;

// 大字を通常漢数字に変換
var result1 = KanjiNumerals.Normalize("壱拾参"); // "一十三"を返します
var result2 = KanjiNumerals.Normalize("二十三"); // "弐拾参"を返します（数値のみの場合）

// 通常漢数字を大字に変換
var result3 = KanjiNumerals.ConvertToDaiji("一十三"); // "壱拾参"を返します
var result4 = KanjiNumerals.ConvertToDaiji("二三年四月foo"); // "弐参年肆月foo"を返します

// 位取り表記に変換
var result5 = KanjiNumerals.ConvertToPositionalNotation("一千九百五十七"); // "一九五七"を返します
var result6 = KanjiNumerals.ConvertToPositionalNotation("二千二十三年"); // "二〇二三年"を返します

// アラビア数字に変換
var result7 = KanjiNumerals.ConvertToArabicNumerals("壱拾参"); // "13"を返します
var result8 = KanjiNumerals.ConvertToArabicNumerals("参年A"); // "3年A"を返します

// 大数単位を付与した漢数字表記に変換
var result9 = KanjiNumerals.ConvertToLargeNumbersNotation("一九五七"); // "一千九百五十七"を返します
var result10 = KanjiNumerals.ConvertToLargeNumbersNotation("二〇二三年"); // "二千二十三年"を返します
```

## License

MIT License

## Contributing

Please report bugs and feature requests through GitHub Issues. Pull requests are welcome.
