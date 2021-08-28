using SimCaptcha.Common;
using SimCaptcha.Interface;
using SimCaptcha.Models;
using SimCaptcha.Models.Click;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimCaptcha.Click
{
    public class ClickSimCaptchaService : SimCaptchaService
    {
        public IClickVCodeImage VCodeImage { get; set; }

        public IClickRandomCode RandomCode { get; set; }


        #region Ctor

        public ClickSimCaptchaService(ISimCaptchaOptions options, ICacheHelper cacheHelper, IJsonHelper jsonHelper,
            IEncryptHelper encryptHelper, IAppChecker appChecker, ILogHelper logHelper,
            IClickVCodeImage clickVCodeImage, IClickRandomCode clickRandomCode)
            : base(options, cacheHelper, jsonHelper, encryptHelper, appChecker, logHelper)
        {
            this.VCodeImage = clickVCodeImage;
            this.RandomCode = clickRandomCode;
        }

        #endregion

        #region VCodeKeyModel
        protected VCodeKeyModel VCodeKeyModelJsonModel(string jsonStr)
        {
            VCodeKeyModel vCodeKeyModel = JsonHelper.Deserialize<ClickVCodeKeyModel>(jsonStr);

            return vCodeKeyModel;
        }
        #endregion


        #region 效验验证码数据
        protected bool Verify(VCodeKeyModel vCodeKeyModel, VerifyInfoModel verifyInfoModel, int allowOffset)
        {
            ClickVCodeKeyModel vCodeKey = (ClickVCodeKeyModel)vCodeKeyModel;
            ClickVerifyInfoModel verifyInfo = (ClickVerifyInfoModel)verifyInfoModel;

            // 效验点触位置数据
            IList<PointPosModel> rightVCodePos = vCodeKey.VCodePos;
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

            return isPass;
        }
        #endregion


        #region 响应验证码,用户会话唯一标识
        /// <summary>
        /// 响应验证码,用户会话唯一标识
        /// </summary>
        /// <returns></returns>
        public Task<ClickVCodeResponseModel> VCode(CaptchaType captchaType)
        {
            ClickVCodeResponseModel rtnResult = new ClickVCodeResponseModel();

            try
            {
                ClickVCodeImgModel model = CreateVCodeImg();
                string userId = Guid.NewGuid().ToString();
                rtnResult.code = 0;
                rtnResult.message = "获取验证码成功";
                rtnResult.data = new ClickVCodeResponseModel.DataModel
                {
                    userId = userId,
                    vCodeImg = model.ImgBase64,
                    vCodeTip = model.VCodeTip,
                    words = model.Words
                };
                // 生成 vCodeKey: 转为json字符串 -> AES加密
                string vCodekeyJsonStr = JsonHelper.Serialize(new ClickVCodeKeyModel
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


        #region 创建验证码图片及提示,答案
        private ClickVCodeImgModel CreateVCodeImg()
        {
            ClickVCodeImgModel rtnResult = new ClickVCodeImgModel { VCodePos = new List<PointPosModel>() };
            string code = RandomCode.Create(6);
            rtnResult = VCodeImage.Create(code, 200, 200);

            return rtnResult;
        }

        protected override VCodeKeyModel VCodeKeyModelJsonModel(string jsonStr)
        {
            throw new NotImplementedException();
        }

        protected override bool Verify(VCodeKeyModel vCodeKeyModel, VerifyInfoModel verifyInfoModel, int allowOffset)
        {
            throw new NotImplementedException();
        }

        public override Task<VCodeResponseModel> VCode()
        {
            throw new NotImplementedException();
        }

        protected override VCodeImgModel CreateVCodeImg()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
