using System;

namespace NCrunch.Framework
{
	public static class NCrunchEnvironment
	{
		public static bool NCrunchIsResident()
		{
			return Environment.GetEnvironmentVariable("NCrunch") == "1";
		}

		public static string GetOriginalSolutionPath()
		{
			return Environment.GetEnvironmentVariable("NCrunch.OriginalSolutionPath");
		}

		public static string GetOriginalProjectPath()
		{
			return Environment.GetEnvironmentVariable("NCrunch.OriginalProjectPath");
		}

	    public static string[] GetImplicitlyReferencedAssemblyLocations()
	    {
	        var dependencies = Environment.GetEnvironmentVariable("NCrunch.ImplicitlyReferencedAssemblyLocations");
	        if (dependencies == null)
	            return null;

	        return dependencies.Split(';');
	    }

	    public static string[] GetAllAssemblyLocations()
	    {
            var dependencies = Environment.GetEnvironmentVariable("NCrunch.AllAssemblyLocations");
            if (dependencies == null)
                return null;

            return dependencies.Split(';');
        }
	}
}
