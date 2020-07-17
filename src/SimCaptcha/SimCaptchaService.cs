using SimCaptcha.Common;
using SimCaptcha.Implement;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Project: SimCaptcha
// https://github.com/yiyungent/SimCaptcha
// Author: yiyun <yiyungent@gmail.com>

namespace SimCaptcha
{
    /// <summary>
    /// 验证码服务端
    /// </summary>
    public class SimCaptchaService
    {
        #region Const
        private const string CachePrefixTicket = "Cache:SimCaptcha:Ticket:";

        private const string CachePrefixVCodeKey = "Cache:SimCaptcha:VCodeKey:";
        #endregion

        #region Fields
        private ISimCaptchaOptions _options;

        private ICacheHelper _cacheHelper;

        private IEncryptHelper _encryptHelper;

        private ILogHelper _logHelper;
        #endregion

        #region Properties
        public IRandomCode RandomCode { get; set; }

        public IVCodeImage VCodeImage { get; set; }

        public IJsonHelper JsonHelper { get; set; }

        public IAppChecker AppChecker { get; set; }
        #endregion

        #region Ctor
        public SimCaptchaService(ISimCaptchaOptions options, ICache cache, IVCodeImage vCodeImage, IJsonHelper jsonHelper)
        {
            this._options = options;
            this._cacheHelper = new CacheHelper(cache);

            this.VCodeImage = vCodeImage;
            this.JsonHelper = jsonHelper;

            this.AppChecker = new DefaultAppChecker((SimCaptchaOptions)options);
            this.RandomCode = new RandomCodeHanZi();
            this._encryptHelper = new AesEncryptHelper();
        }
        public SimCaptchaService(ISimCaptchaOptions options, ICache cache, IVCodeImage vCodeImage, IJsonHelper jsonHelper, IRandomCode randomCode)
        {
            this._options = options;
            this._cacheHelper = new CacheHelper(cache);

            this.VCodeImage = vCodeImage;
            this.JsonHelper = jsonHelper;

            this.AppChecker = new DefaultAppChecker((SimCaptchaOptions)options);
            this.RandomCode = randomCode;
            this._encryptHelper = new AesEncryptHelper();
        }
        public SimCaptchaService(ISimCaptchaOptions options, ICacheHelper cacheHelper, IVCodeImage vCodeImage, IJsonHelper jsonHelper, IAppChecker appChecker)
        {
            this._options = options;
            this._cacheHelper = cacheHelper;

            this.VCodeImage = vCodeImage;
            this.JsonHelper = jsonHelper;

            this.AppChecker = appChecker;
            this.RandomCode = new RandomCodeHanZi();
            this._encryptHelper = new AesEncryptHelper();
        }

        public SimCaptchaService(ISimCaptchaOptions options, ICacheHelper cacheHelper, IVCodeImage vCodeImage, IJsonHelper jsonHelper, IAppChecker appChecker, IRandomCode randomCode)
        {
            this._options = options;
            this._cacheHelper = cacheHelper;

            this.VCodeImage = vCodeImage;
            this.JsonHelper = jsonHelper;

            this.AppChecker = appChecker;
            this.RandomCode = randomCode;
            this._encryptHelper = new AesEncryptHelper();
        }
        #endregion

        #region Set
        public SimCaptchaService Set(ISimCaptchaOptions options)
        {
            this._options = options;
            return this;
        }

        public SimCaptchaService Set(IJsonHelper jsonHelper)
        {
            this.JsonHelper = jsonHelper;
            return this;
        }
        public SimCaptchaService Set(ICacheHelper cacheHelper)
        {
            this._cacheHelper = cacheHelper;
            return this;
        }
        public SimCaptchaService Set(IVCodeImage vCodeImage)
        {
            this.VCodeImage = vCodeImage;
            return this;
        }
        public SimCaptchaService Set(IAppChecker appChecker)
        {
            this.AppChecker = appChecker;
            return this;
        }
        public SimCaptchaService Set(IRandomCode randomCode)
        {
            this.RandomCode = randomCode;
            return this;
        }
        public SimCaptchaService Set(ILogHelper logHelper)
        {
            this._logHelper = logHelper;
            return this;
        }
        public SimCaptchaService Set(IEncryptHelper encryptHelper)
        {
            this._encryptHelper = encryptHelper;
            return this;
        }
        #endregion

