using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.OptionsFormHandlers;
using BrowserChooser3.Tests.TestHelpers.Fixtures;
using FluentAssertions;
using Xunit;

namespace BrowserChooser3.Tests
{
    /// <summary>
    /// OptionsFormDragDropHandlersクラスの単体テスト
    /// </summary>
    /// <remarks>
    /// <para>
    /// 従来30件が <c>new Mock&lt;OptionsForm&gt;()</c> を理由にスキップされていたうえ、
    /// アサーションは <c>NotThrow()</c> / <c>NotBeNull()</c> のみだった。
    /// IOptionsFormContextの抽出により、ドラッグ効果の決定・ハイライトの遷移・
    /// 並び替え結果という実際の副作用を検証できるようになった。
    /// </para>
    /// <para>
    /// ファイルドロップによるブラウザ追加経路（<c>LstBrowsers_DragDrop</c> /
    /// <c>ListViewBrowsers_DragDrop</c>）は AddEditBrowserForm をモーダル表示するため
    /// ヘッドレスでは実行できない。ここでは拡張子フィルタで弾かれる分岐（子フォームを
    /// 開かずに抜ける経路）のみを対象にする。
    /// </para>
    /// </remarks>
    public class OptionsFormDragDropHandlersTests : IDisposable
    {
        private readonly FakeOptionsFormContext _context = new();
        private readonly Dictionary<int, Browser> _browsers = new();
        private readonly Dictionary<int, Protocol> _protocols = new();
        private bool _modified;
        private int _rebuildCount;

        private OptionsFormDragDropHandlers CreateHandlers() =>
            new(_context, new Settings(), _browsers, _protocols,
                v => _modified = v, () => _rebuildCount++);

        /// <summary>
        /// ドラッグ元として使うListViewItemを保持したDragEventArgsを構築します。
        /// </summary>
        private static DragEventArgs DragArgsWithItem(ListViewItem item, int x = 0, int y = 0) =>
            new(new DataObject("System.Windows.Forms.ListViewItem", item),
                0, x, y, DragDropEffects.Move, DragDropEffects.None);

        /// <summary>
        /// 何のデータも持たないDragEventArgsを構築します。
        /// </summary>
        private static DragEventArgs EmptyDragArgs(int x = 0, int y = 0) =>
            new(new DataObject(), 0, x, y, DragDropEffects.Move, DragDropEffects.None);

        /// <summary>
        /// ハンドル生成済みのListViewを作ります。
        /// </summary>
        /// <remarks>
        /// ネイティブハンドルが無いと <see cref="ListView.HitTest(int,int)"/> も
        /// <see cref="ListView.SelectedItems"/> も正しく機能しないため、
        /// 明示的に <c>CreateControl()</c> する。
        /// </remarks>
        private static ListView CreateRealizedListView(params string[] itemTexts)
        {
            var listView = new ListView
            {
                View = View.Details,
                MultiSelect = false,
                Size = new Size(300, 200)
            };
            listView.Columns.Add("URL", 280);
            foreach (var text in itemTexts)
            {
                listView.Items.Add(new ListViewItem(text));
            }
            listView.CreateControl();
            return listView;
        }

        public void Dispose() => _context.Dispose();

        #region DragEnter — 効果の決定

        [Fact]
        public void LstURLs_DragEnter_WithListViewItemData_ShouldSetMoveEffect()
        {
            // Arrange
            var handlers = CreateHandlers();
            var e = DragArgsWithItem(new ListViewItem("http://example.com"));

            // Act
            handlers.LstURLs_DragEnter(new object(), e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.Move, "ListViewItemのドラッグは並び替えとして受け入れる");
        }

        [Fact]
        public void LstURLs_DragEnter_WithUnrelatedData_ShouldSetNoneEffect()
        {
            // Arrange
            var handlers = CreateHandlers();
            var e = EmptyDragArgs();

            // Act
            handlers.LstURLs_DragEnter(new object(), e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.None, "対象外のデータはドロップを受け付けない");
        }

        [Fact]
        public void ListViewURLs_DragEnter_WithListViewItemData_ShouldSetMoveEffect()
        {
            // Arrange
            var handlers = CreateHandlers();
            var e = DragArgsWithItem(new ListViewItem("http://example.com"));

            // Act
            handlers.ListViewURLs_DragEnter(null, e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.Move);
        }

        [Fact]
        public void ListViewURLs_DragEnter_WithUnrelatedData_ShouldSetNoneEffect()
        {
            // Arrange
            var handlers = CreateHandlers();
            var e = EmptyDragArgs();

            // Act
            handlers.ListViewURLs_DragEnter(null, e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.None);
        }

