using Application.Repositories.Base;
using Domain.Constants.AppEnum;
using Domain.Constants.AppConstants;

namespace Infrastructure.Services
{
    public class LoanInterestRateService : ILoanInterestRate
    {
        public Task<decimal> CalculateInterestRate(int months, int loanRate, CancellationToken cancellationToken = default)
        {
            // loan rate base on account level
            switch (loanRate)
            {
                // calculate interest rate base on base loan rate
                // risk factor base on loan term
                // round to 4 decimal places
                case (int)LoanRate.BaseRate:
                    decimal riskRate = CalculateRiskFactor(months);
                    decimal interestRate = AppConstants.BaseLoanRate * riskRate;
                    return Task.FromResult(Math.Round(interestRate, 4));

                case (int)LoanRate.SpecialRate:
                    decimal specialRiskRate = CalculateRiskFactor(months);
                    decimal specialInterestRate = AppConstants.SpecialLoanRate * specialRiskRate;
                    return Task.FromResult(Math.Round(specialInterestRate, 4));

                case (int)LoanRate.PremiumRate:
                    decimal premiumRiskRate = CalculateRiskFactor(months);
                    decimal premiumInterestRate = AppConstants.PremiumLoanRate * premiumRiskRate;
                    return Task.FromResult(Math.Round(premiumInterestRate, 4));

                default:
                    return Task.FromResult(0M);
            }
        }

        public Task<decimal> CalculateTotal(decimal amount, int months, decimal interestRate, CancellationToken cancellationToken = default)
        {
            // total = amount * interest rate * months
            // round to 0 decimal places
            decimal interest = amount * interestRate * months;
            decimal total = amount + interest;

            return Task.FromResult(Math.Round(total, 0));
        }

        private decimal CalculateRiskFactor(int months)
        {
            // risk factor base on loan term
            if (months < 1)
                throw new ArgumentException("Loan term must be >= 1 month");
            // risk factor = 1 + risk step * (months - 1)
            return 1 + AppConstants.RiskStep * (months - 1);
        }
    }
}