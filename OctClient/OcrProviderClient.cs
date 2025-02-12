using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OctClient
{
    /// <summary>
    /// octProvider との通信を担当するクラスです。
    /// このクラスは起動時もフォームのボタン押下でも再利用可能です。
    /// </summary>
    public class OctProviderClient
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public OctProviderClient(string host = "localhost", int port = 5002)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// "check" コマンドを送信し、サービスからの応答を非同期で取得します。
        /// </summary>
        /// <returns>サービスからの応答文字列。失敗時は null。</returns>
        public async Task<string> CheckAsync()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(Host, Port);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(stream, Encoding.ASCII))
                    {
                        await writer.WriteLineAsync("check");
                        string response = await reader.ReadLineAsync();
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                // 必要に応じてログ出力などを行ってください。
                return null;
            }
        }

        /// <summary>
        /// "get_file" コマンドを送信し、サービスからファイルを取得します。
        /// 取得したファイルはアプリケーションの実行ディレクトリに保存され、そのパスを返します。
        /// エラー発生時には "ERROR:" で始まるメッセージや null を返します。
        /// </summary>
        /// <returns>保存したファイルパス、またはエラーメッセージ。</returns>
        public async Task<string> GetFileAsync()
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(Host, Port);
                    using (NetworkStream stream = client.GetStream())
                    {
                        // ※ コマンドはテキストモードで送信
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 1024, true) { AutoFlush = true })
                        {
                            await writer.WriteLineAsync("get_file");
                        }

                        // ※ その後の応答はバイナリ形式で送信されるため BinaryReader を使用
                        using (BinaryReader binReader = new BinaryReader(stream, Encoding.UTF8, true))
                        {
                            // ファイル名の受信
                            string fileName = binReader.ReadString();
                            if (fileName.StartsWith("ERROR:"))
                            {
                                return fileName;
                            }
                            // ファイルサイズの受信
                            long fileSize = binReader.ReadInt64();

                            // 受信するファイルのパス（実行ディレクトリに保存）
                            string savedFilePath = Path.Combine(Environment.CurrentDirectory, fileName);

                            // ファイルの内容を受信し、保存
                            using (FileStream fileStream = File.Create(savedFilePath))
                            {
                                byte[] buffer = new byte[8192];
                                long remainingBytes = fileSize;
                                while (remainingBytes > 0)
                                {
                                    int bytesToRead = (int)Math.Min(remainingBytes, buffer.Length);
                                    int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
                                    if (bytesRead == 0)
                                    {
                                        break; // 予期せぬ終了
                                    }
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                    remainingBytes -= bytesRead;
                                }
                            }
                            return savedFilePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 必要に応じてログ出力などを行ってください。
                return null;
            }
        }
    }
}