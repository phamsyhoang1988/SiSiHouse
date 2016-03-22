using SiSiHouse.Common;
using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.WorkerServices
{
    public interface IUserService
    {
        User Login(string account, string password);

        IList<User> GetUserList(UserCondition condition, DataTablesModel table, out int totalUser);

        User GetUserInfo(long userID);

        bool UpdateUserInfo(User data);

        bool CheckAccountExist(long userID, string account);
    }
}
