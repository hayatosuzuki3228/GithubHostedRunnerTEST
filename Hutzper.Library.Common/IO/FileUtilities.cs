using System.Diagnostics;

namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// ファイル関連ユーティリティクラス
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// 拡張子を維持してファイル名を変更する
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ChangeFileNameKeepExtension(string source, string destination)
        {
            var changedFileName = string.Empty;

            try
            {
                changedFileName = System.IO.Path.ChangeExtension(System.IO.Path.GetFileNameWithoutExtension(destination), System.IO.Path.GetExtension(source));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return changedFileName;
        }

        /// <summary>
        /// ディレクトリ削除＆失敗時リネーム
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="isNeedToRenameAtFailed"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool DeleteAndVerify(DirectoryInfo directoryInfo, bool isNeedToRenameAtFailed = true, string suffix = "deleted")
        {
            try
            {
                if (directoryInfo.Exists)
                {
                    directoryInfo.Delete(true);
                }

                directoryInfo.Refresh();
                if (directoryInfo.Exists && isNeedToRenameAtFailed)
                {
                    var newName = $"{System.IO.Path.GetDirectoryName(directoryInfo.FullName)}_{suffix}_{DateTime.Now:yyyyMMdd-HHmmss}";
                    var renamed = new DirectoryInfo(newName);

                    if (renamed.Exists == false)
                    {
                        directoryInfo.MoveTo(renamed.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return directoryInfo.Exists;
        }

        /// <summary>
        /// ファイル削除＆失敗時リネーム
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="isNeedToRenameAtFailed"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool DeleteAndVerify(FileInfo fileInfo, bool isNeedToRenameAtFailed = true, string suffix = "deleted")
        {
            try
            {
                if (fileInfo.Exists)
                {
                    if (fileInfo.Directory is null) return false;
                    DirectoryInfo directoryInfo = fileInfo.Directory;
                    if (directoryInfo.Exists) directoryInfo.Delete(true);
                }

                fileInfo.Refresh();

                if (fileInfo.Exists && isNeedToRenameAtFailed)
                {
                    var fileExt = System.IO.Path.GetExtension(fileInfo.FullName);
                    var newName = fileInfo.FullName.Replace(fileExt, $"_{suffix}_{DateTime.Now:yyyyMMdd-HHmmss}{fileExt}");

                    var renamed = new FileInfo(newName);

                    if (renamed.Exists == false)
                    {
                        fileInfo.MoveTo(renamed.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return fileInfo.Exists;
        }

        /// <summary>
        /// 指定した空き容量までファイルを削除する
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>削除できたらtrue</returns>
        public static bool DeleteFilesToFreeSpace(DirectoryInfo? directory, float FreeParent = 0.05f)
        {
            if (directory == null) return false;
            bool isDelete = false;

            string[] patterns = new string[] { "*.bmp", "*.jpg", "*.jpeg", "*.png", "*.dat" };
            List<FileInfo> files = patterns.SelectMany(pattern =>
                                    directory.GetFiles(pattern, SearchOption.AllDirectories)).ToList();
            Queue<FileInfo> fileQueue = new Queue<FileInfo>(files.OrderBy(f => f.CreationTime));

            while (GetAvailableFreeSpace(directory.Root.FullName) < FreeParent)
            {
                if (fileQueue.Count <= 0) break;
                FileInfo? oldestFile = fileQueue.Dequeue();
                if (!File.Exists(oldestFile.FullName)) continue;
                if (oldestFile != null)
                {
                    DeleteAndVerify(oldestFile);
                    isDelete = true;
                    continue;
                }
            }
            DeleteEmptyDirectories(directory.FullName);
            return isDelete;
        }

        /// <summary>
        /// ドライブの利用可能な空き容量を取得する
        /// </summary>
        /// <param name="drivePath"></param>
        /// <returns>空き容量のパーセント</returns>
        public static float GetAvailableFreeSpace(string drivePath)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drivePath.StartsWith(drive.RootDirectory.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return drive.AvailableFreeSpace / (float)drive.TotalSize;
                }
            }
            return 0;
        }

        /// <summary>
        /// 指定されたディレクトリから再帰的に空のディレクトリを削除する
        /// </summary>
        /// <param name="rootPath"></param>
        public static void DeleteEmptyDirectories(string rootPath)
        {
            bool IsDirectoryEmpty(DirectoryInfo directory)
            {
                return directory.GetFiles().Length == 0 && directory.GetDirectories().Length == 0;
            }

            DirectoryInfo directory = new DirectoryInfo(rootPath);
            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {
                DeleteEmptyDirectories(subDir.FullName);
            }
            if (IsDirectoryEmpty(directory))
            {
                DeleteAndVerify(directory);
            }
        }

        /// <summary>
        /// ファイルが指定されたディレクトリ内に存在するか
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool IsFileInDirectory(DirectoryInfo directoryInfo, FileInfo fileInfo)
        {
            var directoryFullPath = directoryInfo.FullName;

            if (false == directoryFullPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directoryFullPath += Path.DirectorySeparatorChar;
            }

            return fileInfo.FullName.StartsWith(directoryFullPath, StringComparison.OrdinalIgnoreCase) & fileInfo.Exists;
        }

        /// <summary>
        /// サブフォルダまで含めてファイルを再帰的に取得
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="validExtensions"></param>
        /// <returns></returns>
        public static List<FileInfo> GetFilesRecursively(DirectoryInfo directoryInfo, params string[] validExtensions)
        {
            var files = new List<FileInfo>();

            try
            {
                if (0 < validExtensions.Length)
                {
                    // 現在のディレクトリ内のすべてのファイルを取得
                    foreach (var file in directoryInfo.EnumerateFiles())
                    {
                        // ファイルの拡張子が指定された拡張子のいずれかと一致する場合にリストに追加
                        foreach (var extension in validExtensions)
                        {
                            if (true == file.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                            {
                                files.Add(file);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    files.AddRange(directoryInfo.EnumerateFiles());
                }

                // サブディレクトリ内のファイルを再帰的に列挙
                foreach (var subDirectory in directoryInfo.EnumerateDirectories())
                {
                    files.AddRange(FileUtilities.GetFilesRecursively(subDirectory, validExtensions));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return files;
        }
    }
}