using System;
using System.Windows.Forms;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormListHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// 従来28件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由に全てスキップされていた。
    /// IOptionsFormContextの抽出により、OptionsForm本体を構築せずに
    /// 「リストの選択有無に応じて編集/削除ボタンの有効状態と注釈ラベルの可視状態が
    /// 切り替わる」という本来の振る舞いを検証できるようになった。
    /// </remarks>
    public class OptionsFormListHandlersTests
    {
        /// <summary>
        /// 選択済みアイテムを持つListViewを作成します。
        /// </summary>
        private static ListView CreateListViewWithSelection(bool hasSelection)
        {
            var listView = new ListView { MultiSelect = false };
            listView.Items.Add(new ListViewItem("item"));

            if (hasSelection)
            {
                // SelectedIndicesはハンドル生成後でないと反映されないため、
                // ListViewItem.Selected を使ってから参照する
                listView.CreateControl();
                listView.Items[0].Selected = true;
            }

            return listView;
        }

        /// <summary>
        /// 注釈ラベルに設定された可視状態を取得します。
        /// </summary>
        /// <remarks>
        /// <see cref="Control.Visible"/> のゲッターは「実際に画面に見えているか」を返すため、
        /// 親を表示していないテストでは、ハンドラーが <c>Visible = true</c> を設定しても
        /// falseが返る。親から切り離したうえで参照すると、そのコントロール自身に
        /// 設定された値が読める。
        /// </remarks>
        private static bool GetVisibleSetting(Control control)
        {
            var parent = control.Parent;
            if (parent == null) return control.Visible;

            parent.Controls.Remove(control);
            try
            {
                return control.Visible;
            }
            finally
            {
                parent.Controls.Add(control);
            }
        }

        #region ブラウザリスト

        [Fact]
        public void LstBrowsers_SelectedIndexChanged_WithSelection_ShouldEnableButtonsAndShowNote()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("cmdBrowserEdit");
            var cloneButton = context.AddButton("cmdBrowserClone");
            var deleteButton = context.AddButton("cmdBrowserDelete");
            var defaultButton = context.AddButton("cmdBrowserDefault");
            var noteLabel = context.AddLabel("lblDoubleClickBrowsersNote");

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            handlers.LstBrowsers_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeTrue();
            cloneButton.Enabled.Should().BeTrue();
            deleteButton.Enabled.Should().BeTrue();
            defaultButton.Enabled.Should().BeTrue();
            GetVisibleSetting(noteLabel).Should().BeTrue();
        }

        [Fact]
        public void LstBrowsers_SelectedIndexChanged_WithoutSelection_ShouldDisableButtonsAndHideNote()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("cmdBrowserEdit", enabled: true);
            var cloneButton = context.AddButton("cmdBrowserClone", enabled: true);
            var deleteButton = context.AddButton("cmdBrowserDelete", enabled: true);
            var defaultButton = context.AddButton("cmdBrowserDefault", enabled: true);
            var noteLabel = context.AddLabel("lblDoubleClickBrowsersNote", visible: true);

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: false);

            // Act
            handlers.LstBrowsers_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeFalse();
            cloneButton.Enabled.Should().BeFalse();
            deleteButton.Enabled.Should().BeFalse();
            defaultButton.Enabled.Should().BeFalse();
            GetVisibleSetting(noteLabel).Should().BeFalse();
        }

        [Fact]
        public void LstBrowsers_SelectedIndexChanged_WithNonListViewSender_ShouldLeaveButtonsUnchanged()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("cmdBrowserEdit", enabled: true);
            var handlers = new OptionsFormListHandlers(context);

            // Act
            // senderがListViewでない場合は何もせず抜ける
            handlers.LstBrowsers_SelectedIndexChanged(new object(), EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeTrue("senderが不正な場合は状態を変更しない");
        }

        [Fact]
        public void LstBrowsers_SelectedIndexChanged_WithMissingControls_ShouldNotThrow()
        {
            // Arrange
            // 対象のボタンが1つも存在しない状態（タブが未構築の場合に相当）
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            var action = () => handlers.LstBrowsers_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region Auto URLsリスト

        [Fact]
        public void LstURLs_SelectedIndexChanged_WithSelection_ShouldEnableTabButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var tab = context.AddTabPage("tabAutoURLs");
            var editButton = new Button { Name = "btnEdit", Enabled = false };
            var deleteButton = new Button { Name = "btnDelete", Enabled = false };
            var moveUpButton = new Button { Name = "btnMoveUp", Enabled = false };
            var moveDownButton = new Button { Name = "btnMoveDown", Enabled = false };
            var noteLabel = new Label { Name = "lblDoubleClickURLsNote", Visible = false };
            tab.Controls.AddRange(new Control[] { editButton, deleteButton, moveUpButton, moveDownButton, noteLabel });

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            handlers.LstURLs_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeTrue();
            deleteButton.Enabled.Should().BeTrue();
            moveUpButton.Enabled.Should().BeTrue();
            moveDownButton.Enabled.Should().BeTrue();
            GetVisibleSetting(noteLabel).Should().BeTrue();
        }

        [Fact]
        public void LstURLs_SelectedIndexChanged_WithoutSelection_ShouldDisableTabButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var tab = context.AddTabPage("tabAutoURLs");
            var editButton = new Button { Name = "btnEdit", Enabled = true };
            var deleteButton = new Button { Name = "btnDelete", Enabled = true };
            var noteLabel = new Label { Name = "lblDoubleClickURLsNote", Visible = true };
            tab.Controls.AddRange(new Control[] { editButton, deleteButton, noteLabel });

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: false);

            // Act
            handlers.LstURLs_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeFalse();
            deleteButton.Enabled.Should().BeFalse();
            GetVisibleSetting(noteLabel).Should().BeFalse();
        }

        [Fact]
        public void LstURLs_SelectedIndexChanged_WithoutAutoUrlsTab_ShouldNotThrow()
        {
            // Arrange
            // tabAutoURLsが存在しない（タブ未構築）場合でも落ちないこと
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            var action = () => handlers.LstURLs_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_SelectedIndexChanged_WithNullSender_ShouldNotThrow()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var handlers = new OptionsFormListHandlers(context);

            // Act
            var action = () => handlers.LstURLs_SelectedIndexChanged(null, EventArgs.Empty);

            // Assert
            action.Should().NotThrow();
        }

        #endregion

        #region プロトコルリスト

        [Fact]
        public void LstProtocols_SelectedIndexChanged_WithSelection_ShouldEnableButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("btnEdit");
            var deleteButton = context.AddButton("btnDelete");
            var noteLabel = context.AddLabel("lblDoubleClickProtocolsNote");

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            handlers.LstProtocols_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeTrue();
            deleteButton.Enabled.Should().BeTrue();
            GetVisibleSetting(noteLabel).Should().BeTrue();
        }

        [Fact]
        public void LstProtocols_SelectedIndexChanged_WithoutSelection_ShouldDisableButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("btnEdit", enabled: true);
            var deleteButton = context.AddButton("btnDelete", enabled: true);
            var noteLabel = context.AddLabel("lblDoubleClickProtocolsNote", visible: true);

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: false);

            // Act
            handlers.LstProtocols_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeFalse();
            deleteButton.Enabled.Should().BeFalse();
            GetVisibleSetting(noteLabel).Should().BeFalse();
        }

        #endregion

        #region ファイルタイプリスト

        [Fact]
        public void LstFileTypes_SelectedIndexChanged_WithSelection_ShouldEnableButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("cmdEditFileType");
            var deleteButton = context.AddButton("cmdDeleteFileType");
            var noteLabel = context.AddLabel("lblDoubleClickFileTypesNote");

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: true);

            // Act
            handlers.LstFileTypes_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeTrue();
            deleteButton.Enabled.Should().BeTrue();
            GetVisibleSetting(noteLabel).Should().BeTrue();
        }

        [Fact]
        public void LstFileTypes_SelectedIndexChanged_WithoutSelection_ShouldDisableButtons()
        {
            // Arrange
            using var context = new FakeOptionsFormContext();
            var editButton = context.AddButton("cmdEditFileType", enabled: true);
            var deleteButton = context.AddButton("cmdDeleteFileType", enabled: true);
            var noteLabel = context.AddLabel("lblDoubleClickFileTypesNote", visible: true);

            var handlers = new OptionsFormListHandlers(context);
            using var listView = CreateListViewWithSelection(hasSelection: false);

            // Act
            handlers.LstFileTypes_SelectedIndexChanged(listView, EventArgs.Empty);

            // Assert
            editButton.Enabled.Should().BeFalse();
            deleteButton.Enabled.Should().BeFalse();
            GetVisibleSetting(noteLabel).Should().BeFalse();
        }

        #endregion
    }
}
