using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.XmlConfigFile;

namespace Sriracha.Deploy.Data.Tests.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskTests
	{
		private class TestData
		{
			public XmlConfigFileTask Sut { get; set; }

			public static TestData Create()
			{
				TestData returnValue = new TestData()
				{
					Sut = new XmlConfigFileTask()
					{
						Options = new XmlConfigFileTaskOptions
						{
							XmlTemplate = new StringBuilder()
											.Append("<container>")
												.Append("<enviornmentValue></enviornmentValue>")
												.Append("<serverValue/>")
												.Append("<staticValue theValue=\"\"/>")
											.Append("</container>")
										.ToString(),
							XPathValueList = new List<XmlConfigFileTaskOptions.XPathValueItem>
							{
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/environmentValue",
									ConfigLevel = EnumConfigLevel.Environment,
									ValueName = Guid.NewGuid().ToString()
								},
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/serverValue",
									ConfigLevel = EnumConfigLevel.Machine,
									ValueName = Guid.NewGuid().ToString()
								},
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/staticValue/@theValue",
									ConfigLevel = EnumConfigLevel.Static,
									ValueName = Guid.NewGuid().ToString()
								}
							}
						}
					}
				};
				return returnValue;
			}

		}

		[Test]
		public void CanCreateXmlConfigTask()
		{
			string fieldName = Guid.NewGuid().ToString();
			var task = new XmlConfigFileTask()
			{
				XmlTemplate = new StringBuilder()
								.Append("<container>")
									.Append("<environmentValue></environmentValue>")
								.Append("</container>")
							.ToString(),
				Options = new XmlConfigFileTaskOptions
				{
					XPathValueList = new List<XmlConfigFileTaskOptions.XPathValueItem>
					{
						new XmlConfigFileTaskOptions.XPathValueItem 
						{
							XPath = "/container/enviornmentValue",
							ConfigLevel = EnumConfigLevel.Environment,
							ValueName = fieldName
						}
					}
				}
			};
			//var x = Task
		}

		[Test]
		public void CanFindEnvironmentValues()
		{
			var testData = TestData.Create();
			var environmentSettings = testData.Sut.GetEnvironmentTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.Sut.GetEnvironmentTaskParameterList().First().FieldName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}
	}
}
