using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.ViewModels;
using System.Collections.Generic;

namespace SiSiHouse.Models.Repositories
{
    public interface IUserRepository
    {
        User GetUserByAccount(string account, string password);

        IList<User> GetUserList(UserCondition condition, DataTablesModel table, out int totalData);

        User GetUserInfo(long userID);

        int UpdateUserInfo(User data);

        bool CheckAccountExist(long userID, string account);
    }
}
