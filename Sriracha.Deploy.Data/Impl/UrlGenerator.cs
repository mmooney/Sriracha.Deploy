using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class UrlGenerator : IUrlGenerator
	{
		private string GetRootUrl()
		{
			string rootUrl = ConfigurationManager.AppSettings["SiteUrl"];
			if (string.IsNullOrEmpty(rootUrl))
			{
				throw new ConfigurationErrorsException("Missing required appSetting SiteUrl");
			}
			return rootUrl;
		}

		private string CreateUrl(string urlSuffix)
		{
			string url = GetRootUrl();
			if (!url.EndsWith("/"))
			{
				url += "/";
			}
			url += urlSuffix;
			return url;
		}

		public string DeployStatusUrl(string deployBatchRequestId)
		{
			return CreateUrl("#/deploy/batchStatus/" + deployBatchRequestId);
		}

		public string ViewBuildUrl(string buildId)
		{
			return CreateUrl("#/build/" + buildId);
		}
	}
}
