using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace FileWatcher_v2_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximizeBox = false;
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string mkdirpath = DocumentsPath + @"\FileWatcherExceptionSkiper";
            if (Directory.Exists(mkdirpath))
            {
                Console.WriteLine("Aready Created");
            }
            else
            {
                Directory.CreateDirectory(mkdirpath);
            }


            // 監視を停止する
            fileSystemWatcher1.IncludeSubdirectories = false;

            // ファイル変更、作成、削除のイベントをファイル変更メソッドにあげる
            fileSystemWatcher1.Changed += new FileSystemEventHandler(fileChanged);
            fileSystemWatcher1.Created += new FileSystemEventHandler(fileChanged);
            fileSystemWatcher1.Deleted += new FileSystemEventHandler(fileChanged);

            // ファイル名変更のイベントをファイル名変更メソッドにあげる
            fileSystemWatcher1.Renamed += new RenamedEventHandler(fileRenamed);

            // 監視対象ディレクトリを指定する
            fileSystemWatcher1.Path = mkdirpath;
            {
                try
                {
                    

                }
                catch 
                {
                    
                }
            }

            // 監視対象の拡張子を指定する（全てを指定する場合は空にする）
            fileSystemWatcher1.Filter = "*";

            // サブディレクトリ配下も含めるか指定する
            fileSystemWatcher1.IncludeSubdirectories = true;

            // 監視する変更を指定する
            fileSystemWatcher1.NotifyFilter = NotifyFilters.LastAccess |
                               NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName;

            // 通知を格納する内部バッファ 既定値は 8192 (8 KB)  4 KB ～ 64 KB
            fileSystemWatcher1.InternalBufferSize = 1024 * 8;
        }
        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            // フォルダが存在しない場合は作成
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // フォルダ内のファイルをコピー
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destPath = Path.Combine(targetDir, fileName);
                File.Copy(filePath, destPath, true); // true は上書きを許可
            }

            // サブフォルダも再帰的にコピー
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string subDirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(targetDir, subDirName);
                CopyDirectory(subDir, destSubDir);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "監視するフォルダを選択してください";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // 選択されたフォルダのパス
                    string selectedFolderPath = folderBrowserDialog.SelectedPath;

                    // "C:\" にフォルダ名を付けてコピー
                    string destinationPath = Path.Combine("C:\\", Path.GetFileName(selectedFolderPath));
                    CopyDirectory(selectedFolderPath, destinationPath);

                    // パスが選択された場合、そのパスを監視対象として設定する
                    fileSystemWatcher1.Path = selectedFolderPath;

                    // 監視を実行する
                    fileSystemWatcher1.EnableRaisingEvents = true;
                }
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {

            // 監視を停止する
            fileSystemWatcher1.IncludeSubdirectories = false;
        }
        private static void fileChanged(object sender, FileSystemEventArgs e)
        {

            string s = String.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} :");


            //変更があったときに結果を表示する
            switch (e.ChangeType)
            {
                case System.IO.WatcherChangeTypes.Changed:
                    s += $"【{e.ChangeType.ToString()}】{e.FullPath}";

                    break;
                case System.IO.WatcherChangeTypes.Created:
                    s += $"【{e.ChangeType.ToString()}】{e.FullPath}";

                    break;
                case System.IO.WatcherChangeTypes.Deleted:
                    s += $"【{e.ChangeType.ToString()}】{e.FullPath}";
                    break;
            }
            MessageBox.Show(s);
        }

        /// <summary>
        /// ファイル名変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void fileRenamed(object sender, RenamedEventArgs e)
        {
            string s = String.Format($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} :");
            s += $"【{e.ChangeType.ToString()}】{e.OldName} → {e.FullPath}";
            MessageBox.Show(s);

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "監視するファイルを選択してください";
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                fileDialog.Filter = "すべてのファイル|*.*";

                DialogResult result = fileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // 選択されたファイルのパス
                    string selectedFilePath = fileDialog.FileName;

                    // "C:\" にファイル名を付けてコピー
                    string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(selectedFilePath));
                    File.Copy(selectedFilePath, destinationPath);

                    // パスが選択された場合、そのパスを監視対象として設定する
                    fileSystemWatcher1.Path = Path.GetDirectoryName(selectedFilePath);

                    // 監視を実行する
                    fileSystemWatcher1.EnableRaisingEvents = true;
                }
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //リンク先に移動したことにする
            linkLabel1.LinkVisited = true;
            //ブラウザで開く
            System.Diagnostics.Process.Start("https://github.com/frost-1256/FileWatcher");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // メッセージボックスを表示
            DialogResult result = MessageBox.Show("監視が終了しますがよろしいですか?", "警告", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
            else if (result == System.Windows.Forms.DialogResult.No)
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 監視を終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // メッセージボックスを表示
            DialogResult result = MessageBox.Show("監視が終了しますがよろしいですか?", "警告", MessageBoxButtons.YesNo);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                fileSystemWatcher1.IncludeSubdirectories = false;
            }
            else if (result == System.Windows.Forms.DialogResult.No)
            {

            }
        }

        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }
    }
}
