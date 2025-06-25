using Hutzper.Library.Common;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.Forms;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Forms;
using Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    /// <summary>
    /// メニュー画面
    /// </summary>
    public partial class MenuForm : ServiceCollectionSharingForm
    {
        #region フィールド

        /// <summary>
        /// アプリ設定
        /// </summary>
        private readonly ProjectInspectionSetting AppConfig;

        private readonly FormsHelper FormsHelper;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MenuForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MenuForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();

            // エラー
            if (null == this.Services)
            {
                throw new Exception("services is null");
            }

            this.FormsHelper = new FormsHelper(this, this.Services);

            // 設定ファイルロード
            var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
            this.AppConfig = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
            this.AppConfig.Load();

            // バージョン表示
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var version = asm.GetName().Version;
            this.Text = $"{this.Text} ver.{version?.ToString() ?? Application.ProductVersion}";

            // 画像取得のみの場合
            if (true == (this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false))
            {
                this.buttonMenuOperation.Text = $"画像収集";
            }
            else
            {
                this.buttonMenuOperation.Text = $"検査";
            }

            #region パスの作成
            try
            {
                foreach (var path in this.AppConfig.Path)
                {
                    try
                    {
                        if (false == path.Value.Exists)
                        {
                            path.Value.Create();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion

            // PLC通信
            if (false == string.IsNullOrEmpty(this.AppConfig.PlcTcpCommunicatorParameter?.IpAddress) && 0 < this.AppConfig.PlcTcpCommunicatorParameter.PortNumber)
            {
                this.buttonMenuPlcDiagnostics.Enabled = true;
                this.buttonMenuPlcDiagnostics.BackColor = this.buttonMenuConfiguration.BackColor;
            }
            else
            {
                this.buttonMenuPlcDiagnostics.Enabled = false;
                this.buttonMenuPlcDiagnostics.BackColor = Color.DarkGray;
            }
        }

        #endregion

        /// <summary>
        /// Formイベント:画面表示
        /// </summary>
        private void MenuForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // 自動検査開始が有効な場合
                if (true == this.AppConfig.Operation.ActivateInspectionAtStartup)
                {
                    var buttons = new List<Button>(new[] { this.buttonMenuOperation, this.buttonMenuMaintenance, this.buttonMenuConfiguration, this.buttonMenuExit });

                    buttons.ForEach(b => b.Enabled = false);

                    Task.Run(() =>
                    {
                        this.InvokeSafely(() =>
                        {
                            this.buttonMenuOperation_Click(this.buttonMenuOperation, new EventArgs());

                            buttons.ForEach(b => b.Enabled = true);
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// Formイベント:FormClosing
        /// </summary>
        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 運用開始ボタンクリック
        /// </summary>
        private void buttonMenuOperation_Click(object sender, EventArgs e)
        {
            try
            {
                #region ストレージチェック
                {
                    var storageCheckParameter = new StorageStatusCheckParameter();
                    storageCheckParameter.DirectoryPath.Add(this.AppConfig.Path.Data.FullName); // データ保存先

                    var storageChecker = new StorageStatusChecker();
                    storageChecker.Initialize(storageCheckParameter);

                    var checkResult = storageChecker.Check();   // チェック実行

                    // エラーが1つでもある場合
                    if (false == checkResult.All(r => r.IsNormal))
                    {
                        var errorDrives = new List<string>();
                        checkResult.ForEach(r =>
                        {
                            // エラーの場合
                            if (false == r.IsNormal && r is StorageStatusCheckResult s)
                            {
                                // エラーとなったパスのログを出力
                                Serilog.Log.Warning($"{this}, Storage check error: path = {s.DirectoryInfo?.FullName}");

                                // ドライブ名を取得
                                if (s.DriveInfo is not null && false == errorDrives.Contains(s.DriveInfo.Name.Substring(0, 1).ToUpper()))
                                {
                                    errorDrives.Add(s.DriveInfo.Name.Substring(0, 1).ToUpper());
                                }
                            }
                        });

                        // エラーメッセージ
                        var errorMessages = new List<string>();
                        errorMessages.Add($"保存先のドライブ {string.Join(", ", errorDrives)}を認識出来ませんでした。");
                        errorMessages.Add("データの保存を行うことが出来ません。");
                        errorMessages.Add("続行しますか？");

                        // 続行するかどうかの確認ダイアログを表示
                        using var confirmDialog = this.FormsHelper.NewConfirmationForm();
                        if (DialogResult.Yes != confirmDialog.ShowWarning(this, MessageBoxButtons.YesNo, "保存先の確認", $"{string.Join(Environment.NewLine, errorMessages)}"))
                        {
                            return;
                        }

                        Serilog.Log.Warning($"{this}, storage check result is abnormal, but operation start was selected.");
                    }
                    else
                    {
                        // 空き容量チェック
                        var storageSpaceChecker = new StorageSpaceChecker();
                        storageSpaceChecker.AddTargetPath(this.AppConfig.Path.Data.FullName);
                        storageSpaceChecker.SetLowerLimit(StorageSpaceChecker.LowerLimitUnit.BasisPoint, (long)(this.AppConfig.Operation.LowerLimitPercentageOfAutoDeleteImage * 100));

                        // 下限値未満の場合
                        if (true == storageSpaceChecker.IsBelowLowerLimit)
                        {
                            var drive = new DriveInfo(Path.GetPathRoot(this.AppConfig.Path.Data.FullName) ?? string.Empty);

                            if (drive.IsReady)
                            {
                                var totalSize = drive.TotalSize;
                                var freeSpace = drive.TotalFreeSpace;

                                var freeSpaceGB = freeSpace / (1024.0 * 1024 * 1024);
                                var freePercentage = (double)freeSpace / totalSize * 100;

                                // 警告メッセージ
                                var warnMessages = new List<string>();
                                warnMessages.Add($"保存先のドライブ 容量が少なくなっています");
                                warnMessages.Add($"残容量: {freeSpaceGB:F2}GB ({freePercentage:F2}%)");
                                warnMessages.Add("既存のデータは古いものから削除されます。");
                                warnMessages.Add("続行しますか？");

                                // 続行するかどうかの確認ダイアログを表示
                                using var confirmDialog = this.FormsHelper.NewConfirmationForm();
                                if (DialogResult.Yes != confirmDialog.ShowWarning(this, MessageBoxButtons.YesNo, "保存先の確認", $"{string.Join(Environment.NewLine, warnMessages)}"))
                                {
                                    return;
                                }

                                Serilog.Log.Warning($"{this}, storage remaining capacity is low, but operation start was selected. {freeSpaceGB:F2}GB ({freePercentage:F2}%)");
                            }
                            else
                            {
                                throw new Exception("drive is not ready");
                            }
                        }
                    }
                }
                #endregion

                using var dialog = this.Services?.ServiceProvider?.GetRequiredService<OperationForm>();

                Serilog.Log.Information($"{this}, operation begin");

                dialog?.ShowDialog(this);

                Serilog.Log.Information($"{this}, operation end");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// メンテナンス画面呼び出しボタンクリック
        /// </summary>
        private void buttonMenuMaintenance_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = this.Services?.ServiceProvider?.GetRequiredService<MaintenanceForm>();

                Serilog.Log.Information($"{this}, maintenance begin");

                dialog?.ShowDialog(this);

                Serilog.Log.Information($"{this}, maintenance end");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// PLC通信診断画面呼び出しボタンクリック
        /// </summary>
        private void buttonMenuPlcDiagnostics_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = this.Services?.ServiceProvider?.GetRequiredService<PlcDiagnosticsForm>();

                Serilog.Log.Information($"{this}, plc diagnostics begin");

                dialog?.ShowDialog(this);

                Serilog.Log.Information($"{this}, plc diagnostics end");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定画面呼び出しボタンクリック
        /// </summary>
        private void buttonMenuConfiguration_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = this.Services?.ServiceProvider?.GetRequiredService<ConfigurationForm>();

                Serilog.Log.Information($"{this}, configuration begin");

                dialog?.ShowDialog(this);

                Serilog.Log.Information($"{this}, configuration end");

                #region 変更チェック
                {
                    // 設定ファイルロード
                    var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
                    var tempConfig = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
                    tempConfig.Load();

                    if (true == (tempConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false))
                    {
                        this.buttonMenuOperation.Text = $"画像収集";
                    }
                    else
                    {
                        this.buttonMenuOperation.Text = $"検査";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 終了ボタンクリック
        /// </summary>
        private void buttonMenuExit_Click(object sender, EventArgs e)
        {
            using var confirmDialog = this.FormsHelper.NewConfirmationForm();
            if (DialogResult.OK == confirmDialog.ShowQuestion(this, MessageBoxButtons.OKCancel, "終了確認", "終了しますか?"))
            {
                this.Close();
            }
        }
    }
}
