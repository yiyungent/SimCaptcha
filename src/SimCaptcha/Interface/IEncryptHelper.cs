// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha.Interface
{
    public interface IEncryptHelper
    {
        string Encrypt(string toEncrypt, string key, params string[] others);

        string Decrypt(string toDecrypt, string key, params string[] others);
    }
}
