var ngSriracha = angular.module("ngSriracha", ["ngResource"]);

ngSriracha.config(function ($routeProvider) {
	$routeProvider
		.when("/", {
			templateUrl: "/templates/project-list-template.html",
			controller: "ProjectListController"
		})
		.when(Sriracha.Navigation.Project.CreateUrl, {
			templateUrl: "templates/edit-project-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.ViewUrl, {
			templateUrl: "templates/view-project-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.EditUrl, {
			templateUrl: "templates/edit-project-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.DeleteUrl, {
			templateUrl: "templates/delete-project-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.CreateUrl, {
			templateUrl: "templates/edit-component-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.ViewUrl, {
			templateUrl: "templates/view-component-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.EditUrl, {
			templateUrl: "templates/edit-component-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.DeleteUrl, {
			templateUrl: "templates/delete-component-template.html",
			controller: "ProjectController"
		})
		.when("/project/:projectId/component/:componentId/step/create", {
			templateUrl: "templates/edit-deploymentstep-template.html",
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
		component: $resource("/api/project/:projectId/component"),
		deploymentStep: $resource("/api/project/:projectId/component/:componentId/step", { projectId: "@projectId", componentId: "@componentId" }),
		taskMetadata: $resource("/api/taskmetadata")
	}
});

ngSriracha.directive("taskConfig", function () {
	return {
		templateUrl: "templates/edit-deploymentstep-options-template.html",
		link: function (scope, element, attrs) {
			scope.taskType = attrs.taskType;
			console.log(attrs);
		}
	}
});

ngSriracha.controller("ProjectListController", function ($scope, $routeParams, SrirachaResource) {
	$scope.projectList = SrirachaResource.project.query({});
	$scope.goToDeleteProject = function (project) {
		Sriracha.Navigation.Project.Delete(project.id);
	}
});

ngSriracha.controller("ProjectController", function ($scope, $routeParams, SrirachaResource) {
	if ($routeParams.projectId) {
		$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
			if ($routeParams.componentId && $scope.project.componentList) {
				_.each($scope.project.componentList, function (item) {
					if (item.id == $routeParams.componentId) {
						$scope.component = new SrirachaResource.component(item);
						
						$scope.taskMetadataList = SrirachaResource.taskMetadata.query({}, function () {
							console.log($scope.taskMetadataList);
						});
						return false;
					}
				});
			}
			else {
				$scope.component = new SrirachaResource.component({ projectId: $routeParams.projectId });
			}
		});
	}
	else {
		$scope.project = new SrirachaResource.project({});
	}

	$scope.cancelDeleteProject = function() {
		Sriracha.Navigation.Project.List();
	}

	$scope.deleteProject = function () {
		SrirachaResource.project.delete(
			{ id: $routeParams.projectId },
			function () {
				Sriracha.Navigation.Project.List();
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	};
	
	$scope.cancelProject = function () {
		Sriracha.Navigation.Project.List();
	};

	$scope.saveProject = function () {
		$scope.project.$save(
			$scope.project,
			function (item) {
				Sriracha.Navigation.Project.View(item.id);
			},
			function (error) {
				$scope.reportError("ERROR: " + JSON.stringify(error));
			}
		);
	};

	$scope.cancelComponent = function() {
		Sriracha.Navigation.Project.View($routeParams.projectId);
	}

	$scope.saveComponent = function () {
		$scope.component.$save(
			$scope.component,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.deleteComponent = function () {
		SrirachaResource.component.delete(
			{
				projectId: $routeParams.projectId,
				id: $routeParams.componentId
			},
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.reportError = function (error) {
		alert(error);
	};

	$scope.getViewComponentUrl = function (component) {
		if (component) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Component.ViewUrl, { projectId: component.projectId, componentId: component.id });
		}
	}
	$scope.getEditComponentUrl = function (component) {
		if (component) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Component.EditUrl, { projectId: component.projectId, componentId: component.id });
		}
	}
	$scope.getDeleteComponentUrl = function (component) {
		if (component) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Component.DeleteUrl, { projectId: component.projectId, componentId: component.id });
		}
	}
});

