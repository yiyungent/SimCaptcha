using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimCaptcha.Models.Click
{
    public class ClickVerifyInfoModel : VerifyInfoModel
    {
        public IList<PointPosModel> VCodePos { get; set; }
    }
}
