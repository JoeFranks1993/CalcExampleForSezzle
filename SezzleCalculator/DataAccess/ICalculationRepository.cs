using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SezzleCalculator.Models;

namespace SezzleCalculator.DataAccess
{
    public interface ICalculationRepository
    {
        List<Calculation> GetCalculationsOrderDescOnIndex(int limit);

        void Add(Calculation calculation);
    }
}
