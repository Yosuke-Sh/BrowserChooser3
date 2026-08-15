using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using BrowserChooser3.Tests.TestHelpers.MockFactories;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormBrowserHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// <para>
    /// 従来24件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由にスキップされていたうえ、
    /// 唯一実行されていた分岐（<c>AddBrowser_Click</c>）は AddEditBrowserForm を
    /// モーダル表示するため、ヘッドレスなテストではどのみち意味のある検証ができない。
    /// </para>
    /// <para>
    /// IOptionsFormContextの抽出を機に、子フォームを開かずに完結する分岐——
    /// 未選択時のガード、辞書に存在しないTagの防御、削除/複製の実際の副作用——を
    /// 対象にする。<c>DetectBrowsers_Click</c> はレジストリ/Program Filesを直接
    /// 走査する <c>DetectedBrowsers.DoBrowserDetectionAsync</c> を呼ぶため、
    /// (4-2 の残課題である IRegistryReader/IFileSystemProbe 抽象化が無い現状では)
    /// 引き続きスキップする。
    /// </para>
    /// </remarks>
    public class OptionsFormBrowserHandlersTests : IDisposable
    {
        private readonly FakeOptionsFormContext _context = new();
        private readonly Dictionary<int, Browser> _browsers = new();
        private readonly Dictionary<int, Protocol> _protocols = new();
        private bool _modified;

        private OptionsFormBrowserHandlers CreateHandlers(ImageList? icons = null) =>
            new(_context, new Settings(), _browsers, _protocols, icons ?? new ImageList(), v => _modified = v);

        /// <summary>
        /// tabBrowsers配下にlstBrowsersを備えたコンテキストを構築します。
        /// </summary>
        /// <remarks>
        /// ListViewはネイティブハンドル生成前だと選択状態を追跡しないため、
        /// 親へ追加する前に <c>CreateControl()</c> する。
        /// </remarks>
        private ListView SetupBrowsersTab()
        {
            var tab = _context.AddTabPage("tabBrowsers");
            var listView = new ListView { Name = "lstBrowsers", MultiSelect = false };
            listView.Columns.Add("Name");
            listView.Columns.Add("Target");
            listView.Columns.Add("Row");
            listView.Columns.Add("Column");
            listView.Columns.Add("Hotkey");
            listView.Columns.Add("Arguments");
            listView.CreateControl();
            tab.Controls.Add(listView);
            return listView;
        }

        public void Dispose() => _context.Dispose();

        #region EditBrowser_Click

        [Fact]
        public void EditBrowser_Click_WithoutSelection_ShouldNotModifyAndShouldNotThrow()
        {
            // Arrange
            SetupBrowsersTab();
            var handlers = CreateHandlers();

            // Act
            // 未選択時は情報メッセージのみでAddEditBrowserFormを開かない
            var action = () => handlers.EditBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
            _modified.Should().BeFalse();
        }

        [Fact]
        public void EditBrowser_Click_WithSelectionButUnknownTag_ShouldNotModify()
        {
            // Arrange
            // ListViewのTagが辞書に存在しないIDを指している防御的な分岐
            var listView = SetupBrowsersTab();
            var item = new ListViewItem("Orphan") { Tag = 999 };
            listView.Items.Add(item);
            item.Selected = true;
            listView.SelectedItems.Count.Should().Be(1, "テストの前提として選択状態が成立していること");

            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.EditBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
            _modified.Should().BeFalse();
        }

        [Fact]
        public void EditBrowser_Click_WithoutBrowsersTab_ShouldNotThrow()
        {
            // Arrange
            // タブが未構築の状態でも落ちないこと
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.EditBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region DeleteBrowser_Click

        [Fact]
        public void DeleteBrowser_Click_WithoutSelection_ShouldNotRemoveAnything()
        {
            // Arrange
            var listView = SetupBrowsersTab();
            var browser = BrowserFactory.Create("Chrome");
            _browsers[1] = browser;
            listView.Items.Add(new ListViewItem("Chrome") { Tag = 1 });

            var handlers = CreateHandlers();

            // Act
            handlers.DeleteBrowser_Click(null, EventArgs.Empty);

            // Assert
            _browsers.Should().ContainKey(1, "未選択なら削除されない");
            listView.Items.Count.Should().Be(1);
            _modified.Should().BeFalse();
        }

        [Fact]
        public void DeleteBrowser_Click_WithSelection_ShouldRemoveFromDictionaryAndListView()
        {
            // Arrange
            // テスト環境ではShowQuestionStaticはYesを返すため、実際に削除まで進む
            var listView = SetupBrowsersTab();
            var browser = BrowserFactory.Create("Chrome");
            _browsers[1] = browser;
            var item = new ListViewItem("Chrome") { Tag = 1 };
            listView.Items.Add(item);
            item.Selected = true;

            var handlers = CreateHandlers();

            // Act
            handlers.DeleteBrowser_Click(null, EventArgs.Empty);

            // Assert
            _browsers.Should().NotContainKey(1, "選択されたブラウザは辞書から削除される");
            listView.Items.Count.Should().Be(0, "ListViewからも削除される");
            _modified.Should().BeTrue("削除は変更として記録される");
        }

        [Fact]
        public void DeleteBrowser_Click_WithoutBrowsersTab_ShouldNotThrow()
        {
            // Arrange
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.DeleteBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region CloneBrowser_Click

        [Fact]
        public void CloneBrowser_Click_WithoutSelection_ShouldNotModify()
        {
            // Arrange
            SetupBrowsersTab();
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.CloneBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
            _modified.Should().BeFalse();
        }

        [Fact]
        public void CloneBrowser_Click_WithSelectionButUnknownTag_ShouldNotModify()
        {
            // Arrange
            var listView = SetupBrowsersTab();
            var item = new ListViewItem("Orphan") { Tag = 999 };
            listView.Items.Add(item);
            item.Selected = true;

            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.CloneBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
            _modified.Should().BeFalse("辞書に存在しないTagでは複製元を取得できず中断する");
        }

        [Fact]
        public void CloneBrowser_Click_WithoutBrowsersTab_ShouldNotThrow()
        {
            // Arrange
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.CloneBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region コンストラクタ — 実際に使われる引数の組み合わせ

        [Fact]
        public void Constructor_WithNullImageList_ShouldAllowSubsequentCallsWithoutThrowing()
        {
            // Arrange
            // アイコンリスト未生成（Browsersパネル未構築）の状態でも動作すること
            SetupBrowsersTab();
            var handlers = new OptionsFormBrowserHandlers(_context, new Settings(), _browsers, _protocols, null, v => _modified = v);

            // Act
            var action = () => handlers.DeleteBrowser_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region DetectBrowsers_Click

        // DetectBrowsers_Click は DetectedBrowsers.DoBrowserDetectionAsync を通じて
        // レジストリ/Program Files を直接走査するためヘッドレスなユニットテストでは
        // 実行しない（IRegistryReader/IFileSystemProbe 抽象化が無い現状の既知の制約、
        // 計画の 4-2 残課題を参照）。

        #endregion
    }
}
