// <copyright file="ValueStringBuilder.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

using System.Buffers;

namespace Aloe.Utils.Wafu.Kansuji;

/// <summary>
/// Stackalloc と ArrayPool を組み合わせた高速かつ低GCの StringBuilder 構造体です。
/// </summary>
internal ref struct ValueStringBuilder : IDisposable
{
    /// <summary>
    /// プールに返却する配列
    /// </summary>
    private char[]? _arrayToReturnToPool;

    /// <summary>
    /// 文字バッファ
    /// </summary>
    private Span<char> _chars;

    /// <summary>
    /// 現在の位置
    /// </summary>
    private int _pos;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">初期バッファ</param>
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        this._arrayToReturnToPool = null;
        this._chars = initialBuffer;
        this._pos = 0;
    }

    /// <summary>
    /// Gets 文字列の長さを取得します。
    /// </summary>
    public int Length => this._pos;

    /// <summary>
    /// 指定された位置の文字を取得します。
    /// </summary>
    /// <param name="index">文字の位置</param>
    public char this[int index] => this._chars[index];

    /// <summary>
    /// 文字を追加します。
    /// </summary>
    /// <param name="c">追加する文字</param>
    public void Append(char c)
    {
        if (this._pos >= this._chars.Length)
        {
            this.Grow();
        }

        this._chars[this._pos++] = c;
    }

    /// <summary>
    /// 文字列を追加します。
    /// </summary>
    /// <param name="s">追加する文字列</param>
    public void Append(string s)
    {
        var needed = s.Length;
        if (this._pos + needed > this._chars.Length)
        {
            this.GrowTo(this._pos + needed);
        }

        s.AsSpan().CopyTo(this._chars.Slice(this._pos));
        this._pos += needed;
    }

    /// <summary>
    /// 文字列を返却し、リソースを解放します。
    /// </summary>
    /// <returns>構築された文字列</returns>
    public override string ToString()
    {
        string result = this._chars.Slice(0, this._pos).ToString();
        this.Dispose();
        return result;
    }

    /// <summary>
    /// リソースを解放します。
    /// </summary>
    public void Dispose()
    {
        if (this._arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(this._arrayToReturnToPool);
            this._arrayToReturnToPool = null;
        }
    }

    /// <summary>
    /// バッファを拡張します。
    /// </summary>
    private void Grow()
    {
        char[] newArray = ArrayPool<char>.Shared.Rent(this._chars.Length * 2);
        this._chars.CopyTo(newArray);
        if (this._arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(this._arrayToReturnToPool);
        }

        this._chars = newArray;
        this._arrayToReturnToPool = newArray;
    }

    /// <summary>
    /// バッファを拡張します。
    /// </summary>
    private void GrowTo(int requiredLength)
    {
        int newSize = Math.Max(this._chars.Length * 2, requiredLength);
        char[] newArray = ArrayPool<char>.Shared.Rent(newSize);
        this._chars.CopyTo(newArray);
        if (this._arrayToReturnToPool != null)
        {
            ArrayPool<char>.Shared.Return(this._arrayToReturnToPool);
        }

        this._chars = newArray;
        this._arrayToReturnToPool = newArray;
    }
}
