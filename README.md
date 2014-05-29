# Sriracha Deployment System

[TeamCity Build Status: Core
![TeamCity Build Status](http://build.mmdbsolutions.com/app/rest/builds/buildType:(id:bt7)/statusIcon "TeamCity Build Status")] (http://build.mmdbsolutions.com/viewType.html?buildTypeId=bt7&guest=1)

[TeamCity Build Status: Raven DB
![TeamCity Build Status](http://build.mmdbsolutions.com/app/rest/builds/buildType:(id:Sriracha_SrirachaBuild_SrirachaBuildRavenDBServerTests)/statusIcon "TeamCity Build Status")] (http://build.mmdbsolutions.com/viewType.html?buildTypeId=Sriracha_SrirachaBuild_SrirachaBuildRavenDBServerTests&guest=1)

[TeamCity Build Status: SQL Server
![TeamCity Build Status](http://build.mmdbsolutions.com/app/rest/builds/buildType:(id:Sriracha_SrirachaBuild_SrirachaBuildContinuousSqlServerTests)/statusIcon "TeamCity Build Status")] (http://build.mmdbsolutions.com/viewType.html?buildTypeId=Sriracha_SrirachaBuild_SrirachaBuildContinuousSqlServerTests&guest=1)

Sriracha is a free, open source deployment system.  It manages the release builds of your projects, and automates and tracks deployments to various environments.

## How Does It Work?
Sriracha is a combination of a website and Windows service that runs on any server in your network.  Preferably you'll want that server to have WMI access to the machines you are deploying to, but it's not necessary.  

Sriracha picks up where you're build server leaves off.  Once your build server has created your release build, you can add a line to your build script to publish it as a package to Sriracha REST(-like) API.  Then in the Sriracha website you can configure how and where to deploy that build using a simple web interface.  

Once you've set up your deployment steps for a component and the parameters for a given environment, you can run a deployment of that component, or a batch deployment of several components.

## Key Features
* Easy, intuitive setup of projects/components/environments
* Dashboard overview of what versions are deployed to which environments
* Unopinionated deployment - deploy how you want to, Sriracha does not dictate and specific deployment package format or technique.  If you already have deployment scripts that you maintain and run manually, you can automate those with Sriracha
* Publish builds to the deployment tool from Team City or Cruise Control or anything else with single command line, autocreating  projects and components and branches if they don’t already exist
* Easily promote a previous deployment of a specific build to a new environment
* Define a common set of deployment steps for each component, specifying the environment-specific and machine-specific parameters 
* Fully extensible - All deployment steps are built as extensions.  Add your own deployment steps with some simple .NET code, implementing a few simple interfaces.
* Database agnostic - Can be used with any transactionally-safe database.  Data providers for Raven DB and SQL Server have already been built.  Additional providers can be created by implementing the necessary repository interfaces.  A large battery of automated tests (650+) have been defined to ensure that any data provider implementation behaves as expected.
* A full REST-like API for every action that is available in the website, so any other web or desktop front end could be created for it, or anyone could build some automation/monitoring tools to interact with it
* Workflows for requesting and approving deployments on a per-environment basis
* Per-project and per-environment permissions, controlling who can submit builds, request deployments, approve deployments, and start deployments
* "Run-As" functionality, allowing the deployment process to impersonate a specific network user 
* Remove user impersonation, allowing the deployment to impersonate a user on a local administrator on the target machine even if it's not a network user in the same domain, as is often necessary when deploying in Amazon EC2.
* Configurable email notification subscriptions for when builds are published, and when deployments requested/approved/completed/failed/etc
* No need to install an agent on the target machines, as long one of the existing deployment methods can access the target server (e.g. dropkick or another command line tool).
* Configurable parallel deployment plans - When deploying a series of components across many different machines, you can define which components should be deployed all by themselves (like a database), and which components can be deployed in parallel with other components (like a series of websites being deployed to multiple servers at a time), and Sriracha will determine the fastest possible way to deploy those components.  This allows you to optimize your deployment times as much as your environment and configuration will allow.
* Offline deployment packages - If your production environment is completely locked down and won't allow remote access, you can still use Sriracha.  It will allow you to download an offline deployment package, which you can then copy to the restricted servers and run from there.  It will run the exact same deployment code with the same configurable settings.  Once you deployment is complete, you can tell it to phone-home to the Sriracha server to publish the deployment results back to the central system, allowing you to maintain a consistent history of deployments across all environments.

## Frequently Asked Questions
* How much does it cost?

Sriracha is a free open source project, using a permissibile MIT licence.  Feel free to use it or modify it however you'd like, or build your own custom extensions or providers.

(Note: if you build custom proprietary extensions, you would not be eligible for Raven DB's open source license any more, in which case you'd have to purchase a Raven DB license or use different database type)

* How can I get started? 

We will be building out the Wiki documentation and creating some YouTube walkthrough videos in the coming months.  In the meantime, feel free to contact us at sriracha@mmdbsolutions.com with questions or to schedule a demo.

* Do we really need yet another automated deployment tool?  Why should I use this instead of the 100 other tools out there?

We built Sriracha because it filled a need that we had, which was a free, extensible, an unopinionated deployment tool for Windows applications.  Many of our clients had existing deployment scripts, but needed a tool for automating and tracking those deployments, but did not want to completely recreate their whole deployment strategy.  Some flinched at having to get a PO approved to purchase some software, or were concerned about getting invested with a freemium product, then being trapped into paying for it down the line.  Others didn't like having to install an agent.  Others still needed a way to ensure that they were using the same deployment process and history tracking in restricted production servers that were using more-accessible environments like QA and Staging.  Lastly, every one had something specifically weird they needed to do during their deployments, and we needed the ability to build custom deployment tasks for those situations, and wanted them to be first-class citizens just like any other deployment task in the system.

For all these reasons, we decided to build a tool that was exactly what we wanted.  We did not want to get bogged down with the viability building a commercial product, but instead wanted to focus our efforts on building the features that were of most use to our clients and would be of most use to the community. There are a lot of VERY good commercial tools out there, such as [Inedo BuildMaster](http://inedo.com/buildmaster/overview) and [Octopus Deploy](http://octopusdeploy.com/), and there is a very good chance that they will meet your needs. If they do, by all means defintiely use them.  

However we wanted to provide a free and open source option.  While it may never be as polished and feature-rich as some commercial products, we feel having a diverse community of options is good for everyone.

In the end, we wanted to push the automated deployment and DevOps story in Windows a few steps farther.  To do this, we want to get as many people was possible using something to automate their processes.  [While there is a lot more to DevOps than tools](http://mooneyblog.mmdbsolutions.com/index.php/2013/11/06/devops-reality/), every piece of friction you can smooth out helps nudge the whole movement forward.  

* Does it work in the cloud?

Yes, Sriracha can run on Amazon EC servers (which is how we run it ourselves).  It can deploy to Amazon EC2 Windows servers ([using a few WMI firewall tricks here](http://mooneyblog.mmdbsolutions.com/index.php/2013/11/01/accessing-an-amazon-vm-through-wmi/)) and can deploy to Azure Cloud Services ([using the MMDB.Azure.Management library described here](http://mooneyblog.mmdbsolutions.com/index.php/2014/04/17/windows-azure-5-deploying-via-c-using-mmdb-azure-management/)).  

Several other deployment task types are still in development to simplify cloud deployments, such as publishing to Amazon S3 buckets, manipulating Amazon EC2 load balancers, and publishing to Azure Websites.

* Why multiple databases?

Sriracha was originally built with [Raven DB](http://ravendb.net/), and most of the our existing setups have used this.  Raven DB is very awesome, and we have used it for several projects. While there is can be some learning curve and quirks, it makes development go so fast that it was a slam dunk decision for the initial implementation.

However, we knew that Raven DB would not be the best options for everyone.  While Raven DB is free for open source projects like Sriracha, if you have a proprietary or commercial product you need to buy a license.  In other words, if you were building priority extensions for Sriracha, you would have to buy a Raven DB license.  We would definitely encourage you to do so, because Raven DB is great, and definitely worth the cost of a license, but at the same time we know how hard it is to get software purchase approved at some companies, and we know it would be a deal breaker for some folks.

Also, while Raven DB is really simple to set up and get started with, many companies already have big shiny SQL Servers and big shiny DBAs to support them, any it makes sense for them to keep using the technologies that they are already familiar with.

Again, our goal here is to drive adoption as much as possible, and the reality is that a choice of database technology would prevent some people from using the tool.  At the same time, the reality is that in the grand scheme of things the data access for Sriracha is really simple.  We're not building a web scale application that needs to support millions of users but optimizing as close to the metal as possible, this is a relatively small application with a handful of object types that need to be persistent.  So while in many cases it is not a good idea to take the lowest common denominator approach necessary to support dumb database-agnostic repository interfaces, and while it is a LOT more work than just coding against a given DB technology, we felt it was a worthwhile tradeoff to be able to offer users a choice of database.

Currently Raven DB and SQL Server are supported, and Postgres is almost certainly next.

* OK, sounds good, but we don't know where to start, know anyone who can help?

Glad you asked!  Mike Mooney and MMDB Solutions have years of experience in supporting various companies of all sizes, including development, automated testing, deployment automation, and training.  

Please contact us at info@mmdbsolutions.com and we'll be happy to set up a call to talk about how we can help you get started using Sriracha, or just helping your team develop the culture and tools necessary to lead their own revolution.

* Is there a hosted/supported version available?

MMDB Solutions would be happy to set up and maintain a hosted version for you.  Please contact us at info@mmdbsolutions.com to discuss options.