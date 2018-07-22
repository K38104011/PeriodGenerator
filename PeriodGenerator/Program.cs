using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PeriodGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            System.Console.WriteLine("Periods:");
            var startDate = new DateTime(2000, 2, 1);
            var endDate = DateTime.Now;
            var monthsPerPeriod = 1;
            var request = new GeneratePeriodRequest
            {
                StartDate = startDate,
                EndDate = endDate,
                MonthsPerPeriod = monthsPerPeriod
            };
            foreach (var period in GeneratePeriods(request))
            {
                Console.WriteLine(period);
            }
            var getPeriodResult = GetPeriod(new GetPeriodRequest{
                StartDate = startDate,
                CurrentDate = endDate,
                MonthsPerPeriod = monthsPerPeriod
            });
            System.Console.WriteLine("Current periods:");
            foreach (var period in getPeriodResult.CurrentPeriod.Months){
                System.Console.WriteLine(period);
            }
            System.Console.WriteLine("Previous periods:");
            foreach (var period in getPeriodResult.PreviousPeriod.Months){
                System.Console.WriteLine(period);
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds);
        }

        public class GeneratePeriodRequest
        {
            public DateTime StartDate { get; set; }
            public int MonthsPerPeriod { get; set; }
            public DateTime EndDate { get; set; }
        }
        
        public class GetPeriodRequest
        {
            public DateTime StartDate { get; set; }
            public int MonthsPerPeriod { get; set; }
            public DateTime CurrentDate { get; set; }
            public int ExtendMonths { get; set; }
        }

        public class GetPeriodResult
        {
            public Period CurrentPeriod { get; set; }
            public Period PreviousPeriod { get; set; }
            public GetPeriodResult()
            {
                CurrentPeriod = new Period();
                PreviousPeriod = new Period();
            }
        }

        public class Period
        {
            public List<DateTime> Months = new List<DateTime>();
        }

        public static GetPeriodResult GetPeriod(GetPeriodRequest request)
        {
            var periods = GeneratePeriods(new GeneratePeriodRequest
            {
                StartDate = request.StartDate,
                EndDate = request.CurrentDate,
                MonthsPerPeriod = request.MonthsPerPeriod
            });
            periods.Reverse();
            var currentPeriodStartDate = periods
                .First(periodStartDate =>
                    periodStartDate <= request.CurrentDate);
            var previousPeriodStartDate = periods
                .FirstOrDefault(periodStartDate =>
                    periodStartDate < currentPeriodStartDate);
            var result = new GetPeriodResult();
            result.CurrentPeriod.Months = GenerateMonthsInPeriod(currentPeriodStartDate, request.MonthsPerPeriod);
            if (previousPeriodStartDate != default(DateTime))
            {
                result.PreviousPeriod.Months = GenerateMonthsInPeriod(previousPeriodStartDate, request.MonthsPerPeriod);
            }
            return result;
        }

        private static List<DateTime> GenerateMonthsInPeriod(DateTime startDate, int monthsPerPeriod){
            return Enumerable.Range(0, monthsPerPeriod)
                .Select(month => startDate.AddMonths(month))
                .ToList();
        }

        public static List<DateTime> GeneratePeriods(GeneratePeriodRequest request)
        {
            var result = new List<DateTime>(){
                request.StartDate
            };
            do
            {
                var nextPeriod = request.StartDate
                    .AddMonths(request.MonthsPerPeriod);
                result.Add(nextPeriod);
                request.StartDate = nextPeriod;
            }
            while (request.StartDate < request.EndDate);
            return result;
        }

    }
}
