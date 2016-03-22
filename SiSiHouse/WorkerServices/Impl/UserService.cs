using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService()
            : this(new UserRepository())
        {
        }

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public User Login(string account, string password)
        {
            return _repository.GetUserByAccount(account, password);
        }

        public IList<User> GetUserList(UserCondition condition, DataTablesModel table, out int totalUser)
        {
            return _repository.GetUserList(condition, table, out totalUser);
        }

        public User GetUserInfo(long userID)
        {
            return _repository.GetUserInfo(userID);
        }

        public bool CheckAccountExist(long userID, string account)
        {
            return _repository.CheckAccountExist(userID, account);
        }

        public bool UpdateUserInfo(User data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = (_repository.UpdateUserInfo(data) == 1);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
