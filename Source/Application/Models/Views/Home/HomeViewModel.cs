using System.Security.Cryptography.X509Certificates;

namespace Application.Models.Views.Home
{
	public class HomeViewModel
	{
		#region Properties

		public virtual X509Certificate2 ClientCertificate { get; set; }

		#endregion
	}
}