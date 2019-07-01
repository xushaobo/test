using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace HT_Tools2
{
    public class PFunc
    {
        /// <summary>
        ///     获取MAC地址(返回第一个物理以太网卡的mac地址)
        /// </summary>
        /// <returns>成功返回mac地址，失败返回null</returns>
        public static string GetMacAddress()
        {
            string macAddress = null;
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var adapter in nics)
                    if (adapter.NetworkInterfaceType.ToString().Equals("Ethernet")) //是以太网卡
                    {
                        var fRegistryKey =
                            "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" +
                            adapter.Id + "\\Connection";
                        var rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        if (rk != null)
                        {
                            // 区分 PnpInstanceID     
                            // 如果前面有 PCI 就是本机的真实网卡    
                            // MediaSubType 为 01 则是常见网卡，02为无线网卡。    
                            var fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                            var fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                            if (fPnpInstanceID.Length > 3 && fPnpInstanceID.Substring(0, 3) == "PCI") //是物理网卡
                            {
                                macAddress = adapter.GetPhysicalAddress().ToString();
                                break;
                            }

                            if (fMediaSubType == 1) //虚拟网卡
                            {
                            }
                            else if (fMediaSubType == 2) //无线网卡(上面判断Ethernet时已经排除了)
                            {
                            }
                        }
                    }
            }
            catch
            {
                macAddress = null;
            }

            return macAddress;
        }

        public static string GetComputerName()
        {
            return Environment.MachineName;
        }

        public static string GetCheckStr()
        {
            var temp = GetComputerName() + GetMacAddress();

            return EncryptWithMd5(temp.Replace(" ", "")).ToUpper();
        }

        public static string EncryptWithMd5(string source)
        {
            var sor = Encoding.UTF8.GetBytes(source);
            var md5 = MD5.Create();
            var result = md5.ComputeHash(sor);
            var strbul = new StringBuilder(40);
            for (var i = 0; i < result.Length; i++)
                strbul.Append(result[i].ToString("x3")); //加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            return strbul.ToString();
        }

        //加密
        public static string SetKey(string str)
        {
            try
            {
                var param = new CspParameters {KeyContainerName = "Eycf84jd++--"};
                using (var rsa = new RSACryptoServiceProvider(param))
                {
                    var plaindata = Encoding.Default.GetBytes(str);

                    var encryptdata = rsa.Encrypt(plaindata, false);
                    return Convert.ToBase64String(encryptdata);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //解密
        public static string GetKey(string str)
        {
            try
            {
                var param = new CspParameters {KeyContainerName = "Eycf84jd++--"};
                using (var rsa = new RSACryptoServiceProvider(param))
                {
                    var encryptdata = Convert.FromBase64String(str);
                    var decryptdata = rsa.Decrypt(encryptdata, false);
                    return Encoding.Default.GetString(decryptdata);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}