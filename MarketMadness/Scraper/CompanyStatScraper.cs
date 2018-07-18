using System;
using System.Linq;
using MarketMadness.Agent;
using MarketMadness.DataLayer;

namespace MarketMadness.Scraper
{
	public class CompanyStatScraper
	{
		public static void Start()
		{
			ModelMadness madness = new ModelMadness();
			string[] stockNames = (from q in madness.StockNames where q.HasData != null && ((bool) q.HasData) orderby q.Stock select q.Stock).ToArray();
			
			foreach (string stock in stockNames)
			{
				GrabProfile(stock);
				GrabStats(stock);
			}

			Console.WriteLine("Done");
		}

		private static void GrabProfile(string symbol)
		{
			Console.Write("Grabbing Profile for " + symbol);

			string url = @"http://finance.yahoo.com/q/pr?s=" + symbol;

			AgentSession session = new AgentSession();
			AgentAction action = new AgentAction(url, false);
			AgentDocument document;

			try
			{
				document = AgentHandler.Instance.PerformAction(session, action);
			}
			catch
			{
				return;
			}

			string doc = document.ResponseString.ToLower();

			var extractor = new ProgressiveDataExtractor(doc, ">details<");

			if (extractor.NotFound)
			{
				Console.WriteLine(" - not found");
				return;
			}

			Console.WriteLine(" - found");

			ModelMadness madness = new ModelMadness();

			string sector = extractor.ExtractString("sector");
			Sector dbSector = null;
			Industry dbIndustry = null;

			if (sector != null && sector != "n/a")
			{
				sector = sector.Replace("&amp;", "&");

				string industry = extractor.ExtractString("industry").Replace("&amp;", "&");

				var obj = (from q in madness.Sectors where q.Sector1 == sector select q).ToArray();
				
				if (obj.Length == 1)
					dbSector = obj[0];

				else
				{
					dbSector = new Sector {Sector1 = sector};
					madness.AddToSectors(dbSector);
				}

				var obj2 = (from q in madness.Industries
				            where q.Sector.Sector1 == sector && q.Iindustry == industry
				            select q).ToArray();

				if (obj2.Length == 1)
					dbIndustry = obj2[0];

				else
				{
					dbIndustry = new Industry {Iindustry = industry, Sector = dbSector};
					madness.AddToIndustries(dbIndustry);
				}
			}

			StockName stock = (from q in madness.StockNames where q.Stock == symbol select q).First();

			CompanyProfile profile = new CompanyProfile
			                         	{
											Sector = dbSector,
											Industry = dbIndustry,
											num_employees = extractor.ExtractInt("full time employees"),
											StockName = stock,
											scrape_day = DateTime.Now.Date
			                         	};

			profile.summary = extractor.ExtractString("business summary");

			profile.cgq = extractor.ExtractDecimal("corporate governance", "<b");

			extractor.ExtractString("key executives");

			int totalAge = 0;
			int numAges = 0;

			long totalPay = 0;
			int numPays = 0;

			int? curAge;
			long? curPay;

			do
			{
				curAge = extractor.ExtractInt("yfnc_tabledata1", "</b");
				
				if (curAge > 111)
					curAge = null;

				if (curAge != null)
				{
					numAges++;
					totalAge += (int) curAge;

					curPay = extractor.ExtractLong("yfnc_tabledata1", "nowrap");

					if (curPay != null)
					{
						numPays++;
						totalPay += (long)curPay;
					}
				}
			} while (curAge != null);

			profile.avg_executive_age = totalAge == 0 ? null : (int?)totalAge / numAges;
			profile.avg_executive_pay = totalPay == 0 ? null : (long?)totalPay / numPays;

			madness.AddToCompanyProfiles(profile);
			madness.SaveChanges();
		}

		private static void GrabStats(string symbol)
		{
			Console.Write("Grabbing Key Stats for " + symbol);

			string url = @"http://finance.yahoo.com/q/ks?s=" + symbol;

			AgentSession session = new AgentSession();
			AgentAction action = new AgentAction(url, false);
			AgentDocument document;
			
			try
			{
				document = AgentHandler.Instance.PerformAction(session, action);
			}
			catch
			{
				return;
			}

			var doc = document.ResponseString;

			var extractor = new ProgressiveDataExtractor(doc, "Valuation Measures");

			if (extractor.NotFound)
			{
				Console.WriteLine(" - Not found");
				return;
			}

			Console.WriteLine(" - found");
			
			ModelMadness madness = new ModelMadness();
			
			StockName stock = (from q in madness.StockNames where q.Stock == symbol select q).First();

			CompanyStat stats = new CompanyStat
			                    	{
										market_cap = extractor.ExtractLong("Market Cap"),
										enterprise_value = extractor.ExtractLong("Enterprise Value"),
										trailing_pe = extractor.ExtractDecimal("Trailing P/E"),
										forward_pe = extractor.ExtractDecimal("Forward P/E"),
										peg_ratio = extractor.ExtractDecimal("PEG Ratio"),
										price_sales = extractor.ExtractDecimal("Price/Sales"),
										price_book = extractor.ExtractDecimal("Price/Book"),
										ent_value_rev = extractor.ExtractLong("Enterprise Value/Revenue"),
										ent_value_ebitda = extractor.ExtractDecimal("Enterprise Value/EBITDA"),
										fiscal_year_ends = extractor.ExtractString("Fiscal Year Ends"),
										most_recent_quarter = extractor.ExtractString("Most Recent Quarter"),
										profit_margin = extractor.ExtractDecimal("Profit Margin"),
										operating_margin = extractor.ExtractDecimal("Operating Margin"),
										return_on_assets = extractor.ExtractDecimal("Return on Assets"),
										return_on_equity = extractor.ExtractDecimal("Return on Equity"),
										revenue = extractor.ExtractLong("Revenue"),
										revenue_per_share = extractor.ExtractDecimal("Revenue Per Share"),
										qrt_rev_growth = extractor.ExtractDecimal("Qtrly Revenue Growth"),
										gross_profit = extractor.ExtractLong("Gross Profit"),
										ebitda = extractor.ExtractLong("EBITDA"),
										net_income_a_c = extractor.ExtractLong("Net Income Avl to Common"),
										diluted_eps = extractor.ExtractDecimal("Diluted EPS"),
										qrt_earnings_growth = extractor.ExtractDecimal("Qtrly Earnings Growth"),
										total_cash = extractor.ExtractLong("Total Cash"),
										cash_per_share = extractor.ExtractDecimal("Total Cash Per Share"),
										total_debt = extractor.ExtractLong("Total Debt"),
										total_debt_equit = extractor.ExtractDecimal("Total Debt/Equity"),
										current_ratio = extractor.ExtractDecimal("Current Ratio"),
										book_value_p_share = extractor.ExtractDecimal("Book Value Per Share"),
										operating_cash_flow = extractor.ExtractLong("Operating Cash Flow"),
										levered_free_cash_flow = extractor.ExtractLong("Levered Free Cash Flow"),
										StockName = stock,
										scrape_day = DateTime.Now.Date
			                    	};

			madness.AddToCompanyStats(stats);
			madness.SaveChanges();
		}
	}
}
