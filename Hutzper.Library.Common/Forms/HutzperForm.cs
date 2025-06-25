namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// ジェネリクス型のHutzperBaseFormを直接継承しないための、中間継承クラス
    /// </summary>
    /// <remarks>デザイナは開けません。</remarks>
    public partial class HutzperForm : HutzperBaseForm, ILoggable
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperForm() : this(null)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperForm(IServiceCollectionSharing? serviceCollectionSharing) : base(serviceCollectionSharing)
        {
            InitializeComponent();
        }

        #endregion
    }
}