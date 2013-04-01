var ngSriracha = angular.module("ngSriracha", ["ngResource"]);

ngSriracha.config(function ($routeProvider) {
	$routeProvider
		.when("/", {
			templateUrl: "/templates/project-list-template.html",
			controller: "ProjectListController"
		})
		.when("/project/create", {
			templateUrl: "templates/edit-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/:id", {
			templateUrl: "templates/view-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/edit/:id", {
			templateUrl: "templates/edit-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/delete/:id", {
			templateUrl: "templates/delete-project-template.html",
			controller: "ProjectController"
		})
		.otherwise({
			template: "<h1>Not Found</h1>"
		})
	;
});

ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project")
	}
});

ngSriracha.controller("ProjectListController", function ($scope, $routeParams, SrirachaResource) {
	$scope.projectList = SrirachaResource.project.query({});

	$scope.delete = function (project) {
		window.location.href = "/#/project/delete/" + project.Id;
	}
});

ngSriracha.controller("ProjectController", function ($scope, $routeParams, SrirachaResource) {
	if ($routeParams.id) {
		$scope.project = SrirachaResource.project.get({ id: $routeParams.id });
	}
	else {
		$scope.project = new SrirachaResource.project({});
	}
	$scope.cancel = function () {
		window.history.back();
	};

	$scope.save = function () {
		$scope.project.$save();
		window.history.back();
	};

	$scope.delete = function () {
		SrirachaResource.project.delete({ id: $routeParams.id });
		window.history.back();
	};
});
