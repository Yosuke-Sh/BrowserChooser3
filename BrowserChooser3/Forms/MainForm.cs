using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Classes.Utilities;
using BrowserChooser3.CustomControls;
using System.Drawing.Drawing2D;

namespace BrowserChooser3.Forms
{
    /// <summary>
    /// メインウィンドウフォーム
    /// ブラウザ選択画面の表示、UI操作、ブラウザ起動を管理します
    /// </summary>
    public partial class MainForm : Form
    {
        private Settings? _settings;
        private List<Browser>? _browsers;
        private string _currentUrl = string.Empty;
        private System.Windows.Forms.Timer? _countdownTimer;
        private int _currentDelay;
        private Browser? _defaultBrowser;
        private Label? _countdownLabel;
        private bool _isPaused = false;

        // Aero効果関連
        private bool _hasAero = false;
        private string _currentText = string.Empty;

        // Browser Chooser 2互換のUI要素
        private Button? _btnInfo;
        private Button? _btnAppStub;
        private Button? _btnOptions;
        private Button? _btnCancel;
        private Button? _btnCopyToClipboard;
        private Button? _btnCopyToClipboardAndClose;
        private CheckBox? _chkAutoClose;
        private CheckBox? _chkAutoOpen;
        private Label? _lblShortcutMessage;
        private System.Windows.Forms.Timer? _tmrDelay;
        private ContextMenuStrip? _cmOptions;

        /// <summary>
        /// MainFormクラスの新しいインスタンスを初期化します
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            InitializeApplication();
        }

