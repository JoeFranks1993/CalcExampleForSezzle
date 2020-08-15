using SezzleCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SezzleCalculator.Managers
{
    public interface ICalculationManager
    {
        public List<Calculation> GetCalulations(int limit);

        public Calculation AddCalculation(string calculation);
    }
}