        #region 验证码效验
        public VCodeCheckResponseModel VCodeCheck(VerifyInfoModel verifyInfo, string userIp)
        {
            VCodeCheckResponseModel rtnResult = new VCodeCheckResponseModel();
            // 允许的偏移量(点触容错)
            int allowOffset = 10;

            // appId 效验: 这通常需要你自己根据业务实现 IAppChecker
            #region AppId效验
            AppCheckModel appCheckResult = AppChecker.CheckAppId(verifyInfo.AppId);
            if (!appCheckResult.Pass)
            {
                // -6 appId 效验不通过 -> 不允许验证, 提示错误信息
                rtnResult = new VCodeCheckResponseModel { code = -6, message = appCheckResult.Message };
                return rtnResult;
            }
            #endregion

            #region 尝试从内存中取出对应的 VCodeKey
            // 获取此用户会话的验证码效验 vCodeKey
            string cacheKeyVCodeKey = CachePrefixVCodeKey + verifyInfo.UserId;
            if (!_cacheHelper.Exists(cacheKeyVCodeKey))
            {
                // 验证码无效，1.此验证码已被销毁
                rtnResult = new VCodeCheckResponseModel { code = -5, message = "验证码过期, 获取新验证码" };
                return rtnResult;
            }
            string rightVCodeKey = _cacheHelper.Get<string>(cacheKeyVCodeKey);
            // AES解密
            string vCodeKeyJsonStr = _encryptHelper.Decrypt(rightVCodeKey, _options.EncryptKey);
            // json -> 对象
            VCodeKeyModel vCodeKeyModel = null;
            try
            {
                // TODO: fixed: 临时修复, 直接将全部为0的字节去除, 
                byte[] bytes = Encoding.UTF8.GetBytes(vCodeKeyJsonStr);
                byte[] remove0Bytes = bytes.Where(m => m != 0).ToArray();
                string remove0ByteStr = Encoding.UTF8.GetString(remove0Bytes);

                // 能够转换为 对象, 则说明 vCodeKey 无误, 可以使用
                //vCodeKeyModel = JsonHelper.Deserialize<VCodeKeyModel>(vCodeKeyJsonStr);
                vCodeKeyModel = JsonHelper.Deserialize<VCodeKeyModel>(remove0ByteStr);
            }
            catch (Exception ex)
            {
                // TODO: BUG: 经加密再解密后的jsonStr，虽然看起来一样,但发生了一点改变, 导致无法转换
                // '0x00' is invalid after a single JSON value. Expected end of data. LineNumber: 0 | BytePositionInLine: 110.
                _logHelper?.Write(ex.ToString());
            }
            if (vCodeKeyModel == null)
            {
                // 验证码无效，被篡改导致解密失败
                rtnResult.code = -3;
                rtnResult.message = "验证码无效, 获取新验证码";
                return rtnResult;
            }
            #endregion

            #region 验证码是否过期
            // 验证码是否过期
            bool isExpired = ((DateTimeHelper.NowTimeStamp13() - vCodeKeyModel.TS) / 1000) > _options.ExpiredSec;
            if (isExpired)
            {
                // 验证码过期
                rtnResult.code = -4;
                rtnResult.message = "验证码过期, 获取新验证码";
                RemoveCacheVCodeKey(verifyInfo.UserId);
                return rtnResult;
            }
            #endregion

            #region 效验点触位置数据
            // 效验点触位置数据
            IList<PointPosModel> rightVCodePos = vCodeKeyModel.VCodePos;
            IList<PointPosModel> userVCodePos = verifyInfo.VCodePos;
            // 验证码是否正确
            bool isPass = false;
            if (userVCodePos.Count != rightVCodePos.Count)
            {
                // 验证不通过
                isPass = false;
            }
            else
            {
                isPass = true;
                for (int i = 0; i < userVCodePos.Count; i++)
                {
                    int xOffset = userVCodePos[i].X - rightVCodePos[i].X;
                    int yOffset = userVCodePos[i].Y - rightVCodePos[i].Y;
                    // x轴偏移量
                    xOffset = Math.Abs(xOffset);
                    // y轴偏移量
                    yOffset = Math.Abs(yOffset);
                    // 只要有一个点的任意一个轴偏移量大于allowOffset，则验证不通过
                    if (xOffset > allowOffset || yOffset > allowOffset)
                    {
                        isPass = false;
                    }
                }
            }

            #endregion

            #region 未通过->错误次数达到上限?
            if (!isPass)
            {
                // 本次没通过验证 -> 错误次数+1
                vCodeKeyModel.ErrorNum++;
                // 错误次数是否达上限
                bool isMoreThanErrorNum = vCodeKeyModel.ErrorNum > _options.AllowErrorNum;
                if (isMoreThanErrorNum)
                {
                    // 错误 -> 2.code:-2 验证码错误 且 错误次数已达上限 -> message: 这题有点难，为你换一个试试吧
                    rtnResult.code = -2;
                    rtnResult.message = "这题有点难, 为你换一个试试吧";
                    RemoveCacheVCodeKey(verifyInfo.UserId);
                    return rtnResult;
                }
                else
                {
                    // 错误 -> 1.code:-1 验证码错误 且 错误次数未达上限 -> message: 点错啦，请重试
                    string vCodekeyJsonStrTemp = JsonHelper.Serialize(vCodeKeyModel);
                    // AES加密 vCodekeyJsonStrTemp
                    string vCodeKeyStrTemp = _encryptHelper.Encrypt(vCodekeyJsonStrTemp, _options.EncryptKey);
                    // 更新 Cache 中的 vCodeKey
                    _cacheHelper.Insert<string>(CachePrefixVCodeKey + verifyInfo.UserId, vCodeKeyStrTemp);

                    rtnResult.code = -1;
                    rtnResult.message = "点错啦，请重试";
                    return rtnResult;
                }
            }
            #endregion

            #region 验证通过->下发ticket
            // 正确 -> code:0 下发票据 ticket
            TicketModel ticketModel = new TicketModel { IP = userIp, IsPass = true, TS = DateTimeHelper.NowTimeStamp13() };
            string ticketJsonStr = JsonHelper.Serialize(ticketModel);
            // 对 ticketJsonStr 加密
            string ticket = _encryptHelper.Encrypt(ticketJsonStr, _options.EncryptKey);
            // 内存中存一份ticket, 用于效验
            _cacheHelper.Insert<string>(CachePrefixTicket + verifyInfo.UserId, ticket);

            rtnResult.code = 0;
            rtnResult.message = "验证通过";
            rtnResult.data = new VCodeCheckResponseModel.DataModel { appId = verifyInfo.AppId, ticket = ticket };
            return rtnResult;
            #endregion
        }
        #endregion

