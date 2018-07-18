using System;
using MarketMadness.Scraper;

namespace MarketMadness.Main
{
	public class Startup
	{
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length > 0)
				{
					if (args[0] == "train" && args.Length == 1)
						Train.Run(false, null, null);

					else if (args[0] == "train" && args.Length == 3)
						Train.Run(true, double.Parse(args[1]), double.Parse(args[2]));

					else
					{
						Console.WriteLine(@"To use command line arguments,");
						Console.WriteLine(@"For Resilient training: MarketMadness train");
						Console.WriteLine(@"For sigmoid BPN:        MarketMadness train [learn rate] [momentum]");
						return;
					}
				}
				else
				{
					var startDate = GetQuotes.GetMaxDate();

					string choice;

					Console.WriteLine(@"1 - Scrape Company Data");
					Console.WriteLine(@"2 - Get Quotes (last day: " + startDate.ToString("D"));
					Console.WriteLine(@"3 - Pull down Historical Raptures");
					Console.WriteLine(@"4 - Check Weather");
					Console.WriteLine(@"5 - Start Oil Slick");
					Console.WriteLine(@"6 - Build New Training Set");
					Console.WriteLine(@"7 - Prune against Training Set");
					Console.WriteLine(@"8 - Train");
					Console.WriteLine(@"9 - Make Yourself Rich (Run Network on current data)");

					Console.Write(@"Enter Choice: ");

					choice = Console.ReadLine();

					switch (choice)
					{
						case "1":
							ScrapeNYSE.Start();
							CompanyStatScraper.Start();
							break;
						case "2":
							GetQuotes.PullAll(startDate.AddDays(1), DateTime.Now);
							break;
						case "3":
							PullDownRapture.Historical();
							break;
						case "4":
							RainMaker.Dance();
							break;
						case "5":
							OilSlick.PerformOilSlick();
							break;
						case "6":
							CreateTraining.Create();
							break;
						case "7":
							PruneTraining.Run();
							break;
						case "8":

							Console.Write(@"(B)PN or (R)esilient: ");
							string lm = Console.ReadLine();

							if (lm == "R")
								Train.Run(false, null, null);

							else if (lm == "B")
							{
								Console.Write(@"Learning Rate [.0001]: ");
								var rate = Console.ReadLine();
								
								Console.Write(@"Momentum [0.0]: ");
								var momentum = Console.ReadLine();

								Train.Run(true, rate == "" ? 0.0001 : double.Parse(rate), momentum == "" ? 0.0 : double.Parse(momentum));
							}
							else
								Console.WriteLine(@"B or R Only!!!");

							break;
						case "9":
							MakeMeRich.Go();
							break;
						
						default:
							throw new Exception(choice + " is not a valid entry!!!");
					}

					Console.WriteLine(@"Done");
				}
			}
			catch (Exception ex)
			{
				while (ex != null)
				{
					Train.Log("\nError:\n");
					Train.Log(ex.Message);
					Train.Log(ex.StackTrace);

					ex = ex.InnerException;
				}
			}
		}
	}
}