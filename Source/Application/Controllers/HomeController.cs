using System;
using System.Threading.Tasks;
using Application.Models.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Application.Controllers
{
	public class HomeController : Controller
	{
		#region Constructors

		public HomeController(ILoggerFactory loggerFactory)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
		}

		#endregion

		#region Properties

		protected internal virtual ILogger Logger { get; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> Index()
		{
			var model = new HomeViewModel
			{
				ClientCertificate = this.HttpContext.Connection.ClientCertificate
			};

			return await Task.FromResult(this.View(model));
		}

		#endregion
	}
}