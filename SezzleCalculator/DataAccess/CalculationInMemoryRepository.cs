using SezzleCalculator.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace SezzleCalculator.DataAccess
{
    public class CalculationInMemoryRepository : ICalculationRepository // In a real solution we would implement this interface on a class that talked to a DB, but in memory caching will do here.
    {
        private static ConcurrentBag<Calculation> _calculationRepo;

        public CalculationInMemoryRepository()
        {
            // Check if the static calculation repo has been instantiated before. If not, do it now. 
            if (_calculationRepo == null) _calculationRepo = new ConcurrentBag<Calculation>();
        }

        public void Add(Calculation calculation)
        {

            calculation.Index = _calculationRepo.Count + 1; // setting the equivalent of a primary key. 
            _calculationRepo.Add(calculation);
        }

        public List<Calculation> GetCalculationsOrderDescOnIndex(int limit)
        {

            return _calculationRepo.OrderByDescending(x => x.Index).Take(limit).ToList();

        }
    }
}
