using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NLog;

namespace Sriracha.Deploy.Data.Impl
{
	public class RegexResolver : IRegexResolver
	{
		private readonly Logger _logger;
		private readonly Regex _fieldReplacementRegex;

		public RegexResolver(Logger logger)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_fieldReplacementRegex = new Regex(@"[a-zA-Z\d]+<</(.+)/", RegexOptions.Compiled);
		}
		
		public void ResolveValues(object dataObject)
		{
			if(dataObject == null)
			{
				throw new ArgumentNullException();
			}
			_logger.Trace("RegexResolver.ResolveValues for {0}: {1}",dataObject.GetType().FullName, dataObject.ToJson());
			var propInfoList = dataObject.GetType().GetProperties();
			foreach(var propInfo in propInfoList)
			{
				var objectValue = propInfo.GetValue(dataObject, null);
				if (objectValue == null)
				{
					_logger.Trace("{0}.{1} is null", dataObject.GetType().FullName, propInfo.Name);
				}
				else 
				{
					string stringValue = objectValue.ToString();
					if (!_fieldReplacementRegex.IsMatch(stringValue))
					{
						_logger.Trace("{0} is not a replacement field", propInfo.Name);
					}
					else 
					{
						string sourceFieldName = stringValue.Substring(0, stringValue.IndexOf("<<"));
						string regexString = stringValue.Substring(stringValue.IndexOf("<<")+2);
						regexString = regexString.Substring(1, regexString.Length-2);	//pull off the leading and trailing /
					
						var sourceFieldPropInfo = dataObject.GetType().GetProperty(sourceFieldName);
						if(sourceFieldPropInfo == null)
						{
							sourceFieldPropInfo = dataObject.GetType().GetProperties().FirstOrDefault(i=>sourceFieldName.Equals(i.Name, StringComparison.CurrentCultureIgnoreCase));
						}
						if(sourceFieldPropInfo == null)
						{
							throw new Exception(string.Format("Source field {0} not found", sourceFieldName));
						}
						object sourceValueObject= sourceFieldPropInfo.GetValue(dataObject, null);
						if(sourceValueObject == null)
						{
							_logger.Trace("Source field {0} is null, setting {1} to null", sourceFieldName, propInfo.Name);
							propInfo.SetValue(dataObject, null, null);
						}
						else 
						{
							string sourceValue = sourceValueObject.ToString();
							var regexObject = new Regex(regexString);
							var match = regexObject.Match(sourceValue);
							if(match == null || !match.Success)
							{
								_logger.Trace("Regex {0} doesn't match source value {1}, setting {2} to null", regexString, sourceValue, propInfo.Name);
								propInfo.SetValue(dataObject, null, null);
							}
							else 
							{
								string matchedValue = match.Captures[0].Value;
								_logger.Trace("Regex {0} matches source value {1}, setting {2} to {3}", regexString, sourceValue, propInfo.Name, matchedValue);
								propInfo.SetValue(dataObject, matchedValue, null);
							}
						}
					}
				}
			}
		}
	}
}
