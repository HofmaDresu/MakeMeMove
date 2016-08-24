using System;

namespace MacroEatMobile.Core
{

	public class MacrosVariance
	{
		public decimal CaloriesMin { get; set; }
		public decimal CaloriesMax { get; set; }

		public decimal ProteinMin { get; set; }
		public decimal ProteinMax { get; set; }

		public decimal FatMin { get; set; }
		public decimal FatMax { get; set; }

		public decimal CarbohydratesMin { get; set; }
		public decimal CarbohydratesMax { get; set; }

		public string Type { get; set; }
	}
}

