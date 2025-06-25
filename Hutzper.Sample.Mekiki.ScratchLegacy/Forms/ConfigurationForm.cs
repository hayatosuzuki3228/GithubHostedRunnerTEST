using Hutzper.Library.Common;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Forms;
using Hutzper.Library.Forms.ImageView;
using Hutzper.Sample.Mekiki.ScratchLegacy.IO.Configuration;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms
{
    /// <summary>
    /// システム設定画面
    /// </summary>
    public partial class ConfigurationForm : ServiceCollectionSharingForm
    {
        #region フィールド

        /// <summary>
        /// アプリ設定
        /// </summary>
        private readonly ProjectInspectionSetting AppConfig;

        /// <summary>
        /// Formヘルパ
        /// </summary>
        private readonly FormsHelper FormsHelper;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigurationForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigurationForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
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

            // マルチ画像レイアウト
            foreach (MultiImageLayoutType layoutType in Enum.GetValues(typeof(MultiImageLayoutType)))
            {
                this.ucMultiImageLayoutType.Items.Add(layoutType.DescriptionOf());
            }

            // 統計情報リセットタイミング
            foreach (StatisticsResetTiming timing in Enum.GetValues(typeof(StatisticsResetTiming)))
            {
                this.ucStatisticsResetTiming.Items.Add(timing.DescriptionOf());
            }
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// 画面表示
        /// </summary>
        private void ConfigurationForm_Shown(object sender, EventArgs e)
        {
            try
            {
                // 運用設定
                this.ucOnOffOperationActivateInspectionAtStartup.Value = this.AppConfig.Operation.ActivateInspectionAtStartup;
                this.ucOperationImageSavingIntervalSec.ValueDouble = this.AppConfig.Operation.ImageSavingIntervalSeconds;
                this.ucMultiImageLayoutType.SelectedItem = this.AppConfig.GuiOperation.MultiImageLayoutType.DescriptionOf();
                this.ucStatisticsResetTiming.SelectedItem = this.AppConfig.GuiOperation.StatisticsResetTiming.DescriptionOf();

                // Insight連携設定
                this.ucOnOffInsightUse.Value = this.AppConfig.InsightLinkageControllerParameter?.IsUse ?? false;

                // パス設定(ReadOnly)
                this.ucPathData.Value = this.AppConfig.Path.Data;
                this.ucPathLog.Value = this.AppConfig.Path.Log;
                this.ucPathTemp.Value = this.AppConfig.Path.Temp;

                // Inspection
                this.ucOnOffImageAcquisitionOnly.Value = this.AppConfig.InspectionControllerParameter?.ImageAcquisitionOnly ?? false;
                this.nudReferenceConveyingSpeedMillPerSecond.ValueDouble = this.AppConfig.InspectionControllerParameter?.ReferenceConveyingSpeedMillPerSecond ?? 0;
                this.nudSpecifiedConveyingSpeedMillPerSecond.ValueDouble = this.AppConfig.InspectionControllerParameter?.SpecifiedConveyingSpeedMillPerSecond ?? 0;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"configuration save failed");
            }
        }

        /// <summary>
        /// 保存ボタンクリック
        /// </summary>
        private void buttonSaveConfig_Click(object sender, EventArgs e)
        {
            this.updateConfiguration();
            this.saveConfiguration();
        }

        /// <summary>
        /// 終了ボタンクリック
        /// </summary>
        private void buttonExit_Click(object sender, EventArgs e)
        {
            try
            {
                this.updateConfiguration();

                // 設定ファイルロード
                var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();
                var previousSetting = new ProjectInspectionSetting(this.Services, appConfigFile.FullName);
                previousSetting.Load();

                // 変更がある場合
                if (false == this.AppConfig.Equals(previousSetting))
                {
                    using var confirmDialog = this.FormsHelper.NewConfirmationForm();

                    var dialogResult = confirmDialog.ShowQuestion(this, MessageBoxButtons.YesNoCancel, "終了確認", "変更があります。\n保存してから終了しますか?");

                    if (DialogResult.Cancel == dialogResult)
                    {
                        return;
                    }
                    else
                    {
                        if (DialogResult.Yes == dialogResult)
                        {
                            this.saveConfiguration();
                        }

                        this.Close();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 統計情報リセットボタン:クリックイベント
        /// </summary>
        private void buttonStatisticsManualReset_Click(object sender, EventArgs e)
        {
            try
            {
                using var confirmDialog = this.FormsHelper.NewConfirmationForm();

                var statisticsFile = new FileInfo(Path.Combine(this.AppConfig.Path.Data.FullName, $"statistics.json"));
                if (false == statisticsFile.Exists)
                {
                    confirmDialog.ShowInfo(this, MessageBoxButtons.OK, "リセット確認", $"検査結果データはありません");
                    return;
                }

                if (DialogResult.OK != confirmDialog.ShowQuestion(this, MessageBoxButtons.OKCancel, "リセット確認", $"保持している検査結果データをリセットします。{Environment.NewLine}よろしいですか?"))
                {
                    return;
                }

                statisticsFile.Delete();
                Serilog.Log.Information($"{this}, inspection result data has been reset(manual).");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"failed to reset inspection result data due to an exception.");
            }
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// 設定更新
        /// </summary>
        private void updateConfiguration()
        {
            try
            {
                // 運用設定
                this.AppConfig.Operation.ActivateInspectionAtStartup = this.ucOnOffOperationActivateInspectionAtStartup.Value;
                this.AppConfig.Operation.ImageSavingIntervalSeconds = this.ucOperationImageSavingIntervalSec.ValueDouble;
                if (0 == this.AppConfig.Operation.ImageSavingIntervalSeconds)
                {
                    this.AppConfig.Operation.ImageSavingIntervalSeconds = (int)0;
                }

                // マルチ画像レイアウト
                foreach (MultiImageLayoutType layoutType in Enum.GetValues(typeof(MultiImageLayoutType)))
                {
                    if (this.ucMultiImageLayoutType.SelectedItem?.ToString() == layoutType.DescriptionOf())
                    {
                        this.AppConfig.GuiOperation.MultiImageLayoutType = layoutType;
                    }
                }

                // 統計情報リセットタイミング
                foreach (StatisticsResetTiming timing in Enum.GetValues(typeof(StatisticsResetTiming)))
                {
                    if (this.ucStatisticsResetTiming.SelectedItem?.ToString() == timing.DescriptionOf())
                    {
                        this.AppConfig.GuiOperation.StatisticsResetTiming = timing;
                    }
                }

                // 画像保存条件を更新する
                if (0 == this.AppConfig.Operation.ImageSavingIntervalSeconds)
                {
                    this.AppConfig.Operation.ImageSavingCondition = ImageSavingCondition.Always;
                }
                else if (0 < this.AppConfig.Operation.ImageSavingIntervalSeconds)
                {
                    this.AppConfig.Operation.ImageSavingCondition = ImageSavingCondition.SpecificInterval;
                }
                else
                {
                    this.AppConfig.Operation.ImageSavingCondition = ImageSavingCondition.None;
                }

                // Insight連携設定
                if (this.AppConfig.InsightLinkageControllerParameter is not null)
                {
                    this.AppConfig.InsightLinkageControllerParameter.IsUse = this.ucOnOffInsightUse.Value;
                }

                // Inspection
                if (this.AppConfig.InspectionControllerParameter is not null)
                {
                    this.AppConfig.InspectionControllerParameter.ImageAcquisitionOnly = this.ucOnOffImageAcquisitionOnly.Value;
                    this.AppConfig.InspectionControllerParameter.ReferenceConveyingSpeedMillPerSecond = this.nudReferenceConveyingSpeedMillPerSecond.ValueDouble;
                    this.AppConfig.InspectionControllerParameter.SpecifiedConveyingSpeedMillPerSecond = this.nudSpecifiedConveyingSpeedMillPerSecond.ValueDouble;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定保存
        /// </summary>
        private void saveConfiguration()
        {
            try
            {
                // 保存
                this.AppConfig.Save();

                Serilog.Log.Information($"{this}, configuration save succeed");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, $"configuration save failed");
            }
        }

        #endregion
    }
}
