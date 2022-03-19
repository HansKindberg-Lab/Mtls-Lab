# Mtls-Lab

This is a web-application to lab with mTLS. It's about configuring an deploying.

![.github/workflows/Docker-deploy.yml](https://github.com/HansKindberg-Lab/Mtls-Lab/actions/workflows/Docker-deploy.yml/badge.svg)

Web-application, without configuration, pushed to Docker Hub: https://hub.docker.com/r/hanskindberg/mtls-lab

## Configuration

- [Example](/Source/Application/appsettings.Development.json)

## Development

To be able to run in Visual Studio on Windows you need to add the following to **C:\Windows\System32\drivers\etc\hosts**:

	127.0.0.1 mtls-lab.local
	127.0.0.1 mtls.mtls-lab.local

## Why

I want to deploy an [IdentityServer](https://github.com/DuendeSoftware/IdentityServer)-implementation to OpenShift (Kubernetes). The [IdentityServer](https://github.com/DuendeSoftware/IdentityServer)-solution needs to be able to handle client-certificate-authentication (mTLS).

Both the [IdentityServer](https://github.com/DuendeSoftware/IdentityServer)-way:

- [TLS Client Certificates](https://docs.duendesoftware.com/identityserver/v5/tokens/authentication/mtls/)

and interactively (as a user, in the browser, you choose to sign in with a certificate and you get a certificate-picker pop-up)

- https://github.com/HansKindberg/IdentityServer-Extensions/blob/master/Source/Implementations/Identity-Server/Application/Controllers/AuthenticateController.cs#L89

I have struggled a lot to find a solution for this. With Windows IIS I know how to do it, https://github.com/HansKindberg/IdentityServer-Extensions/blob/master/Source/Implementations/Identity-Server/Application/Web.config:

	<configuration>
		<location path="Authenticate/Certificate">
			<system.webServer>
				<handlers>
					<add modules="AspNetCoreModuleV2" name="aspNetCore" path="*" resourceType="Unspecified" verb="*" />
				</handlers>
				<security>
					<access sslFlags="Ssl, SslNegotiateCert, SslRequireCert" />
				</security>
			</system.webServer>
		</location>
		<location path="connect/mtls">
			<system.webServer>
				<handlers>
					<add modules="AspNetCoreModuleV2" name="aspNetCore" path="*" resourceType="Unspecified" verb="*" />
				</handlers>
				<security>
					<access sslFlags="Ssl, SslNegotiateCert, SslRequireCert" />
				</security>
			</system.webServer>
		</location>
	</configuration>

Path-based mTLS seems to be supported in Apache to.

The IdentityServer-people suggests NGINX as a good choice, using sub-domain for mTLS:

- https://leastprivilege.com/2020/02/07/mutual-tls-and-proof-of-possession-access-tokens-part-1-setup/

Then I asked a question about how to laborate with NGINX locally on Windows:

- https://github.com/DuendeSoftware/IdentityServer/discussions/823

They answered with a second opportunity, to do it with Kestrel, and then I started to think of this solution.

The idea is to configure the IdentityServer-application something like this:

	{
		"AllowedHosts": "id.example.org;mtls.id.example.org",
		"Kestrel": {
			"Endpoints": {
				"Default": {
					"Sni": {
						"mtls.id.example.org": {
							"ClientCertificateMode": "RequireCertificate"
						},
						"*": {
							"ClientCertificateMode": "NoCertificate"
						} 
					},
					"Url": "https://*:5000"
				}
			}
		}
		...
	}

Then you create two CNAME's in you DNS and point them to an IP handled by your OpenShift-cluster:

- id.example.org
- mtls.id.example.org

In OpenShift you have a route ................................... more to come

## How to configure Kestrel

Many examples are through code but I want to do it in appsettings.json.

- [Configure endpoints for the ASP.NET Core Kestrel web server](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints)