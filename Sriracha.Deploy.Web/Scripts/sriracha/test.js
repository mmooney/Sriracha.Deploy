ngSriracha.config(function ($routeProvider) {
	$routeProvider
		.when(Sriracha.Navigation.Project.ViewUrl, {
			templateUrl: "templates/project-view-template.html",
			controller: "ProjectController"
		})
