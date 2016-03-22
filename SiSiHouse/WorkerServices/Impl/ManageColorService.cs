using SiSiHouse.Models.Conditions;
using SiSiHouse.Models.Entities;
using SiSiHouse.Models.Repositories;
using SiSiHouse.Models.Repositories.Impl;
using SiSiHouse.ViewModels;
using System.Collections.Generic;
using System.Transactions;

namespace SiSiHouse.WorkerServices.Impl
{
    public class ManageColorService : IManageColorService
    {
        private readonly IManageColorRepository _repository;

        public ManageColorService()
            : this(new ManageColorRepository())
        {
        }

        public ManageColorService(ManageColorRepository repository)
        {
            _repository = repository;
        }

        public IList<Color> GetColorList(ColorCondition condition, DataTablesModel tableSetting, out int totalData)
        {
            return _repository.GetColorList(condition, tableSetting, out totalData);
        }

        public Color GetColorInfo(int colorID)
        {
            return _repository.GetColorInfo(colorID);
        }

        public bool UpdateColorInfo(Color data)
        {
            var res = false;

            using (var transaction = new TransactionScope())
            {
                res = (_repository.UpdateColorInfo(data) == 1);

                if (res)
                    transaction.Complete();
            }

            return res;
        }
    }
}
