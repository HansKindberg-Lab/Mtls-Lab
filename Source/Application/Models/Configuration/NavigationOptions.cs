using System.Collections.Generic;

namespace Application.Models.Configuration
{
	public class NavigationOptions
	{
		#region Properties

		public virtual IList<NavigationItemOptions> Items { get; } = new List<NavigationItemOptions>();

		#endregion
	}
}