using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
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

            // インストール時に渡されたパラメータ "targetdir" を取得
            string targetDir = Context.Parameters["targetdir"];
            if (!string.IsNullOrEmpty(targetDir))
            {
                // パスの末尾に "\" が無い場合は追加
                if (!targetDir.EndsWith("\\"))
                {
                    targetDir += "\\";
                }

                // 指定ディレクトリの DirectoryInfo を取得
                DirectoryInfo di = new DirectoryInfo(targetDir);
                DirectorySecurity ds = di.GetAccessControl();

                // BuiltinUsers（標準ユーザー）に対して Modify 権限を追加
                FileSystemAccessRule accessRule = new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                ds.AddAccessRule(accessRule);
                di.SetAccessControl(ds);
            }

            // 以下のログ出力処理はコメントアウトして無効にします。
            // File.AppendAllText(logPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : 例外発生 - " + ex.ToString() + Environment.NewLine);
        }
    }
}