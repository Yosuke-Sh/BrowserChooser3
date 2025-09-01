using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.SystemServices;
using BrowserChooser3.Classes.Services.UI;
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
        private string _initialUrl = string.Empty;
        
        /// <summary>
        /// URL表示用テキストボックス
        /// </summary>
        private TextBox? _urlDisplayTextBox;
        
        private System.Windows.Forms.Timer? _countdownTimer;
        private int _currentDelay;
        private Browser? _defaultBrowser;
        private Label? _countdownLabel;
        private bool _isPaused = false;

        private string _currentText = string.Empty;

        private ContextMenuStrip? _cmOptions;
        
        // ツールチップ
        private ToolTip? _toolTip;

        // システムトレイ関連
        private NotifyIcon? _notifyIcon;
        private bool _isInTray = false;

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
            Logger.LogDebug("MainForm.InitializeApplication", "Start");
            
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
                
                // StartUp設定の適用
                ApplyStartupSettings();
                
                // ツールチップの初期化
                InitializeToolTips();
                
                // ブラウザボタンの作成
                CreateBrowserButtons();
                
                // URL表示用テキストボックスの作成
                CreateURLDisplayTextBox();
                
                // カウントダウンラベルの作成
                CreateCountdownLabel();
                
                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // UI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();
                
                // キーボードイベントの設定
                KeyPreview = true;
                KeyDown += MainForm_KeyDown;
                
                // フォームリサイズイベントの設定
                Resize += MainForm_Resize;
                
                // フォームLoadイベントの設定
                Load += MainForm_Load;
                

                
                // URL短縮解除の設定
                SetupURLUnshortening();
                
                // 初期化完了後にURL表示ラベルを更新（起動時のURLが設定されている場合）
                if (!string.IsNullOrEmpty(_currentUrl))
                {
                    UpdateURLLabel();
                }
                
                // フォームの初期化完了を通知
                Logger.LogDebug("MainForm.InitializeApplication", "フォーム初期化完了");
                
                Logger.LogDebug("MainForm.InitializeApplication", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.InitializeApplication", "初期化エラー", ex.Message, ex.StackTrace ?? "");
                MessageBoxService.ShowErrorStatic($"アプリケーションの初期化に失敗しました: {ex.Message}", "エラー");
            }
        }

        /// <summary>
        /// StartUp設定を適用します
        /// </summary>
        private void ApplyStartupSettings()
        {
            Logger.LogDebug("MainForm.ApplyStartupSettings", "Start");
            
            try
            {
                if (_settings == null) return;

                // 起動遅延の処理
                if (_settings.StartupDelay > 0)
                {
                    Logger.LogDebug("MainForm.ApplyStartupSettings", $"起動遅延を適用: {_settings.StartupDelay}秒");
                    var startupTimer = new System.Windows.Forms.Timer
                    {
                        Interval = _settings.StartupDelay * 1000,
                        Enabled = true
                    };
                    startupTimer.Tick += (sender, e) =>
                    {
                        startupTimer.Stop();
                        startupTimer.Dispose();
                        ShowForm();
                    };
                    
                    // フォームを非表示にする
                    this.Hide();
                    return;
                }



                // システムトレイで起動の処理
                if (_settings.StartInTray)
                {
                    Logger.LogDebug("MainForm.ApplyStartupSettings", "システムトレイで起動を適用");
                    InitializeSystemTray();
                    MinimizeToTray();
                }

                Logger.LogDebug("MainForm.ApplyStartupSettings", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.ApplyStartupSettings", "StartUp設定適用エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// フォームを表示します
        /// </summary>
        private void ShowForm()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowForm));
                return;
            }

            if (_isInTray)
            {
                ShowFromTray();
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
        }

        /// <summary>
        /// システムトレイに最小化します
        /// </summary>
        private void MinimizeToTray()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(MinimizeToTray));
                return;
            }

            if (_notifyIcon != null)
            {
                _isInTray = true;
                _notifyIcon.Visible = true;
                Hide();
                ShowInTaskbar = false;
                
                Logger.LogDebug("MainForm.MinimizeToTray", "システムトレイに最小化");
            }
        }

        /// <summary>
        /// システムトレイから復元します
        /// </summary>
        private void ShowFromTray()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowFromTray));
                return;
            }

            if (_notifyIcon != null)
            {
                _isInTray = false;
                _notifyIcon.Visible = false;
                Show();
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;
                Activate();
                
                Logger.LogDebug("MainForm.ShowFromTray", "システムトレイから復元");
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
        /// フォームの設定
        /// </summary>
        private void ConfigureForm()
        {
            Logger.LogDebug("MainForm.ConfigureForm", "Start");
            
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
            
            // 背景グラデーション設定
            if (_settings?.EnableBackgroundGradient == true)
            {
                this.Paint += MainForm_Paint;
            }
            ClientSize = new Size(600, 300);   // 初期サイズ
            
            // サイズ変更イベントの設定
            Resize += MainForm_Resize;
            
            // 透明化設定の適用（Windows11スタイルも含む）
            ApplyTransparencySettings();
            
            // 透明化が無効な場合の背景色設定
            if (_settings?.EnableTransparency != true)
            {
                Logger.LogDebug("MainForm.ConfigureForm", "背景色設定開始", $"現在の背景色: {BackColor}");
                
                // 変更前のブラウザボタン数を記録
                var browserButtonsBefore = Controls.OfType<Button>().Where(b => b.Tag is Browser).ToList();
                Logger.LogDebug("MainForm.ConfigureForm", "変更前のブラウザボタン数", browserButtonsBefore.Count);
                
                // 設定値をそのまま反映（Settings.BackgroundColorValue は常に不透明で正規化済み）
                BackColor = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                Logger.LogDebug("MainForm.ConfigureForm", $"Applied BackColor: {BackColor}");
                
                // 変更後のブラウザボタン数を記録
                var browserButtonsAfter = Controls.OfType<Button>().Where(b => b.Tag is Browser).ToList();
                Logger.LogDebug("MainForm.ConfigureForm", "変更後のブラウザボタン数", browserButtonsAfter.Count);
                
                StyleXP(); // 透明化が無効の場合のスタイル設定
                // 子コントロールは既定色に保ち、フォーム背景色の影響を受けにくくする
                ApplyDefaultBackColorToChildControls();
            }
            
            Logger.LogDebug("MainForm.ConfigureForm", "End");
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
                    this.TransparencyKey = Color.Magenta; // 固定の透明色を使用
                    this.Opacity = _settings.Opacity;
                    
                    // 背景色は通常の背景色を維持（TransparencyKeyで指定した色のみ透明化）
                    var bg = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                    this.BackColor = bg;
                    
                    Logger.LogTrace("MainForm.ApplyTransparencySettings", "透明化設定を適用", 
                        $"EnableTransparency: {_settings?.EnableTransparency}, " +
                        $"Opacity: {_settings?.Opacity}, " +
                        $"BackColor: {this.BackColor}, " +
                        $"TransparencyKey: {this.TransparencyKey}");
                    
                    // 角を丸くする設定
                    if (_settings?.RoundedCornersRadius > 0)
                    {
                        ApplyRoundedCorners(_settings.RoundedCornersRadius);
                    }
                    
                    Logger.LogDebug("MainForm.ApplyTransparencySettings", 
                        $"透明化設定を適用: Opacity={_settings?.Opacity}, TransparencyKey=Magenta, HideTitleBar={_settings?.HideTitleBar}, RoundedCornersRadius={_settings?.RoundedCornersRadius}");
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
                    
                    Logger.LogTrace("MainForm.ApplyTransparencySettings", "透明化を無効に設定", 
                        $"EnableTransparency: {_settings?.EnableTransparency}, " +
                        $"Opacity: {this.Opacity}, " +
                        $"BackColor: {this.BackColor}");
                    
                    // リージョンをクリア（角を丸くする設定を無効化）
                    this.Region = null;
                    
                    // 透明化解除後の描画問題を解決するため、フォームを強制再描画
                    this.Refresh();
                    
                    Logger.LogDebug("MainForm.ApplyTransparencySettings", "透明化を無効にしました");
                }
                
                // Windows 11スタイルの適用
                ApplyWindows11Style();

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
                
                Logger.LogDebug("MainForm.ApplyWindows11Style", "Windows 11スタイル設定を適用しました");
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
                
                Logger.LogDebug("MainForm.ApplyRoundedCorners", $"角を丸くする設定を適用しました（半径: {radius}）");
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
        /// 背景グラデーション描画イベント
        /// </summary>
        private void MainForm_Paint(object? sender, PaintEventArgs e)
        {
            if (_settings?.EnableBackgroundGradient == true)
            {
                try
                {
                    var rect = new Rectangle(0, 0, this.Width, this.Height);
                    var darkerColor = Color.FromArgb(255, 
                        Math.Max(0, _settings.BackgroundColorValue.R - 50),
                        Math.Max(0, _settings.BackgroundColorValue.G - 50),
                        Math.Max(0, _settings.BackgroundColorValue.B - 50));
                    using var brush = new LinearGradientBrush(rect, _settings.BackgroundColorValue, darkerColor, LinearGradientMode.Vertical);
                    
                    e.Graphics.FillRectangle(brush, rect);
                }
                catch (Exception ex)
                {
                    Logger.LogError("MainForm.MainForm_Paint", "背景グラデーション描画エラー", ex.Message);
                }
            }
        }

        /// <summary>
        /// フォームLoadイベントの処理
        /// </summary>
        private void MainForm_Load(object? sender, EventArgs e)
        {
            try
            {
                Logger.LogDebug("MainForm.MainForm_Load", "フォームLoad開始");
                
                // 初期URLが設定されている場合は更新
                if (!string.IsNullOrEmpty(_initialUrl))
                {
                    UpdateURL(_initialUrl);
                    Logger.LogDebug("MainForm.MainForm_Load", "初期URL更新完了", _initialUrl);
                }
                
                Logger.LogDebug("MainForm.MainForm_Load", "フォームLoad完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.MainForm_Load", "フォームLoad処理エラー", ex.Message);
            }
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
                
                // URL表示テキストボックスの位置とサイズを調整
                if (_urlDisplayTextBox != null)
                {
                    _urlDisplayTextBox.Location = new Point(20, ClientSize.Height - 110);
                    _urlDisplayTextBox.Size = new Size(ClientSize.Width - 60, 20);
                }
                
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
            
            Logger.LogDebug("MainForm.RecalculateButtonLayout", $"Layout settings - Width: {buttonWidth}, Height: {buttonHeight}, GapWidth: {gapWidth}, GapHeight: {gapHeight}");
            
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

        }

        /// <summary>
        /// 子コントロールの背景色を既定色に戻す（フォームのBackColor変更の影響を抑制）
        /// </summary>
        private void ApplyDefaultBackColorToChildControls()
        {
            Logger.LogDebug("MainForm.ApplyDefaultBackColorToChildControls", "子コントロール背景色設定開始");
            
            foreach (Control control in Controls)
            {
                try
                {
                    switch (control)
                    {
                        case Button:
                        case Label:
                        case TextBox:
                        case CheckBox:
                        case ListView:
                        case Panel:
                            // 透明色を設定（エラーが発生した場合はスキップ）
                            try
                            {
                                control.BackColor = Color.Transparent;
                                Logger.LogDebug("MainForm.ApplyDefaultBackColorToChildControls", $"コントロール背景色を透明に設定", control.Name);
                            }
                            catch (InvalidOperationException)
                            {
                                Logger.LogDebug("MainForm.ApplyDefaultBackColorToChildControls", $"コントロールは透明色をサポートしません", control.Name, control.GetType().Name);
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning("MainForm.ApplyDefaultBackColorToChildControls", $"コントロール背景色設定エラー", control.Name, ex.Message);
                }
            }
            
            Logger.LogDebug("MainForm.ApplyDefaultBackColorToChildControls", "子コントロール背景色設定完了");
        }



        /// <summary>
        /// ブラウザボタンの作成
        /// </summary>
        private void CreateBrowserButtons()
        {
            Logger.LogDebug("MainForm.CreateBrowserButtons", "Start", $"ブラウザ数: {_browsers?.Count ?? 0}");
            Logger.LogDebug("MainForm.CreateBrowserButtons", "既存のボタン数", Controls.OfType<Button>().Where(b => b.Tag is Browser).Count().ToString());
            
            var buttonWidth = _settings?.IconWidth ?? 90;
            var buttonHeight = _settings?.IconHeight ?? 100;
            var gapWidth = _settings?.IconGapWidth ?? 0;
            var gapHeight = _settings?.IconGapHeight ?? 0;
            
            Logger.LogDebug("MainForm.CreateBrowserButtons", $"Icon settings - Width: {buttonWidth}, Height: {buttonHeight}, GapWidth: {gapWidth}, GapHeight: {gapHeight}, Scale: {_settings?.IconScale ?? 1.0}");
            
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
                    Logger.LogDebug("MainForm.CreateBrowserButtons", "アイコン取得開始", browser.Name, browser.Target);
                    
                    var browserIcon = ImageUtilities.GetImage(browser, true);
                    if (browserIcon != null)
                    {
                        // アイコンのサイズを調整（ボタンサイズとスケールに合わせる）
                        var baseIconSize = Math.Min(buttonWidth - 10, buttonHeight - 30); // マージンを確保
                        var iconScale = _settings?.IconScale ?? 1.0;
                        var iconSize = (int)(baseIconSize * iconScale);
                        var resizedIcon = new Bitmap(browserIcon, new Size(iconSize, iconSize));
                        
                        Logger.LogDebug("MainForm.CreateBrowserButtons", $"Icon size calculation - Base: {baseIconSize}, Scale: {iconScale}, Final: {iconSize}");
                        
                        button.Image = resizedIcon;
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                        button.TextImageRelation = TextImageRelation.Overlay;
                        
                        Logger.LogDebug("MainForm.CreateBrowserButtons", "アイコン設定成功", browser.Name, iconSize, browser.Target);
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
            
            Logger.LogDebug("MainForm.CreateBrowserButtons", "End");
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
            Logger.LogDebug("MainForm.OpenOptionsForm", "Start");
            
            try
            {
                var optionsForm = new OptionsForm(_settings!);
                var result = optionsForm.ShowDialog(this);
                
                if (result == DialogResult.OK)
                {
                    // 設定が変更された場合、フォームを再構築
                    RefreshForm();
                }
                
                Logger.LogDebug("MainForm.OpenOptionsForm", "End", result);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.OpenOptionsForm", "オプション画面表示エラー", ex.Message, ex.StackTrace ?? "");
                MessageBoxService.ShowErrorStatic($"オプション画面の表示に失敗しました: {ex.Message}", "エラー");
            }
        }

        /// <summary>
        /// フォームを再構築
        /// </summary>
        private void RefreshForm()
        {
            Logger.LogDebug("MainForm.RefreshForm", "Start");
            
            try
            {
                // 設定を再読み込み（既存の設定を保持）
                var newSettings = Settings.Load(Application.StartupPath);
                if (newSettings != null)
                {
                    _settings = newSettings;
                    Settings.Current = _settings;
                    _browsers = _settings?.Browsers ?? new List<Browser>();
                    
                    Logger.LogDebug("MainForm.RefreshForm", "設定再読み込み完了", _browsers?.Count ?? 0);
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
                Logger.LogDebug("MainForm.RefreshForm", "ConfigureForm呼び出し前");
                ConfigureForm();
                Logger.LogDebug("MainForm.RefreshForm", "ConfigureForm呼び出し完了");
                
                // ツールチップの初期化
                InitializeToolTips();
                
                // ブラウザボタンを再作成
                Logger.LogDebug("MainForm.RefreshForm", "CreateBrowserButtons呼び出し前");
                CreateBrowserButtons();
                Logger.LogDebug("MainForm.RefreshForm", "CreateBrowserButtons呼び出し完了");
                
                // カウントダウンラベルを再作成
                CreateCountdownLabel();
                
                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // UI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();
                


                
                // URL短縮解除の設定
                SetupURLUnshortening();
                
                // リフレッシュ処理を再開
                this.ResumeLayout(false);
                
                // フォームを強制再描画（透明化解除後の描画問題を解決）
                this.Refresh();
                
                Logger.LogDebug("MainForm.RefreshForm", "End");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.RefreshForm", "フォーム再構築エラー", ex.Message, ex.StackTrace ?? "");
            }
        }

        /// <summary>
        /// ブラウザボタンのクリックイベント
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
                    MessageBoxService.ShowErrorStatic($"ブラウザの起動に失敗しました: {ex.Message}", "エラー");
                }
            }
        }

        /// <summary>
        /// ブラウザを起動
        /// </summary>
        private void LaunchBrowser(Browser browser, string url)
        {
            Logger.LogDebug("MainForm.LaunchBrowser", "Start", browser.Name, url);
            
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = browser.Target,
                Arguments = string.IsNullOrEmpty(url) ? browser.Arguments : $"{browser.Arguments} \"{url}\"",
                UseShellExecute = true
            };
            
            System.Diagnostics.Process.Start(startInfo);
            
            Logger.LogDebug("MainForm.LaunchBrowser", "End", browser.Name);
        }

        /// <summary>
        /// 初期URLを設定（Loadイベントで使用）
        /// </summary>
        public void SetInitialURL(string url)
        {
            _initialUrl = url;
            Logger.LogDebug("MainForm.SetInitialURL", "初期URL設定", url);
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

            Logger.LogDebug("MainForm.UpdateURL", "URL更新", url);
            _currentUrl = url;
            UpdateURLLabel();
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
            Logger.LogDebug("MainForm.OnURLUpdated", "URL更新完了", url);
            
            // URL表示ラベルを更新
            UpdateURLLabel();
        }
        
        /// <summary>
        /// URL表示テキストボックスを更新
        /// </summary>
        private void UpdateURLLabel()
        {
            try
            {
                Logger.LogDebug("MainForm.UpdateURLLabel", "URL表示更新開始", $"URL: {_currentUrl}, ShowURL: {_settings?.ShowURL}");
                
                if (_urlDisplayTextBox != null)
                {
                    if (!string.IsNullOrEmpty(_currentUrl))
                    {
                        // URLが長すぎる場合は省略表示
                        var displayUrl = _currentUrl.Length > 100 ? _currentUrl.Substring(0, 97) + "..." : _currentUrl;
                        _urlDisplayTextBox.Text = displayUrl;
                        _urlDisplayTextBox.Visible = _settings?.ShowURL == true;
                        Logger.LogDebug("MainForm.UpdateURLLabel", "URL表示設定完了", $"DisplayURL: {displayUrl}, Visible: {_urlDisplayTextBox.Visible}");
                    }
                    else
                    {
                        _urlDisplayTextBox.Text = "";
                        _urlDisplayTextBox.Visible = false;
                        Logger.LogDebug("MainForm.UpdateURLLabel", "URL表示を非表示に設定");
                    }
                }
                else
                {
                    Logger.LogWarning("MainForm.UpdateURLLabel", "URL表示テキストボックスがnullです");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.UpdateURLLabel", "URL表示テキストボックス更新エラー", ex.Message);
            }
        }

        /// <summary>
        /// UI要素の位置調整
        /// </summary>
        private void AdjustCompatibilityUILayout()
        {
            Logger.LogDebug("MainForm.AdjustCompatibilityUILayout", "Start");
            
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
                    btnOptions.Location = new Point(ClientSize.Width - 35, 10);
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
                    btnCancel.Location = new Point(ClientSize.Width - 35, 120); // btnCopyToClipboardAndCloseの下
                    btnCancel.ImageAlign = ContentAlignment.MiddleCenter;
                    btnCancel.Size = new Size(28, 28);
                    btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                }

                if (chkAutoClose != null)
                {
                    chkAutoClose.Location = new Point(20, ClientSize.Height - 80);
                    chkAutoClose.Size = new Size(400, 24);
                    // 設定から自動閉じるの状態を読み込み
                    chkAutoClose.Checked = !(_settings?.AllowStayOpen ?? false);
                }



                // 遅延タイマーの設定
                if (tmrDelay != null)
                {
                    tmrDelay.Interval = 1000;
                    tmrDelay.Tick += TmrDelay_Tick;
                }

                // コンテキストメニュー
                CreateContextMenu();

                Logger.LogDebug("MainForm.AdjustCompatibilityUILayout", "End");
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
                
                Logger.LogDebug("MainForm.LoadIcons", "アイコン読み込み完了");
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



        }

        /// <summary>
        /// URL表示用テキストボックスの作成
        /// </summary>
        private void CreateURLDisplayTextBox()
        {
            _urlDisplayTextBox = new TextBox
            {
                Name = "txtURLDisplay",
                ReadOnly = true,
                Location = new Point(20, ClientSize.Height - 110),
                Size = new Size(ClientSize.Width - 60, 25),
                Font = new Font("Segoe UI", 7.0f, FontStyle.Regular, GraphicsUnit.Point, 0),
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Left,
                Anchor=AnchorStyles.Left | AnchorStyles.Bottom,
                Visible = true
            };
            
            Controls.Add(_urlDisplayTextBox);
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
            
            Logger.LogDebug("MainForm.StartCountdown", "カウントダウン開始", _currentDelay);
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
                MessageBoxService.ShowInfoStatic("URLをクリップボードにコピーしました", "情報");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.btnCopyToClipboard_Click", "クリップボードコピーエラー", ex.Message);
                MessageBoxService.ShowErrorStatic($"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー");
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
                MessageBoxService.ShowErrorStatic($"クリップボードへのコピーに失敗しました: {ex.Message}", "エラー");
            }
        }
        


        /// <summary>
        /// 自動クローズチェックボックスの変更イベント
        /// </summary>
        private void chkAutoClose_CheckedChanged(object? sender, EventArgs e)
        {
            Logger.LogInfo("MainForm.chkAutoClose_CheckedChanged", "自動クローズ設定変更", chkAutoClose.Checked);
            // 設定に反映（AllowStayOpenは逆の値）
            if (_settings != null)
            {
                _settings.AllowStayOpen = !chkAutoClose.Checked;
                Logger.LogDebug("MainForm.chkAutoClose_CheckedChanged", "AllowStayOpen設定を更新", _settings.AllowStayOpen);
                
                // 設定を保存
                try
                {
                    _settings.DoSave();
                    Logger.LogDebug("MainForm.chkAutoClose_CheckedChanged", "設定を保存しました");
                }
                catch (Exception ex)
                {
                    Logger.LogError("MainForm.chkAutoClose_CheckedChanged", "設定の保存に失敗", ex.Message);
                }
            }
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
        /// キーボードイベントの処理
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
        /// 矢印キーによるフォーカス移動
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
        /// ループ減算
        /// </summary>
        private int MinusLoop(int start, int max)
        {
            start = start - 1;
            if (start == 0) return max;
            return start;
        }

        /// <summary>
        /// ループ加算
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
        /// 自動閉じるチェックボックスの変更イベント（重複を削除）
        /// </summary>
        private void ChkAutoClose_CheckedChanged(object? sender, EventArgs e)
        {
            // このメソッドは重複しているため、chkAutoClose_CheckedChangedを使用
            chkAutoClose_CheckedChanged(sender, e);
        }



        /// <summary>
        /// 遅延タイマーのティックイベント
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
                // カウントダウン表示のみ
            }
            else
            {
                tmrDelay!.Enabled = false;

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
        /// フォームを閉じる際の処理
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                // システムトレイアイコンのクリーンアップ
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }

                // タイマーのクリーンアップ
                if (_countdownTimer != null)
                {
                    _countdownTimer.Stop();
                    _countdownTimer.Dispose();
                    _countdownTimer = null;
                }

                // ツールチップのクリーンアップ
                if (_toolTip != null)
                {
                    _toolTip.Dispose();
                    _toolTip = null;
                }

                // コンテキストメニューのクリーンアップ
                if (_cmOptions != null)
                {
                    _cmOptions.Dispose();
                    _cmOptions = null;
                }

                base.OnFormClosing(e);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.OnFormClosing", "フォーム終了処理エラー", ex.Message);
            }
        }

        /// <summary>
        /// システムトレイを初期化します
        /// </summary>
        private void InitializeSystemTray()
        {
            try
            {
                if (_notifyIcon != null) return;

                _notifyIcon = new NotifyIcon
                {
                    Icon = Icon.FromHandle(Properties.Resources.BrowserChooser3.GetHicon()),
                    Text = "Browser Chooser 3",
                    Visible = false
                };

                // コンテキストメニューの作成
                var contextMenu = new ContextMenuStrip();
                
                var showItem = new ToolStripMenuItem("表示(&S)");
                showItem.Click += (sender, e) => ShowFromTray();
                contextMenu.Items.Add(showItem);
                
                contextMenu.Items.Add(new ToolStripSeparator());
                
                var exitItem = new ToolStripMenuItem("終了(&X)");
                exitItem.Click += (sender, e) => Application.Exit();
                contextMenu.Items.Add(exitItem);

                _notifyIcon.ContextMenuStrip = contextMenu;
                _notifyIcon.DoubleClick += (sender, e) => ShowFromTray();

                Logger.LogDebug("MainForm.InitializeSystemTray", "システムトレイ初期化完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.InitializeSystemTray", "システムトレイ初期化エラー", ex.Message);
            }
        }


    }
}
