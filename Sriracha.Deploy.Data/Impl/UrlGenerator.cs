using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class UrlGenerator : IUrlGenerator
	{
		public string DeployStatusUrl(string deployBatchRequestId)
		{
			string url = GetRootUrl();
			if(!url.EndsWith("/"))
			{
				url += "/";
			}
			url += "#/deploy/batchStatus/" + deployBatchRequestId;
			return url;
		}

		private string GetRootUrl()
		{
			string rootUrl = ConfigurationManager.AppSettings["SiteUrl"];
			if(string.IsNullOrEmpty(rootUrl))
			{
				throw new ConfigurationErrorsException("Missing required appSetting SiteUrl");
			}
			return rootUrl;
		}
	}
}
