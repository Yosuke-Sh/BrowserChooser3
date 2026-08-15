using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using BrowserChooser3.Tests.TestHelpers.MockFactories;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormProtocolHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// <para>
    /// 従来29件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由にスキップされていた。
    /// IOptionsFormContextの抽出により、選択されたプロトコルが実際に辞書と
    /// ListViewの両方から削除されることなどを検証できるようになった。
    /// </para>
    /// <para>
    /// 追加・編集は子フォーム（AddEditProtocolForm）をモーダル表示するため、
    /// ヘッドレスなテストでは実行できない。ここでは削除経路と、
    /// 未選択・タブ未構築といった防御的な分岐を対象にする。
    /// </para>
    /// </remarks>
    public class OptionsFormProtocolHandlersTests
    {
        /// <summary>
        /// プロトコルタブとlstProtocolsを備えたコンテキストを構築します。
        /// </summary>
        /// <remarks>
        /// ListViewはネイティブハンドルが生成されていないと選択状態
        /// （<see cref="ListView.SelectedItems"/>）を追跡しない。
        /// TabPage配下に入れるとハンドルが生成されず選択が成立しないため、
        /// 先に単体でハンドルを生成してからタブへ追加する。
        /// </remarks>
        private static ListView SetupProtocolsTab(FakeOptionsFormContext context)
        {
            var tab = context.AddTabPage("tabProtocols");
            var listView = new ListView { Name = "lstProtocols", MultiSelect = false };

            // 親へ追加する前にハンドルを生成する（追加後だと生成されない）
            listView.CreateControl();
            tab.Controls.Add(listView);
            return listView;
        }

        [Fact]
        public void DeleteProtocol_Click_WithSelection_ShouldRemoveFromDictionaryAndListView()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var listView = SetupProtocolsTab(context);

            var browser = BrowserFactory.Create("Target Browser");
            var protocols = new Dictionary<int, Protocol>
            {
                { 1, new Protocol { Name = "mailto", BrowserGuid = browser.Guid } }
            };
            var browsers = new Dictionary<int, Browser> { { 1, browser } };

            var item = new ListViewItem("mailto") { Tag = 1 };
            listView.Items.Add(item);
            item.Selected = true;
            listView.SelectedItems.Count.Should().Be(1, "テストの前提として選択状態が成立していること");

            var modified = false;
            var handlers = new OptionsFormProtocolHandlers(context, protocols, browsers, v => modified = v);

            // Act
            // テスト環境ではMessageBoxServiceの確認ダイアログはYesを返す
            handlers.DeleteProtocol_Click(null, EventArgs.Empty);

            // Assert
            protocols.Should().NotContainKey(1, "選択されたプロトコルは辞書から削除される");
            listView.Items.Count.Should().Be(0, "ListViewからも削除される");
            modified.Should().BeTrue("削除は変更として記録される");
        }

        [Fact]
        public void DeleteProtocol_Click_WithoutSelection_ShouldNotRemoveAnything()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var listView = SetupProtocolsTab(context);

            var protocols = new Dictionary<int, Protocol>
            {
                { 1, new Protocol { Name = "mailto" } }
            };
            listView.Items.Add(new ListViewItem("mailto") { Tag = 1 });

            var modified = false;
            var handlers = new OptionsFormProtocolHandlers(
                context, protocols, new Dictionary<int, Browser>(), v => modified = v);

            // Act
            handlers.DeleteProtocol_Click(null, EventArgs.Empty);

            // Assert
            protocols.Should().ContainKey(1, "未選択なら削除されない");
            listView.Items.Count.Should().Be(1);
            modified.Should().BeFalse();
        }

        [Fact]
        public void DeleteProtocol_Click_WithUnknownTag_ShouldNotRemoveAnything()
        {
            // Arrange
            // ListViewのTagが辞書に存在しないIDを指している場合
            using var context = new FakeOptionsFormContext();
            var listView = SetupProtocolsTab(context);

            var protocols = new Dictionary<int, Protocol>
            {
                { 1, new Protocol { Name = "mailto" } }
            };

            var item = new ListViewItem("orphan") { Tag = 999 };
            listView.Items.Add(item);
            item.Selected = true;

            var handlers = new OptionsFormProtocolHandlers(
                context, protocols, new Dictionary<int, Browser>(), _ => { });

            // Act
            handlers.DeleteProtocol_Click(null, EventArgs.Empty);

            // Assert
            protocols.Should().ContainKey(1, "対応するプロトコルが無ければ何も削除しない");
        }

        [Fact]
        public void DeleteProtocol_Click_WithoutProtocolsTab_ShouldNotThrow()
        {
            // Arrange
            // タブが未構築の状態でも落ちないこと
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormProtocolHandlers(
                context, new Dictionary<int, Protocol>(), new Dictionary<int, Browser>(), _ => { });

            // Act
            var action = () => handlers.DeleteProtocol_Click(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void EditProtocol_Click_WithoutSelection_ShouldNotModify()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            SetupProtocolsTab(context);

            var modified = false;
            var handlers = new OptionsFormProtocolHandlers(
                context, new Dictionary<int, Protocol>(), new Dictionary<int, Browser>(), v => modified = v);

            // Act
            // 未選択なら子フォームを開かずに抜ける
            handlers.EditProtocol_Click(null, EventArgs.Empty);

            // Assert
            modified.Should().BeFalse();
        }
    }
}
