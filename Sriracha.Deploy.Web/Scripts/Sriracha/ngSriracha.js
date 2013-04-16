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
		.when("/project/:projectId/component/create", {
			templateUrl: "templates/edit-component-template.html",
			controller: "ProjectController"
		})
		.when("/project/:projectId/component/:componentId", {
			templateUrl: "templates/view-component-template.html",
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
			if ($routeParams.componentId && $scope.project.ComponentList) {
				_.each($scope.project.ComponentList, function (item) {
					if (item.Id == $routeParams.componentId) {
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

	$scope.reportError = function (error) {
		alert(error);
	};


	$scope.save = function () {
		event.preventDefault();
		if ($scope.deploymentStep) {
			$scope.deploymentStep.componentId = $routeParams.componentId;
			$scope.deploymentStep.projectId = $routeParams.projectId;
			var item = new SrirachaResource.deploymentStep($scope.deploymentStep);
			item.$save($scope.deploymentStep, function () {
				window.history.back();
			});
		}
		else if ($scope.component) {
			$scope.projectId = $routeParams.projectId;
			$scope.component.$save($scope.component, function () {
				window.history.back();
			});
		}
		else {
			$scope.project.$save($scope.project, function () {
				window.history.back();
			});
		}
		return false;
	};

});

