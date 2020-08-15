using SezzleCalculator.DataAccess;
using SezzleCalculator.Models;
using System;
using System.Collections.Generic;
using NCalc;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace SezzleCalculator.Managers
{
    // In this project the logic in the manager is pretty thin. The reason we would put a class like this between our controller and our data is to implment any buisness logic here.
    // This decouples the repo from our buisness logic and keeps the controller thin. 
    public class CalculationManager : ICalculationManager
    {
        private ICalculationRepository repo;

        public CalculationManager() : this (new CalculationInMemoryRepository())
        {
        }

        public CalculationManager(ICalculationRepository repo)
        {
            this.repo = repo;
        }

        public Calculation AddCalculation(string calculation)
        {
            var expression = new Expression(calculation);
            var calc = new Calculation() { CalcString = calculation, Result = Convert.ToDouble(expression.Evaluate())};

            if (Double.IsNaN(calc.Result)) throw new Exception("Invalid Result, Cannot Be NaN");
            repo.Add(calc);

            return calc;
        }

        public List<Calculation> GetCalulations(int limit)
        {
            return repo.GetCalculationsOrderDescOnIndex(limit);
        }
    }
}
