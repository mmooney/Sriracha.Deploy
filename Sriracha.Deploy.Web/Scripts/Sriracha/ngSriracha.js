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
		.when("/project/:projectId", {
			templateUrl: "templates/view-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/edit/:projectId", {
			templateUrl: "templates/edit-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/delete/:projectId", {
			templateUrl: "templates/delete-project-template.html",
			controller: "ProjectController"
		})
		.when("/project/:projectId/component/create", {
			templateUrl: "templates/edit-component-template.html",
			controller: "ProjectController"
		})
		.otherwise({
			template: "<h1>Not Found</h1>"
		})
	;
});

ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project"),
		component: $resource("/api/project/:projectId/component")
}
});

ngSriracha.controller("ProjectListController", function ($scope, $routeParams, SrirachaResource) {
	$scope.projectList = SrirachaResource.project.query({});

	$scope.delete = function (project) {
		window.location.href = "/#/project/delete/" + project.Id;
	}
});

ngSriracha.controller("ProjectController", function ($scope, $routeParams, SrirachaResource) {
	if ($routeParams.projectId) {
		$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId });
		if ($routeParams.componentId && project.ComponentList) {
			_.each(project.ComponentList, function (item) {
				if (item.Id == $routeParams.componentId) {
					$scope.component = item;
					return false;
				}
			});
		}
		else {
			$scope.component = new SrirachaResource.component({projectId: $routeParams.projectId});
		}
	}
	else {
		$scope.project = new SrirachaResource.project({});
	}
	$scope.cancel = function () {
		window.history.back();
	};

	$scope.save = function () {
		if ($scope.component) {
			$scope.projectId = $routeParams.projectId;
			$scope.component.$save($scope.component);
		}
		$scope.project.$save();
		window.history.back();
	};

	$scope.delete = function () {
		SrirachaResource.project.delete({ id: $routeParams.projectId });
		window.history.back();
	};
});

