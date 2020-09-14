using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace EIPForm
{
    public class EIPLib
    {
        EEIPClient eEIPClient = new EEIPClient();
        List<uint> ipaddress = new List<uint>();

        public enum DataType
        {
            R,  //リレー
            L,  //補助リレー
            T,  //タイマー
            DM, //データメモリ
            EM, //拡張データメモリ
            X,  //入力信号
            Y   //出力信号

        }

        public struct EIP_Status
        {
            public int code { get; set; }
            public  string message { get; set; }
            public byte[] response { get; set; }

            public string value { get; set; }
        }

        /// <summary>
        /// EIPユニットのIPアドレスを登録します。
        /// </summary>
        public List<uint> IpAddressList
        {
            get
            {
                return ipaddress;
            }
            set
            {
                ipaddress = value;
            }
        }

        /// <summary>
        /// 親フォームを取得します。
        /// </summary>
        public EIP_Andon GetFrom { get; set; }

        /// <summary>
        /// ネットワーク内のEthernet/IPデバイスを登録します
        /// </summary>
        public List<Encapsulation.CIPIdentityItem> DeviceList { get; set; }

        /// <summary>
        /// Explicit Massaging サービスを使ってPLCからデータを呼び出します。
        /// </summary>
        /// <param name="dataareaID">更新するデータエリア番号</param>
        /// <param name="instanceid">インスタンスID</param>
        /// <param name="dataType">データ型</param>
        /// <param name="isString">入力データが文字列の場合true</param>
        /// <param name="tcpport">TCPポート</param>
        /// <param name="destination">宛先IPアドレス番号</param>
        /// <return>EIP_Status構造体を返します</return>
        public virtual EIP_Status ReadInstance(byte instanceid, DataType dataType, bool isString, ushort tcpport = 44818, int destination = 0)
        {
            EIP_Status status = new EIP_Status();
            status.code = 0;
            status.message = "";

            try
            {
                //IPアドレスはフォームで選択したものを使用
                //TCPポートは特に変更していない場合はこれで固定 EEIP.dll内の既定値
                eEIPClient.TCPPort = tcpport;
                eEIPClient.IPAddress = Encapsulation.CIPIdentityItem.getIPAddress(IpAddressList[destination]);
                eEIPClient.RegisterSession();

                //CIPメッセージ Assembly(0x04)を使って引数のInstanceIDのIOの値を読み出す
                //戻り値のバッファサイズはPLC側で設定したタグのサイズとなっているので一定ではない
                byte[] response = eEIPClient.GetAttributeSingle(0x04, instanceid, 0x03);
                string responseString = "";

                //送られてきたデータが数値の場合
                //HEX→DEC変換して格納
                if ((dataType == DataType.DM || dataType == DataType.EM) && !isString)
                {
                    int n = 0;
                    int tmpvalue = 0;
                    foreach (byte resAddress in response)
                    {
                        tmpvalue += resAddress * (int)Math.Pow(16, 2 * n);
                        n++;
                    }

                    responseString = tmpvalue.ToString();
                    status.value = responseString;

                    //IDによって書き込みエリアを振り分け
                    //とりあえず殺しとく
                    //GetFrom.Invoke(new EIP_Andon.Form1Update(() => GetFrom.Form1_DataAreaUpdate(responseString + "\n", dataareaID)));
                }

                //送られてきたデータが文字列の場合
                //リトルエンディアンのため上位バイトは i + 1 に格納されている
                if ((dataType == DataType.DM || dataType == DataType.EM) && isString)
                {

                    for (int i = 0; i < response.Length - 1; i++)
                    {
                        if (response[i] != 0x32)
                        {
                            responseString += (char)response[i + 1];
                            responseString += (char)response[i];
                            i++;
                        }
                        else break;
                    }

                    status.value = responseString;
                    //とりあえず殺しておく
                    //GetFrom.Invoke(new EIP_Andon.Form1Update(() => GetFrom.Form1_DataAreaUpdate(responseString + "\n", dataareaID)));
                }

                //ビット形式はとりあえず何もしない
                if (dataType == DataType.R || dataType == DataType.L || dataType == DataType.X || dataType == DataType.Y)
                {
                    
                }

                //戻り値を作る
                status.code = 0;
                status.response = response;

                eEIPClient.UnRegisterSession();

            }
            catch (Exception e)
            {
                status.code = -1;
                status.message = e.Message;
                status.response = null;
            }

            return status;
        }
        
        public virtual void WriteInstance(byte instanceid, DataType dataType, byte[] sendData, bool isString, ushort tcpport = 44818)
        {
            try
            {
                //IPアドレスはフォームで選択したものを使用
                //TCPポートは特に変更していない場合はこれで固定 EEIP.dll内の既定値
                eEIPClient.TCPPort = tcpport;
                eEIPClient.IPAddress = Encapsulation.CIPIdentityItem.getIPAddress(IpAddressList[0]);
                eEIPClient.RegisterSession();

                //CIPメッセージ Assembly(0x04)を使って引数のInstanceIDのIOの値を読み出す
                //戻り値のバッファサイズはPLC側で設定したタグのサイズとなっているので一定ではない

                byte[] code = eEIPClient.SetAttributeSingle(0x04, instanceid, 0xFFFF, sendData);
                if(code!=null)
                {
                    MessageBox.Show(code.ToString());
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                eEIPClient.UnRegisterSession();
            }
        }


        /// <summary>
        /// ネットワーク上にあるEthernet/IPデバイスの情報を取得します。
        /// </summary>
        /// <returns>デバイス名</returns>
        public string[] SearchDevice()
        {
            string[] dev = new string[128];
            int i = 0;

            //実際のCIPIdentityItem情報はDeviceListに格納される
            DeviceList = eEIPClient.ListIdentity();
            foreach(Encapsulation.CIPIdentityItem device in DeviceList)
            {
                dev[i] = Encapsulation.CIPIdentityItem.getIPAddress(device.SocketAddress.SIN_Address) + "\t" + device.ProductName1;
                IpAddressList.Add(device.SocketAddress.SIN_Address);
                i++;
            }

            //戻り値はコンボボックス用
            return dev;
        }

        //フォームをクローズする際に呼び出す処理
        public void Close()
        {
            //eEIPClient.UnRegisterSession();
        }

    }
}
