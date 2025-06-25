using Hutzper.Library.Forms;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    /// <summary>
    /// 検査結果と統計情報表示コントロール
    /// </summary>
    public partial class ResultAndStatisticsDisplay : HutzperUserControl, IResultAndStatisticsDisplay
    {
        #region サブクラス

        /// <summary>
        /// 行の種類
        /// </summary>
        [Serializable]
        [Flags]
        public enum RowKind : long
        {
            Disable = 0,
            Data = 1L << 1,
            Invalid = 1L << 2,
        }

        /// <summary>
        /// 列の種類
        /// </summary>
        [Serializable]
        [Flags]
        public enum ColumnKind : long
        {
            Disable = 0,
            Name = 1L << 0,
            Count = 1L << 1,
        }

        /// <summary>
        /// セル種類
        /// </summary>
        public class CellKind
        {
            public RowKind Row { get; set; }

            public ColumnKind Column { get; set; }

            public CellKind(RowKind row, ColumnKind col)
            {
                this.Row = row;
                this.Column = col;
            }
        }

        /// <summary>
        /// カウントデータ
        /// </summary>
        public class CountData
        {
            public int Value { get; set; }
            public bool IsEmphasized { get; set; }

            public void Initialize()
            {
                this.Value = 0;
                this.IsEmphasized = false;
            }
        }

        #endregion

        #region IResultAndStatisticsDisplay

        /// <summary>
        /// 最後の検査日時
        /// </summary>
        /// <remarks>最後にAddResultに与えたIInspectionResult.DateTime</remarks>
        public DateTime? LatestDateTime { get; private set; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="parameter">初期化するための推論結果判定パラメータ</param>
        public void Initialize(IInferenceResultJudgmentParameter parameter)
        {
            try
            {
                var allClassNames = parameter.AllClassNames.ToArray();
                var classNamesWithoutOk = allClassNames.Where(word => !string.Equals(word, "ok", StringComparison.OrdinalIgnoreCase)).ToArray();

                // 列数とサイズ
                var colums = new List<int>();
                colums.Add(240);
                colums.Add(this.ClientSize.Width - colums.Last() - 2);
                var rowHeight = 48;

                this.OverallStatistics = new()
                {
                    { "total", new CountData() },
                    { "ok", new CountData() },
                    { "ng", new CountData() }
                };
                //this.OverallStatistics.Add("err", new CountData());

                #region 統計表(total,ok,ng,err)
                {
                    this.UcOverallResultTable.RowNumber = this.OverallStatistics.Count;
                    this.UcOverallResultTable.RowHeight = rowHeight;
                    this.UcOverallResultTable.ColumnWidthStrings = TableViewRendererUserControl.ConcatenateByLineBreakString(colums);
                    this.UcOverallResultTable.BorderColor = Color.Black;
                    this.UcOverallResultTable.Size = this.UcOverallResultTable.ViewSize;

                    foreach (var r in this.UcOverallResultTable.Cells)
                    {
                        foreach (var c in r)
                        {
                            c.FontSize = this.Font.Size;
                            switch (c.Index.X)
                            {
                                case 0:
                                    {
                                        var data = this.OverallStatistics.ElementAt(c.Index.Y);

                                        c.Text = this.CapitalizeFirstLetter(data.Key);
                                        c.TextAlign = ContentAlignment.MiddleRight;
                                        c.BackColor = SystemColors.ControlLight;
                                        c.Tag = new CellKind(RowKind.Data, ColumnKind.Name);
                                    }
                                    break;

                                case 1:
                                    {
                                        c.Text = "0";
                                        c.TextAlign = ContentAlignment.MiddleRight;
                                        c.Tag = new CellKind(RowKind.Data, ColumnKind.Count);
                                    }
                                    break;
                            }
                        }
                    }
                }
                #endregion

                this.NgDetailsStatistics = new();
                foreach (var name in classNamesWithoutOk)
                {
                    this.NgDetailsStatistics.Add(name, new CountData());
                }

                #region 統計表(NG詳細)
                {
                    this.UcNgDetailsTable.RowNumber = classNamesWithoutOk.Length;
                    this.UcNgDetailsTable.RowHeight = Math.Min(rowHeight, this.UcNgDetailsTable.Height / this.UcNgDetailsTable.RowNumber);
                    this.UcNgDetailsTable.ColumnWidthStrings = TableViewRendererUserControl.ConcatenateByLineBreakString(colums);
                    this.UcNgDetailsTable.BorderColor = this.UcOverallResultTable.BorderColor;
                    this.UcNgDetailsTable.Size = this.UcNgDetailsTable.ViewSize;

                    foreach (var r in this.UcNgDetailsTable.Cells)
                    {
                        foreach (var c in r)
                        {
                            c.FontSize = this.Font.Size;

                            switch (c.Index.X)
                            {
                                case 0:
                                    {
                                        var data = this.NgDetailsStatistics.ElementAt(c.Index.Y);

                                        c.Text = data.Key;
                                        c.TextAlign = ContentAlignment.MiddleRight;
                                        c.BackColor = SystemColors.ControlLight;
                                        c.Tag = new CellKind(RowKind.Data, ColumnKind.Name);
                                    }
                                    break;

                                case 1:
                                    {
                                        c.Text = "0";
                                        c.TextAlign = ContentAlignment.MiddleRight;
                                        c.Tag = new CellKind(RowKind.Data, ColumnKind.Count);
                                    }
                                    break;
                            }
                        }
                    }
                }
                #endregion

                this.ClearResults();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 結果追加
        /// </summary>
        /// <returns>直前の検査結果日時(日付比較用)</returns>
        public DateTime? AddResult(IInspectionResult result)
        {
            var previousDateTime = this.LatestDateTime;

            try
            {
                // 前回の強調を解除
                this.OverallStatistics.Values.ToList().ForEach(v => v.IsEmphasized = false);
                this.NgDetailsStatistics.Values.ToList().ForEach(v => v.IsEmphasized = false);

                // 最新の日時を更新
                this.LatestDateTime = result.DateTime;

                // 総数を更新
                this.OverallStatistics["total"].Value++;

                // 判定日時を表示
                this.LabelJudgmentDateTime.Text = $"{result.DateTime:yyyy/MM/dd HH:mm:ss}";

                // Ok判定の場合
                if (result.JudgementText.ToLower().Equals("ok"))
                {
                    this.LabelJudgmentResult.Text = $"OK";
                    this.LabelJudgmentResult.ForeColor = Color.White;
                    this.LabelJudgmentResult.BackColor = Color.Blue;

                    this.OverallStatistics["ok"].Value++;
                    this.OverallStatistics["ok"].IsEmphasized = true;
                }
                // NG判定の場合
                else
                {
                    this.LabelJudgmentResult.Text = $"NG";
                    this.LabelJudgmentResult.ForeColor = Color.White;
                    this.LabelJudgmentResult.BackColor = Color.Red;

                    this.OverallStatistics["ng"].Value++;
                    this.OverallStatistics["ng"].IsEmphasized = true;

                    // NG詳細を更新
                    this.NgDetailsStatistics[result.JudgementText].Value++;
                    this.NgDetailsStatistics[result.JudgementText].IsEmphasized = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UpdateStatistics();
            }

            return previousDateTime;
        }

        /// <summary>
        /// 結果クリア
        /// </summary>
        public void ClearResults()
        {
            try
            {
                // 判定結果を初期化
                this.LabelJudgmentResult.Text = $"-";
                this.LabelJudgmentResult.BackColor = Color.LightGray;
                this.LabelJudgmentDateTime.Text = $"検査日時";

                // 最新の日時を初期化
                this.LatestDateTime = null;

                // 統計データを初期化
                this.OverallStatistics.Values.ToList().ForEach(v => v.Initialize());
                this.NgDetailsStatistics.Values.ToList().ForEach(v => v.Initialize());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UpdateStatistics();
            }
        }

        /// <summary>
        /// 現在の統計データを取得
        /// </summary>
        public IStatisticsData GetStatisticsData()
        {
            var statisticsData = new StatisticsData();

            try
            {
                statisticsData.LatestDateTime = this.LatestDateTime;

                foreach (var data in this.OverallStatistics)
                {
                    statisticsData.OverallStatistics.Add(data.Key, data.Value.Value);
                }

                foreach (var data in this.NgDetailsStatistics)
                {
                    statisticsData.NgDetailsStatistics.Add(data.Key, data.Value.Value);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return statisticsData;
        }

        /// <summary>
        /// 統計データを設定して復元
        /// </summary>
        public void SetStatisticsData(IStatisticsData statisticsData)
        {
            try
            {
                this.ClearResults();
                this.LatestDateTime = statisticsData.LatestDateTime;

                foreach (var data in statisticsData.OverallStatistics)
                {
                    if (true == this.OverallStatistics.ContainsKey(data.Key))
                    {
                        this.OverallStatistics[data.Key].Value = data.Value;
                    }
                }

                foreach (var data in statisticsData.NgDetailsStatistics)
                {
                    if (true == this.NgDetailsStatistics.ContainsKey(data.Key))
                    {
                        this.NgDetailsStatistics[data.Key].Value = data.Value;
                    }
                }

                if (this.LatestDateTime is not null)
                {
                    this.LabelJudgmentDateTime.Text = $"{this.LatestDateTime:yyyy/MM/dd HH:mm:ss}";
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UpdateStatistics();
            }
        }

        #endregion

        #region フィールド

        private Dictionary<string, CountData> OverallStatistics = new();
        private Dictionary<string, CountData> NgDetailsStatistics = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResultAndStatisticsDisplay()
        {
            InitializeComponent();
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// 統計表の更新
        /// </summary>
        private void UpdateStatistics()
        {
            try
            {
                #region 統計表(total,ok,ng,err)
                foreach (var r in this.UcOverallResultTable.Cells)
                {
                    foreach (var c in r)
                    {
                        switch (c.Index.X)
                        {
                            case 1:
                                {
                                    var data = this.OverallStatistics.ElementAt(c.Index.Y).Value;

                                    c.Text = $"{data.Value}";
                                    c.BackColor = data.IsEmphasized ? ColorTranslator.FromHtml("#FFF9E6") : SystemColors.ControlLightLight;
                                }
                                break;
                        }
                    }
                }
                #endregion

                #region 統計表(NG詳細)
                foreach (var r in this.UcNgDetailsTable.Cells)
                {
                    foreach (var c in r)
                    {
                        c.FontSize = this.Font.Size;

                        switch (c.Index.X)
                        {
                            case 1:
                                {
                                    var data = this.NgDetailsStatistics.ElementAt(c.Index.Y).Value;

                                    c.Text = $"{data.Value}";
                                    c.BackColor = data.IsEmphasized ? ColorTranslator.FromHtml("#FFF9E6") : SystemColors.ControlLightLight;
                                }
                                break;
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.UcOverallResultTable.UpdateView();
                this.UcNgDetailsTable.UpdateView();
            }
        }

        /// <summary>
        /// 先頭文字を大文字に変換
        /// </summary>
        /// <param name="text">元の文字列</param>
        /// <returns>先頭が大文字に変換された文字列</returns>
        private string CapitalizeFirstLetter(string text)
        {
            if (true == string.IsNullOrEmpty(text))
            {
                return text;
            }

            return char.ToUpper(text[0]) + text.Substring(1);
        }

        #endregion
    }
}
