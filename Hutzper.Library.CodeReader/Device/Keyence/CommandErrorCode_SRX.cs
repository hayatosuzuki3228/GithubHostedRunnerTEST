using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.CodeReader.Device.Keyence
{
    /// <summary>
    /// SRX コマンドエラーコード
    /// </summary>
    [Serializable]
    public enum CommandErrorCode_SRX
    {
        [AliasName("未定義コマンドの受信")]
        Error00 = 0,

        [AliasName("コマンドのフォーマットが異なる(パラメータの数が不正)")]
        Error01 = 1,

        [AliasName("パラメータ1の設定範囲を超えている")]
        Error02 = 2,

        [AliasName("パラメータ2の設定範囲を超えている")]
        Error03 = 3,

        [AliasName("パラメータ2がHEX（16進）コードで指定されていない")]
        Error04 = 4,

        [AliasName("パラメータ2がHEX（16進）コードだが、設定範囲を超えている")]
        Error05 = 5,

        [AliasName("プリセットデータ内に、!が2個以上存在する(プリセットデータ不正)")]
        Error10 = 10,

        [AliasName("エリア指定データ不正")]
        Error11 = 11,

        [AliasName("指定ファイルが存在しない")]
        Error12 = 12,

        [AliasName("%Tmm-LON,bbコマンドのmmの設定範囲が超えている")]
        Error13 = 13,

        [AliasName("%Tmm-KEYENCEコマンドで通信確認できない")]
        Error14 = 14,

        [AliasName("現在の状態では実行が許可されないコマンド(実行エラー)")]
        Error20 = 20,

        [AliasName("バッファオーバー中のため、コマンドを実行できない")]
        Error21 = 21,

        [AliasName("パラメータのロード、セーブ中にエラーが発生したため\r\nコマンドを実行できない")]
        Error22 = 22,

        [AliasName("AutoID Network Navigatorと接続中のため、コマンドが受け付けられない")]
        Error23 = 23,

        [AliasName("SR-Xシリーズの異常が考えられます。当社までご連絡ください。")]
        Error99 = 99,
    }
}