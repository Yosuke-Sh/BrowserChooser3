using System.Xml.Serialization;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// 設定ファイルの世代バックアップと、インポート/エクスポートを担当します。
    ///
    /// ポータブル版は v0.1.4 で廃止されたため、別PCへ設定を持っていく手段が
    /// 存在しなくなっていた。この機能がその需要を満たす。
    /// また、SafeMode（設定破損時の退避）と組で「バックアップから復元」を
    /// 提示できるようにするための土台でもある。
    /// </summary>
    public static class SettingsTransferService
    {
        /// <summary>
        /// 保持するバックアップの世代数
        /// </summary>
        public const int BackupGenerations = 3;

        /// <summary>
        /// 指定した世代のバックアップファイル名を返します（例: BrowserChooser3Config.bak1.xml）。
        /// </summary>
        /// <param name="generation">世代番号（1が最新）</param>
        /// <returns>バックアップファイル名</returns>
        public static string GetBackupFileName(int generation)
            => $"BrowserChooser3Config.bak{generation}.xml";

        /// <summary>
        /// 現行の設定ファイルを世代バックアップへ退避します。
        ///
        /// bak2→bak3、bak1→bak2 と繰り下げてから、現行ファイルを bak1 へコピーします
        /// （移動ではなくコピーのため、現行ファイルはそのまま残ります）。
        /// 設定ファイルが存在しない場合は何もしません。
        /// </summary>
        /// <param name="configDirectory">設定ファイルのあるディレクトリ</param>
        /// <param name="configFileName">設定ファイル名</param>
        /// <returns>バックアップを作成した場合はtrue</returns>
        public static bool CreateGenerationalBackup(string configDirectory, string configFileName)
        {
            try
            {
                var configPath = Path.Combine(configDirectory, configFileName);
                if (!File.Exists(configPath))
                {
                    return false;
                }

                // 古い世代から順に繰り下げる（bak3は上書きされて消える）
                for (var generation = BackupGenerations - 1; generation >= 1; generation--)
                {
                    var source = Path.Combine(configDirectory, GetBackupFileName(generation));
                    if (!File.Exists(source)) continue;

                    var destination = Path.Combine(configDirectory, GetBackupFileName(generation + 1));
                    File.Copy(source, destination, overwrite: true);
                }

                File.Copy(configPath, Path.Combine(configDirectory, GetBackupFileName(1)), overwrite: true);
                Logger.LogDebug("SettingsTransferService.CreateGenerationalBackup", "世代バックアップを作成しました", configDirectory);
                return true;
            }
            catch (Exception ex)
            {
                // バックアップの失敗で保存自体を失敗させない（保存の方が重要）
                Logger.LogWarning("SettingsTransferService.CreateGenerationalBackup", "世代バックアップの作成に失敗しました", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 存在する世代バックアップのパスを新しい順（bak1から）に列挙します。
        /// </summary>
        /// <param name="configDirectory">設定ファイルのあるディレクトリ</param>
        /// <returns>存在するバックアップの完全パス</returns>
        public static IReadOnlyList<string> GetAvailableBackups(string configDirectory)
        {
            var backups = new List<string>();
            for (var generation = 1; generation <= BackupGenerations; generation++)
            {
                var path = Path.Combine(configDirectory, GetBackupFileName(generation));
                if (File.Exists(path))
                {
                    backups.Add(path);
                }
            }
            return backups;
        }

        /// <summary>
        /// 設定ファイルを指定先へエクスポート（コピー）します。
        /// </summary>
        /// <param name="configDirectory">設定ファイルのあるディレクトリ</param>
        /// <param name="configFileName">設定ファイル名</param>
        /// <param name="destinationPath">エクスポート先の完全パス</param>
        /// <returns>成功した場合はtrue</returns>
        public static bool Export(string configDirectory, string configFileName, string destinationPath)
        {
            try
            {
                var configPath = Path.Combine(configDirectory, configFileName);
                if (!File.Exists(configPath))
                {
                    Logger.LogWarning("SettingsTransferService.Export", "エクスポート元の設定ファイルがありません", configPath);
                    return false;
                }

                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory) && !Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                File.Copy(configPath, destinationPath, overwrite: true);
                Logger.LogInfo("SettingsTransferService.Export", "設定をエクスポートしました", destinationPath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("SettingsTransferService.Export", "設定のエクスポートに失敗しました", destinationPath, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// インポート候補のファイルが、設定ファイルとして読み込める内容かどうかを検証します。
        /// 実際に逆シリアライズを試みるため、XMLとして壊れているものや
        /// 別のスキーマのXMLはここで弾かれます。
        /// </summary>
        /// <param name="sourcePath">検証するファイルのパス</param>
        /// <param name="settings">検証に成功した場合の設定オブジェクト</param>
        /// <returns>読み込める場合はtrue</returns>
        public static bool TryValidate(string sourcePath, out Settings? settings)
        {
            settings = null;

            try
            {
                if (!File.Exists(sourcePath))
                {
                    Logger.LogWarning("SettingsTransferService.TryValidate", "インポート元のファイルがありません", sourcePath);
                    return false;
                }

                var serializer = new XmlSerializer(typeof(Settings));
                using var reader = new StreamReader(sourcePath);
                settings = serializer.Deserialize(reader) as Settings;

                if (settings == null)
                {
                    Logger.LogWarning("SettingsTransferService.TryValidate", "設定ファイルとして解釈できませんでした", sourcePath);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("SettingsTransferService.TryValidate", "インポート元の検証に失敗しました", sourcePath, ex.Message);
                settings = null;
                return false;
            }
        }

        /// <summary>
        /// 設定ファイルをインポートします。
        ///
        /// 適用前に必ず現行設定を世代バックアップへ退避するため、
        /// インポートに失敗した場合や内容が意図と違った場合に元へ戻せます。
        /// 検証に失敗したファイルは適用されません。
        /// </summary>
        /// <param name="sourcePath">インポート元の完全パス</param>
        /// <param name="configDirectory">設定ファイルのあるディレクトリ</param>
        /// <param name="configFileName">設定ファイル名</param>
        /// <param name="importedSettings">適用された設定オブジェクト</param>
        /// <returns>成功した場合はtrue</returns>
        public static bool Import(string sourcePath, string configDirectory, string configFileName, out Settings? importedSettings)
        {
            importedSettings = null;

            // 検証してから適用する（壊れたファイルで現行設定を潰さない）
            if (!TryValidate(sourcePath, out var validated) || validated == null)
            {
                return false;
            }

            try
            {
                if (!Directory.Exists(configDirectory))
                {
                    Directory.CreateDirectory(configDirectory);
                }

                // 適用前に現行設定を自動バックアップ
                CreateGenerationalBackup(configDirectory, configFileName);

                File.Copy(sourcePath, Path.Combine(configDirectory, configFileName), overwrite: true);

                // インポートしたファイルが破損扱いで退避されていた可能性を排除する
                validated.SafeMode = false;
                importedSettings = validated;

                Logger.LogInfo("SettingsTransferService.Import", "設定をインポートしました", sourcePath);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError("SettingsTransferService.Import", "設定のインポートに失敗しました", sourcePath, ex.Message);
                importedSettings = null;
                return false;
            }
        }

        /// <summary>
        /// 世代バックアップから設定を復元します。
        /// 復元前に現行設定もバックアップされるため、復元操作自体もやり直せます。
        /// </summary>
        /// <param name="backupPath">復元元のバックアップファイルのパス</param>
        /// <param name="configDirectory">設定ファイルのあるディレクトリ</param>
        /// <param name="configFileName">設定ファイル名</param>
        /// <param name="restoredSettings">復元された設定オブジェクト</param>
        /// <returns>成功した場合はtrue</returns>
        public static bool RestoreFromBackup(string backupPath, string configDirectory, string configFileName, out Settings? restoredSettings)
        {
            restoredSettings = null;

            // 復元元がバックアップ世代そのものの場合、Importをそのまま呼ぶと
            // 先に走る世代ローテーションが復元元ファイルを上書きしてしまう。
            // 一時ファイルへ退避してからインポートすることでこれを避ける。
            string? stagedPath = null;
            try
            {
                if (!File.Exists(backupPath))
                {
                    Logger.LogWarning("SettingsTransferService.RestoreFromBackup", "復元元のバックアップがありません", backupPath);
                    return false;
                }

                stagedPath = Path.Combine(Path.GetTempPath(), $"BrowserChooser3Restore_{Guid.NewGuid():N}.xml");
                File.Copy(backupPath, stagedPath, overwrite: true);

                return Import(stagedPath, configDirectory, configFileName, out restoredSettings);
            }
            catch (Exception ex)
            {
                Logger.LogError("SettingsTransferService.RestoreFromBackup", "バックアップからの復元に失敗しました", backupPath, ex.Message);
                restoredSettings = null;
                return false;
            }
            finally
            {
                if (stagedPath != null)
                {
                    try
                    {
                        if (File.Exists(stagedPath)) File.Delete(stagedPath);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogWarning("SettingsTransferService.RestoreFromBackup", "一時ファイルの削除に失敗しました", stagedPath, ex.Message);
                    }
                }
            }
        }
    }
}
