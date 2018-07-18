using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketMadness.DataLayer
{
	public class InputOutputMadness
	{
		public static int[] InputAndHiddenLayerInfo
		{
			get
			{
				return new[] {27}; //, 27, 13};
			}
		}

		public static double[] CreateInputs(StockMarket.WorkableStockInfo info)
		{
			return new[]
					{
						(double) info.AdjCloseROCPast5,
						(double) info.AdjCloseROCPast15,
						(double) info.AdjCloseROCPast25,
						(double) info.AdjCloseROCPast50,
						(double) info.AdjCloseROCvsIndROCPast5,
						(double) info.AdjCloseROCvsIndROCPast15,
						(double) info.AdjCloseROCvsIndROCPast25,
						(double) info.AdjCloseROCvsIndROCPast50,
						(double) info.AdjCloseROCvsSecROCPast5,
						(double) info.AdjCloseROCvsSecROCPast15,
						(double) info.AdjCloseROCvsSecROCPast25,
						(double) info.AdjCloseROCvsSecROCPast50,
						(double) info.VolumeROCPast5,
						(double) info.VolumeROCPast15,
						(double) info.VolumeROCPast25,
						(double) info.VolumeROCPast50,
						(double) info.OilChangePast5,
						(double) info.OilChangePast15,
						(double) info.OilChangePast25,
						(double) info.OilChangePast50,
						info.PEIndustryDevs,
						info.VolumeIndustryDevs,
						info.ExecAgeIndustryDevs,
						info.EmployeesIndustryDevs,
						info.NetIncomeIndustryDevs,
						info.TotalDebtIndustryDevs,
						info.EnterpriseValueIndDevs
						//info.IsTechnology ? 1 : 0,
						//info.IsBasicMaterials ? 1 : 0,
						//info.IsServices ? 1 : 0,
						//info.IsFinancial ? 1 : 0,
						//info.IsIndustrialGoods ? 1 : 0,
						//info.IsConsumerGoods ? 1 : 0,
						//info.IsHealthcare ? 1 : 0,
						//info.IsUtilities ? 1 : 0,
						//info.IsConglomerates ? 1 : 0,
						//info.WeatherChange
					};
		}

		public static double CreateOutput(StockMarket.WorkableStockInfo info)
		{
			return (double) info.CloseROCNext20;
		}
	}
}
