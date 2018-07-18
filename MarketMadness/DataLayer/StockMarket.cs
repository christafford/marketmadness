using System;
using System.Collections.Generic;
using System.Linq;
using MarketMadness.Util;

namespace MarketMadness.DataLayer
{
	public class StockMarket
	{
		// this is the end product
		public class WorkableStockInfo
		{
			// top level stats
			public simplestockquote ViewToday;
			public simplestockquote View5DaysAgo;
			public simplestockquote View15DaysAgo;
			public simplestockquote View25DaysAgo;
			public simplestockquote View50DaysAgo;

			public decimal IndustryAverage;
			public decimal IndustryAverage5DaysAgo;
			public decimal IndustryAverage15DaysAgo;
			public decimal IndustryAverage25DaysAgo;
			public decimal IndustryAverage50DaysAgo;

			public decimal SectorAverage;
			public decimal SectorAverage5DaysAgo;
			public decimal SectorAverage15DaysAgo;
			public decimal SectorAverage25DaysAgo;
			public decimal SectorAverage50DaysAgo;

			public decimal AdjCloseNext20;

			public double PEIndustryDevs;
			public double VolumeIndustryDevs;
			public double ExecAgeIndustryDevs;
			public double EmployeesIndustryDevs;
			public double NetIncomeIndustryDevs;
			public double TotalDebtIndustryDevs;
			public double EnterpriseValueIndDevs;

			public bool IsTechnology;
			public bool IsBasicMaterials;
			public bool IsServices;
			public bool IsFinancial;
			public bool IsIndustrialGoods;
			public bool IsConsumerGoods;
			public bool IsHealthcare;
			public bool IsUtilities;
			public bool IsConglomerates;

			public int WeatherChange;

			// computed statistics
			public decimal AdjCloseROCPast5
			{
				get
				{
					return (ViewToday.adjusted_close - View5DaysAgo.adjusted_close) / ViewToday.adjusted_close;
				}
			}

			public decimal AdjCloseROCPast15
			{
				get
				{
					return (ViewToday.adjusted_close - View15DaysAgo.adjusted_close) / ViewToday.adjusted_close;
				}
			}

			public decimal AdjCloseROCPast25
			{
				get
				{
					return (ViewToday.adjusted_close - View25DaysAgo.adjusted_close) / ViewToday.adjusted_close;
				}
			}

			public decimal AdjCloseROCPast50
			{
				get
				{
					return (ViewToday.adjusted_close - View50DaysAgo.adjusted_close) / ViewToday.adjusted_close;
				}
			}

