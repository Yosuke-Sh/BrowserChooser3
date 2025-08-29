using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのドラッグ&amp;ドロップイベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormDragDropHandlers
    {
        private readonly OptionsForm _form;
        private readonly Settings _settings;
        private readonly Dictionary<int, Browser> _mBrowser;
        private readonly Dictionary<int, Protocol> _mProtocols;

        private readonly Action<bool> _setModified;
        private readonly Action _rebuildAutoURLs;

        // Auto URLsのドラッグ&ドロップ機能
        private bool _mbURLMouseDown = false;
        private ListViewItem? _mDragHighlight = null;
        private Point _mStartPoint = Point.Empty;

        /// <summary>
        /// OptionsFormDragDropHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="settings">設定オブジェクト</param>
        /// <param name="mBrowser">ブラウザ辞書</param>
        /// <param name="mProtocols">プロトコル辞書</param>

        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <param name="rebuildAutoURLs">Auto URLs再構築アクション</param>
        public OptionsFormDragDropHandlers(
            OptionsForm form,
            Settings settings,
            Dictionary<int, Browser> mBrowser,
            Dictionary<int, Protocol> mProtocols,

            Action<bool> setModified,
            Action rebuildAutoURLs)
        {
            _form = form;
            _settings = settings;
            _mBrowser = mBrowser;
            _mProtocols = mProtocols;

            _setModified = setModified;
            _rebuildAutoURLs = rebuildAutoURLs;
        }

        #region Auto URLs ドラッグ&ドロップ

        /// <summary>
        /// Auto URLsのドラッグ進入イベント
        /// </summary>
        public void LstURLs_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent("System.Windows.Forms.ListViewItem") == true)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
            _mDragHighlight = null;
        }

        /// <summary>
        /// Auto URLsのドラッグオーバーイベント
        /// </summary>
        public void LstURLs_DragOver(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var hitTest = listView.HitTest(point.X, point.Y);

            if (hitTest.Item != null)
            {
                if (_mDragHighlight == null)
                {
                    hitTest.Item.BackColor = SystemColors.Highlight;
                    _mDragHighlight = hitTest.Item;
                }
                else if (_mDragHighlight.Text != hitTest.Item.Text)
                {
                    _mDragHighlight.BackColor = SystemColors.Window;
                    hitTest.Item.BackColor = SystemColors.Highlight;
                    _mDragHighlight = hitTest.Item;
                }
            }
            else
            {
                if (_mDragHighlight != null)
                {
                    _mDragHighlight.BackColor = SystemColors.Window;
                }
            }
        }

        /// <summary>
        /// Auto URLsのドロップイベント
        /// </summary>
        public void LstURLs_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var myItem = e.Data?.GetData("System.Windows.Forms.ListViewItem") as ListViewItem;
            if (myItem == null) return;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var hitTest = listView.HitTest(point.X, point.Y);

            if (hitTest.Item == null)
            {
                // リストの先頭または末尾に移動
                listView.Items.Remove(myItem);
                if (point.X < 0)
                {
                    listView.Items.Insert(0, myItem);
                }
                else
                {
                    listView.Items.Insert(listView.Items.Count, myItem);
                }
            }
            else if (hitTest.Item.Text != myItem.Text)
            {
                // 特定の位置に移動
                var oldIndex = hitTest.Item.Index;
                listView.Items.Remove(myItem);
                listView.Items.Insert(hitTest.Item.Index, myItem);
            }

            // 内部リストを再構築
            _rebuildAutoURLs();

            // ハイライトを削除
            if (_mDragHighlight != null)
            {
                _mDragHighlight.BackColor = SystemColors.Window;
            }
        }

        /// <summary>
        /// Auto URLsのマウスダウンイベント
        /// </summary>
        public void LstURLs_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not ListView listView) return;

            if (listView.SelectedItems.Count > 0)
            {
                _mbURLMouseDown = true;
                _mStartPoint = e.Location;
            }
        }

        /// <summary>
        /// Auto URLsのマウス移動イベント
        /// </summary>
        public void LstURLs_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not ListView listView) return;

            if (_mbURLMouseDown)
            {
                if (_mStartPoint.X != e.Location.X || _mStartPoint.Y != e.Location.Y)
                {
                    if (listView.SelectedItems.Count > 0)
                    {
                        var dataObject = new DataObject("System.Windows.Forms.ListViewItem", listView.SelectedItems[0]);
                        listView.DoDragDrop(dataObject, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Auto URLsのマウスアップイベント
        /// </summary>
        public void LstURLs_MouseUp(object sender, MouseEventArgs e)
        {
            _mbURLMouseDown = false;
        }

        #endregion

        #region ブラウザ ドラッグ&ドロップ

        /// <summary>
        /// ブラウザリストのドラッグ進入イベント
        /// </summary>
        public void LstBrowsers_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
                if (sender is ListView listView)
                {
                    listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// ブラウザリストのドラッグ離脱イベント
        /// </summary>
        public void LstBrowsers_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is ListView listView)
            {
                listView.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
        }

        /// <summary>
        /// ブラウザリストのドロップイベント
        /// </summary>
        public void LstBrowsers_DragDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var files = e.Data?.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;

            foreach (var filePath in files)
            {
                if (Path.GetExtension(filePath.ToLower()) == ".exe")
                {
                    // ブラウザ追加ダイアログを表示
                    try
                    {
                        var addEditForm = new AddEditBrowserForm();
                        var newBrowser = new Browser
                        {
                            Guid = Guid.NewGuid(),
                            Name = Path.GetFileNameWithoutExtension(filePath),
                            Target = filePath,
                            Arguments = "",
                            X = 0,
                            Y = 0,
                            Hotkey = '\0'
                        };

                        if (addEditForm.EditBrowser(newBrowser, _mBrowser, _mProtocols, false))
                        {
                            var finalBrowser = addEditForm.GetData();
                            _mBrowser.Add(_mBrowser.Count + 1, finalBrowser);
                            _setModified(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("OptionsFormDragDropHandlers.LstBrowsers_DragDrop", "ブラウザ追加エラー", ex.Message);
                        MessageBox.Show($"ブラウザ追加に失敗しました: {ex.Message}", "エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion

        #region ListView ドラッグ&ドロップ

        /// <summary>
        /// ListView URLsのドラッグ進入イベント
        /// </summary>
        public void ListViewURLs_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent("System.Windows.Forms.ListViewItem") == true)
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// ListView URLsのドラッグオーバーイベント
        /// </summary>
        public void ListViewURLs_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent("System.Windows.Forms.ListViewItem") == true)
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        /// <summary>
        /// ListView URLsのドラッグ離脱イベント
        /// </summary>
        public void ListViewURLs_DragLeave(object? sender, EventArgs e)
        {
            // 必要に応じてハイライトを削除
        }

        /// <summary>
        /// ListView URLsのドロップイベント
        /// </summary>
        public void ListViewURLs_DragDrop(object? sender, DragEventArgs e)
        {
            if (sender is not ListView listView) return;

            var draggedItem = e.Data?.GetData("System.Windows.Forms.ListViewItem") as ListViewItem;
            if (draggedItem == null) return;

            var point = listView.PointToClient(new Point(e.X, e.Y));
            var hitTest = listView.HitTest(point.X, point.Y);

            if (hitTest.Item != null && hitTest.Item != draggedItem)
            {
                var sourceIndex = draggedItem.Index;
                var targetIndex = hitTest.Item.Index;

                // アイテムを移動
                listView.Items.Remove(draggedItem);
                listView.Items.Insert(targetIndex, draggedItem);
                
                _setModified(true);
            }
        }

        #endregion

        #region ブラウザListView ドラッグ&ドロップ

        /// <summary>
        /// ブラウザListViewのドラッグ進入イベント
        /// </summary>
        public void ListViewBrowsers_DragEnter(object? sender, DragEventArgs e)
        {
            // ファイルドロップかどうかをチェック
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
                if (sender is ListView listView)
                {
                    listView.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// ブラウザListViewのドラッグ離脱イベント
        /// </summary>
        public void ListViewBrowsers_DragLeave(object? sender, EventArgs e)
        {
            // 背景色を元に戻す
            if (sender is ListView listView)
            {
                listView.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
        }

        /// <summary>
        /// ブラウザListViewのドロップイベント
        /// </summary>
        public void ListViewBrowsers_DragDrop(object? sender, DragEventArgs e)
        {
            try
            {
                if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true && sender is ListView listView)
                {
                    var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    
                    if (files != null)
                    {
                        foreach (var filePath in files)
                        {
                            if (Path.GetExtension(filePath.ToLower()) == ".exe")
                            {
                                // ブラウザ追加ダイアログを表示
                                var addEditForm = new AddEditBrowserForm();
                                var newBrowser = new Browser
                                {
                                    Guid = Guid.NewGuid(),
                                    Name = Path.GetFileNameWithoutExtension(filePath),
                                    Target = filePath,
                                    Arguments = "",
                                    X = 0,
                                    Y = 0,
                                    Hotkey = '\0'
                                };

                                if (addEditForm.EditBrowser(newBrowser, _mBrowser, _mProtocols, false))
                                {
                                    var finalBrowser = addEditForm.GetData();
                                    _mBrowser.Add(_mBrowser.Count + 1, finalBrowser);
                                    _setModified(true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormDragDropHandlers.ListViewBrowsers_DragDrop", "ブラウザ追加エラー", ex.Message);
                MessageBox.Show($"ブラウザ追加に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// URLリストビューのドラッグ開始イベント
        /// </summary>
        public void ListViewURLs_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (e.Item is ListViewItem item)
            {
                Logger.LogInfo("OptionsFormDragDropHandlers.ListViewURLs_ItemDrag", "URLドラッグ開始", item.Text);
                if (sender is ListView listView)
                {
                    listView.DoDragDrop(item, DragDropEffects.Move);
                }
            }
        }

        #endregion
    }
}
