using System;
using System.Collections.Generic;

namespace BrowserChooser3.Tests.TestHelpers.Fixtures
{
    /// <summary>
    /// 環境変数の変更を記録し、破棄時に元の値へ戻すフィクスチャ。
    /// </summary>
    /// <remarks>
    /// <para>
    /// PolicyTests や TestConfig は環境変数（TEST_ENVIRONMENT、DISABLE_DIALOGS など）を
    /// 設定するだけで一度も戻していなかった。環境変数はプロセス全体で共有されるため、
    /// 一度設定されると同じテスト実行中の後続テスト全部に影響し続ける。
    /// </para>
    /// <para>
    /// このフィクスチャ経由で設定した変数は、変更前の値（未設定なら未設定の状態）へ確実に戻る。
    /// </para>
    /// </remarks>
    public sealed class EnvironmentFixture : IDisposable
    {
        private readonly Dictionary<string, string?> _originalValues = new(StringComparer.OrdinalIgnoreCase);
        private bool _disposed;

        /// <summary>
        /// 環境変数を設定します。変更前の値は初回の設定時に記録されます。
        /// </summary>
        /// <param name="name">環境変数名</param>
        /// <param name="value">設定する値。nullを渡すと変数を削除します。</param>
        public void Set(string name, string? value)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);

            if (!_originalValues.ContainsKey(name))
            {
                _originalValues[name] = Environment.GetEnvironmentVariable(name);
            }

            Environment.SetEnvironmentVariable(name, value);
        }

        /// <summary>
        /// 環境変数を削除します。変更前の値は復元対象として記録されます。
        /// </summary>
        /// <param name="name">環境変数名</param>
        public void Remove(string name) => Set(name, null);

        /// <summary>
        /// このフィクスチャ経由で変更した全ての環境変数を元の値へ戻します。
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var pair in _originalValues)
            {
                Environment.SetEnvironmentVariable(pair.Key, pair.Value);
            }

            _originalValues.Clear();
        }
    }
}