			public decimal AdjCloseROCvsIndROCPast5
			{
				get
				{
					return ((ViewToday.adjusted_close - View5DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((IndustryAverage - IndustryAverage5DaysAgo) / IndustryAverage);
				}
			}

			public decimal AdjCloseROCvsIndROCPast15
			{
				get
				{
					return ((ViewToday.adjusted_close - View15DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((IndustryAverage - IndustryAverage15DaysAgo) / IndustryAverage);
				}
			}

			public decimal AdjCloseROCvsIndROCPast25
			{
				get
				{
					return ((ViewToday.adjusted_close - View25DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((IndustryAverage - IndustryAverage25DaysAgo) / IndustryAverage);
				}
			}


			public decimal AdjCloseROCvsIndROCPast50
			{
				get
				{
					return ((ViewToday.adjusted_close - View50DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((IndustryAverage - IndustryAverage50DaysAgo) / IndustryAverage);
				}
			}

			public decimal AdjCloseROCvsSecROCPast5
			{
				get
				{
					return ((ViewToday.adjusted_close - View5DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((SectorAverage - SectorAverage5DaysAgo) / SectorAverage);
				}
			}


			public decimal AdjCloseROCvsSecROCPast15
			{
				get
				{
					return ((ViewToday.adjusted_close - View15DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((SectorAverage - SectorAverage15DaysAgo) / SectorAverage);
				}
			}

			public decimal AdjCloseROCvsSecROCPast25
			{
				get
				{
					return ((ViewToday.adjusted_close - View25DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((SectorAverage - SectorAverage25DaysAgo) / SectorAverage);
				}
			}

			public decimal AdjCloseROCvsSecROCPast50
			{
				get
				{
					return ((ViewToday.adjusted_close - View50DaysAgo.adjusted_close) / ViewToday.adjusted_close) -
							((SectorAverage - SectorAverage50DaysAgo) / SectorAverage);
				}
			}

			public decimal VolumeROCPast5
			{
				get
				{
					return (ViewToday.volume - View5DaysAgo.volume) / (decimal) ViewToday.volume;
				}
			}

			public decimal VolumeROCPast15
			{
				get
				{
					return (ViewToday.volume - View15DaysAgo.volume) / (decimal) ViewToday.volume;
				}
			}

			public decimal VolumeROCPast25
			{
				get
				{
					return (ViewToday.volume - View25DaysAgo.volume) / (decimal) ViewToday.volume;
				}
			}

			public decimal VolumeROCPast50
			{
				get
				{
					return (ViewToday.volume - View50DaysAgo.volume) / (decimal) ViewToday.volume;
				}
			}

			public decimal CloseROCNext20
			{
				get
				{
					return (AdjCloseNext20 - ViewToday.adjusted_close) / ViewToday.adjusted_close;
				}
			}

			public decimal OilChangePast5
			{
				get
				{
					return (ViewToday.oil_price - View5DaysAgo.oil_price) / ViewToday.oil_price;
				}
			}
			
			public decimal OilChangePast15
			{
				get
				{
					return (ViewToday.oil_price - View15DaysAgo.oil_price) / ViewToday.oil_price;
				}
			}
			
			public decimal OilChangePast25
			{
				get
				{
					return (ViewToday.oil_price - View25DaysAgo.oil_price) / ViewToday.oil_price;
				}
			}
			
			public decimal OilChangePast50
			{
				get
				{
					return (ViewToday.oil_price - View50DaysAgo.oil_price) / ViewToday.oil_price;
				}
			}
		}

		private class DataForDay
		{
			public class Averagerator
			{
				private int Count = 0;
				private decimal RunningTotal = 0;
				private decimal? Avg = null;

				public void Inc(decimal amount)
				{
					Count++;
					RunningTotal += amount;
				}

				public decimal GetAvg()
				{
					if (Avg == null)
						Avg = RunningTotal / Count;

					return (decimal) Avg;
				}
			}

			public class Deviator
			{
				private List<double> Elements = new List<double>();
				private double? StandardDev = null;
				private double Average;

				public void Add(double element)
				{
					Elements.Add(element);
				}

				public double GetStandardDevs(double element)
				{
					if (Elements.Count == 1)
						return 0;

					if (StandardDev == null)
					{
						Average = Elements.Average();
						StandardDev = MathUtil.GetStandardDeviation(Elements, Average);
					}

					if (StandardDev == 0)
						return 0;

					double stDev = (element - Average) / (double) StandardDev;

					return stDev;
				}
			}

			public IDictionary<int, simplestockquote> ByStockID = new Dictionary<int, simplestockquote>();
			public IDictionary<int, Averagerator> IndustryAVG = new Dictionary<int, Averagerator>();
			public IDictionary<int, Averagerator> SectorAVG = new Dictionary<int, Averagerator>();
			public IDictionary<int, Deviator> IndustryPE = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryVolume = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryAvgExecAge = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryEmployees = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryNetIncome = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryTotalDebt = new Dictionary<int, Deviator>();
			public IDictionary<int, Deviator> IndustryEntValue = new Dictionary<int, Deviator>();

			public void AddStat(simplestockquote stat)
			{
				ByStockID[stat.id] = stat;

				if (!IndustryAVG.ContainsKey((int)stat.industry_id))
					IndustryAVG[(int)stat.industry_id] = new Averagerator();

				IndustryAVG[(int)stat.industry_id].Inc(stat.adjusted_close);

				if (!SectorAVG.ContainsKey((int)stat.sector_id))
					SectorAVG[(int)stat.sector_id] = new Averagerator();

				SectorAVG[(int)stat.sector_id].Inc(stat.adjusted_close);

				if (!IndustryPE.ContainsKey((int)stat.industry_id))
					IndustryPE[(int)stat.industry_id] = new Deviator();

				IndustryPE[(int)stat.industry_id].Add((double) stat.forward_pe);

				if (!IndustryVolume.ContainsKey((int)stat.industry_id))
					IndustryVolume[(int)stat.industry_id] = new Deviator();

				IndustryVolume[(int)stat.industry_id].Add(stat.volume);

				if (!IndustryAvgExecAge.ContainsKey((int)stat.industry_id))
					IndustryAvgExecAge[(int)stat.industry_id] = new Deviator();

				IndustryAvgExecAge[(int)stat.industry_id].Add((double) stat.avg_executive_age);

				if (!IndustryEmployees.ContainsKey((int)stat.industry_id))
					IndustryEmployees[(int)stat.industry_id] = new Deviator();

				IndustryEmployees[(int)stat.industry_id].Add((double)stat.num_employees);

				if (!IndustryNetIncome.ContainsKey((int)stat.industry_id))
					IndustryNetIncome[(int)stat.industry_id] = new Deviator();

				IndustryNetIncome[(int)stat.industry_id].Add((double)stat.net_income_a_c);

				if (! IndustryTotalDebt.ContainsKey((int)stat.industry_id))
					IndustryTotalDebt[(int)stat.industry_id] = new Deviator();

				IndustryTotalDebt[(int)stat.industry_id].Add((double)stat.total_debt);

				if (!IndustryEntValue.ContainsKey((int)stat.industry_id))
					IndustryEntValue[(int)stat.industry_id] = new Deviator();

				IndustryEntValue[(int)stat.industry_id].Add((double)stat.enterprise_value);
			}
		}

		private IDictionary<int, DataForDay> SnapshotByDay = new Dictionary<int, DataForDay>();

		private int DayCount = -1;

		public void Init(bool forTraining)
		{
			Console.WriteLine("Retrieving madness from database...");

			ModelMadness madness = new ModelMadness();
			madness.CommandTimeout = 0;

			simplestockquote[] stats;
			
			if (forTraining)
				stats = (madness.simplestockquotes.Where(q => q.day > new DateTime(2006, 1, 1))
																 .OrderBy(q => q.day))
																 .ToArray();
			else 
				stats = (madness.simplestockquotes.Where(q => q.day > new DateTime(2010, 1, 1) &&
															  q.day <= new DateTime(2010, 12, 09))
																 .OrderBy(q => q.day))
																 .ToArray();
			long lastDay = 0;
			DataForDay currentDay = null;

			// these are in order, from oldest to newest
			foreach (simplestockquote stat in stats)
			{
				if (lastDay != stat.day.Ticks)
				{
					lastDay = stat.day.Ticks;
					currentDay = new DataForDay();

					Console.WriteLine("Analyzing Day " + stat.day.ToString("D"));

					DayCount++;
					SnapshotByDay[DayCount] = currentDay;
				}

				currentDay.AddStat(stat);
			}

			Console.WriteLine("Done retrieving");
		}

		public IList<WorkableStockInfo> GetTrainingData()
		{
			IList<WorkableStockInfo> toReturn = new List<WorkableStockInfo>();

			// don't use the first 50 or the last 20 - first ones won't have necessary historical view, and last ones won't have prediction
			for (int i = 50; i < SnapshotByDay.Count - 20; i++)
			{
				Console.WriteLine("Creating Training Data View for " + i + " of " + (SnapshotByDay.Count - 20));

				DataForDay dataToday = SnapshotByDay[i];
				DataForDay data5DaysAgo = SnapshotByDay[i - 5];
				DataForDay data15DaysAgo = SnapshotByDay[i - 15];
				DataForDay data25DaysAgo = SnapshotByDay[i - 25];
				DataForDay data50DaysAgo = SnapshotByDay[i - 50];
				DataForDay data20DaysFuture = SnapshotByDay[i + 20];

				foreach (simplestockquote stockInfo in dataToday.ByStockID.Values)
				{
					// don't add stocks to the training data if we don't have past info
					if (! data5DaysAgo.ByStockID.ContainsKey(stockInfo.id))
						continue;

					if (! data15DaysAgo.ByStockID.ContainsKey(stockInfo.id))
						continue;

					if (! data25DaysAgo.ByStockID.ContainsKey(stockInfo.id))
						continue;

					if (! data50DaysAgo.ByStockID.ContainsKey(stockInfo.id))
						continue;

					// likewise, don't use stocks without future info
					if (! data20DaysFuture.ByStockID.ContainsKey(stockInfo.id))
						continue;

					if (! (	data5DaysAgo.IndustryAVG.ContainsKey((int)stockInfo.industry_id) &&
							data15DaysAgo.IndustryAVG.ContainsKey((int)stockInfo.industry_id) &&
							data25DaysAgo.IndustryAVG.ContainsKey((int)stockInfo.industry_id) &&
							data50DaysAgo.IndustryAVG.ContainsKey((int)stockInfo.industry_id)))
						continue;

					var workableStockInfo = new WorkableStockInfo
					{
						ViewToday = stockInfo,
						View5DaysAgo = data5DaysAgo.ByStockID[stockInfo.id],
						View15DaysAgo = data15DaysAgo.ByStockID[stockInfo.id],
						View25DaysAgo = data25DaysAgo.ByStockID[stockInfo.id],
						View50DaysAgo = data50DaysAgo.ByStockID[stockInfo.id],
						IndustryAverage = dataToday.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage5DaysAgo = data5DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage15DaysAgo = data15DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage25DaysAgo = data25DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage50DaysAgo = data50DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						SectorAverage = dataToday.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage5DaysAgo = data5DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage15DaysAgo = data15DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage25DaysAgo = data25DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage50DaysAgo = data50DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						PEIndustryDevs = dataToday.IndustryPE[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.forward_pe),
						VolumeIndustryDevs = dataToday.IndustryVolume[(int) stockInfo.industry_id].GetStandardDevs(stockInfo.volume),
						ExecAgeIndustryDevs = dataToday.IndustryAvgExecAge[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.avg_executive_age),
						EmployeesIndustryDevs = dataToday.IndustryEmployees[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.num_employees),
						TotalDebtIndustryDevs = dataToday.IndustryTotalDebt[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.total_debt),
						NetIncomeIndustryDevs = dataToday.IndustryNetIncome[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.net_income_a_c),
						EnterpriseValueIndDevs = dataToday.IndustryEntValue[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.enterprise_value),
						IsBasicMaterials = stockInfo.sector_id == 102,
						IsConglomerates = stockInfo.sector_id == 109,
						IsConsumerGoods = stockInfo.sector_id == 106,
						IsFinancial = stockInfo.sector_id == 104,
						IsHealthcare = stockInfo.sector_id == 107,
						IsIndustrialGoods = stockInfo.sector_id == 105,
						IsServices = stockInfo.sector_id == 103,
						IsTechnology = stockInfo.sector_id == 101,
						IsUtilities = stockInfo.sector_id == 108,
						WeatherChange = (int)stockInfo.from_avg,
						AdjCloseNext20 = data20DaysFuture.ByStockID[stockInfo.id].adjusted_close
					};

					toReturn.Add(workableStockInfo);
				}
			}

			return toReturn;
		}

		public IList<WorkableStockInfo> GetCurrentData()
		{
			IList<WorkableStockInfo> toReturn = new List<WorkableStockInfo>();

			DataForDay dataToday = SnapshotByDay[SnapshotByDay.Count - 1];
			DataForDay data5DaysAgo = SnapshotByDay[SnapshotByDay.Count - 6];
			DataForDay data15DaysAgo = SnapshotByDay[SnapshotByDay.Count - 16];
			DataForDay data25DaysAgo = SnapshotByDay[SnapshotByDay.Count - 26];
			DataForDay data50DaysAgo = SnapshotByDay[SnapshotByDay.Count - 51];

			Console.WriteLine("Creating Prediction Data View");

			foreach (simplestockquote stockInfo in dataToday.ByStockID.Values)
			{
				// don't add stocks to the training data if we don't have past info
				if (! data5DaysAgo.ByStockID.ContainsKey(stockInfo.id))
					continue;

				if (! data15DaysAgo.ByStockID.ContainsKey(stockInfo.id))
					continue;

				if (! data25DaysAgo.ByStockID.ContainsKey(stockInfo.id))
					continue;

				if (! data50DaysAgo.ByStockID.ContainsKey(stockInfo.id))
					continue;

				var workableStockInfo = new WorkableStockInfo
					{
						ViewToday = stockInfo,
						View5DaysAgo = data5DaysAgo.ByStockID[stockInfo.id],
						View15DaysAgo = data15DaysAgo.ByStockID[stockInfo.id],
						View25DaysAgo = data25DaysAgo.ByStockID[stockInfo.id],
						View50DaysAgo = data50DaysAgo.ByStockID[stockInfo.id],
						IndustryAverage = dataToday.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage5DaysAgo = data5DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage15DaysAgo = data15DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage25DaysAgo = data25DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						IndustryAverage50DaysAgo = data50DaysAgo.IndustryAVG[(int)stockInfo.industry_id].GetAvg(),
						SectorAverage = dataToday.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage5DaysAgo = data5DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage15DaysAgo = data15DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage25DaysAgo = data25DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						SectorAverage50DaysAgo = data50DaysAgo.SectorAVG[(int)stockInfo.sector_id].GetAvg(),
						PEIndustryDevs = dataToday.IndustryPE[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.forward_pe),
						VolumeIndustryDevs = dataToday.IndustryVolume[(int) stockInfo.industry_id].GetStandardDevs(stockInfo.volume),
						ExecAgeIndustryDevs = dataToday.IndustryAvgExecAge[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.avg_executive_age),
						EmployeesIndustryDevs = dataToday.IndustryEmployees[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.num_employees),
						TotalDebtIndustryDevs = dataToday.IndustryTotalDebt[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.total_debt),
						NetIncomeIndustryDevs = dataToday.IndustryNetIncome[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.net_income_a_c),
						EnterpriseValueIndDevs = dataToday.IndustryEntValue[(int) stockInfo.industry_id].GetStandardDevs((double) stockInfo.enterprise_value),
						IsBasicMaterials = stockInfo.sector_id == 102,
						IsConglomerates = stockInfo.sector_id == 109,
						IsConsumerGoods = stockInfo.sector_id == 106,
						IsFinancial = stockInfo.sector_id == 104,
						IsHealthcare = stockInfo.sector_id == 107,
						IsIndustrialGoods = stockInfo.sector_id == 105,
						IsServices = stockInfo.sector_id == 103,
						IsTechnology = stockInfo.sector_id == 101,
						IsUtilities = stockInfo.sector_id == 108,
						WeatherChange = (int)stockInfo.from_avg
					};

				toReturn.Add(workableStockInfo);
			}

			return toReturn;
		}
	}
}
