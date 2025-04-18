﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Pipes;
using System.Net;

namespace OctClient
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            string ip = SendPipeCommand("get_ip");
            textIP.Text = ip;
            string folder = SendPipeCommand("get_folder");
            txtFolderPath.Text = folder;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //Properties.Settings.Default.IPAddress = textIP.Text;
            //Properties.Settings.Default.Save();
            if (textIP.Text.Trim() == "")
            {
                MessageBox.Show("IPが設定されていません。");
                return;
            }
            if(txtFolderPath.Text.Trim() == "")
            {
                MessageBox.Show("保存フォルダが指定されていません");
                return ;
            }



            string result = SendPipeCommand("save|" + textIP.Text + "|" + txtFolderPath.Text);
            if (result == "ok")
            {
                MessageBox.Show("保存しました");
            }
            else
            {
                MessageBox.Show("保存に失敗しました");

            }
        }


        /// <summary>
        /// 名前付きパイプ経由でコマンドを送信し、サービスからの応答を返すヘルパーメソッド
        /// </summary>
        /// <param name="command">送信するコマンド文字列</param>
        /// <returns>サービス側からの応答</returns>
        private string SendPipeCommand(string command)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "OctClientPipe", PipeDirection.InOut))
                {
                    // サービス側のパイプが待機している場合、接続（タイムアウトは 2 秒）
                    pipeClient.Connect(2000); 
                    // StreamWriter/Reader を作るときに leaveOpen:true を指定する
                    using (StreamWriter writer = new StreamWriter(pipeClient, System.Text.Encoding.UTF8, 1024, leaveOpen: true) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(pipeClient, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
                    {
                        writer.WriteLine(command);
                        string response = reader.ReadLine();
                        return response;
                    }
                }
            }
            catch (TimeoutException)
            {
                return "接続タイムアウト: サービスが起動していない可能性があります。";
            }
            catch (Exception ex)
            {
                return "エラー: " + ex.Message;
            }
        }

        /// <summary>
        /// 「get_ip」要求: サービスから IP を取得する
        /// </summary>
        private void btnGetIP_Click(object sender, EventArgs e)
        {
            string result = SendPipeCommand("get_ip");
            MessageBox.Show("get_ip: " + result);
        }

        /// <summary>
        /// 「get_file」要求: サービスからファイル処理結果を取得する
        /// </summary>
        private void btnGetFile_Click(object sender, EventArgs e)
        {
            string result = SendPipeCommand("get_file");
            MessageBox.Show("get_file: " + result);
        }

        private void btnTest2_Click(object sender, EventArgs e)
        {
            string result = SendPipeCommand("get_file");
            MessageBox.Show("get_file: " + result);
        }

        /// <summary>
        /// 「chk_connect」要求: テキストボックスに入力された IP を渡して応答を取得する
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            string ipParameter = textIP.Text;
            // IP として成立していない場合はng
            if (!IPAddress.TryParse(ipParameter, out _))
            {
                MessageBox.Show("IP が不正です。");
                return;
            }

            string command = "chk_connect " + ipParameter;
            string result = SendPipeCommand(command);
            MessageBox.Show("chk_connect: " + result);
        }

        private void btnWriteTest_Click(object sender, EventArgs e)
        {

            string result = SendPipeCommand("get_file");
            if(result == "ok")
            {
                MessageBox.Show("受信しました");
            }
            else
            {
                MessageBox.Show("受信に失敗しました（" + result + "）");
            }
   
            /*書き込みテスト*/
            /*
            // Proguramu Files / cca  にtest.txt を作成
            string targetDir = @"C:\Program Files (x86)\cca";

            // 指定ディレクトリが存在しない場合は作成します
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string filePath = Path.Combine(targetDir, "test.txt");
            File.WriteAllText(filePath, "test");
            */
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //画面の終了
            this.Close();
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                txtFolderPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = SendPipeCommand("get_token");
            textBox1.Text = result;

        }
    }
}

