using System;
using System.IO;
using Microsoft.Win32;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// ポリシーベース設定を管理するクラス
    /// レジストリベースのポリシー設定とグループポリシー対応を提供します
    /// </summary>
    public static class Policy
    {
        /// <summary>
        /// 設定ファイルを無視するポリシー
        /// </summary>
        public static bool IgnoreSettingsFile { get; set; } = false;

        /// <summary>
        /// アイコンスケールのポリシー
        /// </summary>
        public static double IconScale { get; private set; } = 1.0;

        /// <summary>
        /// 正規化のポリシー
        /// </summary>
        public static bool Canonicalize { get; private set; } = false;

        /// <summary>
        /// 正規化追加テキストのポリシー
        /// </summary>
        public static string CanonicalizeAppendedText { get; private set; } = string.Empty;

        /// <summary>
        /// フォーカス表示のポリシー
        /// </summary>
        public static bool ShowFocus { get; private set; } = true;

        /// <summary>
        /// Aero効果のポリシー
        /// </summary>
        public static bool UseAero { get; private set; } = false;

        /// <summary>
        /// アクセシビリティレンダリングのポリシー
        /// </summary>
        public static bool AccessibleRendering { get; private set; } = false;

        /// <summary>
        /// ポリシーを初期化します
        /// </summary>
        public static void Initialize()
        {
            Logger.LogDebug("Policy.Initialize", "ポリシー初期化開始");

            try
            {
                // レジストリからポリシー設定を読み込み
                LoadRegistryPolicies();
                
                // グループポリシーから設定を読み込み
                LoadGroupPolicies();
                
                // 環境変数から設定を読み込み
                LoadEnvironmentPolicies();

                Logger.LogDebug("Policy.Initialize", "ポリシー初期化完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("Policy.Initialize", "ポリシー初期化エラー", ex.Message);
            }
        }

        /// <summary>
        /// レジストリからポリシー設定を読み込みます
        /// </summary>
        private static void LoadRegistryPolicies()
        {
            try
            {
                const string policyKey = @"SOFTWARE\Policies\BrowserChooser3";
                
                using var key = Registry.LocalMachine.OpenSubKey(policyKey);
                if (key != null)
                {
                    // 設定ファイル無視ポリシー
                    var ignoreSettings = key.GetValue("IgnoreSettingsFile") as string;
                    if (bool.TryParse(ignoreSettings, out var ignore))
                    {
                        IgnoreSettingsFile = ignore;
                    }

                    // アイコンスケールポリシー
                    var iconScale = key.GetValue("IconScale") as string;
                    if (double.TryParse(iconScale, out var scale))
                    {
                        IconScale = scale;
                    }

                    // 正規化ポリシー
                    var canonicalize = key.GetValue("Canonicalize") as string;
                    if (bool.TryParse(canonicalize, out var can))
                    {
                        Canonicalize = can;
                    }

                    // 正規化追加テキストポリシー
                    var appendedText = key.GetValue("CanonicalizeAppendedText") as string;
                    if (!string.IsNullOrEmpty(appendedText))
                    {
                        CanonicalizeAppendedText = appendedText;
                    }

                    // フォーカス表示ポリシー
                    var showFocus = key.GetValue("ShowFocus") as string;
                    if (bool.TryParse(showFocus, out var focus))
                    {
                        ShowFocus = focus;
                    }

                    // Aero効果ポリシー
                    var useAero = key.GetValue("UseAero") as string;
                    if (bool.TryParse(useAero, out var aero))
                    {
                        UseAero = aero;
                    }

                    // アクセシビリティレンダリングポリシー
                    var accessibleRendering = key.GetValue("AccessibleRendering") as string;
                    if (bool.TryParse(accessibleRendering, out var accessible))
                    {
                        AccessibleRendering = accessible;
                    }

                    Logger.LogDebug("Policy.LoadRegistryPolicies", "レジストリポリシー読み込み完了");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Policy.LoadRegistryPolicies", "レジストリポリシー読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// グループポリシーから設定を読み込みます
        /// </summary>
        private static void LoadGroupPolicies()
        {
            try
            {
                const string groupPolicyKey = @"SOFTWARE\Policies\BrowserChooser3\GroupPolicy";
                
                using var key = Registry.LocalMachine.OpenSubKey(groupPolicyKey);
                if (key != null)
                {
                    // グループポリシー固有の設定を読み込み
                    // 現在はレジストリポリシーと同じ構造を使用
                    LoadRegistryPolicies();
                    
                    Logger.LogDebug("Policy.LoadGroupPolicies", "グループポリシー読み込み完了");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Policy.LoadGroupPolicies", "グループポリシー読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// 環境変数からポリシー設定を読み込みます
        /// </summary>
        private static void LoadEnvironmentPolicies()
        {
            try
            {
                // 環境変数からポリシー設定を読み込み
                var ignoreSettings = Environment.GetEnvironmentVariable("BROWSERCHOOSER_IGNORE_SETTINGS");
                if (bool.TryParse(ignoreSettings, out var ignore))
                {
                    IgnoreSettingsFile = ignore;
                }

                var iconScale = Environment.GetEnvironmentVariable("BROWSERCHOOSER_ICON_SCALE");
                if (double.TryParse(iconScale, out var scale))
                {
                    IconScale = scale;
                }

                var canonicalize = Environment.GetEnvironmentVariable("BROWSERCHOOSER_CANONICALIZE");
                if (bool.TryParse(canonicalize, out var can))
                {
                    Canonicalize = can;
                }

                var appendedText = Environment.GetEnvironmentVariable("BROWSERCHOOSER_CANONICALIZE_TEXT");
                if (!string.IsNullOrEmpty(appendedText))
                {
                    CanonicalizeAppendedText = appendedText;
                }

                Logger.LogDebug("Policy.LoadEnvironmentPolicies", "環境変数ポリシー読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("Policy.LoadEnvironmentPolicies", "環境変数ポリシー読み込みエラー", ex.Message);
            }
        }

        /// <summary>
        /// ポリシー設定をリセットします
        /// </summary>
        public static void Reset()
        {
            IgnoreSettingsFile = false;
            IconScale = 1.0;
            Canonicalize = false;
            CanonicalizeAppendedText = string.Empty;
            ShowFocus = true;
            UseAero = false;
            AccessibleRendering = false;

            Logger.LogDebug("Policy.Reset", "ポリシー設定をリセット");
        }

        /// <summary>
        /// ポリシー設定の概要を取得します
        /// </summary>
        /// <returns>ポリシー設定の概要</returns>
        public static string GetPolicySummary()
        {
            return $"IgnoreSettingsFile: {IgnoreSettingsFile}, " +
                   $"IconScale: {IconScale}, " +
                   $"Canonicalize: {Canonicalize}, " +
                   $"ShowFocus: {ShowFocus}, " +
                   $"UseAero: {UseAero}, " +
                   $"AccessibleRendering: {AccessibleRendering}";
        }
    }
}
