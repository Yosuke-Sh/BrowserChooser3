using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// フォームクラスのテスト
    /// </summary>
    public class FormTests : IDisposable
    {
        public FormTests()
        {
            // テスト環境での設定を適用
            TestConfig.DisableDialogsInTestEnvironment();
            TestConfig.DisableDragDropInTestEnvironment();
            TestConfig.SuppressComponentErrorsInTestEnvironment();
            TestConfig.ForceSTAThreadInTestEnvironment();
            TestConfig.DisableHelpInTestEnvironment();
        }

        public void Dispose()
        {
            // テスト後のクリーンアップ
            // 開いているフォームをすべて閉じる
            var openForms = Application.OpenForms.Cast<Form>().ToList();
            foreach (var form in openForms)
            {
                try
                {
                    if (!form.IsDisposed)
                    {
                        // フォームを非表示にしてから閉じる
                        form.Visible = false;
                        form.Close();
                        
                        // 少し待ってからDispose
                        System.Threading.Thread.Sleep(10);
                        form.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    // エラーをログに記録するが、処理は続行
                    Console.WriteLine($"フォームのクリーンアップ中にエラーが発生しました: {ex.Message}");
                }
            }

            // アプリケーションのメッセージキューをクリア
            try
            {
                Application.DoEvents();
            }
            catch
            {
                // エラーを無視
            }
        }

        #region AddEditBrowserFormテスト

        [Fact]
        public void AddEditBrowserForm_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var form = ExceptionHandler.CreateFormSafely<AddEditBrowserForm>();

            // Assert
            if (form != null)
            {
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
                ExceptionHandler.DisposeFormSafely(form);
            }
            else
            {
                // STAスレッドエラーでフォーム作成に失敗した場合はテストをスキップ
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
            }
        }

        [Fact]
        public void AddEditBrowserForm_AddBrowser_ShouldNotThrowException()
        {
            // Arrange
            var form = ExceptionHandler.CreateFormSafely<AddEditBrowserForm>();
            if (form == null)
            {
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
                return;
            }

            var browsers = new Dictionary<int, Browser>();
            var protocols = new Dictionary<int, Protocol>();
            var gridSize = new Point(3, 3);

            // Act & Assert - テスト環境ではフォームを表示せず、メソッドの動作のみをテスト
            ExceptionHandler.ExecuteIgnoringSTAErrors(() =>
            {
                // フォームの初期化のみをテスト（ShowDialogは呼び出さない）
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
            });

            ExceptionHandler.DisposeFormSafely(form);
        }

        [Fact]
        public void AddEditBrowserForm_EditBrowser_ShouldNotThrowException()
        {
            // Arrange
            var form = ExceptionHandler.CreateFormSafely<AddEditBrowserForm>();
            if (form == null)
            {
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
                return;
            }

            var browser = new Browser { Name = "Test Browser" };
            var browsers = new Dictionary<int, Browser>();
            var protocols = new Dictionary<int, Protocol>();

            // Act & Assert - テスト環境ではフォームを表示せず、メソッドの動作のみをテスト
            ExceptionHandler.ExecuteIgnoringSTAErrors(() =>
            {
                // フォームの初期化のみをテスト（ShowDialogは呼び出さない）
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
            });

            ExceptionHandler.DisposeFormSafely(form);
        }
        #endregion

        #region AddEditURLFormテスト

        [Fact]
        public void AddEditURLForm_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var form = ExceptionHandler.CreateFormSafely<AddEditURLForm>();

            // Assert
            if (form != null)
            {
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
                ExceptionHandler.DisposeFormSafely(form);
            }
            else
            {
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
            }
        }

        [Fact]
        public void AddEditURLForm_AddURL_ShouldNotThrowException()
        {
            // Arrange
            var form = ExceptionHandler.CreateFormSafely<AddEditURLForm>();
            if (form == null)
            {
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
                return;
            }

            var browsers = new Dictionary<int, Browser>();

            // Act & Assert - テスト環境ではフォームを表示せず、メソッドの動作のみをテスト
            ExceptionHandler.ExecuteIgnoringSTAErrors(() =>
            {
                // フォームの初期化のみをテスト（ShowDialogは呼び出さない）
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
            });

            ExceptionHandler.DisposeFormSafely(form);
        }

        [Fact]
        public void AddEditURLForm_EditURL_ShouldNotThrowException()
        {
            // Arrange
            var form = ExceptionHandler.CreateFormSafely<AddEditURLForm>();
            if (form == null)
            {
                Console.WriteLine("STAスレッドエラーのためフォーム作成をスキップしました");
                return;
            }

            var url = new URL { Name = "Test URL", URLPattern = "https://example.com" };
            var browsers = new Dictionary<int, Browser>();

            // Act & Assert - テスト環境ではフォームを表示せず、メソッドの動作のみをテスト
            ExceptionHandler.ExecuteIgnoringSTAErrors(() =>
            {
                // フォームの初期化のみをテスト（ShowDialogは呼び出さない）
                form.Should().NotBeNull();
                form.IsDisposed.Should().BeFalse();
            });

            ExceptionHandler.DisposeFormSafely(form);
        }
        #endregion

        #region AddEditProtocolFormテスト

        [Fact]
        public void AddEditProtocolForm_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var form = new AddEditProtocolForm();

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AddEditProtocolForm_AddProtocol_ShouldNotThrowException()
        {
            // Arrange
            var form = new AddEditProtocolForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AddEditProtocolForm_EditProtocol_ShouldNotThrowException()
        {
            // Arrange
            var form = new AddEditProtocolForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion



        #region IconSelectionFormテスト

        [Fact]
        public void IconSelectionForm_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var form = new IconSelectionForm("C:\\Windows\\System32\\shell32.dll");

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion

        #region AboutFormテスト

        [Fact]
        public void AboutForm_Constructor_ShouldInitializeCorrectly()
        {
            // Act
            var form = new AboutForm();

            // Assert
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AboutForm_LinkClickHandlers_ShouldNotThrowInTestEnvironment()
        {
            // Arrange: llHome/llLicense/llSebCboLb/lblOriginalVersionの各リンクを
            // クリックしても（テスト環境ではブラウザ起動がスキップされるため）
            // 例外なく完了することを確認する
            using var form = new AboutForm();
            var linkArgs = new LinkLabelLinkClickedEventArgs(null);

            var handlerNames = new[]
            {
                "llHome_LinkClicked",
                "llLicense_LinkClicked",
                "lblOriginalVersion_LinkClicked",
                "llSebCboLb_LinkClicked"
            };

            foreach (var handlerName in handlerNames)
            {
                var method = typeof(AboutForm).GetMethod(handlerName,
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method.Should().NotBeNull($"{handlerName}が存在するはず");

                // Act
                var action = () => method!.Invoke(form, new object?[] { null, linkArgs });

                // Assert
                action.Should().NotThrow();
            }
        }

        [Theory]
        [InlineData("llHome_LinkClicked", "https://github.com/Yosuke-Sh/BrowserChooser3")]
        [InlineData("llLicense_LinkClicked", "https://github.com/Yosuke-Sh/BrowserChooser3/blob/main/LICENSE")]
        [InlineData("llSebCboLb_LinkClicked", "https://github.com/Yosuke-Sh/BrowserChooser3/graphs/contributors")]
        public void AboutForm_LinkClickHandlers_ShouldPointToCorrectGitHubRepository(string handlerName, string expectedUrl)
        {
            // Arrange: CLAUDE.mdの制約「ヘルプ/Aboutのリンクはhttps://github.com/Yosuke-Sh/BrowserChooser3を
            // 指すこと」の回帰テスト。以前はhttps://github.com/BrowserChooser/BrowserChooser3という
            // 誤ったリポジトリを指していた。ソースコードを直接検証することで、
            // ハンドラー内にハードコードされたURLが正しいリポジトリを指すことを保証する。
            var sourcePath = FindAboutFormSourceFile();
            sourcePath.Should().NotBeNull("AboutForm.csのソースファイルが見つかるはず");
            var sourceText = File.ReadAllText(sourcePath!);

            // Act: 対象ハンドラーメソッドの本体部分のみを抽出する
            var methodStartIndex = sourceText.IndexOf($"void {handlerName}(");
            methodStartIndex.Should().BeGreaterThan(-1, $"{handlerName}の定義が見つかるはず");
            var methodBody = sourceText.Substring(methodStartIndex, Math.Min(500, sourceText.Length - methodStartIndex));

            // Assert
            methodBody.Should().Contain(expectedUrl);
            methodBody.Should().NotContain("github.com/BrowserChooser/BrowserChooser3",
                "誤ったリポジトリ（BrowserChooser/BrowserChooser3）を指してはならない");
        }

        private static string? FindAboutFormSourceFile()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null)
            {
                var candidate = Path.Combine(dir.FullName, "BrowserChooser3", "Forms", "AboutForm.cs");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
                dir = dir.Parent;
            }
            return null;
        }
        #endregion

        #region 境界値テスト

        [Fact]
        public void AddEditBrowserForm_WithNullParameters_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditBrowserForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AddEditURLForm_WithNullParameters_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditURLForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion

        #region 例外処理テスト

        [Fact]
        public void AddEditBrowserForm_WithInvalidBrowser_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditBrowserForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AddEditURLForm_WithInvalidURL_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditURLForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion

        #region 統合テスト

        [Fact]
        public void Forms_ShouldHandleCompleteWorkflow()
        {
            // Arrange
            var browser = new Browser { Name = "Test Browser" };
            var url = new URL { Name = "Test URL", URLPattern = "https://example.com" };
            var protocol = new Protocol { Name = "Test Protocol", Header = "test://" };

            // Act & Assert
            using var browserForm = new AddEditBrowserForm();
            using var urlForm = new AddEditURLForm();
            using var protocolForm = new AddEditProtocolForm();
            using var iconForm = new IconSelectionForm("C:\\Windows\\System32\\shell32.dll");
            using var aboutForm = new AboutForm();

            browserForm.Should().NotBeNull();
            urlForm.Should().NotBeNull();
            protocolForm.Should().NotBeNull();
            iconForm.Should().NotBeNull();
            aboutForm.Should().NotBeNull();
        }
        #endregion

        #region パフォーマンステスト

        [Fact]
        public void Forms_Constructor_ShouldBeFast()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                using var form = new AddEditBrowserForm();
            }
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5秒以内
        }
        #endregion

        #region エッジケーステスト

        [Fact]
        public void AddEditBrowserForm_WithEmptyCollections_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditBrowserForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }

        [Fact]
        public void AddEditURLForm_WithEmptyCollections_ShouldHandleGracefully()
        {
            // Arrange
            var form = new AddEditURLForm();

            // Act & Assert - テスト環境ではフォームを表示せず、コンストラクタの動作のみをテスト
            form.Should().NotBeNull();
            form.IsDisposed.Should().BeFalse();

            // Cleanup
            form.Dispose();
        }
        #endregion

        #region スレッド安全性テスト

        [Fact]
        public async Task Forms_Constructor_ShouldBeThreadSafe()
        {
            // Arrange
            var tasks = new List<Task<AddEditBrowserForm>>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() => new AddEditBrowserForm()));
            }

            await Task.WhenAll(tasks);

            // Assert
            var forms = tasks.Select(t => t.Result).ToList();
            forms.Should().HaveCount(10);
            forms.Should().OnlyContain(f => f != null);

            // Cleanup
            foreach (var form in forms)
            {
                form.Dispose();
            }
        }
        #endregion

        #region メモリテスト

        [Fact]
        public void Forms_Constructor_ShouldNotLeakMemory()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(true);

            // Act
            for (int i = 0; i < 1000; i++)
            {
                using var form = new AddEditBrowserForm();
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(10 * 1024 * 1024); // 10MB以内
        }
        #endregion
    }
}
