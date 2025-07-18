﻿namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// リモートデバイスパラメータ
    /// </summary>
    public interface IDigitalIORemoteDeviceParameter : IDigitalIODeviceParameter
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// ポート番号
        /// </summary>
        public int PortNumber { get; set; }

        /// <summary>
        /// 再接続
        /// </summary>
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public int ReconnectionAttemptsIntervalSec { get; set; }
    }
}