        [Fact]
        public void ListViewURLs_DragOver_WithUnrelatedData_ShouldLeaveEffectUnchanged()
        {
            // Arrange
            // DragOverは条件を満たすときだけ効果を設定し、それ以外では触らない
            var handlers = CreateHandlers();
            var e = EmptyDragArgs();
            e.Effect = DragDropEffects.Scroll;

            // Act
            handlers.ListViewURLs_DragOver(null, e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.Scroll, "対象外のデータでは効果を書き換えない");
        }

        [Fact]
        public void LstBrowsers_DragEnter_WithFileDrop_ShouldSetCopyEffectAndHighlightList()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            var data = new DataObject(DataFormats.FileDrop, new[] { @"C:\browsers\chrome.exe" });
            var e = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None);

            // Act
            handlers.LstBrowsers_DragEnter(listView, e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.Copy, "実行ファイルのドロップはブラウザ追加として受け入れる");
            listView.BackColor.Should().Be(Color.FromKnownColor(KnownColor.Highlight),
                "ドロップ可能であることを背景色で示す");
        }

        [Fact]
        public void LstBrowsers_DragLeave_ShouldRestoreBackColor()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);

            // Act
            handlers.LstBrowsers_DragLeave(listView, EmptyDragArgs());

            // Assert
            listView.BackColor.Should().Be(Color.FromKnownColor(KnownColor.Window),
                "離脱時にハイライトを解除する");
        }

        [Fact]
        public void ListViewBrowsers_DragEnter_WithoutFileDrop_ShouldSetNoneEffect()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            var e = EmptyDragArgs();

            // Act
            handlers.ListViewBrowsers_DragEnter(listView, e);

            // Assert
            e.Effect.Should().Be(DragDropEffects.None);
            listView.BackColor.Should().NotBe(Color.FromKnownColor(KnownColor.Highlight),
                "受け付けない場合はハイライトしない");
        }

        [Fact]
        public void ListViewBrowsers_DragLeave_ShouldRestoreBackColor()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);

            // Act
            handlers.ListViewBrowsers_DragLeave(listView, EventArgs.Empty);

            // Assert
            listView.BackColor.Should().Be(Color.FromKnownColor(KnownColor.Window));
        }

        #endregion

        #region DragOver — ハイライトの遷移

        [Fact]
        public void LstURLs_DragOver_OverItem_ShouldHighlightThatItem()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("first", "second");
            var target = listView.Items[0];
            var screenPoint = listView.PointToScreen(target.Bounds.Location + new Size(2, 2));

            // Act
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(screenPoint.X, screenPoint.Y));

            // Assert
            target.BackColor.Should().Be(SystemColors.Highlight, "ドロップ先候補をハイライトする");
        }

        [Fact]
        public void LstURLs_DragOver_MovingToAnotherItem_ShouldClearPreviousHighlight()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("first", "second");
            var first = listView.Items[0];
            var second = listView.Items[1];

            var firstPoint = listView.PointToScreen(first.Bounds.Location + new Size(2, 2));
            var secondPoint = listView.PointToScreen(second.Bounds.Location + new Size(2, 2));

            // Act
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(firstPoint.X, firstPoint.Y));
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(secondPoint.X, secondPoint.Y));

            // Assert
            first.BackColor.Should().Be(SystemColors.Window, "前のハイライトは解除される");
            second.BackColor.Should().Be(SystemColors.Highlight, "新しいドロップ先候補がハイライトされる");
        }

        [Fact]
        public void LstURLs_DragOver_OverEmptyArea_ShouldClearHighlight()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("first");
            var first = listView.Items[0];

            var itemPoint = listView.PointToScreen(first.Bounds.Location + new Size(2, 2));
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(itemPoint.X, itemPoint.Y));
            first.BackColor.Should().Be(SystemColors.Highlight, "テストの前提としてハイライトされていること");

            // アイテムが存在しない下方の空白領域へ移動する
            var emptyPoint = listView.PointToScreen(new Point(5, listView.Height - 5));

            // Act
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(emptyPoint.X, emptyPoint.Y));

            // Assert
            first.BackColor.Should().Be(SystemColors.Window, "空白へ出たらハイライトを解除する");
        }

        [Fact]
        public void LstURLs_DragOver_WithNonListViewSender_ShouldNotThrow()
        {
            // Arrange
            // senderがListViewでない場合は何もせずに抜ける防御分岐
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.LstURLs_DragOver(new object(), EmptyDragArgs());

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_DragEnter_ShouldResetHighlightState()
        {
            // Arrange
            // DragEnterはハイライト状態をリセットするため、
            // 直後のDragOverは「初回」として扱われ、確実にハイライトが付く
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("first");
            var first = listView.Items[0];
            var point = listView.PointToScreen(first.Bounds.Location + new Size(2, 2));

            handlers.LstURLs_DragOver(listView, EmptyDragArgs(point.X, point.Y));
            first.BackColor = SystemColors.Window; // 外部要因で色が戻された状況を模す

            // Act
            handlers.LstURLs_DragEnter(listView, EmptyDragArgs());
            handlers.LstURLs_DragOver(listView, EmptyDragArgs(point.X, point.Y));

            // Assert
            first.BackColor.Should().Be(SystemColors.Highlight,
                "DragEnterでハイライト状態がリセットされるため再度ハイライトされる");
        }

        #endregion

        #region DragDrop — 並び替えの結果

        [Fact]
        public void LstURLs_DragDrop_OntoAnotherItem_ShouldMoveItemToThatPosition()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b", "c");
            var dragged = listView.Items[2]; // "c"
            var target = listView.Items[0];  // "a"
            var point = listView.PointToScreen(target.Bounds.Location + new Size(2, 2));

            // Act
            handlers.LstURLs_DragDrop(listView, DragArgsWithItem(dragged, point.X, point.Y));

            // Assert
            listView.Items[0].Text.Should().Be("c", "ドロップ先の位置へ移動する");
            listView.Items.Count.Should().Be(3, "アイテム数は変わらない");
            _rebuildCount.Should().Be(1, "内部リストが再構築される");
        }

        [Fact]
        public void LstURLs_DragDrop_OntoEmptyArea_ShouldMoveItemToEnd()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b", "c");
            var dragged = listView.Items[0]; // "a"
            var emptyPoint = listView.PointToScreen(new Point(5, listView.Height - 5));

            // Act
            handlers.LstURLs_DragDrop(listView, DragArgsWithItem(dragged, emptyPoint.X, emptyPoint.Y));

            // Assert
            listView.Items[listView.Items.Count - 1].Text.Should().Be("a", "空白へのドロップは末尾へ移動する");
            listView.Items.Count.Should().Be(3);
            _rebuildCount.Should().Be(1);
        }

        [Fact]
        public void LstURLs_DragDrop_OntoItself_ShouldKeepOrder()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b", "c");
            var dragged = listView.Items[1]; // "b"
            var point = listView.PointToScreen(dragged.Bounds.Location + new Size(2, 2));

            // Act
            handlers.LstURLs_DragDrop(listView, DragArgsWithItem(dragged, point.X, point.Y));

            // Assert
            listView.Items[0].Text.Should().Be("a");
            listView.Items[1].Text.Should().Be("b", "自分自身へのドロップでは順序が変わらない");
            listView.Items[2].Text.Should().Be("c");
        }

        [Fact]
        public void LstURLs_DragDrop_WithoutItemData_ShouldNotRebuild()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b");

            // Act
            handlers.LstURLs_DragDrop(listView, EmptyDragArgs());

            // Assert
            listView.Items.Count.Should().Be(2);
            _rebuildCount.Should().Be(0, "ドラッグ対象が無ければ再構築しない");
        }

        [Fact]
        public void LstURLs_DragDrop_WithNonListViewSender_ShouldNotRebuild()
        {
            // Arrange
            var handlers = CreateHandlers();

            // Act
            handlers.LstURLs_DragDrop(new object(), DragArgsWithItem(new ListViewItem("a")));

            // Assert
            _rebuildCount.Should().Be(0);
        }

        [Fact]
        public void ListViewURLs_DragDrop_OntoAnotherItem_ShouldMoveAndSetModified()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b", "c");
            var dragged = listView.Items[2]; // "c"
            var target = listView.Items[0];  // "a"
            var point = listView.PointToScreen(target.Bounds.Location + new Size(2, 2));

            // Act
            handlers.ListViewURLs_DragDrop(listView, DragArgsWithItem(dragged, point.X, point.Y));

            // Assert
            listView.Items[0].Text.Should().Be("c");
            _modified.Should().BeTrue("並び替えは変更として記録される");
        }

        [Fact]
        public void ListViewURLs_DragDrop_OntoItself_ShouldNotSetModified()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a", "b");
            var dragged = listView.Items[0];
            var point = listView.PointToScreen(dragged.Bounds.Location + new Size(2, 2));

            // Act
            handlers.ListViewURLs_DragDrop(listView, DragArgsWithItem(dragged, point.X, point.Y));

            // Assert
            listView.Items[0].Text.Should().Be("a");
            _modified.Should().BeFalse("順序が変わらないなら変更扱いにしない");
        }

        [Fact]
        public void ListViewURLs_DragDrop_WithoutItemData_ShouldNotSetModified()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a");

            // Act
            handlers.ListViewURLs_DragDrop(listView, EmptyDragArgs());

            // Assert
            _modified.Should().BeFalse();
        }

        #endregion

        #region ファイルドロップ — 非exeの除外

        [Fact]
        public void LstBrowsers_DragDrop_WithNonExecutableFile_ShouldNotAddBrowser()
        {
            // Arrange
            // .exe以外は子フォームを開かずに無視される（開くとモーダルで停止するため
            // ここが実際にフィルタされていることの確認は重要）
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            var data = new DataObject(DataFormats.FileDrop, new[] { @"C:\docs\readme.txt" });
            var e = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None);

            // Act
            handlers.LstBrowsers_DragDrop(listView, e);

            // Assert
            _browsers.Should().BeEmpty("実行ファイル以外はブラウザとして追加しない");
            _modified.Should().BeFalse();
        }

        [Fact]
        public void LstBrowsers_DragDrop_WithoutFileData_ShouldNotAddBrowser()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();

            // Act
            handlers.LstBrowsers_DragDrop(listView, EmptyDragArgs());

            // Assert
            _browsers.Should().BeEmpty();
        }

        [Fact]
        public void ListViewBrowsers_DragDrop_WithNonExecutableFile_ShouldNotAddBrowser()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();
            var data = new DataObject(DataFormats.FileDrop, new[] { @"C:\docs\readme.txt" });
            var e = new DragEventArgs(data, 0, 0, 0, DragDropEffects.Copy, DragDropEffects.None);

            // Act
            handlers.ListViewBrowsers_DragDrop(listView, e);

            // Assert
            _browsers.Should().BeEmpty();
            _modified.Should().BeFalse();
        }

        [Fact]
        public void ListViewBrowsers_DragDrop_WithoutFileData_ShouldNotAddBrowser()
        {
            // Arrange
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView();

            // Act
            handlers.ListViewBrowsers_DragDrop(listView, EmptyDragArgs());

            // Assert
            _browsers.Should().BeEmpty();
        }

        #endregion

        #region マウス操作によるドラッグ開始条件

        [Fact]
        public void LstURLs_MouseMove_WithoutPrecedingMouseDown_ShouldNotStartDrag()
        {
            // Arrange
            // MouseDownを経ていない移動でDoDragDropが呼ばれると、
            // ヘッドレス環境ではモーダルなドラッグループに入って停止しうる。
            // ガードが効いていることを「例外なく即座に戻る」ことで確認する。
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a");
            listView.Items[0].Selected = true;

            // Act
            var action = () => handlers.LstURLs_MouseMove(listView, new MouseEventArgs(MouseButtons.Left, 1, 50, 50, 0));

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_MouseDown_WithoutSelection_ShouldNotArmDrag()
        {
            // Arrange
            // 未選択でMouseDownしてもドラッグ待機状態にならないため、
            // 続くMouseMoveでドラッグは開始されない
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a");
            listView.SelectedItems.Count.Should().Be(0, "テストの前提として未選択であること");

            // Act
            handlers.LstURLs_MouseDown(listView, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            var action = () => handlers.LstURLs_MouseMove(listView, new MouseEventArgs(MouseButtons.Left, 1, 50, 50, 0));

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_MouseMove_AtSamePositionAsMouseDown_ShouldNotStartDrag()
        {
            // Arrange
            // 同一座標での移動はドラッグとみなさない（クリックとの区別）
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a");
            listView.Items[0].Selected = true;

            handlers.LstURLs_MouseDown(listView, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));

            // Act
            var action = () => handlers.LstURLs_MouseMove(listView, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_MouseUp_ShouldDisarmDrag()
        {
            // Arrange
            // MouseUp後は待機状態が解除され、以降のMouseMoveでドラッグが始まらない
            var handlers = CreateHandlers();
            using var listView = CreateRealizedListView("a");
            listView.Items[0].Selected = true;

            handlers.LstURLs_MouseDown(listView, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));
            handlers.LstURLs_MouseUp(listView, new MouseEventArgs(MouseButtons.Left, 1, 10, 10, 0));

            // Act
            var action = () => handlers.LstURLs_MouseMove(listView, new MouseEventArgs(MouseButtons.Left, 1, 50, 50, 0));

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void LstURLs_MouseDown_WithNonListViewSender_ShouldNotThrow()
        {
            // Arrange
            var handlers = CreateHandlers();

            // Act
            var action = () => handlers.LstURLs_MouseDown(new object(), new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));

            // Assert
            action.Should().NotThrow();
        }

        #endregion
    }
}
