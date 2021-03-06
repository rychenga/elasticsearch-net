﻿using System;
using System.IO;
using System.Linq;
using Tests.Framework.Versions;

namespace Tests.Framework.Configuration
{
	public class YamlConfiguration : TestConfigurationBase
	{
		public override bool TestAgainstAlreadyRunningElasticsearch { get; protected set; } = true;
		public override ElasticsearchVersion ElasticsearchVersion { get; protected set; }
		public override bool ForceReseed { get; protected set; } = true;
		public override TestMode Mode { get; protected set; } = TestMode.Unit;


		public YamlConfiguration(string configurationFile)
		{
			if (!File.Exists(configurationFile)) return;

			var config = File.ReadAllLines(configurationFile)
				.Where(l=>!l.Trim().StartsWith("#"))
				.ToDictionary(ConfigName, ConfigValue);

			this.Mode = GetTestMode(config["mode"]);
			this.ElasticsearchVersion = new ElasticsearchVersion(config["elasticsearch_version"]);
			this.ForceReseed = bool.Parse(config["force_reseed"]);
			this.TestAgainstAlreadyRunningElasticsearch = bool.Parse(config["test_against_already_running_elasticsearch"]);
		}

		private static string ConfigName(string configLine) => Parse(configLine, 0);
		private static string ConfigValue(string configLine) => Parse(configLine, 1);
		private static string Parse(string configLine, int index) => configLine.Split(':')[index].Trim(' ');

		private static TestMode GetTestMode(string mode)
		{
			switch(mode)
			{
				case "unit":
				case "u":
					return TestMode.Unit;
				case "integration":
				case "i":
					return TestMode.Integration;
				case "mixed":
				case "m":
					return TestMode.Mixed;
				default:
					throw new ArgumentException($"Unknown test mode: {mode}");
			}
		}
	}
}
