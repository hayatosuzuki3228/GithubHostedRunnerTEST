using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.Data;
using System.Drawing;
using Point = Hutzper.Library.Common.Drawing.Point;

namespace Hutzper.Library.ImageProcessing.Controller
{
    /// <summary>
    /// Run Length Encoding インタフェース
    /// </summary>
    public interface IRleController : IController
    {
        #region プロパティ

        /// <summary>
        /// 最後に発生したエラー
        /// </summary>
        public RleErrorCode LastError { get; }

        /// <summary>
        /// 算出された画像の高さ
        /// </summary>
        public int ImageHeight { get; }

        /// <summary>
        /// 算出された画像の幅
        /// </summary>
        public int ImageWidth { get; }

        /// <summary>
        /// RLEデータ
        /// </summary>
        public IRleData[] RleData { get; }

        /// <summary>
        /// RLEライン情報
        /// </summary>    
        public IRleLineInfo[] RleLines { get; }

        #endregion

        #region パブリックメソッド

        #region Rleデータ作成 with ラベリング

        /// <summary>
        /// Rleデータ作成開始
        /// </summary>
        /// <param name="doAsynchronousLabeling">非同期ラベリングを実行するかどうか</param>
        /// <returns></returns>
        /// <remarks>ランレングスデータ作成前に呼び出します</remarks>
        public bool StartRleConcatenationWithLabeling(bool doAsynchronousLabeling);

        /// <summary>
        /// ビットマップを2値化してRleデータを追加で作成する
        /// </summary>
        /// <remarks>対応ビットマップのピクセルフォーマットはPixelFormat.Format8bppIndexed</remarks>
        public bool ConcatRleFrom(Bitmap bitmap, int threshold);

        /// <summary>
        /// ビットマップを2値化してRleデータを追加で作成する
        /// </summary>
        /// <remarks>対応ビットマップのピクセルフォーマットはPixelFormat.Format8bppIndexed</remarks>
        public bool ConcatRleFrom(Bitmap bitmap, int thresholdMin, int thresholdMax);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを追加で作成する
        /// </summary>
        public bool ConcatRleFrom(byte[][] bitmap, int threshold);


        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを追加で作成する
        /// </summary>
        public bool ConcatRleFrom(byte[][] bitmap, int thresholdMin, int thresholdMax);

        /// <summary>
        /// 非同期ラベリングの終了
        /// </summary>
        /// <returns></returns>
        /// <remarks>全Rleデータ作成後に呼び出します</remarks>
        public bool EndRleConcatenationWithLabeling(bool doForceStopLabeling = false);

        #endregion

        #region 同期ラベリング用メソッド

        /// <summary>
        /// ビットマップを2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(Bitmap bitmap, int threshold);

        /// <summary>
        /// ビットマップを2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(Bitmap bitmap, int thresholdMin, int thresholdMax);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[][] bitmap, int threshold);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[][] bitmap, int thresholdMin, int thresholdMax);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[] bitmap, int stride, int threshold);

        /// <summary>
        /// 8bitグレイ値を2値化してRleデータを新規で作成する
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="thresholdMin"></param>
        /// <param name="thresholdMax"></param>
        /// <returns></returns>
        public bool CreateNewRleFrom(byte[] bitmap, int stride, int thresholdMin, int thresholdMax);

        /// <summary>
        /// 同期ラベリング
        /// </summary>
        /// <returns></returns>
        /// <remarks>CreateNewRleFromメソッドの後に呼び出します</remarks>
        public bool SynchronousLabeling();


        #endregion

        #region ラベル処理

        /// <summary>
        /// すべてのラベル収集
        /// </summary>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectAllLabels();

        /// <summary>
        /// 有効ラベル収集
        /// </summary>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectValidLabels(bool checkIncluding);

        /// <summary>
        /// 有効ラベル収集
        /// </summary>
        /// <param name="selector">有効判定</param>
        /// <remarks>ラベリング後に呼び出します</remarks>
        public List<IRleLabel> CollectValidLabels(bool checkIncluding, Func<IRleLabel, bool> selector);

        /// <summary>
        /// グレイ値情報取得
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="labels">グレイ値を取得するラベルデータリスト</param>
        /// <param name="minimum">最小グレイ値</param>
        /// <param name="maximum">最大グレイ値</param>
        /// <param name="average">平均値</param>
        public void GetLabelGrayValue(Bitmap bitmap, List<IRleLabel> labels, out double[] minimum, out double[] maximum, out double[] average);

        /// <summary>
        /// ラベルのピクセル位置を取得します。
        /// </summary>
        public void GetLabelPoints(IRleLabel selectedLabel, out List<Point> points);

        /// <summary>
        /// ラベルの重心位置を取得します。
        /// </summary>
        public void GetLabelGravityCenter(IRleLabel selectedLabel, out PointD point);

        /// <summary>
        /// ラベルの開始座標(左上)を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="point"></param>
        public void GetLabelStartPoint(IRleLabel selectedLabel, out Point point);

        /// <summary>
        /// ラベル番号が一致するかどうか
        /// </summary>
        /// <param name="labelNo"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool EqualsLabelIndex(int selectedLabelNo, Point point);

        /// <summary>
        /// 指定座標のラベル番号を取得する
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int IsLabelIndex(Point point);

        /// <summary>
        /// 指定座標のラベル番号を取得する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int IsLabelIndex(int x, int y);

        /// <summary>
        /// ラベルの円形度を取得する(コンパクト性)
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        /// <remarks>円形度 = 4π * S/L^2 (S = 面積, L = 図形の周囲長)</remarks>
        public double GetLabelCompactness(IRleLabel selectedLabel);

        /// <summary>
        /// ラベルの円形度を取得する(コンパクト性)
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contlength"></param>
        /// <returns></returns>
        /// <remarks>円形度 = 4π * S/L^2 (S = 面積, L = 図形の周囲長)</remarks>
        public double GetLabelCompactness(IRleLabel selectedLabel, out double contlength);

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel);

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel, PointD gravityCenter);

        /// <summary>
        /// ラベルの真円度を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelCircularity(IRleLabel selectedLabel, PointD gravityCenter, List<ContourElementData> contours);

        /// <summary>
        /// ラベルの周囲長を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <returns></returns>
        public double GetLabelContlength(IRleLabel selectedLabel);

        /// <summary>
        /// ラベルの輪郭を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contours"></param>
        /// <param name="contourCenter"></param>
        public void GetLabelContour(IRleLabel selectedLabel, out List<ContourElementData> contours, out PointD contourCenter);

        /// <summary>
        /// ラベルの輪郭を取得する
        /// </summary>
        /// <param name="selectedLabel"></param>
        /// <param name="contours"></param>
        /// <param name="contourCenter"></param>
        public void GetLabelContour(IRleLabel selectedLabel, out List<ContourElementData> contours, out PointD contourCenter, out double contlength);

        #endregion

        public Bitmap? ToBitmap();

        #endregion
    }
}