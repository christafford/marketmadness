using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketMadness.Util
{
	public class MathUtil
	{
		public static double GetStandardDeviation(List<double> doubleList, double? precomputedAverage)
		{
			var average = precomputedAverage == null ? doubleList.Average() : (double)precomputedAverage;
			var sumOfDerivation = doubleList.Sum(x => Math.Pow(x, 2));
			var sumOfDerivationAverage = sumOfDerivation / doubleList.Count;
			
			return Math.Sqrt(sumOfDerivationAverage - Math.Pow(average, 2));
		}
	}
}
