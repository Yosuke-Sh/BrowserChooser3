using BrowserChooser3.Classes;
using BrowserChooser3.Classes.Models;
using BrowserChooser3.Classes.Services.BrowserServices;
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

        // コマンドライン引数(-d/-b/--silent)による起動時オーバーライド
        private int? _startupDelayOverride;
        private Guid? _startupBrowserGuid;
        private bool _startupSilentMode;
        
        /// <summary>
        /// URL表示用テキストボックス
        /// </summary>
        private TextBox? _urlDisplayTextBox;
        
        private System.Windows.Forms.Timer? _countdownTimer;
        private int _currentDelay;
        private Browser? _defaultBrowser;
        private Label? _countdownLabel;
        private bool _isPaused = false;

        /// <summary>
        /// 起動メッセージ（Settings.StartupMessage）表示用ラベル
        /// </summary>
        private Label? _startupMessageLabel;

        /// <summary>
        /// 設定の遅延保存用タイマー（連続操作をまとめるためのデバウンス）
        /// </summary>
        private System.Windows.Forms.Timer? _deferredSaveTimer;

        /// <summary>
        /// 遅延保存までの待ち時間（ミリ秒）
        /// </summary>
        private const int DeferredSaveDelayMs = 800;

        private string _currentText = string.Empty;

        private ContextMenuStrip? _cmOptions;
        
        // ツールチップ
        private ToolTip? _toolTip;

        // システムトレイ関連
        private NotifyIcon? _notifyIcon;
        private bool _isInTray = false;

        // RefreshForm等で繰り返し生成されるフォントは、GDIハンドルの積み上がりを避けるため共有する
        private static readonly Font FormFont = new("Segoe UI", 9.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font ButtonFont = new("Segoe UI", 6.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font OverlayBoldFont = new("Segoe UI", 8.0f, FontStyle.Bold, GraphicsUnit.Point, 0);
        private static readonly Font UrlDisplayFont = new("Segoe UI", 7.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font CountdownLabelFont = new("Segoe UI", 8.0f, FontStyle.Regular, GraphicsUnit.Point, 0);
        private static readonly Font StartupMessageFont = new("Segoe UI", 9.0f, FontStyle.Bold, GraphicsUnit.Point, 0);

        /// <summary>
        /// MainFormクラスの新しいインスタンスを初期化します
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
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
                // 設定を読み込み（パスは使用されず、常にユーザーディレクトリから読み込み）
                _settings = Settings.Load("");
                Settings.Current = _settings;
                _browsers = _settings?.Browsers ?? new List<Browser>();

                // 設定ファイルが破損していたためSafeModeで起動した場合はユーザーに通知する。
                // SafeMode中はDoSave/IntSaveが保存を拒否するため、通知しないと
                // 「変更が保存されない」ことに気づかれないまま操作が続いてしまう。
                if (_settings?.SafeMode == true)
                {
                    MessageBoxService.ShowWarningStatic(
                        "設定ファイルが読み込めなかったため、初期設定でセーフモード起動しました。\n" +
                        "破損したファイルはBrowserChooser3Config.corrupt-*.xmlとして退避されています。\n" +
                        "このまま操作しても設定は保存されません。",
                        "セーフモード");
                }

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

                // 起動メッセージラベルの作成
                CreateStartupMessageLabel();

                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // UI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();
                
                // キーボードイベントの設定
                KeyPreview = true;
                KeyDown += MainForm_KeyDown;

                // フォームLoadイベントの設定
                Load += MainForm_Load;

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
                // ここで例外を握りつぶすと、初期化が半分だけ終わった操作可能なウィンドウが
                // そのまま表示されてしまう。呼び出し元（Program.Main）まで例外を伝播させ、
                // アプリケーションを起動失敗として扱わせる（fail-fast）。
                Logger.LogError("MainForm.InitializeApplication", "初期化エラー", ex.Message, ex.StackTrace ?? "");
                MessageBoxService.ShowErrorStatic($"アプリケーションの初期化に失敗しました: {ex.Message}", "エラー");
                throw;
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
                ApplyConfiguredStartupPosition();
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

                // トレイに隠れている間に勝手にブラウザが起動しないよう、カウントダウンを止める
                _countdownTimer?.Stop();
                _isPaused = true;

                Logger.LogDebug("MainForm.MinimizeToTray", "システムトレイに最小化");
            }
        }

        /// <summary>
        /// ブラウザ起動後の終了要否を処理します。
        /// 常駐モードが有効な場合はプロセスを終了せずトレイに格納します。
        /// </summary>
        /// <param name="shouldTerminate">呼び出し元がプロセス終了を想定していたか</param>
        private void HandlePostLaunchTermination(bool shouldTerminate)
        {
            if (!shouldTerminate)
            {
                return;
            }

            RequestClose();
        }

        /// <summary>
        /// アプリケーションを閉じます。
        /// 常駐モードが有効な場合は終了せずシステムトレイに格納します。
        /// タスクトレイメニューの「終了」からのみ、常駐モード中でもプロセスを完全に終了できます。
        /// </summary>
        private void RequestClose()
        {
            if (_settings?.AlwaysResidentInTray ?? false)
            {
                Logger.LogDebug("MainForm.RequestClose", "AlwaysResidentInTrayが有効のためトレイに格納します");
                InitializeSystemTray();
                MinimizeToTray();
                return;
            }

            Application.Exit();
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
                ApplyConfiguredStartupPosition();
                Activate();

                // トレイ格納中に止めたカウントダウンをリセットして再開する
                _isPaused = false;
                if (_countdownTimer != null && _defaultBrowser != null && !string.IsNullOrEmpty(_currentUrl))
                {
                    _currentDelay = _settings?.DefaultDelay ?? 5;
                    UpdateCountdownDisplay();
                    _countdownTimer.Start();
                }

                Logger.LogDebug("MainForm.ShowFromTray", "システムトレイから復元");
            }
        }

        /// <summary>
        /// 他プロセスから引き渡されたURLを受け取り、既存ウィンドウに反映します（単一インスタンス化用）
        /// </summary>
        /// <param name="url">受信したURL</param>
        public void ReceiveExternalURL(string url)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(ReceiveExternalURL), url);
                return;
            }

            Logger.LogInfo("MainForm.ReceiveExternalURL", "他プロセスからURLを受信", url);

            if (_isInTray)
            {
                ShowFromTray();
            }
            else
            {
                Show();
                WindowState = FormWindowState.Normal;
                ApplyConfiguredStartupPosition();
                Activate();
            }

            if (!string.IsNullOrEmpty(url))
            {
                UpdateURL(url);
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
            Font = FormFont;
            
            // サイズの設定（動的サイズ変更対応）
            MinimumSize = new Size(Settings.MinimumWindowWidth, Settings.MinimumWindowHeight);
            
            // 背景グラデーション・グリッド描画設定（RefreshFormから複数回呼ばれても購読が積み上がらないよう、
            // 一旦解除してから必要な場合のみ再購読する）
            this.Paint -= MainForm_Paint;
            if (_settings?.EnableBackgroundGradient == true || _settings?.ShowGrid == true)
            {
                this.Paint += MainForm_Paint;
            }
            // サイズ変更イベントの設定（同上の理由で解除してから再購読する）
            Resize -= MainForm_Resize;
            Resize += MainForm_Resize;

            // 透明化設定の適用（Windows11スタイルも含む）
            ApplyTransparencySettings();
            
            // 透明化が無効な場合の背景色設定
            if (_settings?.EnableTransparency != true)
            {
                Logger.LogDebug("MainForm.ConfigureForm", "背景色設定開始", $"現在の背景色: {BackColor}");

                // 設定値をそのまま反映（Settings.BackgroundColorValue は常に不透明で正規化済み）
                BackColor = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                Logger.LogDebug("MainForm.ConfigureForm", $"Applied BackColor: {BackColor}");

                StyleXP(); // 透明化が無効の場合のスタイル設定
                // 子コントロールは既定色に保ち、フォーム背景色の影響を受けにくくする
                ApplyDefaultBackColorToChildControls();
            }

            ApplyConfiguredWindowSize();

            Logger.LogDebug("MainForm.ConfigureForm", "End", ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// 設定されたウィンドウサイズ（Settings.Width / Height）をClientSizeへ適用します。
        /// 旧形式の値（グリッドの列数・行数）はEffectiveWindowWidth/Heightが既定値へ
        /// フォールバックさせます。
        ///
        /// FormBorderStyleを変更するとClientSizeがdesignerの値（434x126）へ再計算されるため、
        /// ConfigureFormの最後とMainForm_Loadの両方から呼び、スタイル確定後の値を確実に反映させます。
        /// </summary>
        private void ApplyConfiguredWindowSize()
        {
            var desired = new Size(
                _settings?.EffectiveWindowWidth ?? Settings.DefaultWindowWidth,
                _settings?.EffectiveWindowHeight ?? Settings.DefaultWindowHeight);

            if (ClientSize != desired)
            {
                ClientSize = desired;
                Logger.LogDebug("MainForm.ApplyConfiguredWindowSize", "ウィンドウサイズを適用",
                    desired.Width, desired.Height);
            }
        }

        /// <summary>
        /// 設定された起動位置（Settings.StartupPosition）に従ってウィンドウ位置を決定します。
        /// ClientSize確定後に呼ぶこと（サイズが変わると中央位置がずれるため）。
        /// </summary>
        private void ApplyConfiguredStartupPosition()
        {
            var mode = _settings?.StartupPosition ?? Settings.StartupPositionMode.CursorScreenCenter;
            var screen = mode == Settings.StartupPositionMode.CursorScreenCenter
                ? Screen.FromPoint(Cursor.Position)
                : Screen.PrimaryScreen;
            if (screen == null)
            {
                return;
            }

            var area = screen.WorkingArea;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(
                area.Left + Math.Max(0, (area.Width - Width) / 2),
                area.Top + Math.Max(0, (area.Height - Height) / 2));

            Logger.LogDebug("MainForm.ApplyConfiguredStartupPosition", "起動位置を適用",
                mode.ToString(), Location.X, Location.Y);
        }

        /// <summary>
        /// 透明化設定を適用
        /// </summary>
        private void ApplyTransparencySettings()
        {
            try
            {

                if (_settings?.EnableTransparency == true)
                {
                    // 透明化が有効な場合。
                    // TransparencyKeyは使わない。従来はMagentaを固定の透明色にしていたが、
                    // ブラウザアイコン内にマゼンタの画素があるとそこだけ穴が開き、
                    // クリックが背後のウィンドウへ抜けてしまっていた。
                    // ウィンドウ全体の半透明化はOpacityだけで実現できる。
                    this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                    this.TransparencyKey = Color.Empty;
                    this.Opacity = _settings.Opacity;

                    var bg = _settings?.BackgroundColorValue ?? Color.FromArgb(185, 209, 234);
                    this.BackColor = bg;

                    Logger.LogTrace("MainForm.ApplyTransparencySettings", "透明化設定を適用",
                        $"EnableTransparency: {_settings?.EnableTransparency}, " +
                        $"Opacity: {_settings?.Opacity}, " +
                        $"BackColor: {this.BackColor}");
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

                    // 透明化解除後の描画問題を解決するため、フォームを強制再描画
                    this.Refresh();
                    
                    Logger.LogDebug("MainForm.ApplyTransparencySettings", "透明化を無効にしました");
                }
                
                // 角丸は透明化とは独立した設定として扱う。
                // 従来は EnableTransparency が有効なときしか適用されず、
                // 角丸だけを使いたい場合に設定が効かなかった。
                if (_settings?.RoundedCornersRadius > 0)
                {
                    ApplyRoundedCorners(_settings.RoundedCornersRadius);
                }
                else
                {
                    this.Region = null;
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
        /// 背景グラデーション・配置グリッドの描画イベント
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

            DrawLayoutGrid(e.Graphics);
        }

        /// <summary>
        /// ブラウザボタンの配置グリッドに沿った罫線を描画します。
        /// Settings.ShowGrid（既定OFF）が有効な場合のみ描画し、色と線幅は
        /// Settings.GridColor / Settings.GridLineWidth に従います。
        /// </summary>
        /// <param name="graphics">描画先</param>
        private void DrawLayoutGrid(Graphics graphics)
        {
            if (_settings?.ShowGrid != true) return;

            try
            {
                var cellWidth = _settings.IconWidth + _settings.IconGapWidth;
                var cellHeight = _settings.IconHeight + _settings.IconGapHeight;
                if (cellWidth <= 0 || cellHeight <= 0) return;

                var columns = CalculateColumnsPerRow();
                var visibleCount = _browsers?.Count(b => b.Visible && b.IsActive) ?? 0;
                // ボタンが1つも無い場合でも配置枠が分かるよう、最低1行は描画する
                var rows = Math.Max(1, (int)Math.Ceiling(visibleCount / (double)columns));

                var gridWidth = columns * cellWidth;
                var gridHeight = rows * cellHeight;
                var lineWidth = Math.Max(1, _settings.GridLineWidth);

                using var pen = new Pen(Color.FromArgb(_settings.GridColor), lineWidth);

                var originY = EffectiveGridOriginY;

                // 縦線（列の境界）
                for (var col = 0; col <= columns; col++)
                {
                    var x = GridOriginX + (col * cellWidth);
                    graphics.DrawLine(pen, x, originY, x, originY + gridHeight);
                }

                // 横線（行の境界）
                for (var row = 0; row <= rows; row++)
                {
                    var y = originY + (row * cellHeight);
                    graphics.DrawLine(pen, GridOriginX, y, GridOriginX + gridWidth, y);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.DrawLayoutGrid", "グリッド描画エラー", ex.Message);
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

                // 初期ウィンドウサイズ（Settings.Width/Height）を適用する。
                // ConfigureForm内で設定してもFormBorderStyleの変更でClientSizeが
                // designerの値へ再計算されてしまうため、スタイルが確定するLoadで適用する。
                ApplyConfiguredWindowSize();
                ApplyConfiguredStartupPosition();

                // Windows 11のダークモード設定に自動追従（DWM未対応環境では内部でtry/catchされ実害なし）
                var isDarkMode = GeneralUtilities.IsSystemDarkModeEnabled();
                GeneralUtilities.ApplyDarkMode(this, isDarkMode);
                // Mica効果は独自の透明化設定と競合するため、透明化が無効な場合のみ適用する
                if (isDarkMode && _settings?.EnableTransparency != true)
                {
                    GeneralUtilities.ApplyMicaEffect(this);
                }

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
                
                // 角丸設定がある場合はリージョンを更新（透明化の有無とは独立）
                if (_settings?.RoundedCornersRadius > 0)
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

                // 再描画が必要な領域のみ無効化する（Refresh()は全面再描画のためリサイズのたびに重い）
                this.Invalidate();

                Logger.LogTrace("MainForm.MainForm_Resize", "フォームサイズ変更完了", ClientSize.Width, ClientSize.Height);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.MainForm_Resize", "サイズ変更エラー", ex.Message, ex.StackTrace ?? "");
                this.ResumeLayout(false);
            }
        }

        /// <summary>
        /// マージン込みの列数計算で使う左右マージン幅（右端ボタンとbtnInfo用のスペース確保）
        /// </summary>
        private const int ColumnLayoutMargin = 120;

        /// <summary>
        /// ブラウザボタン配置の原点X（btnInfoの右側から開始）。グリッド描画もこの原点に合わせる。
        /// </summary>
        private const int GridOriginX = 50;

        /// <summary>
        /// ブラウザボタン配置の既定の原点Y。グリッド描画もこの原点に合わせる。
        /// </summary>
        private const int GridOriginY = 30;

        /// <summary>
        /// 起動メッセージを表示する場合に、ボタン配置を下へずらす量。
        /// ホットキーのオーバーレイラベルはボタン上端の15px上に置かれるため、
        /// この余白が無いとメッセージと重なってしまう。
        /// </summary>
        private const int StartupMessageReservedHeight = 22;

        /// <summary>
        /// 起動メッセージの有無を考慮した、実際のボタン配置の原点Y。
        /// </summary>
        private int EffectiveGridOriginY =>
            GridOriginY + (_settings?.IsStartupMessageVisible == true ? StartupMessageReservedHeight : 0);

        /// <summary>
        /// フォーム幅とアイコン設定から1行あたりのボタン列数を計算します。
        /// ボタン配置・オーバーレイラベル配置・矢印キー移動の3箇所で個別に
        /// 計算されておりマージン定数（120 vs 80）が食い違っていたため、
        /// このメソッドに集約して常に同じ列数を使うようにする。
        /// </summary>
        /// <returns>1行あたりの列数（最低1）</returns>
        private int CalculateColumnsPerRow()
        {
            if (_settings == null) return 1;

            var buttonWidth = _settings.IconWidth;
            var gapWidth = _settings.IconGapWidth;
            var availableWidth = ClientSize.Width - ColumnLayoutMargin;
            return Math.Max(1, availableWidth / (buttonWidth + gapWidth));
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
            var columnsPerRow = CalculateColumnsPerRow();

            // ラベルをフォーム全体から都度検索すると総当たりでO(n^2)になるため、事前に名前で索引化する
            var overlayLabelsByName = Controls.OfType<Label>()
                .Where(l => l.Name.StartsWith("lblOverlay_"))
                .ToDictionary(l => l.Name);
            var nameLabelsByName = Controls.OfType<Label>()
                .Where(l => l.Name.StartsWith("lblName_"))
                .ToDictionary(l => l.Name);

            var buttonIndex = 0;
            foreach (Control control in Controls)
            {
                if (control is Button button && button.Tag is Browser)
                {
                    var row = buttonIndex / columnsPerRow;
                    var col = buttonIndex % columnsPerRow;
                    var x = GridOriginX + (col * (buttonWidth + gapWidth)); // btnInfoの右側から開始
                    var y = EffectiveGridOriginY + (row * (buttonHeight + gapHeight));

                    button.Location = new Point(x, y);

                    // 対応するオーバーレイラベルの位置も調整
                    if (overlayLabelsByName.TryGetValue($"lblOverlay_{buttonIndex}", out var overlayLabel))
                    {
                        var labelWidth = TextRenderer.MeasureText(overlayLabel.Text, overlayLabel.Font).Width;
                        overlayLabel.Location = new Point(
                            x + (buttonWidth / 2) - (labelWidth / 2),
                            y - 15
                        );
                    }

                    if (nameLabelsByName.TryGetValue($"lblName_{buttonIndex}", out var nameLabel))
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
            // 透明化が無効な場合はサイズ変更可能にする（ただしタイトルバー非表示設定は尊重する）
            if (_settings?.EnableTransparency != true && _settings?.HideTitleBar != true)
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
                            catch (NotSupportedException)
                            {
                                // TextBox等、透明な背景色をサポートしないコントロールでは想定内の失敗
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
                    Font = ButtonFont,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ShowFocusBox = _settings?.ShowFocus ?? true,
                    ShowVisualFocus = _settings?.ShowVisualFocus ?? false,
                    TrapArrowKeys = true,
                    FocusBoxColor = Color.FromArgb(_settings?.FocusBoxColor ?? Color.Blue.ToArgb()),
                    FocusBoxLineWidth = _settings?.FocusBoxLineWidth ?? 2,
                    FocusBoxWidth = _settings?.FocusBoxWidth ?? 2
                };
                
                // ブラウザアイコンの設定
                try
                {
                    Logger.LogDebug("MainForm.CreateBrowserButtons", "アイコン取得開始", browser.Name, browser.Target);
                    
                    // アイコンのサイズを調整（ボタンサイズとスケールに合わせる）
                    var baseIconSize = Math.Min(buttonWidth - 10, buttonHeight - 30); // マージンを確保
                    var iconScale = _settings?.IconScale ?? 1.0;
                    var iconSize = (int)(baseIconSize * iconScale);
                    var resizedIcon = ImageUtilities.GetResizedImage(browser, true, iconSize);
                    if (resizedIcon != null)
                    {
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

                // 右クリックでシークレット/プロファイル起動のメニューを出す
                // （左クリックの既定動作は変更しない）
                button.MouseUp += BrowserButton_MouseUp;
                
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

                // アクセシブルレンダリング設定が有効な場合、スクリーンリーダー向けの情報を付与する
                if (_settings?.IsAccessibleRenderingActive ?? false)
                {
                    button.AccessibleName = browser.Name;
                    button.AccessibleDescription = $"{browser.Name}でURLを開く";
                    button.AccessibleRole = AccessibleRole.PushButton;
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
                Font = OverlayBoldFont,
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
            
            // ホットキーまたはデフォルトブラウザがある場合のみオーバーレイラベルを作成。
            // 以前は数字ホットキーのみ表示対象としており、AddEditBrowserFormで設定可能な
            // 英字ホットキーがオーバーレイに表示されなかった。
            if (browser.Hotkey != '\0' || !string.IsNullOrEmpty(defaultIndicator))
            {
                var overlayLabel = new Label
                {
                    Name = $"lblOverlay_{index}",
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Font = OverlayBoldFont,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // テキストの設定
                if (browser.Hotkey != '\0')
                {
                    overlayLabel.Text = char.ToUpperInvariant(browser.Hotkey).ToString() + defaultIndicator;
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
        /// オプション画面を開く
        /// </summary>
        private void OpenOptionsForm()
        {
            Logger.LogDebug("MainForm.OpenOptionsForm", "Start");
            
            try
            {
                using var optionsForm = new OptionsForm(_settings!);
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
                // OptionsFormは呼び出し元から渡された_settingsインスタンスをそのまま書き換えて保存するため、
                // ここでXMLを再パースしなくてもメモリ上の_settingsは既に最新の内容になっている
                _browsers = _settings?.Browsers ?? new List<Browser>();
                Logger.LogDebug("MainForm.RefreshForm", "設定はメモリ上の値をそのまま使用", _browsers?.Count ?? 0);

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

                // 起動メッセージラベルを再作成（Optionsでの変更を反映）
                CreateStartupMessageLabel();

                // ボタンのツールチップ設定
                SetupButtonToolTips();
                
                // UI要素の位置調整
                AdjustCompatibilityUILayout();
                
                // アイコンの読み込み
                LoadIcons();

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
                    var shouldTerminate = BrowserUtilities.LaunchBrowser(browser, _currentUrl, autoClose);
                    HandlePostLaunchTermination(shouldTerminate);
                }
                catch (Exception ex)
                {
                    Logger.LogError("MainForm.BrowserButton_Click", "ブラウザ起動エラー", browser.Name, ex.Message);
                    MessageBoxService.ShowErrorStatic($"ブラウザの起動に失敗しました: {ex.Message}", "エラー");
                }
            }
        }

        /// <summary>
        /// ブラウザボタンの右クリック処理。
        /// シークレットウィンドウ・プロファイル指定での起動メニューを表示します。
        /// 左クリック（BrowserButton_Click）の既定動作は変更しません。
        /// </summary>
        private void BrowserButton_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (sender is not Button button || button.Tag is not Browser browser) return;

            // プロファイル・シークレットに対応していないブラウザではメニューを出さない
            if (!BrowserLaunchProfiles.SupportsProfilesOrPrivateMode(browser))
            {
                Logger.LogDebug("MainForm.BrowserButton_MouseUp",
                    "プロファイル/シークレット非対応のためメニューを表示しません", browser.Name);
                return;
            }

            // 前回のメニューが残らないよう、都度生成して閉じたら破棄する
            var menu = new ContextMenuStrip();

            var privateItem = new ToolStripMenuItem("シークレットウィンドウで開く(&P)");
            privateItem.Click += (s, _) => LaunchBrowserWithOptions(browser, forcePrivateMode: true, profileOverride: null);
            menu.Items.Add(privateItem);

            // 実際に存在するプロファイルを列挙してサブメニューに並べる。
            // 検出できなかった場合はこの項目自体を出さない（空メニューを見せない）。
            var profiles = BrowserLaunchProfiles.DiscoverProfiles(browser);
            if (profiles.Count > 0)
            {
                var profileItem = new ToolStripMenuItem("プロファイルを選んで開く(&F)");
                foreach (var profile in profiles)
                {
                    var capturedProfile = profile;
                    var item = new ToolStripMenuItem(capturedProfile);
                    item.Click += (s, _) =>
                        LaunchBrowserWithOptions(browser, forcePrivateMode: false, profileOverride: capturedProfile);
                    profileItem.DropDownItems.Add(item);
                }
                menu.Items.Add(profileItem);
            }

            menu.Closed += (s, _) => menu.Dispose();
            menu.Show(button, e.Location);
        }

        /// <summary>
        /// シークレット/プロファイル指定でブラウザを起動し、起動後の終了処理を行います。
        /// </summary>
        /// <param name="browser">対象のブラウザ</param>
        /// <param name="forcePrivateMode">シークレット起動する場合はtrue</param>
        /// <param name="profileOverride">使用するプロファイル名（nullならブラウザ設定に従う）</param>
        private void LaunchBrowserWithOptions(Browser browser, bool forcePrivateMode, string? profileOverride)
        {
            Logger.LogInfo("MainForm.LaunchBrowserWithOptions", "ブラウザ選択（オプション指定）",
                browser.Name, _currentUrl, $"private={forcePrivateMode}, profile={profileOverride ?? "(既定)"}");

            try
            {
                var autoClose = chkAutoClose?.Checked ?? true;
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    autoClose = false;
                }

                var shouldTerminate = BrowserUtilities.LaunchBrowser(
                    browser, _currentUrl, autoClose, forcePrivateMode, profileOverride);
                HandlePostLaunchTermination(shouldTerminate);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.LaunchBrowserWithOptions", "ブラウザ起動エラー", browser.Name, ex.Message);
                MessageBoxService.ShowErrorStatic($"ブラウザの起動に失敗しました: {ex.Message}", "エラー");
            }
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
        /// コマンドライン引数(-d/--delay、-b/--browser、--silent)による起動時オーバーライドを設定します（Loadイベントで使用）。
        /// </summary>
        /// <param name="delayOverride">-d/--delayで指定された遅延秒数。未指定の場合はnull</param>
        /// <param name="browserGuid">-b/--browserで指定されたブラウザのGUID。未指定の場合はnull</param>
        /// <param name="silentMode">--silentが指定された場合はtrue</param>
        public void SetStartupOptions(int? delayOverride, Guid? browserGuid, bool silentMode)
        {
            _startupDelayOverride = delayOverride;
            _startupBrowserGuid = browserGuid;
            _startupSilentMode = silentMode;
            Logger.LogDebug("MainForm.SetStartupOptions", "起動時オーバーライド設定",
                delayOverride?.ToString() ?? "(未指定)", browserGuid?.ToString() ?? "(未指定)", silentMode);
        }

        /// <summary>
        /// 実効的なデフォルト遅延秒数。-d/--delayが指定されていればそれを優先する。
        /// </summary>
        private int EffectiveDefaultDelay => _startupDelayOverride ?? _settings?.DefaultDelay ?? 0;

        /// <summary>
        /// -b/--browserまたは--silentによる起動時オーバーライドを処理します。
        /// 該当する場合は選択画面を出さず即座にブラウザを起動し、trueを返します。
        /// 他プロセスから後続のURLを受信した場合（ReceiveExternalURL）にまで
        /// 効果が及ばないよう、一度処理したらオーバーライドをクリアします。
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <returns>起動時オーバーライドとして処理した場合はtrue</returns>
        private bool TryHandleStartupOverrides(string url)
        {
            if (_startupBrowserGuid == null && !_startupSilentMode)
            {
                return false;
            }

            // -bで指定されたブラウザを優先し、無ければ既定ブラウザ、それも無ければ何もしない
            Browser? targetBrowser = null;
            if (_startupBrowserGuid.HasValue)
            {
                targetBrowser = _browsers?.FirstOrDefault(b => b.Guid == _startupBrowserGuid.Value);
                if (targetBrowser == null)
                {
                    Logger.LogWarning("MainForm.TryHandleStartupOverrides", "-b/--browserで指定されたブラウザが見つかりません", _startupBrowserGuid.Value);
                }
            }
            targetBrowser ??= _defaultBrowser;

            // 一度きりの適用とするため、処理の成否に関わらずここでクリアする
            var wasSilentMode = _startupSilentMode;
            _startupBrowserGuid = null;
            _startupSilentMode = false;

            if (targetBrowser == null)
            {
                Logger.LogWarning("MainForm.TryHandleStartupOverrides", "起動時オーバーライド用のブラウザが見つからないため通常の選択画面を表示します");
                return false;
            }

            Logger.LogInfo("MainForm.TryHandleStartupOverrides", "起動時オーバーライドでブラウザを起動", targetBrowser.Name, url, wasSilentMode);
            BrowserUtilities.LaunchBrowser(targetBrowser, url, _settings?.AllowStayOpen != true);
            RequestClose();
            return true;
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

            // トラッキングパラメータの除去とポリシーによる正規化を、
            // ルーティング判定・表示・起動のすべてより前段で一度だけ適用する。
            // いずれも既定では無効で、有効時のみURLが書き換わる。
            url = URLSanitizer.Sanitize(url, _settings);

            _currentUrl = url;
            UpdateURLLabel();

            // -b/--browserまたは--silentがコマンドラインで指定されている場合は、
            // AutoURLs/Protocolより先に処理し、選択画面を出さず即座に起動する
            if (TryHandleStartupOverrides(url))
            {
                return;
            }

            // AutoURLsとProtocolの処理を実行
            if (ProcessAutoURLsAndProtocols(url))
            {
                // AutoURLsまたはProtocolで処理された場合は、StartupLauncherは呼び出さない
                Logger.LogInfo("MainForm.UpdateURL", "AutoURLsまたはProtocolで処理完了、StartupLauncherはスキップ", url);
                return;
            }

            // AutoURLsとProtocolで処理されなかった場合のみ、StartupLauncherを使用してURLを処理
            var isExpandingShortUrl = StartupLauncher.SetURL(url, _settings?.RevealShortURL ?? false, OnURLUpdated);

            // 短縮URL展開がバックグラウンドで進行中の場合はカウントダウンを開始しない。
            // 展開が完了する前にカウントダウンが0になると、短縮URLのまま
            // デフォルトブラウザで開いてしまうため。展開完了時はOnURLUpdatedが
            // カウントダウンを（再）開始する。
            if (!isExpandingShortUrl && _defaultBrowser != null && EffectiveDefaultDelay > 0)
            {
                StartCountdown();
            }
        }

        /// <summary>
        /// AutoURLsとProtocolの処理を実行
        /// 優先順位: AutoURLs > Protocol
        /// </summary>
        /// <param name="url">処理対象のURL</param>
        /// <returns>処理された場合はtrue</returns>
        private bool ProcessAutoURLsAndProtocols(string url)
        {
            try
            {
                // どのルールが適用されるかの判定は URLRoutingResolver に集約している。
                // Options の URL テスト欄（3-8）が同じ判定を使うため、
                // 「プレビューの表示」と「実際の起動挙動」が食い違わない。
                var routing = URLRoutingResolver.Resolve(_settings, url);
                Logger.LogDebug("MainForm.ProcessAutoURLsAndProtocols", "ルーティング判定",
                    url, routing.Kind.ToString(), routing.MatchedPattern);

                switch (routing.Kind)
                {
                    case URLRoutingKind.AutoUrl when routing.Browser != null:
                    {
                        Logger.LogInfo("MainForm.ProcessAutoURLsAndProtocols", "AutoURLsで処理完了",
                            url, routing.MatchedPattern, routing.Browser.Name);

                        // ルール側でAutoCloseが指定されている場合はそれを優先する
                        // （URL.AutoCloseは従来保存されるだけで参照されていなかった）
                        var autoClose = routing.ForceAutoClose || (chkAutoClose?.Checked ?? true);
                        if (routing.DelaySeconds > 0)
                        {
                            StartAutoURLsCountdown(routing.Browser, url, routing.DelaySeconds, autoClose);
                        }
                        else
                        {
                            var shouldTerminate = BrowserUtilities.LaunchBrowser(routing.Browser, url, autoClose);
                            HandlePostLaunchTermination(shouldTerminate);
                        }
                        return true;
                    }

                    case URLRoutingKind.Protocol when routing.Browser != null:
                    {
                        Logger.LogInfo("MainForm.ProcessAutoURLsAndProtocols", "Protocolで処理完了",
                            url, routing.MatchedPattern, routing.Browser.Name);

                        // プロトコル経路は遅延なしで起動し、処理後は自動終了する
                        var shouldTerminate = BrowserUtilities.LaunchBrowser(routing.Browser, url, true);
                        HandlePostLaunchTermination(shouldTerminate);
                        return true;
                    }

                    case URLRoutingKind.MatchedButBrowserMissing:
                        Logger.LogWarning("MainForm.ProcessAutoURLsAndProtocols",
                            "ルールにマッチしましたが対応するブラウザが見つかりません",
                            url, routing.MatchedPattern, routing.RuleName);
                        return false;

                    default:
                        Logger.LogDebug("MainForm.ProcessAutoURLsAndProtocols", "AutoURLsとProtocolの両方でマッチするパターンなし", url);
                        return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.ProcessAutoURLsAndProtocols", "AutoURLs/Protocol処理エラー", ex.Message, ex.StackTrace ?? "");
                return false;
            }
        }


        /// <summary>
        /// AutoURLs用のカウントダウンを開始
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="delay">遅延時間（秒）</param>
        /// <param name="autoClose">自動終了するか</param>
        private void StartAutoURLsCountdown(Browser browser, string url, int delay, bool autoClose)
        {
            try
            {
                Logger.LogInfo("MainForm.StartAutoURLsCountdown", "AutoURLsカウントダウン開始", 
                    $"Browser: {browser.Name}, Delay: {delay}, AutoClose: {autoClose}");

                // 既存のカウントダウンを停止
                if (_countdownTimer != null)
                {
                    _countdownTimer.Stop();
                    _countdownTimer.Dispose();
                }

                _currentDelay = delay;
                _isPaused = false;

                _countdownTimer = new System.Windows.Forms.Timer
                {
                    Interval = 1000
                };
                _countdownTimer.Tick += (sender, e) => AutoURLsCountdownTimer_Tick(browser, url, autoClose);
                _countdownTimer.Start();

                UpdateAutoURLsCountdownDisplay(browser);
                if (_countdownLabel != null)
                {
                    _countdownLabel.Visible = true;
                }

                Logger.LogDebug("MainForm.StartAutoURLsCountdown", "AutoURLsカウントダウン開始完了");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.StartAutoURLsCountdown", "AutoURLsカウントダウン開始エラー", ex.Message);
            }
        }

        /// <summary>
        /// AutoURLsカウントダウンタイマーの処理
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        /// <param name="url">開くURL</param>
        /// <param name="autoClose">自動終了するか</param>
        private void AutoURLsCountdownTimer_Tick(Browser browser, string url, bool autoClose)
        {
            try
            {
                if (_isPaused) return;

                _currentDelay--;
                UpdateAutoURLsCountdownDisplay(browser);

                if (_currentDelay <= 0)
                {
                    _countdownTimer?.Stop();
                    Logger.LogInfo("MainForm.AutoURLsCountdownTimer_Tick", "AutoURLsブラウザ起動", 
                        $"Browser: {browser.Name}, URL: {url}");

                    var shouldTerminate = BrowserUtilities.LaunchBrowser(browser, url, autoClose);

                    if (shouldTerminate)
                    {
                        Logger.LogInfo("MainForm.AutoURLsCountdownTimer_Tick", "AutoClose実行", "遅延起動後");
                    }
                    HandlePostLaunchTermination(shouldTerminate);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.AutoURLsCountdownTimer_Tick", "AutoURLsカウントダウン処理エラー", ex.Message);
            }
        }

        /// <summary>
        /// AutoURLsカウントダウン表示の更新
        /// </summary>
        /// <param name="browser">起動するブラウザ</param>
        private void UpdateAutoURLsCountdownDisplay(Browser browser)
        {
            if (_countdownLabel != null)
            {
                var status = _isPaused ? " (一時停止)" : "";
                _countdownLabel.Text = $"{browser.Name}で {_currentDelay} 秒後に起動{status}";
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

            // 短縮URL展開が完了したタイミングでカウントダウンを開始する。
            // UpdateURL側では展開中はカウントダウンを開始していないため、
            // ここで初めてデフォルトブラウザによる自動起動の猶予が始まる。
            if (_defaultBrowser != null && EffectiveDefaultDelay > 0 && _countdownTimer == null)
            {
                StartCountdown();
            }
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
                
                // コピーボタンのアイコン読み込み（RefreshFormから再読込される場合、旧Imageを破棄してから差し替える）
                if (btnCopyToClipboard != null)
                {
                    var oldImage = btnCopyToClipboard.Image;
                    var pasteIcon = Properties.Resources.PasteIcon;
                    btnCopyToClipboard.Image = ImageUtilities.ResizeImage(pasteIcon, 28, 28);
                    if (oldImage != null && !ReferenceEquals(oldImage, btnCopyToClipboard.Image))
                    {
                        oldImage.Dispose();
                    }
                }

                // コピー＆クローズボタンのアイコン読み込み（同上）
                if (btnCopyToClipboardAndClose != null)
                {
                    var oldImage = btnCopyToClipboardAndClose.Image;
                    var pasteAndCloseIcon = Properties.Resources.PasteAndCloseIcon;
                    btnCopyToClipboardAndClose.Image = ImageUtilities.ResizeImage(pasteAndCloseIcon, 28, 28);
                    if (oldImage != null && !ReferenceEquals(oldImage, btnCopyToClipboardAndClose.Image))
                    {
                        oldImage.Dispose();
                    }
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
            // RefreshFormから再度呼ばれた場合、旧インスタンスを確実に破棄してから作り直す
            _toolTip?.Dispose();
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
                Font = UrlDisplayFont,
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
        /// 起動メッセージラベルの作成。
        /// Settings.StartupMessage が非空のときだけフォーム上部に表示します。
        /// RefreshFormから再度呼ばれた場合は旧ラベルを除去・破棄してから作り直します。
        /// </summary>
        private void CreateStartupMessageLabel()
        {
            if (_startupMessageLabel != null)
            {
                Controls.Remove(_startupMessageLabel);
                _startupMessageLabel.Dispose();
                _startupMessageLabel = null;
            }

            if (_settings?.IsStartupMessageVisible != true) return;

            _startupMessageLabel = new Label
            {
                Name = "lblStartupMessage",
                Text = _settings.StartupMessage,
                AutoSize = true,
                // btnInfo(左上)の右に置く。ボタン行はEffectiveGridOriginYぶん
                // 下へずれるため、ホットキーのオーバーレイラベルとは重ならない。
                Location = new Point(GridOriginX, 6),
                Font = StartupMessageFont,
                BackColor = Color.Transparent,
                ForeColor = Color.DarkSlateGray
            };

            Controls.Add(_startupMessageLabel);
            _startupMessageLabel.BringToFront();
        }

        /// <summary>
        /// カウントダウンラベルの作成
        /// </summary>
        private void CreateCountdownLabel()
        {
            // RefreshFormから再度呼ばれた場合、旧ラベルをControlsから除去・破棄してから作り直す
            if (_countdownLabel != null)
            {
                Controls.Remove(_countdownLabel);
                _countdownLabel.Dispose();
                _countdownLabel = null;
            }

            _countdownLabel = new Label
            {
                Name = "lblCountdown",
                Text = "",
                AutoSize = true,
                Location = new Point(20, ClientSize.Height - 20),
                Visible = false,
                Font = CountdownLabelFont,
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

            // 既存のタイマーが残っていれば破棄してから新規作成する。
            // 破棄せずに再代入すると旧タイマーのTickイベントが解除されないまま
            // 動き続け、複数のカウントダウンが同時に進行してしまう。
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= CountdownTimer_Tick;
                _countdownTimer.Dispose();
                _countdownTimer = null;
            }

            // -d/--delayでコマンドラインから明示的に指定された場合はSettings.DefaultDelayより優先する
            _currentDelay = _startupDelayOverride ?? _settings?.DefaultDelay ?? 5;
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
                BrowserUtilities.LaunchBrowser(_defaultBrowser!, _currentUrl, chkAutoClose?.Checked ?? true);
                RequestClose();
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
            RequestClose();
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
                RequestClose();
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

                // トグルのたびにXML全体をUIスレッドで同期書き込みすると、
                // 連続操作でそのぶんUIが固まる。少し待ってからまとめて保存する。
                ScheduleDeferredSettingsSave();
            }
        }

        /// <summary>
        /// 設定の保存を遅延実行します。短時間に複数回呼ばれた場合は最後の1回だけ保存されます。
        /// </summary>
        private void ScheduleDeferredSettingsSave()
        {
            if (_deferredSaveTimer == null)
            {
                _deferredSaveTimer = new System.Windows.Forms.Timer { Interval = DeferredSaveDelayMs };
                _deferredSaveTimer.Tick += DeferredSaveTimer_Tick;
            }

            // 既に動いていれば作り直さずタイマーを引き延ばす（デバウンス）
            _deferredSaveTimer.Stop();
            _deferredSaveTimer.Start();
        }

        /// <summary>
        /// 遅延保存タイマーの満了時に、実際の保存を行います。
        /// </summary>
        private void DeferredSaveTimer_Tick(object? sender, EventArgs e)
        {
            _deferredSaveTimer?.Stop();

            try
            {
                _settings?.DoSave();
                Logger.LogDebug("MainForm.DeferredSaveTimer_Tick", "設定を保存しました");
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.DeferredSaveTimer_Tick", "設定の保存に失敗", ex.Message);
            }
        }

        /// <summary>
        /// 保留中の遅延保存があれば、即座に実行します（フォームを閉じる際に使用）。
        /// </summary>
        private void FlushDeferredSettingsSave()
        {
            if (_deferredSaveTimer?.Enabled == true)
            {
                DeferredSaveTimer_Tick(null, EventArgs.Empty);
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
        /// Optionsショートカット（Ctrl+指定キー）が押されたかどうかを判定します。
        /// 以前はe.KeyCode.ToString()（数字キーは"D5"のような文字列になる）と
        /// 設定文字を直接比較していたため、数字をショートカットに設定すると
        /// 永久にマッチしなかった。e.KeyValueは押されたキーの仮想キーコードで
        /// A-Z・0-9キーではASCIIコードと一致するため、文字として正しく比較できる。
        /// また修飾キー無しの単独キーだと入力中の誤爆が起きやすいため、Ctrlを必須化する。
        /// </summary>
        /// <param name="e">キーイベント引数</param>
        /// <returns>Optionsショートカットとして扱う場合はtrue</returns>
        private bool IsOptionsShortcutKey(KeyEventArgs e)
        {
            var optionsShortcutChar = _settings?.OptionsShortcut ?? 'O';
            return e.Control && optionsShortcutChar != char.MinValue &&
                   e.KeyValue >= 0 && e.KeyValue <= char.MaxValue &&
                   char.ToUpperInvariant((char)e.KeyValue) == char.ToUpperInvariant(optionsShortcutChar);
        }

        /// <summary>
        /// キーボードイベントの処理
        /// </summary>
        private void MainForm_KeyDown(object? sender, KeyEventArgs e)
        {
            // オプションショートカット（Ctrl+指定キー）
            if (IsOptionsShortcutKey(e))
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
            

            
            // 数字キー（0-9）または英字キー（A-Z）でホットキー処理。
            // AddEditBrowserFormは任意の1文字をホットキーとして受け付けるが、
            // 以前はここが数字キーしか判定しておらず、英字ホットキーを設定しても
            // 一切反応しなかった。
            if (TryGetHotkeyChar(e, out var pressedChar))
            {
                foreach (var browser in _browsers ?? new List<Browser>())
                {
                    if (browser.Hotkey != '\0' && char.ToUpperInvariant(browser.Hotkey) == pressedChar)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        Logger.LogInfo("MainForm.MainForm_KeyDown", "ホットキー起動", browser.Name, pressedChar);
                        var shouldTerminate = BrowserUtilities.LaunchBrowser(browser, _currentUrl, chkAutoClose?.Checked ?? true);
                        HandlePostLaunchTermination(shouldTerminate);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// キーイベントからブラウザホットキーとして扱う文字を取得します。
        /// 数字キー（0-9）と英字キー（A-Z）のみを対象とし、修飾キーが押されている場合は
        /// 他のショートカット（Ctrl+Optionsショートカット等）と衝突しないよう対象外とします。
        /// </summary>
        /// <param name="e">キーイベント引数</param>
        /// <param name="pressedChar">ホットキー対象の場合、大文字化された文字</param>
        /// <returns>ホットキー対象のキーだった場合はtrue</returns>
        private static bool TryGetHotkeyChar(KeyEventArgs e, out char pressedChar)
        {
            pressedChar = '\0';

            if (e.Control || e.Alt)
            {
                return false;
            }

            var isDigitKey = e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9;
            var isLetterKey = e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z;
            if (!isDigitKey && !isLetterKey)
            {
                return false;
            }

            pressedChar = isDigitKey
                ? (char)('0' + (e.KeyCode == Keys.D0 ? 0 : e.KeyCode - Keys.D1 + 1))
                : char.ToUpperInvariant((char)e.KeyValue);
            return true;
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

            // フォーム幅に基づいて列数を計算（ボタン配置・オーバーレイ配置と同じ計算に統一）
            var columnsPerRow = CalculateColumnsPerRow();
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
                    var shouldTerminate = BrowserUtilities.LaunchBrowser(_defaultBrowser, _currentUrl, chkAutoClose?.Checked ?? true);
                    HandlePostLaunchTermination(shouldTerminate);
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
                // ユーザーが×ボタン等で閉じようとした場合、常駐設定が有効ならトレイに格納してキャンセルする
                if (e.CloseReason == CloseReason.UserClosing && (_settings?.AlwaysResidentInTray ?? false))
                {
                    Logger.LogDebug("MainForm.OnFormClosing", "AlwaysResidentInTrayが有効のためトレイに格納します");
                    e.Cancel = true;
                    InitializeSystemTray();
                    MinimizeToTray();
                    return;
                }

                // システムトレイアイコンのクリーンアップ
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }

                // 保留中の遅延保存があれば、閉じる前に確実に書き出す
                FlushDeferredSettingsSave();
                if (_deferredSaveTimer != null)
                {
                    _deferredSaveTimer.Stop();
                    _deferredSaveTimer.Tick -= DeferredSaveTimer_Tick;
                    _deferredSaveTimer.Dispose();
                    _deferredSaveTimer = null;
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
        /// トレイメニューから使用する既定ブラウザを取得します。
        /// 明示的な既定ブラウザが無い場合は、表示中の最初のブラウザで代用します。
        /// </summary>
        /// <returns>既定ブラウザ。該当が無い場合はnull</returns>
        private Browser? GetDefaultBrowserForTray()
        {
            return _defaultBrowser
                   ?? _browsers?.FirstOrDefault(b => b.IsDefault)
                   ?? _browsers?.FirstOrDefault(b => b.Visible && b.IsActive);
        }

        /// <summary>
        /// トレイメニューから、保持中のURLを既定ブラウザで開きます。
        /// </summary>
        private void LaunchDefaultBrowserFromTray()
        {
            var browser = GetDefaultBrowserForTray();
            if (browser == null || string.IsNullOrEmpty(_currentUrl))
            {
                Logger.LogDebug("MainForm.LaunchDefaultBrowserFromTray", "起動対象のブラウザまたはURLがありません");
                return;
            }

            try
            {
                Logger.LogInfo("MainForm.LaunchDefaultBrowserFromTray", "既定ブラウザで起動", browser.Name, _currentUrl);

                // 常駐を続けるため、起動後にアプリケーションを終了させない
                BrowserUtilities.LaunchBrowser(browser, _currentUrl, false);
            }
            catch (Exception ex)
            {
                Logger.LogError("MainForm.LaunchDefaultBrowserFromTray", "ブラウザ起動エラー", browser.Name, ex.Message);
                MessageBoxService.ShowErrorStatic($"ブラウザの起動に失敗しました: {ex.Message}", "エラー");
            }
        }

        /// <summary>
        /// トレイメニューから設定画面を開きます。
        /// 設定画面はモーダルのため、トレイに隠れたままだと操作できなくなることがある。
        /// 一度ウィンドウを表示してから開き、元がトレイ常駐なら閉じた後に戻す。
        /// </summary>
        private void ShowOptionsFromTray()
        {
            var wasInTray = _isInTray;

            try
            {
                if (wasInTray)
                {
                    ShowFromTray();
                }

                OpenOptionsForm();
            }
            finally
            {
                if (wasInTray && _settings?.AlwaysResidentInTray == true)
                {
                    MinimizeToTray();
                }
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

                // GetHicon()が返すHICONはIcon.FromHandleでは所有されないため、
                // Cloneしたコピーを使い元のハンドルはDestroyIconで解放する
                var trayIconHandle = Properties.Resources.BrowserChooser3.GetHicon();
                Icon trayIcon;
                using (var handleIcon = Icon.FromHandle(trayIconHandle))
                {
                    trayIcon = (Icon)handleIcon.Clone();
                }
                ImageUtilities.DestroyIconHandle(trayIconHandle);

                _notifyIcon = new NotifyIcon
                {
                    Icon = trayIcon,
                    Text = "Browser Chooser 3",
                    Visible = false
                };

                // コンテキストメニューの作成
                var contextMenu = new ContextMenuStrip();

                var showItem = new ToolStripMenuItem("表示(&S)");
                showItem.Click += (sender, e) => ShowFromTray();
                contextMenu.Items.Add(showItem);

                // 保持中のURLを既定ブラウザで開く。URLが無い場合は選べないようにする。
                var openDefaultItem = new ToolStripMenuItem("既定ブラウザで開く(&D)");
                openDefaultItem.Click += (sender, e) => LaunchDefaultBrowserFromTray();
                contextMenu.Items.Add(openDefaultItem);

                var optionsItem = new ToolStripMenuItem("設定(&O)...");
                optionsItem.Click += (sender, e) => ShowOptionsFromTray();
                contextMenu.Items.Add(optionsItem);

                contextMenu.Items.Add(new ToolStripSeparator());

                var exitItem = new ToolStripMenuItem("終了(&X)");
                exitItem.Click += (sender, e) => Application.Exit();
                contextMenu.Items.Add(exitItem);

                // メニューを開くたびに、その時点の状態で有効/無効を切り替える
                contextMenu.Opening += (sender, e) =>
                {
                    openDefaultItem.Enabled =
                        !string.IsNullOrEmpty(_currentUrl) && GetDefaultBrowserForTray() != null;
                };

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
