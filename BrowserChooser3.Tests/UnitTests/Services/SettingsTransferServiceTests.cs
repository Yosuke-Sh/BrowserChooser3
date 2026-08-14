using System.IO;
using System.Xml.Serialization;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// Phase 3-6（設定のインポート/エクスポートと世代バックアップ）の振る舞いテスト。
    ///
    /// 実際の %APPDATA% には触れず、テストごとにGuidベースの一意な一時ディレクトリを
    /// 使って完全に分離する（既存のSettingsTests系がプロセス共有の実ファイルに
    /// 依存してフレーキーになっている問題を持ち込まないため）。
    /// </summary>
    public class SettingsTransferServiceTests : IDisposable
    {
        private readonly string _configDirectory;
        private readonly string _workDirectory;
        private const string ConfigFileName = Settings.BrowserChooserConfigFileName;

        public SettingsTransferServiceTests()
        {
            var root = Path.Combine(Path.GetTempPath(), $"BC3TransferTests_{Guid.NewGuid():N}");
            _configDirectory = Path.Combine(root, "config");
            _workDirectory = Path.Combine(root, "work");
            Directory.CreateDirectory(_configDirectory);
            Directory.CreateDirectory(_workDirectory);
        }

        public void Dispose()
        {
            try
            {
                var root = Path.GetDirectoryName(_configDirectory);
                if (root != null && Directory.Exists(root))
                {
                    Directory.Delete(root, recursive: true);
                }
            }
            catch
            {
                // 後片付けの失敗はテスト結果に影響させない
            }
            GC.SuppressFinalize(this);
        }

        private string WriteConfig(string marker)
        {
            var settings = new Settings { DefaultMessage = marker };
            var path = Path.Combine(_configDirectory, ConfigFileName);
            var serializer = new XmlSerializer(typeof(Settings));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, settings);
            return path;
        }

        private static string ReadDefaultMessage(string path)
        {
            var serializer = new XmlSerializer(typeof(Settings));
            using var reader = new StreamReader(path);
            return ((Settings)serializer.Deserialize(reader)!).DefaultMessage;
        }

        #region 世代バックアップ

        [Fact]
        public void CreateGenerationalBackup_WithNoExistingConfig_ShouldDoNothing()
        {
            var result = SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);

            result.Should().BeFalse();
            SettingsTransferService.GetAvailableBackups(_configDirectory).Should().BeEmpty();
        }

        [Fact]
        public void CreateGenerationalBackup_ShouldCopyConfigToBak1AndKeepOriginal()
        {
            var configPath = WriteConfig("original");

            SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName).Should().BeTrue();

            // 移動ではなくコピーなので現行ファイルは残っていなければならない
            File.Exists(configPath).Should().BeTrue();
            var bak1 = Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(1));
            ReadDefaultMessage(bak1).Should().Be("original");
        }

        [Fact]
        public void CreateGenerationalBackup_RepeatedCalls_ShouldRotateGenerations()
        {
            WriteConfig("gen1");
            SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);

            WriteConfig("gen2");
            SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);

            WriteConfig("gen3");
            SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);

            // bak1が最新、bak3が最も古い
            ReadDefaultMessage(Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(1)))
                .Should().Be("gen3");
            ReadDefaultMessage(Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(2)))
                .Should().Be("gen2");
            ReadDefaultMessage(Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(3)))
                .Should().Be("gen1");
        }

        [Fact]
        public void CreateGenerationalBackup_ShouldNotKeepMoreThanThreeGenerations()
        {
            for (var i = 1; i <= 5; i++)
            {
                WriteConfig($"gen{i}");
                SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);
            }

            SettingsTransferService.GetAvailableBackups(_configDirectory)
                .Should().HaveCount(SettingsTransferService.BackupGenerations);

            var bak4 = Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(4));
            File.Exists(bak4).Should().BeFalse();
        }

        #endregion

        #region エクスポート

        [Fact]
        public void Export_ShouldCopyConfigToDestination()
        {
            WriteConfig("exported");
            var destination = Path.Combine(_workDirectory, "backup.xml");

            SettingsTransferService.Export(_configDirectory, ConfigFileName, destination).Should().BeTrue();
            ReadDefaultMessage(destination).Should().Be("exported");
        }

        [Fact]
        public void Export_WithoutConfigFile_ShouldFailWithoutThrowing()
        {
            var destination = Path.Combine(_workDirectory, "backup.xml");

            SettingsTransferService.Export(_configDirectory, ConfigFileName, destination).Should().BeFalse();
            File.Exists(destination).Should().BeFalse();
        }

        [Fact]
        public void Export_ToMissingDirectory_ShouldCreateIt()
        {
            WriteConfig("exported");
            var destination = Path.Combine(_workDirectory, "nested", "deeper", "backup.xml");

            SettingsTransferService.Export(_configDirectory, ConfigFileName, destination).Should().BeTrue();
            File.Exists(destination).Should().BeTrue();
        }

        #endregion

        #region 検証

        [Fact]
        public void TryValidate_WithValidSettingsFile_ShouldSucceed()
        {
            var path = WriteConfig("valid");

            SettingsTransferService.TryValidate(path, out var settings).Should().BeTrue();
            settings.Should().NotBeNull();
            settings!.DefaultMessage.Should().Be("valid");
        }

        [Fact]
        public void TryValidate_WithMalformedXml_ShouldFail()
        {
            var path = Path.Combine(_workDirectory, "broken.xml");
            File.WriteAllText(path, "<Settings><unclosed>");

            SettingsTransferService.TryValidate(path, out var settings).Should().BeFalse();
            settings.Should().BeNull();
        }

        [Fact]
        public void TryValidate_WithUnrelatedXml_ShouldFail()
        {
            var path = Path.Combine(_workDirectory, "other.xml");
            File.WriteAllText(path, "<?xml version=\"1.0\"?><SomethingElse><Value>1</Value></SomethingElse>");

            SettingsTransferService.TryValidate(path, out var settings).Should().BeFalse();
            settings.Should().BeNull();
        }

        [Fact]
        public void TryValidate_WithMissingFile_ShouldFail()
        {
            var path = Path.Combine(_workDirectory, "does-not-exist.xml");

            SettingsTransferService.TryValidate(path, out var settings).Should().BeFalse();
            settings.Should().BeNull();
        }

        #endregion

        #region インポート

        [Fact]
        public void Import_ShouldReplaceConfigAndBackUpPreviousOne()
        {
            WriteConfig("current");

            var source = Path.Combine(_workDirectory, "incoming.xml");
            var incoming = new Settings { DefaultMessage = "incoming" };
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(source))
            {
                serializer.Serialize(writer, incoming);
            }

            SettingsTransferService.Import(source, _configDirectory, ConfigFileName, out var imported)
                .Should().BeTrue();

            imported.Should().NotBeNull();
            imported!.DefaultMessage.Should().Be("incoming");
            ReadDefaultMessage(Path.Combine(_configDirectory, ConfigFileName)).Should().Be("incoming");

            // 適用前の設定がバックアップから復元できること
            ReadDefaultMessage(Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(1)))
                .Should().Be("current");
        }

        [Fact]
        public void Import_WithInvalidFile_ShouldNotTouchExistingConfig()
        {
            WriteConfig("must-survive");
            var source = Path.Combine(_workDirectory, "broken.xml");
            File.WriteAllText(source, "not xml at all");

            SettingsTransferService.Import(source, _configDirectory, ConfigFileName, out var imported)
                .Should().BeFalse();

            imported.Should().BeNull();
            ReadDefaultMessage(Path.Combine(_configDirectory, ConfigFileName)).Should().Be("must-survive");
            SettingsTransferService.GetAvailableBackups(_configDirectory).Should().BeEmpty();
        }

        [Fact]
        public void Import_ShouldClearSafeModeOnImportedSettings()
        {
            // SafeMode付きのファイルをインポートしても、保存が拒否される状態を持ち込まない
            var source = Path.Combine(_workDirectory, "safemode.xml");
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(source))
            {
                serializer.Serialize(writer, new Settings { SafeMode = true, DefaultMessage = "incoming" });
            }

            SettingsTransferService.Import(source, _configDirectory, ConfigFileName, out var imported)
                .Should().BeTrue();

            imported!.SafeMode.Should().BeFalse();
        }

        [Fact]
        public void ImportedSettings_ShouldPreserveBrowsersRoundTrip()
        {
            var original = new Settings();
            original.Browsers.Add(new Browser
            {
                Name = "Firefox",
                Target = @"C:\Program Files\Mozilla Firefox\firefox.exe",
                ProfileName = "work",
                UsePrivateMode = true
            });

            var source = Path.Combine(_workDirectory, "with-browsers.xml");
            var serializer = new XmlSerializer(typeof(Settings));
            using (var writer = new StreamWriter(source))
            {
                serializer.Serialize(writer, original);
            }

            SettingsTransferService.Import(source, _configDirectory, ConfigFileName, out var imported)
                .Should().BeTrue();

            imported!.Browsers.Should().ContainSingle();
            imported.Browsers[0].Name.Should().Be("Firefox");
            imported.Browsers[0].ProfileName.Should().Be("work");
            imported.Browsers[0].UsePrivateMode.Should().BeTrue();
        }

        #endregion

        #region バックアップからの復元

        [Fact]
        public void RestoreFromBackup_ShouldBringBackPreviousSettings()
        {
            WriteConfig("wanted");
            SettingsTransferService.CreateGenerationalBackup(_configDirectory, ConfigFileName);

            // 設定を上書きしてしまった状況を再現する
            WriteConfig("unwanted");

            var backup = Path.Combine(_configDirectory, SettingsTransferService.GetBackupFileName(1));
            SettingsTransferService.RestoreFromBackup(backup, _configDirectory, ConfigFileName, out var restored)
                .Should().BeTrue();

            restored!.DefaultMessage.Should().Be("wanted");
            ReadDefaultMessage(Path.Combine(_configDirectory, ConfigFileName)).Should().Be("wanted");
        }

        #endregion
    }
}
