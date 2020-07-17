using SimCaptcha.Interface;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Implement
{
    public class AesEncryptHelper : IEncryptHelper
    {
        public string Decrypt(string toDecrypt, string key, params string[] others)
        {
            return Common.AesHelper.DecryptEcbMode(toDecrypt, key);
        }

        public string Encrypt(string toEncrypt, string key, params string[] others)
        {
            return Common.AesHelper.EncryptEcbMode(toEncrypt, key);
        }
    }
}
