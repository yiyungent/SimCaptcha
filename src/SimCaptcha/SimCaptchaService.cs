using SimCaptcha.Common;
using SimCaptcha.Implement;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCaptcha
{
    /// <summary>
    /// 验证码服务端
    /// </summary>
    public class SimCaptchaService
    {
        private const string CachePrefixTicket = "Cache:SimCaptcha:Ticket:";

        private const string CachePrefixVCodeKey = "Cache:SimCaptcha:VCodeKey:";

        private readonly CacheHelper _cacheHelper;

        private SimCaptchaOptions _options;

        public IRandomCode RandomCode { get; set; }

        public IVCodeImage VCodeImage { get; set; }

        public IJsonHelper JsonHelper { get; set; }

        #region Ctor
        public SimCaptchaService(ICache cache, IVCodeImage vCodeImage, IJsonHelper jsonHelper, SimCaptchaOptions options)
        {
            this._cacheHelper = new CacheHelper(cache);
            this._options = options;
            this.RandomCode = new RandomCodeHanZi();

            this.VCodeImage = vCodeImage;
            this.JsonHelper = jsonHelper;
        }
        #endregion

        #region 验证码效验
        public VCodeCheckResponseModel VCodeCheck(VerifyInfoModel verifyInfo, string userIp)
        {
            VCodeCheckResponseModel rtnResult = null;

            // 获取此用户会话的验证码效验 vCodeKey
            string cacheKeyVCodeKey = CachePrefixVCodeKey + verifyInfo.UserId;
            if (!_cacheHelper.Exists(cacheKeyVCodeKey))
            {
                // 验证码无效，1.此验证码已被销毁
                rtnResult = new VCodeCheckResponseModel { code = -5, message = "验证码无效, 获取新验证码" };
                return rtnResult;
            }
            string rightVCodeKey = _cacheHelper.Get(cacheKeyVCodeKey).ToString();
            // AES解密
            string vCodeKeyJsonStr = AesHelper.DecryptEcbMode(rightVCodeKey, _options.AesKey);
            // json -> 对象
            VCodeKeyModel vCodeKeyModel = null;
            try
            {
                // 能够转换为 对象，则说明 vCodeKey 无误，可以使用
                vCodeKeyModel = JsonHelper.Deserialize<VCodeKeyModel>(vCodeKeyJsonStr);
            }
            catch (Exception ex)
            { }
            if (vCodeKeyModel == null)
            {
                // 验证码无效，被篡改导致解密失败
                rtnResult.code = -3;
                rtnResult.message = "验证码无效, 获取新验证码";
                return rtnResult;
            }

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
                    // 只要有一个点的任意一个轴偏移量大于25，则验证不通过
                    if (xOffset > 25 || yOffset > 25)
                    {
                        isPass = false;
                    }
                }
            }

            #endregion

            // 错误次数是否达上限
            bool isMoreThanErrorNum = vCodeKeyModel.ErrorNum > _options.AllowErrorNum;

            // 验证码是否过期
            bool isExpired = ((DateTimeHelper.NowTimeStamp13() - vCodeKeyModel.TS) / 1000) > _options.ExpiredSec;

            if (!isPass && !isMoreThanErrorNum)
            {
                // 错误 -> 1.code:-1 验证码错误 且 错误次数未达上限 -> message: 点错啦，请重试
                vCodeKeyModel.ErrorNum++;
                string vCodekeyJsonStrTemp = JsonHelper.Serialize(vCodeKeyModel);
                // AES加密 vCodekeyJsonStrTemp
                string vCodeKeyStrTemp = AesHelper.EncryptEcbMode(vCodekeyJsonStrTemp, _options.AesKey);
                // 更新 Cache 中的 vCodeKey
                _cacheHelper.Insert<string>(CachePrefixVCodeKey + verifyInfo.UserId, vCodeKeyStrTemp);

                rtnResult.code = -1;
                rtnResult.message = "点错啦，请重试";
                return rtnResult;
            }
            else if (!isPass && isMoreThanErrorNum)
            {
                // 错误 -> 2.code:-2 验证码错误 且 错误次数已达上限 -> message: 这题有点难，为你换一个试试吧
                rtnResult.code = -2;
                rtnResult.message = "这题有点难, 为你换一个试试吧";
                RemoveCacheVCodeKey(verifyInfo.UserId);
                return rtnResult;
            }
            else if (isExpired)
            {
                // 验证码过期
                rtnResult.code = -4;
                rtnResult.message = "验证码过期, 获取新验证码";
                RemoveCacheVCodeKey(verifyInfo.UserId);
                return rtnResult;
            }

            // 正确 -> code:0 下发票据 ticket
            TicketModel ticketModel = new TicketModel { IP = userIp, IsPass = true, TS = DateTimeHelper.NowTimeStamp13() };
            string ticketJsonStr = JsonHelper.Serialize(ticketModel);
            // 对 ticketJsonStr 加密
            string ticket = AesHelper.EncryptEcbMode(ticketJsonStr, _options.AesKey);

            rtnResult.code = 0;
            rtnResult.message = "验证通过";
            rtnResult.data = new VCodeCheckResponseModel.DataModel { appId = verifyInfo.AppId, ticket = ticket };
            return rtnResult;
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
            // 解密ticket -> 转为实体对象
            TicketModel ticketModel = null;
            try
            {
                string ticketJsonStr = AesHelper.DecryptEcbMode(ticket, _options.AesKey);
                ticketModel = JsonHelper.Deserialize<TicketModel>(ticketJsonStr);
            }
            catch (Exception ex)
            { }
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
                string vCodeKey = AesHelper.EncryptEcbMode(vCodekeyJsonStr, _options.AesKey);
                // 答案 保存到 此次用户会话对应的 Cache 中
                _cacheHelper.Insert<string>(CachePrefixVCodeKey + userId, vCodeKey);
            }
            catch (Exception ex)
            {
                rtnResult.code = -1;
                rtnResult.message = "获取验证码失败";
            }

            return Task.FromResult(rtnResult);
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
