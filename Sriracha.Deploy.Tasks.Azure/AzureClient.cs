using MMDB.Shared;
using Sriracha.Deploy.Tasks.Azure.AzureDto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure
{
    public class AzureClient
    {
        private readonly string _subscriptionIdentifier;
        private readonly string _managementCertificate;

        public AzureClient(string subscriptionIdentifier, string managementCertificate)
        {
            _subscriptionIdentifier = subscriptionIdentifier;
            _managementCertificate = managementCertificate;
        }

        public List<HostedService> GetCloudServiceList()
        {
            string listUrl = string.Format("https://management.core.windows.net/{0}/services/hostedservices", _subscriptionIdentifier);
            var request = CreateRequest(listUrl, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(HostedServiceListResponse), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (HostedServiceListResponse)serializer.Deserialize(new StringReader(xml));
                return returnValue.HostedServiceList;
            }
        }


        private static HttpWebRequest CreateRequest(string url, string managementCertificate)
        {
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(managementCertificate));
            var request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));

            request.Headers.Add("x-ms-version", "2010-10-28");
            request.Method = "GET";
            request.ContentType = "application/xml";

            // Attach the certificate to the request.
            request.ClientCertificates.Add(cert);

            return request;
        }

    }
}
