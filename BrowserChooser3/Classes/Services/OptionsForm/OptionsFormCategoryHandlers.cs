using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.Forms;

namespace BrowserChooser3.Classes.Services.OptionsFormHandlers
{
    /// <summary>
    /// OptionsFormのカテゴリ管理イベントハンドラーを管理するクラス
    /// </summary>
    public class OptionsFormCategoryHandlers
    {
        private readonly OptionsForm _form;
        private readonly Action<bool> _setModified;
        private readonly Action _loadCategories;

        /// <summary>
        /// OptionsFormCategoryHandlersクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="form">親フォーム</param>
        /// <param name="setModified">変更フラグ設定アクション</param>
        /// <param name="loadCategories">カテゴリ読み込みアクション</param>
        public OptionsFormCategoryHandlers(
            OptionsForm form,
            Action<bool> setModified,
            Action loadCategories)
        {
            _form = form;
            _setModified = setModified;
            _loadCategories = loadCategories;
        }

        /// <summary>
        /// カテゴリ追加ボタンのクリックイベント
        /// </summary>
        public void BtnAddCategory_Click(object? sender, EventArgs e)
        {
            try
            {
                var addEditForm = new AddEditCategoryForm();
                if (addEditForm.AddCategory(_form))
                {
                    var categoryName = addEditForm.GetCategoryName();
                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        // カテゴリを追加（実際の実装では設定に保存）
                        _loadCategories(); // リストを再読み込み
                        _setModified(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormCategoryHandlers.BtnAddCategory_Click", "カテゴリ追加エラー", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリ編集ボタンのクリックイベント
        /// </summary>
        public void BtnEditCategory_Click(object? sender, EventArgs e)
        {
            try
            {
                var categoryListView = _form.Controls.Find("categoryListView", true).FirstOrDefault() as ListView;
                if (categoryListView?.SelectedItems.Count > 0)
                {
                    var selectedCategory = categoryListView.SelectedItems[0].Text;
                    var addEditForm = new AddEditCategoryForm();
                    if (addEditForm.EditCategory(selectedCategory, _form))
                    {
                        var newCategoryName = addEditForm.GetCategoryName();
                        if (!string.IsNullOrEmpty(newCategoryName) && newCategoryName != selectedCategory)
                        {
                            // カテゴリ名を更新（実際の実装では設定に保存）
                            _loadCategories(); // リストを再読み込み
                            _setModified(true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormCategoryHandlers.BtnEditCategory_Click", "カテゴリ編集エラー", ex.Message);
            }
        }

        /// <summary>
        /// カテゴリ削除ボタンのクリックイベント
        /// </summary>
        public void BtnDeleteCategory_Click(object? sender, EventArgs e)
        {
            try
            {
                var categoryListView = _form.Controls.Find("categoryListView", true).FirstOrDefault() as ListView;
                if (categoryListView?.SelectedItems.Count > 0)
                {
                    var selectedCategory = categoryListView.SelectedItems[0].Text;
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete the category '{selectedCategory}'?",
                        "Confirm Delete",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        // カテゴリを削除（実際の実装では設定に保存）
                        _loadCategories(); // リストを再読み込み
                        _setModified(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("OptionsFormCategoryHandlers.BtnDeleteCategory_Click", "カテゴリ削除エラー", ex.Message);
            }
        }
    }
}
