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
        
        /// <summary>
        /// URL表示ラベル
        /// </summary>
        private Label? _urlLabel;
        private TextBox? _urlTextBox;
        

        private System.Windows.Forms.Timer? _countdownTimer;
        private int _currentDelay;
        private Browser? _defaultBrowser;
        private Label? _countdownLabel;
        private bool _isPaused = false;

        private string _currentText = string.Empty;

        // Browser Chooser 2互換のUI要素（デザイナーファイルで定義済み）
        private ContextMenuStrip? _cmOptions;
        
        // ツールチップ
        private ToolTip? _toolTip;

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
                
                // ツールチップの初期化
                InitializeToolTips();
                
                // ブラウザボタンの作成
                CreateBrowserButtons();
                
                // カウントダウンラベルの作成
                CreateCountdownLabel();
                
                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // Browser Chooser 2互換のUI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();
                
                // キーボードイベントの設定
                KeyPreview = true;
                KeyDown += MainForm_KeyDown;
                
                // AutoCloseとAutoOpenの初期化
                InitializeAutoCloseAndAutoOpen();
                
                // 初期テキストの設定
                UpdateAutoOpenTextWithSpaceKey();
                
                // URL短縮解除の設定
                SetupURLUnshortening();
                
                // 初期化完了後にURL表示ラベルを更新（起動時のURLが設定されている場合）
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    UpdateURLLabel();
                }
                
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
                // HTTP/HTTPS URLの場合のみ短縮URL展開を実行
                if (_currentUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                    _currentUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
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
        /// AutoCloseとAutoOpenの初期化（Browser Chooser 2互換）
        /// </summary>
        private void InitializeAutoCloseAndAutoOpen()
        {
            Logger.LogInfo("MainForm.InitializeAutoCloseAndAutoOpen", "Start");
            
            try
            {
                // AutoCloseの初期化
                if (chkAutoClose != null)
                {
                    chkAutoClose.Checked = true; // デフォルトでチェック
                    chkAutoClose.Text = "Auto Close";
                }
                
                // AutoOpenの初期化
                if (chkAutoOpen != null)
                {
                    // chkAutoOpenを常に表示（デフォルトブラウザの有無に関係なく）
                    chkAutoOpen.Visible = true;
                    
                    if (_defaultBrowser != null && (_settings?.DefaultDelay ?? 0) > 0)
                    {
                        // デフォルトブラウザがある場合
                        chkAutoOpen.Checked = true; // デフォルトでチェック
                        _currentDelay = _settings?.DefaultDelay ?? 5;
                        
                        // タイマーを開始
                        if (tmrDelay != null)
                        {
                            tmrDelay.Enabled = true;
                        }
                        
                        UpdateAutoOpenText();
                    }
                    else
                    {
                        // デフォルトブラウザがない場合でも表示
                        chkAutoOpen.Checked = false;
                        chkAutoOpen.Text = "Auto Open (No default browser set)";
                        if (tmrDelay != null)
                        {
                            tmrDelay.Enabled = false;
                        }
                    }
                }
                
                Logger.LogInfo("MainForm.InitializeAutoCloseAndAutoOpen", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.InitializeAutoCloseAndAutoOpen", "初期化エラー", ex.Message, ex.StackTrace ?? "");
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
            ShowIcon = false;
            TopMost = true;
            CancelButton = btnCancel;
            KeyPreview = true;
            
            // フォントの設定（現代的で日本語・英語両対応）
            Font = new Font("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
            
            // サイズの設定（動的サイズ変更対応）
            MinimumSize = new Size(600, 300);  // 最小サイズを設定
            ClientSize = new Size(600, 220);   // 初期サイズ
            
            // サイズ変更イベントの設定
            Resize += MainForm_Resize;
            
            // 透明化設定の適用（Windows11スタイルも含む）
            ApplyTransparencySettings();
            
            // 透明化が無効な場合の背景色設定
            if (_settings?.EnableTransparency != true)
            {
                // 設定値をそのまま反映（Settings.BackgroundColorValue は常に不透明で正規化済み）
                BackColor = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                Logger.LogInfo("MainForm.ConfigureForm", $"Applied BackColor: {BackColor}");
                StyleXP(); // 透明化が無効の場合のスタイル設定
                // 子コントロールは既定色に保ち、フォーム背景色の影響を受けにくくする
                ApplyDefaultBackColorToChildControls();
            }
            
            Logger.LogInfo("MainForm.ConfigureForm", "End");
        }

        /// <summary>
        /// 透明化設定を適用
        /// </summary>
        private void ApplyTransparencySettings()
        {
            try
            {
                // フォームの基本スタイル設定（最小化・最大化ボタンを含む）
                StartPosition = FormStartPosition.CenterScreen;
                
                if (_settings?.EnableTransparency == true)
                {
                    // 透明化が有効な場合
                    this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                    this.TransparencyKey = Color.FromArgb(_settings.TransparencyColor);
                    this.Opacity = _settings.Opacity;
                    
                    // 背景色を透明化色に設定
                    this.BackColor = Color.FromArgb(_settings.TransparencyColor);
                    
                    // 角を丸くする設定
                    if (_settings?.RoundedCornersRadius > 0)
                    {
                        ApplyRoundedCorners(_settings.RoundedCornersRadius);
                    }
                    
                    Logger.LogInfo("MainForm.ApplyTransparencySettings", 
                        $"透明化設定を適用: Opacity={_settings?.Opacity}, TransparencyKey={_settings?.TransparencyColor}, HideTitleBar={_settings?.HideTitleBar}, RoundedCornersRadius={_settings?.RoundedCornersRadius}");
                }
                else
                {
                    // 透明化が無効な場合
                    this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);
                    this.TransparencyKey = Color.Empty;
                    this.Opacity = 1.0;
                    // 念のため不透明化してから適用
                    var bg = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                    if (bg.A != 255) bg = Color.FromArgb(255, bg.R, bg.G, bg.B);
                    this.BackColor = bg;
                    
                    // リージョンをクリア（角を丸くする設定を無効化）
                    this.Region = null;
                    
                    // 透明化解除後の描画問題を解決するため、フォームを強制再描画
                    this.Refresh();
                    
                    Logger.LogInfo("MainForm.ApplyTransparencySettings", "透明化を無効にしました");
                }
                
                // タイトルバー非表示設定の処理（透明化設定の後に適用）
                if (_settings?.HideTitleBar == true)
                {
                    FormBorderStyle = FormBorderStyle.None;
                }
                else
                {
                    // タイトルバーが表示される場合は最小化・最大化ボタンを確実に有効化
                    FormBorderStyle = FormBorderStyle.Sizable;
                    MaximizeBox = true;
                    MinimizeBox = true;
                    SizeGripStyle = SizeGripStyle.Show;
                }
                
                // Windows 11スタイルの適用
                ApplyWindows11Style();
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.ApplyTransparencySettings", "透明化設定エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// Windows 11スタイルを適用
        /// </summary>
        private void ApplyWindows11Style()
        {
            try
            {
                // Windows 11風の最新スタイルを適用
                if (Environment.OSVersion.Version.Major >= 10)
                {
                    // Windows 11の最新スタイルを強制適用
                    try
                    {
                        if (Environment.OSVersion.Version.Build >= 22000) // Windows 11
                        {
                            // Windows 11の最新スタイルを適用
                            this.WindowState = FormWindowState.Normal;
                            this.FormBorderStyle = FormBorderStyle.Sizable;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("MainForm.ApplyWindows11Style", "Windows 11スタイル適用エラー", ex.Message);
                    }
                }
                
                Logger.LogInfo("MainForm.ApplyWindows11Style", "Windows 11スタイル設定を適用しました");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.ApplyWindows11Style", "Windows 11スタイル適用エラー", ex.Message);
            }
        }

        /// <summary>
        /// 角を丸くする設定を適用
        /// </summary>
        /// <param name="radius">角の半径</param>
        private void ApplyRoundedCorners(int radius)
        {
            try
            {
                // Windows APIを使用して角を丸くする
                var region = CreateRoundedRectangleRegion(0, 0, this.Width, this.Height, radius);
                this.Region = region;
                
                Logger.LogInfo("MainForm.ApplyRoundedCorners", $"角を丸くする設定を適用しました（半径: {radius}）");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.ApplyRoundedCorners", "角を丸くする設定エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// 角が丸い矩形のリージョンを作成
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="radius">角の半径</param>
        /// <returns>角が丸い矩形のリージョン</returns>
        private Region CreateRoundedRectangleRegion(int x, int y, int width, int height, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(x, y, radius * 2, radius * 2, 180, 90); // 左上
            path.AddArc(width - radius * 2, y, radius * 2, radius * 2, 270, 90); // 右上
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90); // 右下
            path.AddArc(x, height - radius * 2, radius * 2, radius * 2, 90, 90); // 左下
            path.CloseFigure();
            return new Region(path);
        }

        /// <summary>
        /// フォームサイズ変更時の処理
        /// </summary>
        private void MainForm_Resize(object? sender, EventArgs e)
        {
            try
            {
                // リサイズ中の描画を一時的に無効化（パフォーマンス向上）
                this.SuspendLayout();
                
                // 透明化が有効で角を丸くする設定がある場合、リージョンを更新
                if (_settings?.EnableTransparency == true && _settings.RoundedCornersRadius > 0)
                {
                    ApplyRoundedCorners(_settings.RoundedCornersRadius);
                }
                
                // ブラウザボタンの再配置
                RecalculateButtonLayout();
                
                // 互換性UIコントロールの位置調整
                AdjustCompatibilityUILayout();
                
                // リサイズ処理を再開
                this.ResumeLayout(false);
                
                // フォームの再描画を強制（透明化解除後の描画問題を解決）
                this.Refresh();
                
                Logger.LogTrace("MainForm.MainForm_Resize", "フォームサイズ変更完了", ClientSize.Width, ClientSize.Height);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.MainForm_Resize", "サイズ変更エラー", ex.Message, ex.StackTrace ?? "");
                this.ResumeLayout(false);
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
            
            Logger.LogInfo("MainForm.RecalculateButtonLayout", $"Layout settings - Width: {buttonWidth}, Height: {buttonHeight}, GapWidth: {gapWidth}, GapHeight: {gapHeight}");
            
            // フォーム幅に基づいて列数を計算（btnInfoのスペースを確保）
            var availableWidth = ClientSize.Width - 120; // 左右マージン（右端ボタンとbtnInfo用のスペース確保）
            var columnsPerRow = Math.Max(1, availableWidth / (buttonWidth + gapWidth));
            
            var buttonIndex = 0;
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Tag is Browser)
                {
                    var row = buttonIndex / columnsPerRow;
                    var col = buttonIndex % columnsPerRow;
                    var x = 50 + (col * (buttonWidth + gapWidth)); // btnInfoの右側から開始
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
                    
                    var nameLabel = Controls.OfType<Label>().FirstOrDefault(l => l.Name == $"lblName_{buttonIndex}");
                    if (nameLabel != null)
                    {
                        var labelWidth = TextRenderer.MeasureText(nameLabel.Text, nameLabel.Font).Width;
                        nameLabel.Location = new Point(
                            x + (buttonWidth / 2) - (labelWidth / 2),
                            y + buttonHeight - 20
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
            // 標準の背景描画に任せる（BackColorをそのまま反映させる）
            base.OnPaint(e);
        }

        /// <summary>
        /// XPスタイルの設定
        /// Aero効果が無効の場合のフォームスタイルを設定します
        /// </summary>
        private void StyleXP()
        {
            // 透明化が無効な場合はサイズ変更可能にする
            if (_settings?.EnableTransparency != true)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = true;
                MinimizeBox = true;
                SizeGripStyle = SizeGripStyle.Show;
            }
            
            if (chkAutoClose != null)
                chkAutoClose.BackColor = Color.Transparent;
            if (chkAutoOpen != null)
                chkAutoOpen.BackColor = Color.Transparent;
        }

        /// <summary>
        /// 子コントロールの背景色を既定色に戻す（フォームのBackColor変更の影響を抑制）
        /// </summary>
        private void ApplyDefaultBackColorToChildControls()
        {
            foreach (Control control in Controls)
            {
                switch (control)
                {
                    case Button:
                    case Label:
                    case TextBox:
                    case CheckBox:
                    case ListView:
                    case Panel:
                        control.BackColor = Color.Transparent;
                        break;
                }
            }
        }



        /// <summary>
        /// ブラウザボタンの作成
        /// </summary>
        private void CreateBrowserButtons()
        {
            Logger.LogInfo("MainForm.CreateBrowserButtons", "Start", $"ブラウザ数: {_browsers?.Count ?? 0}");
            Logger.LogInfo("MainForm.CreateBrowserButtons", "既存のボタン数", Controls.OfType<Button>().Where(b => b.Tag is Browser).Count().ToString());
            
            var buttonWidth = _settings?.IconWidth ?? 90;
            var buttonHeight = _settings?.IconHeight ?? 100;
            var gapWidth = _settings?.IconGapWidth ?? 0;
            var gapHeight = _settings?.IconGapHeight ?? 0;
            
            Logger.LogInfo("MainForm.CreateBrowserButtons", $"Icon settings - Width: {buttonWidth}, Height: {buttonHeight}, GapWidth: {gapWidth}, GapHeight: {gapHeight}, Scale: {_settings?.IconScale ?? 1.0}");
            
            if (_browsers == null) return;
            
            // 既存のブラウザボタンとオーバーレイラベルを削除
            var buttonsToRemove = Controls.OfType<Button>().Where(b => b.Tag is Browser).ToList();
            var labelsToRemove = Controls.OfType<Label>().Where(l => l.Name.StartsWith("lblOverlay_") || l.Name.StartsWith("lblName_")).ToList();
            
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
            
            var visibleBrowsers = _browsers.Where(b => b.Visible && b.IsActive).ToList();
            
            for (int i = 0; i < visibleBrowsers.Count; i++)
            {
                var browser = visibleBrowsers[i];
                
                var button = new FFButton
                {
                    Name = $"btnBrowser_{i}",
                    Text = " ", // スペース1文字を設定してアイコンが表示されるようにする
                    Size = new Size(buttonWidth, buttonHeight),
                    Tag = browser,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ImageAlign = ContentAlignment.MiddleCenter,
                    UseVisualStyleBackColor = false,
                    Font = new Font("Segoe UI", 6.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ShowFocusBox = _settings?.ShowFocus ?? true,
                    ShowVisualFocus = _settings?.ShowVisualFocus ?? false,
                    TrapArrowKeys = true
                };
                
                // ブラウザアイコンの設定
                try
                {
                    Logger.LogInfo("MainForm.CreateBrowserButtons", "アイコン取得開始", browser.Name, browser.Target);
                    
                    var browserIcon = ImageUtilities.GetImage(browser, true);
                    if (browserIcon != null)
                    {
                        // アイコンのサイズを調整（ボタンサイズとスケールに合わせる）
                        var baseIconSize = Math.Min(buttonWidth - 10, buttonHeight - 30); // マージンを確保
                        var iconScale = _settings?.IconScale ?? 1.0;
                        var iconSize = (int)(baseIconSize * iconScale);
                        var resizedIcon = new Bitmap(browserIcon, new Size(iconSize, iconSize));
                        
                        Logger.LogInfo("MainForm.CreateBrowserButtons", $"Icon size calculation - Base: {baseIconSize}, Scale: {iconScale}, Final: {iconSize}");
                        
                        button.Image = resizedIcon;
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.TextImageRelation = TextImageRelation.Overlay;
                        
                        Logger.LogInfo("MainForm.CreateBrowserButtons", "アイコン設定成功", browser.Name, iconSize, browser.Target);
                    }
                    else
                    {
                        Logger.LogWarning("MainForm.CreateBrowserButtons", "アイコンが取得できませんでした", browser.Name, browser.Target);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("MainForm.CreateBrowserButtons", "アイコン設定エラー", browser.Name, ex.Message, browser.Target);
                }
                
                // イベントハンドラーの設定
                button.Click += BrowserButton_Click;
                
                // FFButtonの矢印キーイベントハンドラーを設定
                if (button is FFButton ffButton)
                {
                    ffButton.ArrowKeyUp += FFButton_ArrowKeyUp;
                }
                
                // ブラウザボタンにツールチップを設定
                if (_toolTip != null)
                {
                    var tooltipText = $"{browser.Name}\nパス: {browser.Target}";
                    if (!string.IsNullOrEmpty(browser.Arguments))
                    {
                        tooltipText += $"\n引数: {browser.Arguments}";
                    }
                    _toolTip.SetToolTip(button, tooltipText);
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
        /// ブラウザ名のオーバーレイラベルを作成
        /// </summary>
        private void CreateOverlayLabel(Button button, Browser browser, int index)
        {
            // ブラウザ名のオーバーレイラベルを作成
            var nameLabel = new Label
            {
                Name = $"lblName_{index}",
                AutoSize = true,
                BackColor = Color.Transparent, // 背景を透過
                ForeColor = Color.Black, // 文字色を黒に変更
                Font = new Font("Segoe UI", 8.0f, FontStyle.Bold, GraphicsUnit.Point, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = browser.Name
            };
            
            // 位置の計算（ボタンの中央下部に配置）
            var labelWidth = TextRenderer.MeasureText(nameLabel.Text, nameLabel.Font).Width;
            nameLabel.Location = new Point(
                button.Location.X + (button.Width / 2) - (labelWidth / 2),
                button.Location.Y + button.Height - 20
            );
            
            Controls.Add(nameLabel);
            nameLabel.BringToFront();
            
            // ホットキーとデフォルトブラウザのオーバーレイラベルを作成
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
                var labelWidth2 = TextRenderer.MeasureText(overlayLabel.Text, overlayLabel.Font).Width;
                overlayLabel.Location = new Point(
                    button.Location.X + (button.Width / 2) - (labelWidth2 / 2),
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
                // 設定を再読み込み（既存の設定を保持）
                var newSettings = Settings.Load(Application.StartupPath);
                if (newSettings != null)
                {
                    _settings = newSettings;
                    Settings.Current = _settings;
                    _browsers = _settings?.Browsers ?? new List<Browser>();
                    
                    Logger.LogInfo("MainForm.RefreshForm", "設定再読み込み完了", _browsers?.Count ?? 0);
                }
                else
                {
                    Logger.LogWarning("MainForm.RefreshForm", "設定の再読み込みに失敗しました");
                }
                
                // デフォルトブラウザの再検索
                _defaultBrowser = _browsers?.FirstOrDefault(b => b.IsDefault);
                
                // 既存のブラウザボタンとオーバーレイラベルのみを削除
                var buttonsToRemove = Controls.OfType<Button>().Where(b => b.Tag is Browser).ToList();
                var labelsToRemove = Controls.OfType<Label>().Where(l => l.Name.StartsWith("lblOverlay_") || l.Name.StartsWith("lblName_")).ToList();
                
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
                
                // リフレッシュ中の描画を一時的に無効化（パフォーマンス向上）
                this.SuspendLayout();
                
                // フォームを再設定（Windows11スタイルも含む）
                ConfigureForm();
                
                // ツールチップの初期化
                InitializeToolTips();
                
                // ブラウザボタンを再作成
                CreateBrowserButtons();
                
                // カウントダウンラベルを再作成
                CreateCountdownLabel();
                
                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // Browser Chooser 2互換のUI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();
                
                // AutoCloseとAutoOpenの再初期化
                InitializeAutoCloseAndAutoOpen();
                
                // 初期テキストの設定
                UpdateAutoOpenTextWithSpaceKey();
                
                // URL短縮解除の設定
                SetupURLUnshortening();
                
                // リフレッシュ処理を再開
                this.ResumeLayout(false);
                
                // フォームを強制再描画（透明化解除後の描画問題を解決）
                this.Refresh();
                
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
                    var autoClose = chkAutoClose?.Checked ?? true;
                    
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
            // UIスレッドで実行する必要があるため、InvokeRequiredをチェック
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateURL), url);
                return;
            }

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
            
            // URL表示ラベルを更新
            UpdateURLLabel();
        }
        
        /// <summary>
        /// URL表示ラベルを更新
        /// </summary>
        private void UpdateURLLabel()
        {
            try
            {
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    // URLが長すぎる場合は省略表示
                    var displayUrl = _currentUrl.Length > 100 ? _currentUrl.Substring(0, 97) + "..." : _currentUrl;
                    
                    // URLラベルの更新
                    if (_urlLabel != null)
                    {
                        _urlLabel.Text = displayUrl;
                        _urlLabel.Visible = _settings?.ShowURL == true;
                    }
                    
                    // URLテキストボックスの更新
                    if (_urlTextBox != null)
                    {
                        _urlTextBox.Text = _currentUrl;
                        _urlTextBox.Visible = _settings?.ShowURL == true;
                    }
                }
                else
                {
                    // URLラベルの更新
                    if (_urlLabel != null)
                    {
                        _urlLabel.Text = "";
                        _urlLabel.Visible = false;
                    }
                    
                    // URLテキストボックスの更新
                    if (_urlTextBox != null)
                    {
                        _urlTextBox.Text = "";
                        _urlTextBox.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.UpdateURLLabel", "URL表示ラベル更新エラー", ex.Message);
            }
        }

        /// <summary>
        /// Browser Chooser 2互換のUI要素の位置調整
        /// </summary>
        private void AdjustCompatibilityUILayout()
        {
            Logger.LogInfo("MainForm.AdjustCompatibilityUILayout", "Start");
            
            try
            {
                // デザイナーファイルで定義されたUI要素の位置を調整
                if (btnInfo != null)
                {
                    btnInfo.Location = new Point(2, 1);
                    btnInfo.Size = new Size(24, 24);
                }

                if (btnOptions != null)
                {
                    btnOptions.Location = new Point(ClientSize.Width - 35, 15);
                    btnOptions.ImageAlign = ContentAlignment.MiddleCenter;
                    btnOptions.Size = new Size(28, 28);
                    btnOptions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }
                
                if (btnCopyToClipboard != null)
                {
                    btnCopyToClipboard.Location = new Point(ClientSize.Width - 35, 50);
                    btnCopyToClipboard.ImageAlign = ContentAlignment.MiddleCenter;
                    btnCopyToClipboard.Size = new Size(28, 28);
                    btnCopyToClipboard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }

                if (btnCopyToClipboardAndClose != null)
                {
                    btnCopyToClipboardAndClose.Location = new Point(ClientSize.Width - 35, 85);
                    btnCopyToClipboardAndClose.ImageAlign = ContentAlignment.MiddleCenter;
                    btnCopyToClipboardAndClose.Size = new Size(28, 28);
                    btnCopyToClipboardAndClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }
                if (btnCancel != null)
                {
                    btnCancel.Location = new Point(ClientSize.Width - 35, 113); // btnCopyToClipboardAndCloseの下
                    btnCancel.ImageAlign = ContentAlignment.MiddleCenter;
                    btnCancel.Size = new Size(28, 28);
                    btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }

                if (chkAutoClose != null)
                {
                    chkAutoClose.Location = new Point(20, ClientSize.Height - 80);
                    chkAutoClose.Size = new Size(400, 24);
                }

                if (chkAutoOpen != null)
                {
                    chkAutoOpen.Location = new Point(20, ClientSize.Height - 50);
                    chkAutoOpen.Size = new Size(450, 22);
                }

                // 遅延タイマーの設定
                if (tmrDelay != null)
                {
                    tmrDelay.Interval = 1000;
                    tmrDelay.Tick += TmrDelay_Tick;
                }

                // コンテキストメニュー
                CreateContextMenu();

                Logger.LogInfo("MainForm.AdjustCompatibilityUILayout", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.AdjustCompatibilityUILayout", "UI位置調整エラー", ex.Message, ex.StackTrace ?? "");
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
                if (btnInfo != null)
                {
                    btnInfo.Image = Properties.Resources.Icon122;
                }
                
                // オプションボタンのアイコン読み込み
                if (btnOptions != null)
                {
                    btnOptions.Image = Properties.Resources.Icon128;
                }
                
                // コピーボタンのアイコン読み込み
                if (btnCopyToClipboard != null)
                {
                    var pasteIcon = Properties.Resources.PasteIcon;
                    btnCopyToClipboard.Image = ImageUtilities.ResizeImage(pasteIcon, 28, 28);
                }
                
                // コピー＆クローズボタンのアイコン読み込み
                if (btnCopyToClipboardAndClose != null)
                {
                    var pasteAndCloseIcon = Properties.Resources.PasteAndCloseIcon;
                    btnCopyToClipboardAndClose.Image = ImageUtilities.ResizeImage(pasteAndCloseIcon, 28, 28);
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
        /// ツールチップの初期化
        /// </summary>
        private void InitializeToolTips()
        {
            _toolTip = new ToolTip();
            _toolTip.IsBalloon = false;
            _toolTip.ToolTipTitle = "Browser Chooser";
            _toolTip.ShowAlways = true;
            _toolTip.AutoPopDelay = 5000;
            _toolTip.InitialDelay = 1000;
            _toolTip.ReshowDelay = 500;
        }

        /// <summary>
        /// ボタンにツールチップを設定
        /// </summary>
        private void SetupButtonToolTips()
        {
            if (_toolTip == null) return;

            // 基本ボタンのツールチップ設定
            if (btnInfo != null)
                _toolTip.SetToolTip(btnInfo, "アプリケーション情報を表示します");

            if (btnOptions != null)
                _toolTip.SetToolTip(btnOptions, "設定画面を開きます");

            if (btnCancel != null)
                _toolTip.SetToolTip(btnCancel, "アプリケーションを終了します");

            if (btnCopyToClipboard != null)
                _toolTip.SetToolTip(btnCopyToClipboard, "URLをクリップボードにコピーします");

            if (btnCopyToClipboardAndClose != null)
                _toolTip.SetToolTip(btnCopyToClipboardAndClose, "URLをクリップボードにコピーしてアプリケーションを終了します");

            if (chkAutoClose != null)
                _toolTip.SetToolTip(chkAutoClose, "ブラウザ起動後にアプリケーションを自動で閉じます");

            if (chkAutoOpen != null)
                _toolTip.SetToolTip(chkAutoOpen, "デフォルトブラウザで自動的にURLを開きます");
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
            if (chkAutoOpen != null && _defaultBrowser != null)
            {
                var pauseText = tmrDelay?.Enabled == false ? "un" : "";
                var browserName = _defaultBrowser.Name ?? "default browser";
                chkAutoOpen.Text = $"Open {browserName} in {_currentDelay} seconds. [Space: {pauseText}pause timer]";
            }
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
            if (e.KeyCode == Keys.Space && tmrDelay != null)
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
                        BrowserUtilities.LaunchBrowser(browser, _currentUrl, chkAutoClose?.Checked ?? true);
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
            Logger.LogInfo("MainForm.ChkAutoClose_CheckedChanged", $"自動閉じる: {chkAutoClose?.Checked}");
            // 設定に反映する処理を追加
        }

        /// <summary>
        /// 自動開くチェックボックスの変更イベント（Browser Chooser 2互換）
        /// </summary>
        private void ChkAutoOpen_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.ChkAutoOpen_CheckedChanged", $"自動開く: {chkAutoOpen?.Checked}");
            
            if (tmrDelay != null)
            {
                tmrDelay.Enabled = chkAutoOpen?.Checked ?? false;
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
                var text = $"Open {_defaultBrowser?.Name} in {_currentDelay} seconds. [Space: {(tmrDelay?.Enabled == false ? "un" : "")}pause timer]";
                
                if (chkAutoOpen != null)
                {
                    chkAutoOpen.Text = text;
                    chkAutoOpen.Invalidate();
                }
            }
            else
            {
                tmrDelay!.Enabled = false;
                
                var text = $"Automatically opening {_defaultBrowser?.Name}.";
                if (chkAutoOpen != null)
                {
                    chkAutoOpen.Text = text;
                    chkAutoOpen.Invalidate();
                }

                if (_defaultBrowser != null)
                {
                    BrowserUtilities.LaunchBrowser(_defaultBrowser, _currentUrl, chkAutoClose?.Checked ?? true);
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
                else if (button.Tag is Browser browser)
                {
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
            if (e.KeyCode == Keys.Space && tmrDelay != null && _defaultBrowser != null)
            {
                if (tmrDelay.Enabled)
                {
                    _isPaused = true;
                    tmrDelay.Stop();
                }
                else
                {
                    _isPaused = false;
                    tmrDelay.Start();
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
            if (chkAutoOpen != null && _defaultBrowser != null)
            {
                var pauseStatus = tmrDelay?.Enabled == false ? "un" : "";
                chkAutoOpen.Text = $"Open {_defaultBrowser.Name} in {_currentDelay} seconds. [Space: {pauseStatus}pause timer]";
            }
        }

        /// <summary>
        /// 基本コントロールを再作成
        /// </summary>
        private void CreateBasicControls()
        {
            Logger.LogInfo("MainForm.CreateBasicControls", "Start");
            
            try
            {
                // 設定ボタン
                btnOptions = Controls.Find("btnOptions", true).FirstOrDefault() as Button;
                if (btnOptions == null)
                {
                    btnOptions = new Button
                    {
                        Name = "btnOptions",
                        Text = "Settings",
                        Size = new Size(80, 25),
                        Location = new Point(ClientSize.Width - 90, 5),
                        Anchor = AnchorStyles.Top | AnchorStyles.Right
                    };
                    btnOptions.Click += (s, e) => OpenOptionsForm();
                    Controls.Add(btnOptions);
                }
                
                // キャンセルボタン
                btnCancel = Controls.Find("btnCancel", true).FirstOrDefault() as Button;
                if (btnCancel == null)
                {
                    btnCancel = new Button
                    {
                        Name = "btnCancel",
                        Text = "Cancel",
                        Size = new Size(80, 25),
                        Location = new Point(ClientSize.Width - 90, ClientSize.Height - 30),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    btnCancel.Click += (s, e) => Close();
                    Controls.Add(btnCancel);
                }
                
                // Auto Close チェックボックス
                chkAutoClose = Controls.Find("chkAutoClose", true).FirstOrDefault() as CheckBox;
                if (chkAutoClose == null)
                {
                    chkAutoClose = new CheckBox
                    {
                        Name = "chkAutoClose",
                        Text = "Auto Close",
                        Size = new Size(100, 20),
                        Location = new Point(20, ClientSize.Height - 50),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                        Checked = true
                    };
                    Controls.Add(chkAutoClose);
                }
                
                // Auto Open チェックボックス
                chkAutoOpen = Controls.Find("chkAutoOpen", true).FirstOrDefault() as CheckBox;
                if (chkAutoOpen == null)
                {
                    chkAutoOpen = new CheckBox
                    {
                        Name = "chkAutoOpen",
                        Text = "Auto Open",
                        Size = new Size(100, 20),
                        Location = new Point(130, ClientSize.Height - 50),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                        Checked = false, // デフォルトは無効
                        Visible = true   // 常に表示
                    };
                    Controls.Add(chkAutoOpen);
                }
                
                // URL表示ラベル
                _urlLabel = Controls.Find("urlLabel", true).FirstOrDefault() as Label;
                if (_urlLabel == null)
                {
                    _urlLabel = new Label
                    {
                        Name = "urlLabel",
                        Text = "",
                        Size = new Size(ClientSize.Width - 40, 20),
                        Location = new Point(20, 10),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        AutoSize = false,
                        TextAlign = ContentAlignment.MiddleLeft,
                        BackColor = Color.Transparent,
                        ForeColor = Color.Black,
                        Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                        Visible = true
                    };
                    Controls.Add(_urlLabel);
                }

                // URL表示テキストボックス
                _urlTextBox = Controls.Find("txtURL", true).FirstOrDefault() as TextBox;
                if (_urlTextBox == null)
                {
                    _urlTextBox = new TextBox
                    {
                        Name = "txtURL",
                        Text = "",
                        Size = new Size(ClientSize.Width - 40, 20),
                        Location = new Point(20, 35),
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        ReadOnly = true,
                        BackColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                        Visible = false
                    };
                    Controls.Add(_urlTextBox);
                }
                
                // カウントダウンタイマー
                if (tmrDelay == null)
                {
                    tmrDelay = new System.Windows.Forms.Timer
                    {
                        Interval = 1000
                    };
                    tmrDelay.Tick += TmrDelay_Tick;
                }
                
                Logger.LogInfo("MainForm.CreateBasicControls", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.CreateBasicControls", "基本コントロール作成エラー", ex.Message);
            }
        }
    }
}