        /// <summary>
        /// アプリケーションの初期化
        /// </summary>
        private void InitializeApplication()
        {
            Logger.LogInfo("MainForm.InitializeApplication", "Start");
            
            try
            {
                // 設定を読み込み
                _settings = Settings.Load(Application.StartupPath);
                Settings.Current = _settings;
                _browsers = _settings?.Browsers ?? new List<Browser>();

                // デフォルトブラウザの検索
                _defaultBrowser = _browsers?.FirstOrDefault(b => b.IsDefault);
                
                // カウントダウンの初期値を設定
                _currentDelay = _settings?.DefaultDelay ?? 5;

                // フォームの設定
                ConfigureForm();
                
                // ブラウザボタンの作成
                CreateBrowserButtons();
                
                // カウントダウンラベルの作成
                CreateCountdownLabel();
                
                // Browser Chooser 2互換のUI要素を作成
                CreateCompatibilityUI();
                
                // アイコンの読み込み
                LoadIcons();
                
                // キーボードイベントの設定
                KeyPreview = true;
                KeyDown += MainForm_KeyDown;
                
                // 初期テキストの設定
                UpdateAutoOpenTextWithSpaceKey();
                
                // URL短縮解除の設定
                SetupURLUnshortening();
                
                Logger.LogInfo("MainForm.InitializeApplication", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.InitializeApplication", "初期化エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"アプリケーションの初期化に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// URL短縮解除の設定
        /// </summary>
        private void SetupURLUnshortening()
        {
            if (_settings?.RevealShortURL == true && !string.IsNullOrEmpty(_currentUrl))
            {
                var parts = URLUtilities.DetermineParts(_currentUrl);
                if (parts.IsProtocol == Settings.TriState.True && 
                    (parts.Protocol == "http" || parts.Protocol == "https"))
                {
                    var userAgent = _settings.UserAgent ?? "Mozilla/5.0";
                    URLUtilities.UnshortenURLAsync(_currentUrl, userAgent, (expandedUrl) =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => UpdateURL(expandedUrl)));
                        }
                        else
                        {
                            UpdateURL(expandedUrl);
                        }
                    });
                }
            }
        }



        /// <summary>
        /// フォームの設定
        /// </summary>
        private void ConfigureForm()
        {
            Logger.LogInfo("MainForm.ConfigureForm", "Start");
            
            // フォームの基本設定（動的サイズ変更対応）
            Text = _settings?.DefaultMessage ?? "Choose a Browser";
            FormBorderStyle = FormBorderStyle.Sizable;  // サイズ変更可能に変更
            MaximizeBox = true;   // 最大化ボタンを有効
            MinimizeBox = true;   // 最小化ボタンを有効
            ShowIcon = false;
            SizeGripStyle = SizeGripStyle.Show;  // サイズグリップを表示
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            CancelButton = _btnCancel;
            KeyPreview = true;
            
            // フォントの設定（現代的で日本語・英語両対応）
            Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
            
            // サイズの設定（動的サイズ変更対応）
            MinimumSize = new Size(600, 300);  // 最小サイズを設定
            ClientSize = new Size(900, 400);   // 初期サイズ
            
            // サイズ変更イベントの設定
            Resize += MainForm_Resize;
            
            // Aero効果の適用
            if (_settings?.UseAero == true && GeneralUtilities.IsAeroEnabled())
            {
                GeneralUtilities.MakeFormGlassy(this);
                _hasAero = true; // Aero効果が有効な場合のフラグを立てる
                Logger.LogInfo("MainForm.ConfigureForm", "Aero効果を適用");
            }
            else
            {
                // Aero効果が無効の場合の背景色設定
                BackColor = Color.FromArgb(185, 209, 234);
                StyleXP(); // Aero効果が無効の場合のスタイル設定
            }
            
            Logger.LogInfo("MainForm.ConfigureForm", "End");
        }

        /// <summary>
        /// フォームサイズ変更時の処理
        /// </summary>
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            try
            {
                // ブラウザボタンの再配置
                RecalculateButtonLayout();
                
                // 互換性UIコントロールの位置調整
                AdjustCompatibilityUILayout();
                
                Logger.LogTrace("MainForm.MainForm_Resize", "フォームサイズ変更完了", ClientSize.Width, ClientSize.Height);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.MainForm_Resize", "サイズ変更エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// ブラウザボタンのレイアウトを再計算
        /// </summary>
        private void RecalculateButtonLayout()
        {
            if (_browsers == null || _settings == null) return;

            var buttonWidth = _settings.IconWidth;
            var buttonHeight = _settings.IconHeight;
            var gapWidth = _settings.IconGapWidth;
            var gapHeight = _settings.IconGapHeight;
            
            // フォーム幅に基づいて列数を計算
            var availableWidth = ClientSize.Width - 80; // 左右マージン（右端ボタン用のスペース確保）
            var columnsPerRow = Math.Max(1, availableWidth / (buttonWidth + gapWidth));
            
            var buttonIndex = 0;
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Tag is Browser)
                {
                    var row = buttonIndex / columnsPerRow;
                    var col = buttonIndex % columnsPerRow;
                    var x = 30 + (col * (buttonWidth + gapWidth));
                    var y = 30 + (row * (buttonHeight + gapHeight));
                    
                    button.Location = new Point(x, y);
                    
                    // 対応するオーバーレイラベルの位置も調整
                    var overlayLabel = Controls.OfType<Label>().FirstOrDefault(l => l.Name == $"lblOverlay_{buttonIndex}");
                    if (overlayLabel != null)
                    {
                        var labelWidth = TextRenderer.MeasureText(overlayLabel.Text, overlayLabel.Font).Width;
                        overlayLabel.Location = new Point(
                            x + (buttonWidth / 2) - (labelWidth / 2),
                            y - 15
                        );
                    }
                    
                    buttonIndex++;
                }
            }
        }

        /// <summary>
        /// フォームの描画処理
        /// Aero効果の有無に応じて背景を描画します
        /// </summary>
        /// <param name="e">描画イベント引数</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!_hasAero)
            {
                // Aero効果が無効の場合: グラデーション背景を描画
                using var brush = new LinearGradientBrush(
                    DisplayRectangle,
                    Color.FromArgb(185, 209, 234),
                    Color.FromArgb(132, 151, 173),
                    LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, DisplayRectangle);
            }
            else if (_hasAero && _settings?.BackgroundColor != Color.Transparent.ToArgb())
            {
                // カスタム背景色を使用
                using var brush = new LinearGradientBrush(
                    DisplayRectangle,
                    Color.FromArgb(185, 209, 234),
                    Color.FromArgb(_settings?.BackgroundColor ?? Color.White.ToArgb()),
                    LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, DisplayRectangle);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// XPスタイルの設定
        /// Aero効果が無効の場合のフォームスタイルを設定します
        /// </summary>
        private void StyleXP()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            if (_chkAutoClose != null)
                _chkAutoClose.BackColor = Color.Transparent;
            if (_chkAutoOpen != null)
                _chkAutoOpen.BackColor = Color.Transparent;
        }

        /// <summary>
        /// 互換性UIコントロールのレイアウトを調整
        /// </summary>
        private void AdjustCompatibilityUILayout()
        {
            if (_chkAutoClose != null)
            {
                _chkAutoClose.Location = new Point(20, ClientSize.Height - 80);
            }
            
            if (_chkAutoOpen != null)
            {
                _chkAutoOpen.Location = new Point(20, ClientSize.Height - 50);
            }
            
            if (_btnOptions != null)
            {
                _btnOptions.Location = new Point(ClientSize.Width - 35, 15);
            }
            
            if (_btnCopyToClipboard != null)
            {
                _btnCopyToClipboard.Location = new Point(ClientSize.Width - 35, 50);
            }
            
            if (_btnCopyToClipboardAndClose != null)
            {
                _btnCopyToClipboardAndClose.Location = new Point(ClientSize.Width - 35, 85);
            }
            
            if (_countdownLabel != null)
            {
                _countdownLabel.Location = new Point(20, ClientSize.Height - 20);
            }
            
            // オーバーレイラベルの位置も調整
            AdjustOverlayLabels();
        }

        /// <summary>
        /// ブラウザボタンの作成
        /// </summary>
        private void CreateBrowserButtons()
        {
            Logger.LogInfo("MainForm.CreateBrowserButtons", "Start", $"ブラウザ数: {_browsers?.Count ?? 0}");
            
            var buttonWidth = _settings?.IconWidth ?? 90;
            var buttonHeight = _settings?.IconHeight ?? 100;
            var gapWidth = _settings?.IconGapWidth ?? 0;
            var gapHeight = _settings?.IconGapHeight ?? 0;
            
            if (_browsers == null) return;
            
            // 既存のブラウザボタンとオーバーレイラベルを削除
            var buttonsToRemove = Controls.OfType<Button>().Where(b => b.Tag is Browser).ToList();
            var labelsToRemove = Controls.OfType<Label>().Where(l => l.Name.StartsWith("lblOverlay_")).ToList();
            
            foreach (var btn in buttonsToRemove)
            {
                Controls.Remove(btn);
                btn.Dispose();
            }
            
            foreach (var lbl in labelsToRemove)
            {
                Controls.Remove(lbl);
                lbl.Dispose();
            }
            
            var visibleBrowsers = _browsers.Where(b => b.Visible).ToList();
            
            for (int i = 0; i < visibleBrowsers.Count; i++)
            {
                var browser = visibleBrowsers[i];
                
                var button = new FFButton
                {
                    Name = $"btnBrowser_{i}",
                    Text = browser.Name, // テキストはブラウザ名のみ
                    Size = new Size(buttonWidth, buttonHeight),
                    Tag = browser,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    UseVisualStyleBackColor = true,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ShowFocusBox = _settings?.ShowFocus ?? true,
                    TrapArrowKeys = true
                };
                
                // イベントハンドラーの設定
                button.Click += BrowserButton_Click;
                
                // FFButtonの矢印キーイベントハンドラーを設定
                if (button is FFButton ffButton)
                {
                    ffButton.ArrowKeyUp += FFButton_ArrowKeyUp;
                }
                
                Controls.Add(button);
                
                // ホットキーとデフォルトブラウザのオーバーレイラベルを作成
                CreateOverlayLabel(button, browser, i);
                
                Logger.LogTrace("MainForm.CreateBrowserButtons", "ブラウザボタン作成", browser.Name);
            }
            
            // レイアウトを再計算
            RecalculateButtonLayout();
            
            Logger.LogInfo("MainForm.CreateBrowserButtons", "End");
        }

        /// <summary>
        /// ホットキーとデフォルトブラウザのオーバーレイラベルを作成
        /// </summary>
        private void CreateOverlayLabel(Button button, Browser browser, int index)
        {
            var defaultIndicator = "";
            if (_settings?.DefaultBrowserGuid == browser.Guid)
            {
                defaultIndicator = " / D";
            }
            
            // ホットキーまたはデフォルトブラウザがある場合のみオーバーレイラベルを作成
            if ((browser.Hotkey != '\0' && char.IsDigit(browser.Hotkey)) || !string.IsNullOrEmpty(defaultIndicator))
            {
                var overlayLabel = new Label
                {
                    Name = $"lblOverlay_{index}",
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                
                // テキストの設定
                if (browser.Hotkey != '\0' && char.IsDigit(browser.Hotkey))
                {
                    overlayLabel.Text = browser.Hotkey.ToString() + defaultIndicator;
                }
                else if (!string.IsNullOrEmpty(defaultIndicator))
                {
                    overlayLabel.Text = "D";
                }
                
                // 位置の計算（ボタンの中央上部に配置）
                var labelWidth = TextRenderer.MeasureText(overlayLabel.Text, overlayLabel.Font).Width;
                overlayLabel.Location = new Point(
                    button.Location.X + (button.Width / 2) - (labelWidth / 2),
                    button.Location.Y - 15
                );
                
                Controls.Add(overlayLabel);
                overlayLabel.BringToFront();
                
                Logger.LogTrace("MainForm.CreateOverlayLabel", "オーバーレイラベル作成", $"{browser.Name}: {overlayLabel.Text}");
            }
        }

        /// <summary>
        /// オーバーレイラベルの位置を調整
        /// </summary>
        private void AdjustOverlayLabels()
        {
            if (_browsers == null || _settings == null) return;
            
            var buttonWidth = _settings.IconWidth;
            var gapWidth = _settings.IconGapWidth;
            var gapHeight = _settings.IconGapHeight;
            
            // フォーム幅に基づいて列数を計算
            var availableWidth = ClientSize.Width - 80;
            var columnsPerRow = Math.Max(1, availableWidth / (buttonWidth + gapWidth));
            
            var buttonIndex = 0;
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Tag is Browser)
                {
                    var overlayLabel = Controls.OfType<Label>().FirstOrDefault(l => l.Name == $"lblOverlay_{buttonIndex}");
                    if (overlayLabel != null)
                    {
                        var labelWidth = TextRenderer.MeasureText(overlayLabel.Text, overlayLabel.Font).Width;
                        overlayLabel.Location = new Point(
                            button.Location.X + (buttonWidth / 2) - (labelWidth / 2),
                            button.Location.Y - 15
                        );
                    }
                    buttonIndex++;
                }
            }
        }

        /// <summary>
        /// オプション画面を開く
        /// </summary>
        private void OpenOptionsForm()
        {
            Logger.LogInfo("MainForm.OpenOptionsForm", "Start");
            
            try
            {
                var optionsForm = new OptionsForm(_settings!);
                var result = optionsForm.ShowDialog(this);
                
                if (result == DialogResult.OK)
                {
                    // 設定が変更された場合、フォームを再構築
                    RefreshForm();
                }
                
                Logger.LogInfo("MainForm.OpenOptionsForm", "End", result);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.OpenOptionsForm", "オプション画面表示エラー", ex.Message, ex.StackTrace ?? "");
                MessageBox.Show($"オプション画面の表示に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// フォームを再構築
        /// </summary>
        private void RefreshForm()
        {
            Logger.LogInfo("MainForm.RefreshForm", "Start");
            
            try
            {
                // 既存のコントロールをクリア
                Controls.Clear();
                
                // フォームを再設定
                ConfigureForm();
                
                // ブラウザボタンを再作成
                CreateBrowserButtons();
                
                // カウントダウンラベルを再作成
                CreateCountdownLabel();
                
                Logger.LogInfo("MainForm.RefreshForm", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.RefreshForm", "フォーム再構築エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// ブラウザボタンのクリックイベント（Browser Chooser 2互換）
        /// </summary>
        private void BrowserButton_Click(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is Browser browser)
            {
                Logger.LogInfo("MainForm.BrowserButton_Click", "ブラウザ選択", browser.Name, _currentUrl);
                
                try
                {
                    var autoClose = _chkAutoClose?.Checked ?? true;
                    
                    // Ctrl+クリックで自動終了を無効化
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        autoClose = false;
                    }
                    
                    // BrowserUtilitiesを使用してブラウザを起動
                    BrowserUtilities.LaunchBrowser(browser, _currentUrl, autoClose);
                }
                catch (Exception ex)
                {
                    Logger.LogError("MainForm.BrowserButton_Click", "ブラウザ起動エラー", browser.Name, ex.Message);
                    MessageBox.Show($"ブラウザの起動に失敗しました: {ex.Message}", "エラー", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// ブラウザを起動
        /// </summary>
        private void LaunchBrowser(Browser browser, string url)
        {
            Logger.LogInfo("MainForm.LaunchBrowser", "Start", browser.Name, url);
            
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = browser.Target,
                Arguments = string.IsNullOrEmpty(url) ? browser.Arguments : $"{browser.Arguments} \"{url}\"",
                UseShellExecute = true
            };
            
            System.Diagnostics.Process.Start(startInfo);
            
            Logger.LogInfo("MainForm.LaunchBrowser", "End", browser.Name);
        }

        /// <summary>
        /// URLを更新
        /// </summary>
        public void UpdateURL(string url)
        {
            Logger.LogInfo("MainForm.UpdateURL", "URL更新", url);
            _currentUrl = url;
            
            // StartupLauncherを使用してURLを処理
            StartupLauncher.SetURL(url, _settings?.RevealShortURL ?? false, OnURLUpdated);
            
            // デフォルトブラウザがある場合はカウントダウンを開始
            if (_defaultBrowser != null && (_settings?.DefaultDelay ?? 0) > 0)
            {
                StartCountdown();
            }
        }

        /// <summary>
        /// URL更新時のコールバック
        /// </summary>
        private void OnURLUpdated(string url)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(OnURLUpdated), url);
                return;
            }
            
            _currentUrl = url;
            Logger.LogInfo("MainForm.OnURLUpdated", "URL更新完了", url);
        }

        /// <summary>
        /// Browser Chooser 2互換のUI要素を作成
        /// </summary>
        private void CreateCompatibilityUI()
        {
            Logger.LogInfo("MainForm.CreateCompatibilityUI", "Start");
            
            try
            {
                // Aboutボタン（アイコン付き）
                _btnInfo = new Button
                {
                    AccessibleName = "About",
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(14, 52),
                    Margin = new Padding(0),
                    Name = "btnInfo",
                    Size = new Size(24, 24),
                    TabIndex = 1,
                    UseVisualStyleBackColor = false
                };
                

                
                _btnInfo.Click += BtnInfo_Click;
                Controls.Add(_btnInfo);

                // アプリケーションスタブボタン（Browser Chooser 2互換）
                _btnAppStub = new Button
                {
                    BackColor = Color.Transparent,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(56, 1),
                    Name = "btnAppStub",
                    Size = new Size(75, 80),
                    TabIndex = 0,
                    TabStop = false,
                    UseVisualStyleBackColor = false,
                    Visible = false
                };
                Controls.Add(_btnAppStub);

                // オプションボタン（アイコン付き）
                _btnOptions = new Button
                {
                    AccessibleName = "Options",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(ClientSize.Width - 35, 15),
                    Margin = new Padding(0),
                    Name = "btnOptions",
                    Size = new Size(28, 28),
                    TabIndex = 2,
                    UseVisualStyleBackColor = false,
                    ImageAlign = ContentAlignment.MiddleCenter
                };
                

                
                _btnOptions.Click += BtnOptions_Click;
                Controls.Add(_btnOptions);

                // キャンセルボタン
                _btnCancel = new Button
                {
                    BackColor = Color.Transparent,
                    DialogResult = DialogResult.Cancel,
                    FlatAppearance = { BorderSize = 0 },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(370, 12),
                    Name = "btnCancel",
                    Size = new Size(0, 0),
                    TabIndex = 6,
                    TabStop = false,
                    Text = "Cancel",
                    UseVisualStyleBackColor = false
                };
                Controls.Add(_btnCancel);

                // 自動閉じるチェックボックス（Browser Chooser 2互換）
                _chkAutoClose = new FFCheckBox
                {
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Checked = true,
                    CheckState = CheckState.Checked,
                    Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                    ForeColor = SystemColors.ActiveCaptionText,
                    Location = new Point(20, ClientSize.Height - 80),
                    Name = "chkAutoClose",
                    Size = new Size(400, 24),
                    TabIndex = 5,
                    Text = "ブラウザを選択後に自動的に閉じる",
                    UseCompatibleTextRendering = true,
                    UseVisualStyleBackColor = true,
                    ShowFocusBox = _settings?.ShowFocus ?? true,
                    UsesAero = _settings?.UseAero ?? true
                };
                _chkAutoClose.CheckedChanged += ChkAutoClose_CheckedChanged;
                Controls.Add(_chkAutoClose);

                // 自動開くチェックボックス（Browser Chooser 2互換）
                _chkAutoOpen = new FFCheckBox
                {
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                    ForeColor = SystemColors.ActiveCaptionText,
                    Location = new Point(20, ClientSize.Height - 50),
                    Name = "chkAutoOpen",
                    Size = new Size(450, 22),
                    TabIndex = 6,
                    Text = "指定秒数後にデフォルトブラウザを開く [space key:Timerの一時停止/再開]",
                    UseVisualStyleBackColor = false,
                    ShowFocusBox = _settings?.ShowFocus ?? true,
                    UsesAero = _settings?.UseAero ?? true
                };
                _chkAutoOpen.CheckedChanged += ChkAutoOpen_CheckedChanged;
                Controls.Add(_chkAutoOpen);

                // クリップボードにコピーボタン（アイコン付き）
                _btnCopyToClipboard = new Button
                {
                    AccessibleName = "Copy URL to clipboard and keep open",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.Transparent,
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(ClientSize.Width - 35, 50),
                    Margin = new Padding(0),
                    Name = "btnCopyToClipboard",
                    Size = new Size(28, 28),
                    TabIndex = 3,
                    UseVisualStyleBackColor = false,
                    ImageAlign = ContentAlignment.MiddleCenter
                };
                

                
                _btnCopyToClipboard.Click += BtnCopyToClipboard_Click;
                Controls.Add(_btnCopyToClipboard);

                // クリップボードにコピーして閉じるボタン（アイコン付き）
                _btnCopyToClipboardAndClose = new Button
                {
                    AccessibleName = "Copy URL to clipboard and close",
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.Transparent,
                    FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.Transparent },
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(ClientSize.Width - 35, 85),
                    Margin = new Padding(0),
                    Name = "btnCopyToClipboardAndClose",
                    Size = new Size(28, 28),
                    TabIndex = 4,
                    UseVisualStyleBackColor = false,
                    ImageAlign = ContentAlignment.MiddleCenter
                };
                

                
                _btnCopyToClipboardAndClose.Click += BtnCopyToClipboardAndClose_Click;
                Controls.Add(_btnCopyToClipboardAndClose);

                // ショートカットメッセージラベル
                _lblShortcutMessage = new Label
                {
                    AutoSize = true,
                    BackColor = Color.Black,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Regular, GraphicsUnit.Point, 0),
                    ForeColor = Color.White,
                    Location = new Point(2, 1),
                    Name = "lblShortcutMessage",
                    Size = new Size(60, 50),
                    TabIndex = 7,
                    Text = "Shortcut\r\n/ Default\r\n(if set):",
                    TextAlign = ContentAlignment.MiddleRight
                };
                Controls.Add(_lblShortcutMessage);

                // 遅延タイマー
                _tmrDelay = new System.Windows.Forms.Timer
                {
                    Interval = 1000
                };
                _tmrDelay.Tick += TmrDelay_Tick;

                // コンテキストメニュー
                CreateContextMenu();

                Logger.LogInfo("MainForm.CreateCompatibilityUI", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.CreateCompatibilityUI", "UI作成エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// アイコンの読み込み
        /// </summary>
        private void LoadIcons()
        {
            try
            {
                // Aboutボタンのアイコン読み込み
                if (_btnInfo != null)
                {
                    _btnInfo.Image = Properties.Resources.Icon122;
                }
                
                // アプリケーションスタブボタンのアイコン読み込み
                if (_btnAppStub != null)
                {
                    _btnAppStub.Image = Properties.Resources.BrowserChooserIcon;
                }
                
                // オプションボタンのアイコン読み込み
                if (_btnOptions != null)
                {
                    _btnOptions.Image = Properties.Resources.Icon128;
                }
                
                // コピーボタンのアイコン読み込み
                if (_btnCopyToClipboard != null)
                {
                    var pasteIcon = Properties.Resources.PasteIcon;
                    _btnCopyToClipboard.Image = ImageUtilities.ResizeImage(pasteIcon, 20, 20);
                }
                
                // コピー＆クローズボタンのアイコン読み込み
                if (_btnCopyToClipboardAndClose != null)
                {
                    var pasteAndCloseIcon = Properties.Resources.PasteAndCloseIcon;
                    _btnCopyToClipboardAndClose.Image = ImageUtilities.ResizeImage(pasteAndCloseIcon, 20, 20);
                }
                
                Logger.LogInfo("MainForm.LoadIcons", "アイコン読み込み完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.LoadIcons", "アイコン読み込みエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// コンテキストメニューの作成
        /// </summary>
        private void CreateContextMenu()
        {
            _cmOptions = new ContextMenuStrip();
            
            var miEditMode = new ToolStripMenuItem("&Edit");
            miEditMode.Click += MiEditMode_Click;
            
            var toolStripSeparator1 = new ToolStripSeparator();
            
            var toolStripMenuItem2 = new ToolStripMenuItem("ToolStripMenuItem2");
            var toolStripMenuItem3 = new ToolStripMenuItem("ToolStripMenuItem3");
            
            _cmOptions.Items.AddRange(new ToolStripItem[] { miEditMode, toolStripSeparator1, toolStripMenuItem2, toolStripMenuItem3 });
        }

        /// <summary>
        /// カウントダウンラベルの作成
        /// </summary>
        private void CreateCountdownLabel()
        {
            _countdownLabel = new Label
            {
                Name = "lblCountdown",
                Text = "",
                AutoSize = true,
                Location = new Point(20, ClientSize.Height - 20),
                Visible = false,
                Font = new Font("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                ForeColor = Color.DarkBlue,
                BackColor = Color.LightYellow
            };
            
            Controls.Add(_countdownLabel);
        }

        /// <summary>
        /// カウントダウンの開始
        /// </summary>
        private void StartCountdown()
        {
            if (_defaultBrowser == null) return;
            
            _currentDelay = _settings?.DefaultDelay ?? 5;
            _isPaused = false;
            
            _countdownTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
            
            UpdateCountdownDisplay();
            _countdownLabel!.Visible = true;
            
            Logger.LogInfo("MainForm.StartCountdown", "カウントダウン開始", _currentDelay);
        }

        /// <summary>
        /// カウントダウンタイマーの処理
        /// </summary>
        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            if (_isPaused) return;
            
            _currentDelay--;
            UpdateCountdownDisplay();
            
            if (_currentDelay <= 0)
            {
                _countdownTimer?.Stop();
                LaunchBrowser(_defaultBrowser!, _currentUrl);
                Application.Exit();
            }
        }

        /// <summary>
        /// カウントダウン表示の更新
        /// </summary>
        private void UpdateCountdownDisplay()
        {
            if (_countdownLabel != null)
            {
                var status = _isPaused ? " (一時停止)" : "";
                _countdownLabel.Text = $"デフォルトブラウザで {_currentDelay} 秒後に起動{status}";
            }
        }

        /// <summary>
        /// 自動開くテキストの更新（Browser Chooser 2互換）
        /// </summary>
        private void UpdateAutoOpenText()
        {
            if (_chkAutoOpen != null && _defaultBrowser != null)
            {
                var pauseText = _tmrDelay?.Enabled == false ? "un" : "";
                var browserName = _defaultBrowser.Name ?? "default browser";
                _chkAutoOpen.Text = $"Open {browserName} after {_currentDelay} seconds.  [Space: ({pauseText}pause) timer]";
            }
        }

        /// <summary>
        /// フォーム読み込み時の処理
        /// </summary>
        private void MainForm_Load(object? sender, EventArgs e)
        {
            // 既存のInitializeApplication()の内容をここに移動
            InitializeApplication();
        }

        /// <summary>
        /// 情報ボタンのクリックイベント
        /// </summary>
        private void btnInfo_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.btnInfo_Click", "About画面を開く");
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        /// <summary>
        /// オプションボタンのクリックイベント
        /// </summary>
        private void btnOptions_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.btnOptions_Click", "オプション画面を開く");
            OpenOptionsForm();
        }

        /// <summary>
        /// キャンセルボタンのクリックイベント
        /// </summary>
        private void btnCancel_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.btnCancel_Click", "アプリケーションを終了");
            Application.Exit();
        }

        /// <summary>
        /// クリップボードコピーボタンのクリックイベント
        /// </summary>
        private void btnCopyToClipboard_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.btnCopyToClipboard_Click", "URLをクリップボードにコピー", _currentUrl);
            try
            {
                Clipboard.SetText(_currentUrl);
                MessageBox.Show("URLをクリップボードにコピーしました", "情報", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.btnCopyToClipboard_Click", "クリップボードコピーエラー", ex.Message);
                MessageBox.Show($"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// クリップボードコピー＆クローズボタンのクリックイベント
        /// </summary>
        private void btnCopyToClipboardAndClose_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.btnCopyToClipboardAndClose_Click", "URLをコピーして終了", _currentUrl);
            try
            {
                Clipboard.SetText(_currentUrl);
                Application.Exit();
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.btnCopyToClipboardAndClose_Click", "クリップボードコピーエラー", ex.Message);
                MessageBox.Show($"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 自動クローズチェックボックスの変更イベント
        /// </summary>
        private void chkAutoClose_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.chkAutoClose_CheckedChanged", "自動クローズ設定変更", chkAutoClose.Checked);
            // 設定を保存する処理を追加
        }

        /// <summary>
        /// 自動オープンチェックボックスの変更イベント
        /// </summary>
        private void chkAutoOpen_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.chkAutoOpen_CheckedChanged", "自動オープン設定変更", chkAutoOpen.Checked);
            // 設定を保存する処理を追加
        }

        /// <summary>
        /// 遅延タイマーの処理
        /// </summary>
        private void tmrDelay_Tick(object? sender, EventArgs e)
        {
            CountdownTimer_Tick(sender, e);
        }

        /// <summary>
        /// 編集モードメニューのクリックイベント
        /// </summary>
        private void miEditMode_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.miEditMode_Click", "編集モードを開く");
            OpenOptionsForm();
        }

        /// <summary>
        /// キーボードイベントの処理（Browser Chooser 2互換）
        /// </summary>
        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // オプションショートカット
            if (e.KeyCode.ToString().ToLower() == (_settings?.OptionsShortcut ?? 'O').ToString().ToLower())
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                TopMost = false;
                OpenOptionsForm();
                TopMost = true;
                return;
            }
            
            // 矢印キーとTabキーの処理
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                HandleArrowKeyUp(e.KeyCode);
                return;
            }
            
            // スペースキーでカウントダウンを一時停止/再開
            if (e.KeyCode == Keys.Space && _tmrDelay != null)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                _isPaused = !_isPaused;
                UpdateAutoOpenText();
                return;
            }
            
            // 数字キー（0-9）でホットキー処理
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                var keyNumber = e.KeyCode == Keys.D0 ? 0 : e.KeyCode - Keys.D1 + 1;
                
                foreach (var browser in _browsers ?? new List<Browser>())
                {
                    if (char.IsDigit(browser.Hotkey) && int.Parse(browser.Hotkey.ToString()) == keyNumber)
                    {
                        Logger.LogInfo("MainForm.MainForm_KeyDown", "ホットキー起動", browser.Name, keyNumber);
                        BrowserUtilities.LaunchBrowser(browser, _currentUrl, _chkAutoClose?.Checked ?? true);
                        return;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// 矢印キーによるフォーカス移動（Browser Chooser 2互換）
        /// </summary>
        private void HandleArrowKeyUp(Keys keyData)
        {
            if (_browsers == null || _settings == null) return;

            var currentButton = ActiveControl as Button;
            if (currentButton?.Tag is not Browser currentBrowser) return;

            // 現在のボタンの位置を取得
            var currentIndex = _browsers.IndexOf(currentBrowser);
            if (currentIndex == -1) return;

            var buttonWidth = _settings.IconWidth;
            var buttonHeight = _settings.IconHeight;
            var gapWidth = _settings.IconGapWidth;
            var gapHeight = _settings.IconGapHeight;
            
            // フォーム幅に基づいて列数を計算
            var availableWidth = ClientSize.Width - 80;
            var columnsPerRow = Math.Max(1, availableWidth / (buttonWidth + gapWidth));
            var rows = (_browsers.Count + columnsPerRow - 1) / columnsPerRow;

            var currentRow = currentIndex / columnsPerRow;
            var currentCol = currentIndex % columnsPerRow;

            int targetRow = currentRow;
            int targetCol = currentCol;

            switch (keyData)
            {
                case Keys.Up:
                    targetRow = MinusLoop(currentRow, rows);
                    break;
                case Keys.Down:
                    targetRow = AddLoop(currentRow, rows);
                    break;
                case Keys.Left:
                    targetCol = MinusLoop(currentCol, columnsPerRow);
                    break;
                case Keys.Right:
                    targetCol = AddLoop(targetCol, columnsPerRow);
                    break;
            }

            // ターゲット位置のブラウザを探す
            var targetIndex = targetRow * columnsPerRow + targetCol;
            if (targetIndex < _browsers.Count)
            {
                var targetBrowser = _browsers[targetIndex];
                var targetButton = Controls.OfType<Button>().FirstOrDefault(b => b.Tag == targetBrowser);
                if (targetButton != null)
                {
                    targetButton.Focus();
                    Logger.LogTrace("MainForm.HandleArrowKeyUp", "フォーカス移動", 
                        $"{currentBrowser.Name} -> {targetBrowser.Name}");
                }
            }
        }

        /// <summary>
        /// ループ減算（Browser Chooser 2互換）
        /// </summary>
        private int MinusLoop(int start, int max)
        {
            start = start - 1;
            if (start == 0) return max;
            return start;
        }

        /// <summary>
        /// ループ加算（Browser Chooser 2互換）
        /// </summary>
        private int AddLoop(int start, int max)
        {
            start = start + 1;
            if (start > max) return 1;
            return start;
        }

        #region Browser Chooser 2互換イベントハンドラー

        /// <summary>
        /// Aboutボタンのクリックイベント
        /// </summary>
        private void BtnInfo_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.BtnInfo_Click", "About画面を開く");
            try
            {
                var aboutForm = new AboutForm();
                aboutForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.BtnInfo_Click", "About画面表示エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// オプションボタンのクリックイベント
        /// </summary>
        private void BtnOptions_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.BtnOptions_Click", "オプション画面を開く");
            try
            {
                OpenOptionsForm();
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.BtnOptions_Click", "オプション画面表示エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// キャンセルボタンのクリックイベント
        /// </summary>
        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.BtnCancel_Click", "キャンセル");
            Close();
        }

        /// <summary>
        /// クリップボードにコピーボタンのクリックイベント
        /// </summary>
        private void BtnCopyToClipboard_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.BtnCopyToClipboard_Click", "URLをクリップボードにコピー");
            try
            {
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    Clipboard.SetText(_currentUrl);
                    Logger.LogInfo("MainForm.BtnCopyToClipboard_Click", "URLをクリップボードにコピー完了");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.BtnCopyToClipboard_Click", "クリップボードコピーエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// クリップボードにコピーして閉じるボタンのクリックイベント
        /// </summary>
        private void BtnCopyToClipboardAndClose_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.BtnCopyToClipboardAndClose_Click", "URLをクリップボードにコピーして閉じる");
            try
            {
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    Clipboard.SetText(_currentUrl);
                    Logger.LogInfo("MainForm.BtnCopyToClipboardAndClose_Click", "URLをクリップボードにコピー完了");
                }
                Close();
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.BtnCopyToClipboardAndClose_Click", "クリップボードコピーエラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// 自動閉じるチェックボックスの変更イベント
        /// </summary>
        private void ChkAutoClose_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.ChkAutoClose_CheckedChanged", $"自動閉じる: {_chkAutoClose?.Checked}");
            // 設定に反映する処理を追加
        }

        /// <summary>
        /// 自動開くチェックボックスの変更イベント（Browser Chooser 2互換）
        /// </summary>
        private void ChkAutoOpen_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.ChkAutoOpen_CheckedChanged", $"自動開く: {_chkAutoOpen?.Checked}");
            
            if (_tmrDelay != null)
            {
                _tmrDelay.Enabled = _chkAutoOpen?.Checked ?? false;
                UpdateAutoOpenText();
            }
        }

        /// <summary>
        /// 遅延タイマーのティックイベント（Browser Chooser 2互換）
        /// </summary>
        private void TmrDelay_Tick(object? sender, EventArgs e)
        {
            if (_currentDelay == 0)
            {
                _currentDelay = _settings?.DefaultDelay ?? 5;
            }
            
            // カウントダウン
            _currentDelay--;

            if (_currentDelay > 0)
            {
                var text = $"Open {_defaultBrowser?.Name} in {_currentDelay} seconds. [Space: {(_tmrDelay?.Enabled == false ? "un" : "")}pause timer]";
                
                if (_chkAutoOpen != null)
                {
                    _chkAutoOpen.Text = text;
                    _chkAutoOpen.Invalidate();
                }
            }
            else
            {
                _tmrDelay!.Enabled = false;
                
                var text = $"Automatically opening {_defaultBrowser?.Name}.";
                if (_chkAutoOpen != null)
                {
                    _chkAutoOpen.Text = text;
                    _chkAutoOpen.Invalidate();
                }

                if (_defaultBrowser != null)
                {
                    BrowserUtilities.LaunchBrowser(_defaultBrowser, _currentUrl, _chkAutoClose?.Checked ?? true);
                }
            }
        }

        /// <summary>
        /// 編集モードメニューのクリックイベント
        /// </summary>
        private void MiEditMode_Click(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.MiEditMode_Click", "編集モード");
            // 編集モードの処理を実装
        }

        /// <summary>
        /// FFButtonの矢印キーイベントハンドラー
        /// </summary>
        private void FFButton_ArrowKeyUp(object? sender, Keys keyData)
        {
            Logger.LogTrace("MainForm.FFButton_ArrowKeyUp", $"矢印キー: {keyData}");
            HandleArrowKeyUp(keyData);
        }

        #endregion

        /// <summary>
        /// フォーカス表示の処理
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        public void HandleGotFocus(object sender, EventArgs e)
        {
            var title = _settings?.DefaultMessage ?? "Browser Chooser 3"; // フォールバック

            if (sender is Button button)
            {
                if (button.Tag == null)
                {
                    title = button.AccessibleName ?? title;
                    _currentText = title;
                }
                else if (button.Tag is int index && _browsers != null && index < _browsers.Count)
                {
                    var browser = _browsers[index];
                    _currentText = $"Open {browser.Name}";

                    if (_settings?.ShowURL == true)
                    {
                        title = $"{_currentText}{_settings.Separator}{_currentUrl}";
                    }
                    else
                    {
                        title = _currentText;
                    }
                }
            }

            Text = title.Length > 256 ? title.Substring(0, 256) : title;
        }

        /// <summary>
        /// フォーカス喪失の処理
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント引数</param>
        public void HandleLostFocus(object sender, EventArgs e)
        {
            _currentText = _settings?.DefaultMessage ?? "Browser Chooser 3";
            
            if (_settings?.ShowURL == true)
            {
                if (string.IsNullOrEmpty(_currentText))
                {
                    Text = _currentUrl.Length > 256 ? _currentUrl.Substring(0, 256) : _currentUrl;
                }
                else
                {
                    var fullText = $"{_currentText}{_settings.Separator}{_currentUrl}";
                    Text = fullText.Length > 256 ? fullText.Substring(0, 256) : fullText;
                }
            }
            else
            {
                Text = _currentText.Length > 256 ? _currentText.Substring(0, 256) : _currentText;
            }
        }

        /// <summary>
        /// キーアップイベントの処理
        /// 矢印キーとTabキーでフォーカスを移動します
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">キーイベント引数</param>
        /// <returns>処理された場合はtrue</returns>
        protected bool HandleKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || 
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || 
                e.KeyCode == Keys.Tab)
            {
                // フォーカスを移動
                e.SuppressKeyPress = true;
                e.Handled = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// スペースキーによるタイマー一時停止/再開の処理
        /// </summary>
        /// <param name="e">キーイベント引数</param>
        private void HandleSpaceKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && _tmrDelay != null && _defaultBrowser != null)
            {
                if (_tmrDelay.Enabled)
                {
                    _isPaused = true;
                    _tmrDelay.Stop();
                }
                else
                {
                    _isPaused = false;
                    _tmrDelay.Start();
                }

                UpdateAutoOpenText();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 自動オープンテキストを更新します（スペースキー対応版）
        /// </summary>
        private void UpdateAutoOpenTextWithSpaceKey()
        {
            if (_chkAutoOpen != null && _defaultBrowser != null)
            {
                var pauseStatus = _tmrDelay?.Enabled == false ? "un" : "";
                _chkAutoOpen.Text = $"Open {_defaultBrowser.Name} in {_currentDelay} seconds. [Space: {pauseStatus}pause timer]";
            }
        }
    }
}
