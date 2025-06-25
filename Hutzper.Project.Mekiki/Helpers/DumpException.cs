using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hutzper.Project.Mekiki.Helpers;

public class DumpException
{
    [Flags]
    public enum MiniDumpType
    {
        Normal = 0x00000000
    }
    static public void DeleteOldDMP(int filesToKeep = 20)
    {
        Task.Run(() =>
        {
            string folderPath = "../dump/";
            Directory.CreateDirectory(folderPath);
            List<FileInfo> logFiles = Directory.GetFiles(folderPath, "*.dmp")
                                         .Select(file => new FileInfo(file))
                                         .OrderByDescending(file => file.CreationTime)
                                         .ToList();
            if (logFiles.Count > filesToKeep)
            {
                IEnumerable<FileInfo> filesToDelete = logFiles.Skip(filesToKeep);
                foreach (FileInfo file in filesToDelete) file.Delete();
            }
        });
    }
    static public void Write(Exception ex)
    {
        Serilog.Log.Warning(ex, ex.Message);

        [DllImport("DbgHelp.dll")]
        static extern bool MiniDumpWriteDump(IntPtr hProcess, int ProcessId, SafeHandle hFile, MiniDumpType DumpType, IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallbackParam);

        // ダンプファイルのパスを指定します。
        DateTime dateTime = DateTime.Now;
        string useDatetime = dateTime.ToString("yyyy-MM-dd_HH-mm-ss");
        var dumpDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "Mekiki", "dump"
                        );
        Directory.CreateDirectory(dumpDir); // フォルダがなければ作成
        string dumpFilePath = Path.Combine(dumpDir, $"{useDatetime}.dmp");

        // ダンプファイルを生成します。
        using (var fs = new FileStream(dumpFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            Process currentProcess = Process.GetCurrentProcess();
            if (MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fs.SafeFileHandle, MiniDumpType.Normal, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero))
            {
                Serilog.Log.Information($"ダンプファイルを '{dumpFilePath}' に生成しました。");
            }
        }
    }
}
