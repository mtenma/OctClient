using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data.SqlTypes;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace OctClient.CustomActions
{
    [RunInstaller(true)]
    public class FolderPermissionInstaller : Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            // ログファイルのパスを %TEMP% フォルダーに設定
            string logPath = @"C:\tmp\CustomActionLog.txt";

            try
            {
                // ログに開始時刻を書き込む
                File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : Installメソッドが呼ばれました。" + Environment.NewLine);

                // インストールパラメータからターゲットディレクトリを取得
                string targetDir = Context.Parameters["targetdir"];
                if (!string.IsNullOrEmpty(targetDir))
                {
                    if (!targetDir.EndsWith("\\"))
                    {
                        targetDir += "\\";
                    }

                    // 対象フォルダのアクセス権変更処理
                    DirectoryInfo di = new DirectoryInfo(targetDir);
                    DirectorySecurity ds = di.GetAccessControl();

                    FileSystemAccessRule accessRule = new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                        FileSystemRights.Modify,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow);

                    ds.AddAccessRule(accessRule);
                    di.SetAccessControl(ds);

                    File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : アクセス権を変更しました。ターゲット：" + targetDir + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : targetdir パラメータが空です。" + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                // 例外発生時もログに出力
                File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : 例外発生 - " + ex.ToString() + Environment.NewLine);
            }
        }
    }
}