        #region ticket效验
        /// <summary>
        /// ticket效验
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="appSecret"></param>
        /// <param name="ticket"></param>
        /// <param name="userId">用户唯一标识</param>
        /// <param name="userIp"></param>
        /// <param name="aesKey"></param>
        /// <returns></returns>
        public TicketVerifyResponseModel TicketVerify(string appId, string appSecret, string ticket, string userId, string userIp)
        {
            TicketVerifyResponseModel rtnResult = null;

            // appId, appSecret效验: 这通常需要你自己根据业务实现 IAppChecker
            #region AppId,AppSecret效验
            AppCheckModel appCheckResult = AppChecker.Check(appId, appSecret);
            if (!appCheckResult.Pass)
            {
                // -7 AppId,AppSecret效验不通过
                rtnResult = new TicketVerifyResponseModel { code = -7, message = appCheckResult.Message };
                return rtnResult;
            }
            #endregion

            // 解密ticket -> 转为实体对象
            TicketModel ticketModel = null;
            try
            {
                string ticketJsonStr = _encryptHelper.Decrypt(ticket, _options.EncryptKey);

                // TODO: fixed: 临时修复, 直接将全部为0的字节去除, 
                byte[] bytes = Encoding.UTF8.GetBytes(ticketJsonStr);
                byte[] remove0Bytes = bytes.Where(m => m != 0).ToArray();
                string remove0ByteStr = Encoding.UTF8.GetString(remove0Bytes);

                // 能够转换为 对象, 则说明 vCodeKey 无误, 可以使用
                ticketModel = JsonHelper.Deserialize<TicketModel>(remove0ByteStr);

                //ticketModel = JsonHelper.Deserialize<TicketModel>(ticketJsonStr);
            }
            catch (Exception ex)
            {
                // TODO: AES加解密后多出0, 导致无法转为json对象, 和验证码效验时一样
                // '0x00' is invalid after a single JSON value. Expected end of data. LineNumber: 0 | BytePositionInLine: 110.
                _logHelper?.Write(ex.Message);
            }
            if (ticketModel == null)
            {
                // ticket无效，被篡改
                rtnResult = new TicketVerifyResponseModel { code = -4, message = "ticket无效" };
                return rtnResult;
            }
            // 从内存中取出此用户会话保存的独有Ticket，进行比对
            string cacheKeyTicket = CachePrefixTicket + userId;
            if (!_cacheHelper.Exists(cacheKeyTicket))
            {
                // ticket无效，1.此ticket已被效验过一次,用完销毁 2.其它原因: 伪造ticket
                rtnResult = new TicketVerifyResponseModel { code = -5, message = "ticket无效" };
                return rtnResult;
            }
            string rightTicket = _cacheHelper.Get(cacheKeyTicket).ToString();
            if (ticket != rightTicket)
            {
                // ticket无效，1.篡改ticket
                rtnResult = new TicketVerifyResponseModel { code = -6, message = "ticket无效" };
                RemoveCacheTicket(userId);
                return rtnResult;
            }
            if (!ticketModel.IsPass)
            {
                // ticket 标识 验证不通过
                rtnResult = new TicketVerifyResponseModel { code = -1, message = "验证不通过" };
                RemoveCacheTicket(userId);
                return rtnResult;
            }
            int secOffset = (int)((DateTimeHelper.NowTimeStamp13() - ticketModel.TS) / 1000);
            if (secOffset > _options.ExpiredSec)
            {
                // ticket 已过期
                rtnResult = new TicketVerifyResponseModel { code = -2, message = "ticket过期" };
                RemoveCacheTicket(userId);
                return rtnResult;
            }
            if (ticketModel.IP != userIp)
            {
                // ip不匹配
                rtnResult = new TicketVerifyResponseModel { code = -3, message = "ip不匹配" };
                RemoveCacheTicket(userId);
                return rtnResult;
            }
            // 验证通过
            rtnResult = new TicketVerifyResponseModel { code = 0, message = "验证通过" };
            RemoveCacheTicket(userId);

            return rtnResult;
        }
        #endregion

