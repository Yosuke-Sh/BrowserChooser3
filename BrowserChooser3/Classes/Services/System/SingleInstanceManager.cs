using System.IO.Pipes;
using System.Text;
using BrowserChooser3.Classes.Utilities;

namespace BrowserChooser3.Classes.Services.SystemServices
{
    /// <summary>
    /// アプリケーションの単一インスタンス化と、既存インスタンスへのURL引き渡しを管理します
    /// </summary>
    public class SingleInstanceManager : IDisposable
    {
        private const string DefaultMutexName = "Local\\BrowserChooser3_SingleInstance_Mutex";
        private const string DefaultPipeName = "BrowserChooser3_SingleInstance_Pipe";
        private const int ClientConnectTimeoutMs = 500;

        private readonly string _mutexName;
        private readonly string _pipeName;

        private Mutex? _mutex;
        private Thread? _serverThread;
        private CancellationTokenSource? _serverCancellation;

        /// <summary>
        /// 既存インスタンスから新しいURLを受信したときに発火します（UIスレッド外から呼ばれます）
        /// </summary>
        public event Action<string>? UrlReceived;

        /// <summary>
        /// 既定のMutex名・パイプ名で初期化します
        /// </summary>
        public SingleInstanceManager() : this(DefaultMutexName, DefaultPipeName)
        {
        }

        /// <summary>
        /// テストなどで名前を分離したい場合に使用するコンストラクタ
        /// </summary>
        /// <param name="mutexName">使用するMutex名</param>
        /// <param name="pipeName">使用する名前付きパイプ名</param>
        public SingleInstanceManager(string mutexName, string pipeName)
        {
            _mutexName = mutexName;
            _pipeName = pipeName;
        }

        /// <summary>
        /// このプロセスが最初のインスタンス（オーナー）かどうかを判定し、
        /// オーナーであればパイプサーバーを起動します。
        /// オーナーでなければ何もしません（呼び出し側が<see cref="TrySendUrlToExistingInstance"/>を使うこと）。
        /// </summary>
        /// <returns>このプロセスが最初のインスタンスの場合はtrue</returns>
        public bool TryAcquire()
        {
            _mutex = new Mutex(true, _mutexName, out var createdNew);

            if (createdNew)
            {
                Logger.LogDebug("SingleInstanceManager.TryAcquire", "単一インスタンスのオーナーとして起動");
                StartPipeServer();
                return true;
            }

            Logger.LogDebug("SingleInstanceManager.TryAcquire", "既存インスタンスが検出されました");
            _mutex.Dispose();
            _mutex = null;
            return false;
        }

        /// <summary>
        /// 既存の常駐インスタンスに対して、名前付きパイプ経由でURLを送信します
        /// </summary>
        /// <param name="url">送信するURL</param>
        /// <param name="pipeName">送信先の名前付きパイプ名（省略時は既定値）</param>
        /// <returns>送信に成功した場合はtrue</returns>
        public static bool TrySendUrlToExistingInstance(string url, string pipeName = DefaultPipeName)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
                client.Connect(ClientConnectTimeoutMs);

                using var writer = new StreamWriter(client, Encoding.UTF8) { AutoFlush = true };
                writer.WriteLine(url ?? string.Empty);

                Logger.LogInfo("SingleInstanceManager.TrySendUrlToExistingInstance", "既存インスタンスへURLを送信", url ?? string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("SingleInstanceManager.TrySendUrlToExistingInstance", "既存インスタンスへの送信に失敗", ex.Message);
                return false;
            }
        }

        private void StartPipeServer()
        {
            _serverCancellation = new CancellationTokenSource();
            var token = _serverCancellation.Token;

            _serverThread = new Thread(() => RunPipeServer(token))
            {
                IsBackground = true
            };
            _serverThread.Start();
        }

        private void RunPipeServer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using var server = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                    // WaitForConnection()は同期ブロッキングでキャンセルできないため、
                    // 非同期版をCancellationTokenと組み合わせて使用する
                    try
                    {
                        server.WaitForConnectionAsync(token).GetAwaiter().GetResult();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    using var reader = new StreamReader(server, Encoding.UTF8);
                    var url = reader.ReadLine();

                    if (!string.IsNullOrEmpty(url))
                    {
                        Logger.LogInfo("SingleInstanceManager.RunPipeServer", "他プロセスからURLを受信", url);
                        UrlReceived?.Invoke(url);
                    }
                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Logger.LogError("SingleInstanceManager.RunPipeServer", "パイプサーバーエラー", ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Mutexとパイプサーバーを解放します
        /// </summary>
        public void Dispose()
        {
            _serverCancellation?.Cancel();

            // サーバースレッドが停止するまで待つ（テストでのリーク・干渉を避けるため）。
            // WaitForConnectionAsync(token)がキャンセルで即座に抜けるため通常は待たずに
            // 終わる。ここでの待ち時間は「万一抜けなかった場合の上限」であり、
            // 終了処理を長引かせないよう短くしている。
            _serverThread?.Join(TimeSpan.FromMilliseconds(500));

            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            _mutex = null;
        }
    }
}
