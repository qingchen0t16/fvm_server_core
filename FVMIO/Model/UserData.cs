using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVMIO.Model
{
    /// <summary>
    /// 用户的基本资料数据
    /// </summary>
    [Serializable]
    public class UserData
    {
        public int UserID;
        public string Account, UserName, Sex;
        public int Level;
        public long Exp;
        public UserMoneyData Money;
    }
}
