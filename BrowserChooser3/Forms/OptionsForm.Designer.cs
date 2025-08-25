namespace BrowserChooser3.Forms
{
    partial class OptionsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            pictureBox1 = new PictureBox();
            treeSettings = new TreeView();
            tabSettings = new TabControl();
            saveButton = new Button();
            cancelButton = new Button();
            helpButton = new Button();
            resetButton = new Button();
            categoryPanel = new Panel();
            categoryItemsListView = new ListView();
            btnDeleteCategory = new Button();
            btnEditCategory = new Button();
            btnAddCategory = new Button();
            categoryListView = new ListView();
            lblHiddenBrowserGuid = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            categoryPanel.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 79);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(161, 161);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // treeSettings
            // 
            treeSettings.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeSettings.Location = new Point(12, 12);
            treeSettings.Name = "treeSettings";
            treeSettings.Size = new Size(200, 420);
            treeSettings.TabIndex = 1;
            treeSettings.AfterSelect += TreeSettings_AfterSelect;
            // 
            // tabSettings
            // 
            tabSettings.Appearance = TabAppearance.FlatButtons;
            tabSettings.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabSettings.ItemSize = new Size(0, 1);
            tabSettings.Location = new Point(220, 12);
            tabSettings.Name = "tabSettings";
            tabSettings.SelectedIndex = 0;
            tabSettings.Size = new Size(750, 430);
            tabSettings.SizeMode = TabSizeMode.Fixed;
            tabSettings.TabIndex = 2;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            saveButton.Location = new Point(685, 450);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(85, 35);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save";
            saveButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cancelButton.Location = new Point(785, 450);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(85, 35);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            // 
            // helpButton
            // 
            helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            helpButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            helpButton.Location = new Point(885, 450);
            helpButton.Name = "helpButton";
            helpButton.Size = new Size(85, 35);
            helpButton.TabIndex = 5;
            helpButton.Text = "Help";
            helpButton.UseVisualStyleBackColor = true;
            // 
            // resetButton
            // 
            resetButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            resetButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            resetButton.Location = new Point(585, 450);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(85, 35);
            resetButton.TabIndex = 6;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += ResetToDefaults_Click;
            // 
            // categoryPanel
            // 
            categoryPanel.Controls.Add(categoryItemsListView);
            categoryPanel.Controls.Add(btnDeleteCategory);
            categoryPanel.Controls.Add(btnEditCategory);
            categoryPanel.Controls.Add(btnAddCategory);
            categoryPanel.Controls.Add(categoryListView);
            categoryPanel.Location = new Point(230, 15);
            categoryPanel.Name = "categoryPanel";
            categoryPanel.Size = new Size(630, 420);
            categoryPanel.TabIndex = 7;
            categoryPanel.Visible = false;
            // 
            // categoryItemsListView
            // 
            categoryItemsListView.FullRowSelect = true;
            categoryItemsListView.GridLines = true;
            categoryItemsListView.Location = new Point(320, 10);
            categoryItemsListView.Name = "categoryItemsListView";
            categoryItemsListView.Size = new Size(270, 300);
            categoryItemsListView.TabIndex = 4;
            categoryItemsListView.UseCompatibleStateImageBehavior = false;
            categoryItemsListView.View = View.Details;
            // 
            // btnDeleteCategory
            // 
            btnDeleteCategory.Location = new Point(210, 320);
            btnDeleteCategory.Name = "btnDeleteCategory";
            btnDeleteCategory.Size = new Size(90, 40);
            btnDeleteCategory.TabIndex = 3;
            btnDeleteCategory.Text = "Delete Category";
            btnDeleteCategory.UseVisualStyleBackColor = true;
            // 
            // btnEditCategory
            // 
            btnEditCategory.Location = new Point(110, 320);
            btnEditCategory.Name = "btnEditCategory";
            btnEditCategory.Size = new Size(90, 40);
            btnEditCategory.TabIndex = 2;
            btnEditCategory.Text = "Edit Category";
            btnEditCategory.UseVisualStyleBackColor = true;
            // 
            // btnAddCategory
            // 
            btnAddCategory.Location = new Point(10, 320);
            btnAddCategory.Name = "btnAddCategory";
            btnAddCategory.Size = new Size(90, 40);
            btnAddCategory.TabIndex = 1;
            btnAddCategory.Text = "Add Category";
            btnAddCategory.UseVisualStyleBackColor = true;
            // 
            // categoryListView
            // 
            categoryListView.FullRowSelect = true;
            categoryListView.GridLines = true;
            categoryListView.Location = new Point(10, 10);
            categoryListView.Name = "categoryListView";
            categoryListView.Size = new Size(200, 300);
            categoryListView.TabIndex = 0;
            categoryListView.UseCompatibleStateImageBehavior = false;
            categoryListView.View = View.Details;
            // 
            // lblHiddenBrowserGuid
            // 
            lblHiddenBrowserGuid.AutoSize = true;
            lblHiddenBrowserGuid.Location = new Point(0, 0);
            lblHiddenBrowserGuid.Name = "lblHiddenBrowserGuid";
            lblHiddenBrowserGuid.Size = new Size(0, 28);
            lblHiddenBrowserGuid.TabIndex = 7;
            lblHiddenBrowserGuid.Visible = false;
            // 
            // OptionsForm
            // 
            AutoScaleDimensions = new SizeF(11F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(983, 494);
            Controls.Add(lblHiddenBrowserGuid);
            Controls.Add(categoryPanel);
            Controls.Add(resetButton);
            Controls.Add(helpButton);
            Controls.Add(cancelButton);
            Controls.Add(saveButton);
            Controls.Add(tabSettings);
            Controls.Add(treeSettings);
            Controls.Add(pictureBox1);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MaximizeBox = false;
            MinimumSize = new Size(600, 400);
            Name = "OptionsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            TopMost = true;
            Resize += OptionsForm_Resize;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            categoryPanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TreeView treeSettings;
        /// <summary>
        /// 設定タブコントロール
        /// </summary>
        public TabControl tabSettings;
        private Button saveButton;
        private Button cancelButton;
        private Button helpButton;
        private Button resetButton;
        private Panel categoryPanel;
        private ListView categoryListView;
        private Button btnAddCategory;
        private Button btnEditCategory;
        private Button btnDeleteCategory;
        private ListView categoryItemsListView;
        private Label lblHiddenBrowserGuid;
    }
}
