using System;
using System.IO;
using Encog;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Pattern;
using Encog.Neural.Networks.Prune;
using Encog.Persist;

namespace MarketMadness.Main
{
	public class PruneTraining
	{
		public static void Run()
		{
			Log("Loading training data");

			var encog = new EncogPersistedCollection("market-training.dat", FileMode.Open);
			var trainingSet = (BasicMLDataSet) encog.Find("market-training");
			
			Log("Figuring out best system");

			var pattern = new FeedForwardPattern
			              	{
			              		InputNeurons = trainingSet.InputSize,
			              		OutputNeurons = trainingSet.IdealSize,
			              		ActivationFunction = new ActivationTANH()
			              	};

			var prune = new PruneIncremental(trainingSet, pattern, 100, new ConsoleStatusReportable());

			prune.AddHiddenLayer(1, 50);
			prune.AddHiddenLayer(0, 50);

			prune.Process();

			Log("Done!!!!");
		}
		public static void Log(string str)
		{
			File.AppendAllText("prune.txt", DateTime.Now + @": " + str + "\r\n");
		}
	}
}
