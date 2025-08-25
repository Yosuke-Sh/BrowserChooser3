using System;
using System.Windows.Forms;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// カテゴリの追加・編集用フォーム
    /// </summary>
    public partial class AddEditCategoryForm : Form
    {
        private string _categoryName = "";

        /// <summary>
        /// AddEditCategoryFormクラスの新しいインスタンスを初期化します
        /// </summary>
        public AddEditCategoryForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// カテゴリを追加します
        /// </summary>
        /// <param name="parentForm">親フォーム</param>
        /// <returns>追加が成功した場合はtrue</returns>
        public bool AddCategory(Form parentForm)
        {
            this.Text = "カテゴリ追加";
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;
            this.BringToFront();
            this.ShowDialog(parentForm);
            return DialogResult == DialogResult.OK;
        }

        /// <summary>
        /// カテゴリを編集します
        /// </summary>
        /// <param name="categoryName">編集するカテゴリ名</param>
        /// <param name="parentForm">親フォーム</param>
        /// <returns>編集が成功した場合はtrue</returns>
        public bool EditCategory(string categoryName, Form parentForm)
        {
            this.Text = "カテゴリ編集";
            txtCategoryName.Text = categoryName;
            this.StartPosition = FormStartPosition.CenterParent;
            this.TopMost = true;
            this.BringToFront();
            this.ShowDialog(parentForm);
            return DialogResult == DialogResult.OK;
        }

        /// <summary>
        /// 入力されたカテゴリ名を取得します
        /// </summary>
        /// <returns>カテゴリ名</returns>
        public string GetCategoryName()
        {
            return _categoryName;
        }

        /// <summary>
        /// フォームの初期化
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCategoryName = new System.Windows.Forms.Label();
            this.txtCategoryName = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblCategoryName
            // 
            this.lblCategoryName.AutoSize = true;
            this.lblCategoryName.Location = new System.Drawing.Point(12, 15);
            this.lblCategoryName.Name = "lblCategoryName";
            this.lblCategoryName.Size = new System.Drawing.Size(67, 15);
            this.lblCategoryName.TabIndex = 0;
            this.lblCategoryName.Text = "カテゴリ名:";
            // 
            // txtCategoryName
            // 
            this.txtCategoryName.Location = new System.Drawing.Point(85, 12);
            this.txtCategoryName.Name = "txtCategoryName";
            this.txtCategoryName.Size = new System.Drawing.Size(200, 23);
            this.txtCategoryName.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(85, 50);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(210, 50);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddEditCategoryForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(297, 85);
            this.TopMost = true;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtCategoryName);
            this.Controls.Add(this.lblCategoryName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddEditCategoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblCategoryName = null!;
        private System.Windows.Forms.TextBox txtCategoryName = null!;
        private System.Windows.Forms.Button btnOK = null!;
        private System.Windows.Forms.Button btnCancel = null!;

        /// <summary>
        /// OKボタンのクリックイベント
        /// </summary>
        private void btnOK_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("カテゴリ名を入力してください。", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _categoryName = txtCategoryName.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
