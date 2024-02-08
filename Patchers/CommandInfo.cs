using System;
using System.Collections.Generic;
using System.Text;

namespace ShipMaid.Patchers
{
	public class CommandInfo
	{
		/// <summary>
		/// The category to display info on command
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// The description of what the command does
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// A function that should return the string that is wanted to be displayed
		/// </summary>
		public Func<string> DisplayTextSupplier { get; set; }

		/// <summary>
		/// The title of the description. e.g For '>SCAN,'SCAN' would be the title
		/// </summary>
		public string Title { get; set; } = null;

		// For callbacks

		/// <summary>
		/// The node that will trigger your callback function
		/// </summary>
		public TerminalNode TriggerNode { get; set; }
	}
}