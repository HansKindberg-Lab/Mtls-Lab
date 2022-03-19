using System;
using System.ComponentModel;
using System.Net;
using Application.Models.ComponentModel;
using Application.Models.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public class Startup
	{
		#region Constructors

		public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
		{
			this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
			this.HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
		}

		#endregion

		#region Properties

		protected internal virtual IConfiguration Configuration { get; }
		protected internal virtual IWebHostEnvironment HostEnvironment { get; }

		#endregion

		#region Methods

		public virtual void Configure(IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder
				.UseDeveloperExceptionPage()
				.UseForwardedHeaders()
				.UseStaticFiles()
				.UseRouting()
				.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			TypeDescriptor.AddAttributes(typeof(IPAddress), new TypeConverterAttribute(typeof(IpAddressTypeConverter)));
			TypeDescriptor.AddAttributes(typeof(IPNetwork), new TypeConverterAttribute(typeof(IpNetworkTypeConverter)));

			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.AllowedHosts.Clear();
				options.KnownNetworks.Clear();
				options.KnownProxies.Clear();
			});

			var forwardedHeadersSection = this.Configuration.GetSection("ForwardedHeaders");
			services.Configure<ForwardedHeadersOptions>(forwardedHeadersSection);

			var navigationSection = this.Configuration.GetSection("Navigation");
			services.Configure<NavigationOptions>(navigationSection);

			services.AddControllersWithViews();
		}

		#endregion
	}
}