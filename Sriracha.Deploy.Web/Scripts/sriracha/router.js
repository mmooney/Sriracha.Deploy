ngSriracha.config(function ($routeProvider, SrirachaNavigatorProvider) {
	var navigator = SrirachaNavigatorProvider.$get();
	$routeProvider
		.when("/", {
			templateUrl: "/templates/home-template.html",
			controller: "HomeController"
		})

		//Projects
		.when(navigator.project.list.url, {
			templateUrl: "templates/project-list-template.html",
			controller: "ProjectController",
		})
		.when(navigator.project.create.url, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.view.url, {
			templateUrl: "templates/project-view-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.edit.url, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.delete.url, {
			templateUrl: "templates/project-delete-template.html",
			controller: "ProjectController"
		})

		//Components
		.when(navigator.component.create.url, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.component.view.url, {
			templateUrl: "templates/component-view-template.html",
			controller: "ProjectController"
		})
		.when(navigator.component.edit.url, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.component.delete.url, {
			templateUrl: "templates/component-delete-template.html",
			controller: "ProjectController"
		})

		//Deployment Steps
		.when(Sriracha.Navigation.DeploymentStep.CreateUrl, {
			templateUrl: "templates/deploymentstep-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.DeploymentStep.EditUrl, {
			templateUrl: "templates/deploymentstep-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.DeploymentStep.DeleteUrl, {
			templateUrl: "templates/deploymentstep-delete-template.html",
			controller: "ProjectController"
		})

		//Branches
		.when(Sriracha.Navigation.Branch.CreateUrl, {
			templateUrl: "templates/branch-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Branch.EditUrl, {
			templateUrl: "templates/branch-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Branch.DeleteUrl, {
			templateUrl: "templates/branch-delete-template.html",
			controller: "ProjectController"
		})

		//Environments
		.when(Sriracha.Navigation.Environment.CreateUrl, {
			templateUrl: "templates/environment-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.EditUrl, {
			templateUrl: "templates/environment-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.DeleteUrl, {
			templateUrl: "templates/environment-delete-template.html",
			controller: "ProjectController"
		})

		//Builds
		.when(Sriracha.Navigation.Build.ListUrl, {
			templateUrl: "templates/build-list-template.html",
			controller: "BuildController"
		})
		.when(Sriracha.Navigation.Build.SubmitUrl, {
			templateUrl: "templates/build-submit-template.html",
			controller: "BuildController"
		})
		.when(Sriracha.Navigation.Build.ViewUrl, {
			templateUrl: "templates/build-view-template.html",
			controller: "BuildController"
		})
		.when(Sriracha.Navigation.Build.DeleteUrl, {
			templateUrl: "templates/build-delete-template.html",
			controller: "BuildController"
		})

		//Deployments
		.when(navigator.deployment.batchRequest.url, {
			templateUrl: "templates/deployment-batchRequest-template.html",
			controller: "deployBatchRequestController"
		})
		.when(navigator.deployment.batchStatus.url, {
			templateUrl: "templates/deployment-batchStatus-template.html",
			controller: "deployBatchStatusController"
		})
		.when(Sriracha.Navigation.Deployment.SubmitUrl, {
			templateUrl: "templates/deployment-submit-template.html",
			controller: "DeployController"
		})
		.when(Sriracha.Navigation.Deployment.ViewUrl, {
			templateUrl: "templates/deployment-view-template.html",
			controller: "DeployController"
		})

		//System Log
		.when(Sriracha.Navigation.SystemLog.ListUrl, {
			templateUrl: "templates/systemlog-list-template.html",
			controller: "SystemLogController"
		})

		.otherwise({
			template: "<h1>Not Found</h1>"
		})

	;
});

