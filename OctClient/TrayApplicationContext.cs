using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace OctClient
{
    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public TrayApplicationContext()
        {
            // コンテキストメニューの作成
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // 「ウィンドウ表示」メニュー項目
            ToolStripMenuItem showWindowItem = new ToolStripMenuItem("ウィンドウ表示");
            showWindowItem.Click += ShowWindowItem_Click;
            contextMenu.Items.Add(showWindowItem);

            // 区切り線（必要に応じて）
            contextMenu.Items.Add(new ToolStripSeparator());

            // 「終了」メニュー項目
            ToolStripMenuItem exitItem = new ToolStripMenuItem("終了");
            exitItem.Click += ExitItem_Click;
            contextMenu.Items.Add(exitItem);

            // NotifyIcon の初期化
            trayIcon = new NotifyIcon
            {
                Icon = new Icon("app.ico"),  // 先ほど追加したアイコンファイル
                ContextMenuStrip = contextMenu,
                Visible = true,
                Text = "TrayAppSample"
            };

            // ダブルクリックでウィンドウ表示する場合（オプション）
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
        }

        // ダブルクリック時のイベントハンドラ
        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowWindow();
        }

        // 「ウィンドウ表示」メニュークリック時のイベントハンドラ
        private void ShowWindowItem_Click(object sender, EventArgs e)
        {
            ShowWindow();
        }

        // ウィンドウを表示するメソッド
        private void ShowWindow()
        {
            // 既存のウィンドウを探す
            FormSettings existingForm = Application.OpenForms.OfType<FormSettings>().FirstOrDefault();
            if (existingForm != null)
            {
                // 既存のウィンドウが見つかった場合は、それを前面に表示
                existingForm.WindowState = FormWindowState.Normal; // 最小化されている場合は元に戻す
                existingForm.Activate(); // ウィンドウをアクティブにして前面に表示
                return;
            }
            // 既にウィンドウが表示中かどうかのチェックなど、必要に応じて処理を追加してください。
            FormSettings form = new FormSettings();
            form.Show();
        }

        // 「終了」メニュークリック時のイベントハンドラ
        private void ExitItem_Click(object sender, EventArgs e)
        {
            trayIcon.Visible = false;  // アイコンを非表示にする
            Application.Exit();
        }
    }
}
