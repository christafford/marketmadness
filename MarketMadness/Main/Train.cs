using System;
using System.IO;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Persist;
using MarketMadness.DataLayer;

namespace MarketMadness.Main
{
	public class Train
	{
		public static void Run(bool useBPN, double? learnRate, double? momentum)
		{
			Log("Using " + 
				(useBPN ? "Back-Propagation training with learnRate = " + learnRate + ", momentum = " + momentum 
						: "Resilient training"));

			Log("Loading training data");

			var encog = new EncogPersistedCollection("market-training.dat", FileMode.Open);
			var trainingSet = (BasicMLDataSet) encog.Find("market-training");
			BasicNetwork network;

			if (File.Exists("market-network.dat"))
			{
				Log("Loading network");
				encog = new EncogPersistedCollection("market-network.dat", FileMode.Open);
				network = (BasicNetwork)encog.Find("market-network");
			}
			else
			{
				Log("Creating new network");

				network = new BasicNetwork();

				// input & hiddens
				foreach (var num in InputOutputMadness.InputAndHiddenLayerInfo)
					network.AddLayer(new BasicLayer(num));

				// output
				network.AddLayer(new BasicLayer(1));

				network.Structure.FinalizeStructure();
				network.Reset();
			}

			BasicTraining train;

			if (useBPN)
				train = new Backpropagation(network, trainingSet, (double) learnRate, (double) momentum);
			else
				train = new ResilientPropagation(network, trainingSet);

			//train = new NeuralGeneticAlgorithm(network, new FanInRandomizer(), new TrainingSetScore(trainingSet), 500, 0.1, 0.25);

			Log("Beginning Training");

			int epoch = 1;
			do
			{
				train.Iteration();
				Log("Epoch #" + epoch + " Error:" + train.Error);
				epoch++;

				if (epoch % 100 == 0)
				{
					Log("Saving current data");
					
					encog = new EncogPersistedCollection("market-network.dat", FileMode.Create);
					encog.Create();
					encog.Add("market-network", network);
				}
			}
			while (true);
		}

		public static void Log(string str)
		{
			Console.WriteLine(str);
			File.AppendAllText("train.txt", DateTime.Now + ": " + str + "\r\n");
		}
	}
}
