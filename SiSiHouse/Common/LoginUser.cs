using System;
using System.Collections.Generic;

namespace SiSiHouse.Common
{
    /// <summary>
    /// Login user entity class
    /// </summary>
    [Serializable]
    public class LoginUser
    {
        public long USER_ID { get; set; }
        public string ACCOUNT { get; set; }
        public string FULL_NAME { get; set; }
        public int? ROLE_ID { get; set; }
        //public List<int> FunctionList { get; set; }
    }
}