        #region 响应验证码,用户会话唯一标识
        /// <summary>
        /// 响应验证码,用户会话唯一标识
        /// </summary>
        /// <returns></returns>
        public Task<VCodeResponseModel> VCode()
        {
            VCodeResponseModel rtnResult = new VCodeResponseModel();
            try
            {
                VCodeImgModel model = CreateVCodeImg();
                string userId = Guid.NewGuid().ToString();
                rtnResult.code = 0;
                rtnResult.message = "获取验证码成功";
                rtnResult.data = new VCodeResponseModel.DataModel
                {
                    userId = userId,
                    vCodeImg = model.ImgBase64,
                    vCodeTip = model.VCodeTip,
                    words = model.Words
                };
                // 生成 vCodeKey: 转为json字符串 -> AES加密
                string vCodekeyJsonStr = JsonHelper.Serialize(new VCodeKeyModel
                {
                    ErrorNum = 0,
                    TS = DateTimeHelper.NowTimeStamp13(),
                    VCodePos = model.VCodePos
                });
                string vCodeKey = _encryptHelper.Encrypt(vCodekeyJsonStr, _options.EncryptKey);
                // 答案 保存到 此次用户会话对应的 Cache 中
                _cacheHelper.Insert<string>(CachePrefixVCodeKey + userId, vCodeKey);
            }
            catch (Exception ex)
            {
                rtnResult.code = -1;
                rtnResult.message = "获取验证码失败";

                _logHelper?.Write(ex.ToString());
            }

            // TODO: 在.net framework 4.0下未测试
            // Task 参考: https://www.cnblogs.com/yaopengfei/p/8183530.html
#if NETFULL40
            return Task.Factory.StartNew(() => { return rtnResult; });
#else
            return Task.FromResult(rtnResult);
#endif
        }
        #endregion




        #region 清除目标用户会话存在内存中的ticket
        /// <summary>
        /// 清除目标用户会话存在内存中的ticket
        /// </summary>
        /// <param name="userId"></param>
        private void RemoveCacheTicket(string userId)
        {
            if (_cacheHelper.Exists(CachePrefixTicket + userId))
            {
                _cacheHelper.Remove(CachePrefixTicket + userId);
            }
        }
        #endregion

        #region 清除目标用户会话存在内存中的vCodeKey
        /// <summary>
        /// 清除目标用户会话存在内存中的vCodeKey
        /// </summary>
        /// <param name="userId"></param>
        private void RemoveCacheVCodeKey(string userId)
        {
            if (_cacheHelper.Exists(CachePrefixVCodeKey + userId))
            {
                _cacheHelper.Remove(CachePrefixVCodeKey + userId);
            }
        }
        #endregion

        #region 创建验证码图片及提示,答案
        private VCodeImgModel CreateVCodeImg()
        {
            VCodeImgModel rtnResult = new VCodeImgModel { VCodePos = new List<PointPosModel>() };
            string code = RandomCode.Create(6);
            rtnResult = VCodeImage.Create(code, 200, 200);

            return rtnResult;
        }
        #endregion

    }
}
