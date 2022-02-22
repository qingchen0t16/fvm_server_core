using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVMIO.Model
{
    /// <summary>
    /// 用户等级
    /// </summary>
    [Serializable]
    public class UserLevel
    {
        public int Level;
        public long Exp;
        public long NeedExp => (long)Math.Round(Level * 100.99F + (Level - 1) * 1024);
    }
}
