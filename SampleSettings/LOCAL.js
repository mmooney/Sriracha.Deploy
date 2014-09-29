{
	SourceWebsitePath: ".\\Publish\\Website",
	TargetWebsitePath: "C:\\Sriracha\\Website",
	ConnectionString:"data source=(local); initial catalog=Sriracha; User ID=someUser; Password=somePassword",
	RepositoryAssemblyName: "Sriracha.Deploy.SqlServer.dll",
	WebUserName: "NT AUTHORITY\\NETWORK SERVICE",
	VirtualDirectorySite: "Sriracha",
	VirtualDirectoryName: "",
	WebsiteAuthenticationMode: "Forms",
	RavenDBConnectionString: "Url=http://localhost:8080;Database=Sriracha",
	ApplicationPoolName: "SrirachaAppPool",
	SiteUrl: "http://sriracha.youcompany.com",
	EmailConnectionString: "host=mail.yourcompany.com;port=25;UserName=someUserName;Password=somePassword",
	EncryptionKey: "REPLACE_ME",

	ServiceStartMode: "Automatic",
	ServiceUserName: "NT AUTHORITY\\SYSTEM",
	ServiceUserPassword: "",
	ServiceName: "Sriracha Deployment System",
	ServiceSourcePath: ".\\Publish\\WindowsService",
	ServiceExeName: "Sriracha.Deploy.Server.exe",
	ServiceTargetPath: "C:\\Sriracha\\Service",
	AutoStartService: true,
	
	SourceCommandLinePath: ".\\Publish\\CommandLine",
	SourceOfflineExePath: ".\\Publish\\OfflineExe",
	TargetCommandLinePath: "C:\\Sriracha\\CommandLine",
	CommandLineExeName: "Sriracha.Deploy.CommandLine.exe",

	TargetMachineUserName: "",
	TargetMachinePassword: ""
}