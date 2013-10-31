{
	SourceWebsitePath: ".\\Sriracha.Deploy.Web",
	TargetWebsitePath: "C:\\Sriracha\\Website",
	WebUserName: "NT AUTHORITY\\NETWORK SERVICE",
	VirtualDirectorySite: "Sriracha",
	VirtualDirectoryName: "",
	WebsiteAuthenticationMode: "Windows",
	RavenDBConnectionString: "Url=http://localhost:8080;Database=Sriracha",
	ApplicationPoolName: "Sriracha",
	SiteUrl: "http://localhost:1234",
	EmailConnectionString: "Host=apprelay.css.cable.comcast.com;Port=25",
	EncryptionKey: "REPLACE_ME_REPLACE_ME",
	
	ServiceStartMode : "Automatic",
	ServiceUserName : "NT AUTHORITY\\LOCAL SERVICE",
	ServiceUserPassword : "",
	ServiceName: "SrirachaDeploymentSystem",
	ServiceSourcePath: ".\\Sriracha.Deploy.Server\\bin\\Debug",
	ServiceExeName: "Sriracha.Deploy.Server.exe",
	ServiceTargetPath: "C:\\Sriracha\\WindowsService",
	AutoStartService: "true",
	
	SourceCommandLinePath: ".\\Sriracha.Deploy.CommandLine\\bin\\Debug",
	TargetCommandLinePath: "C:\\Sriracha\\Cmd",
	CommandLineExeName: "Sriracha.Deploy.CommandLine.exe"
}