using System;
using System.Text;
using System.Security.Cryptography;

namespace SimCaptcha.Common
{
    /// <summary>
    /// AES加解密
    /// </summary>
    public class AesHelper
    {
        /// <summary>
        /// AES加解密
        /// </summary>
        private const string ALGORITHM = "AES";

        /// <summary>
        /// 默认的初始化向量值
        /// </summary>
        private const string IV_DEFAULT = "g8v20drvOmIx2PuR";

        /// <summary>
        /// 默认加密的KEY
        /// </summary>
        private const string KEY_DEFAULT = "8G5M4Ff9hel8fUA9";

        #region 其它填充模式(但是php目前所知填充模式只有ZeroPadding，于是其他语言就只能跟着它来了)
        /// <summary>
        /// 工作模式：CBC
        /// </summary>
        private const PaddingMode TRANSFORM_CBC_PKCS5 = PaddingMode.PKCS7;

        /// <summary>
        /// 工作模式：ECB
        /// </summary>
        private const PaddingMode TRANSFORM_ECB_PKCS5 = PaddingMode.PKCS7;

        /// <summary>
        /// 工作模式：CBC
        /// </summary>
        private const PaddingMode TRANSFORM_CBC_ZEROS = PaddingMode.Zeros;

        /// <summary>
        /// 工作模式：ECB
        /// </summary>
        private const PaddingMode TRANSFORM_ECB_ZEROS = PaddingMode.Zeros;
        #endregion

        /// <summary>
        /// 工作模式：CBC
        /// </summary>
        private const PaddingMode TRANSFORM_CBC = PaddingMode.Zeros;

        /// <summary>
        /// 工作模式：ECB
        /// </summary>
        private const PaddingMode TRANSFORM_ECB = PaddingMode.Zeros;

        #region 基于CBC工作模式的AES加密
        /// <summary>
        /// 基于CBC工作模式的AES加密
        /// </summary>
        /// <param name="toEncrypt">待加密字符串</param>
        /// <param name="key">秘钥，如果不填则使用默认值</param>
        /// <param name="iv">初始化向量值，如果不填则使用默认值</param>
        /// <returns></returns>
        [Obsolete("SimCaptcha 加解密默认全使用ECB模式, 请注意", true)]
        public static string EncryptCbcMode(string toEncrypt, string key = KEY_DEFAULT, string iv = IV_DEFAULT)
        {
            if (!string.IsNullOrEmpty(toEncrypt))
            {
                // 如果key或iv为空，则使用默认值
                if (key == null || key.Length != 16)
                {
                    key = KEY_DEFAULT;
                }
                if (iv == null || iv.Length != 16)
                {
                    iv = IV_DEFAULT;
                }

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                try
                {
                    RijndaelManaged rDel = new RijndaelManaged
                    {
                        Key = keyArray,
                        IV = ivArray,
                        // 加密模式
                        Mode = CipherMode.CBC,
                        Padding = TRANSFORM_CBC
                    };
                    ICryptoTransform cTransform = rDel.CreateEncryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    // 然后转成BASE64返回
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("基于CBC工作模式的AES加密失败,toEncrypt:{0},key:{1}", toEncrypt, key));
                    throw ex;
                }
            }

            return null;
        }
        #endregion

        #region 基于CBC工作模式的AES解密
        /// <summary>
        /// 基于CBC工作模式的AES解密
        /// </summary>
        /// <param name="toDecrypt">AES加密之后的字符串</param>
        /// <param name="key">秘钥，如果不填则使用默认值</param>
        /// <param name="iv">初始化向量值，如果不填则使用默认值</param>
        /// <returns></returns>
        [Obsolete("SimCaptcha 加解密默认全使用ECB模式, 请注意", true)]
        public static string DecryptCbcMode(string toDecrypt, string key = KEY_DEFAULT, string iv = IV_DEFAULT)
        {
            if (!string.IsNullOrEmpty(toDecrypt))
            {
                // 如果key或iv为空，则使用默认值
                if (key == null || key.Length != 16)
                {
                    key = KEY_DEFAULT;
                }
                if (iv == null || iv.Length != 16)
                {
                    iv = IV_DEFAULT;
                }

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] ivArray = UTF8Encoding.UTF8.GetBytes(iv);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                try
                {
                    RijndaelManaged rDel = new RijndaelManaged
                    {
                        Key = keyArray,
                        IV = ivArray,
                        Mode = CipherMode.CBC,
                        Padding = TRANSFORM_CBC
                    };
                    ICryptoTransform cTransform = rDel.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    return UTF8Encoding.UTF8.GetString(resultArray);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("基于CBC工作模式的AES解密失败,toDecrypt:{0},key:{1}", toDecrypt, key));
                    throw ex;
                }
            }

            return null;
        }
        #endregion

        #region 基于ECB工作模式的AES加密
        /// <summary>
        /// 基于ECB工作模式的AES加密
        /// </summary>
        /// <param name="toEncrypt">待加密字符串</param>
        /// <param name="key">秘钥，如果不填则使用默认值</param>
        /// <returns></returns>
        public static string EncryptEcbMode(string toEncrypt, string key = KEY_DEFAULT)
        {
            if (!string.IsNullOrEmpty(toEncrypt))
            {
                // 如果key或iv为空，则使用默认值
                if (key == null || key.Length != 16)
                {
                    key = KEY_DEFAULT;
                }

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                try
                {
                    RijndaelManaged rDel = new RijndaelManaged
                    {
                        Key = keyArray,
                        Mode = CipherMode.ECB,
                        // 加密模式
                        Padding = TRANSFORM_ECB,
                    };
                    ICryptoTransform cTransform = rDel.CreateEncryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    // 然后转成BASE64返回
                    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("基于ECB工作模式的AES加密失败,toEncrypt:{0},key:{1}", toEncrypt, key));
                    throw ex;
                }
            }

            return null;
        }
        #endregion

        #region 基于ECB工作模式的AES解密
        /// <summary>
        /// 基于ECB工作模式的AES解密
        /// </summary>
        /// <param name="toDecrypt">AES加密之后的字符串</param>
        /// <param name="key">秘钥，如果不填则使用默认值</param>
        /// <returns></returns>
        public static string DecryptEcbMode(string toDecrypt, string key = KEY_DEFAULT)
        {
            if (!string.IsNullOrEmpty(toDecrypt))
            {
                // 如果key为空，则使用默认值
                if (key == null || key.Length != 16)
                {
                    key = KEY_DEFAULT;
                }

                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
                byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

                try
                {
                    RijndaelManaged rDel = new RijndaelManaged
                    {
                        Key = keyArray,
                        Mode = CipherMode.CBC,
                        Padding = TRANSFORM_ECB
                    };
                    ICryptoTransform cTransform = rDel.CreateDecryptor();
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                    return UTF8Encoding.UTF8.GetString(resultArray);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("基于ECB工作模式的AES解密失败,toDecrypt:{0},key:{1}", toDecrypt, key));
                    throw ex;
                }
            }

            return null;
        }
        #endregion
    }
}