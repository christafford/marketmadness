using System;
using MarketMadness.DataLayer;

namespace MarketMadness.Util
{
	public class Logger
	{
		private static Logger _Instance = null;
		public static Logger Instance
		{
			get
			{
				if (_Instance == null)
					_Instance = new Logger();

				return _Instance;
			}
		}

		private Logger() { }

		public void Debug(string message)
		{
			ModelMadness madness = new ModelMadness();
			Log log = new Log
						{
							message = message,
							transaction_date = DateTime.Now
						};
			madness.AddToLogs(log);
			madness.SaveChanges();
		}

		public void Error(string message, Exception ex)
		{
			ModelMadness madness = new ModelMadness();
			Log log = new Log
						{
							message = message,
							exception_message = ex.Message,
							exception_stacktrace = ex.StackTrace,
							transaction_date = DateTime.Now
						};
			madness.AddToLogs(log);
			madness.SaveChanges();
		}
	}
}