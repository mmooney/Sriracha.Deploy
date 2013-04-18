var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices"]);

ngSriracha.config(function ($routeProvider) {
	$routeProvider
		.when("/", {
			templateUrl: "/templates/project-list-template.html",
			controller: "ProjectListController"
		})
		.when(Sriracha.Navigation.Project.CreateUrl, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.ViewUrl, {
			templateUrl: "templates/project-view-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.EditUrl, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.DeleteUrl, {
			templateUrl: "templates/project-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.CreateUrl, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.ViewUrl, {
			templateUrl: "templates/component-view-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.EditUrl, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.DeleteUrl, {
			templateUrl: "templates/component-delete-template.html",
			controller: "ProjectController"
		})
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
		.when(Sriracha.Navigation.Environment.CreateUrl, {
			templateUrl: "templates/environment-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.ViewUrl, {
			templateUrl: "templates/environment-view-template.html",
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
		.otherwise({
			template: "<h1>Not Found</h1>"
		})
	;
});

ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project"),
		component: $resource("/api/project/:projectId/component"),
		branch: $resource("/api/project/:projectId/branch"),
		environment: $resource("/api/project/:projectId/environment"),
		deploymentStep: $resource("/api/project/:projectId/component/:componentId/step", { projectId: "@projectId", componentId: "@componentId" }),
		taskMetadata: $resource("/api/taskmetadata")
	}
});

ngSriracha.directive("taskConfig", function () {
	return {
		templateUrl: "templates/deploymentstep-options-edit-template.html",
		link: function (scope, element, attrs) {
			scope.taskTypeName = attrs.taskTypeName;
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
				var component = _.find($scope.project.componentList, function (c) { return c.id == $routeParams.componentId; });
				if (component) {
					$scope.component = new SrirachaResource.component(component);
						
					$scope.taskMetadataList = SrirachaResource.taskMetadata.query({});

					if ($routeParams.deploymentStepId && $scope.component.deploymentStepList) {
						var deploymentStepItem = _.find(component.deploymentStepList, function (d) { return d.id == $routeParams.deploymentStepId });
						if(deploymentStepItem) {
							deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
							$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
						}
					}
				}
				if (!$scope.deploymentStep) {
					$scope.deploymentStep = new SrirachaResource.deploymentStep({ projectId: $routeParams.projectId, componentId: $routeParams.componentId });
				}
			}
			if(!$scope.component) {
				$scope.component = new SrirachaResource.component({ projectId: $routeParams.projectId });
			}

			if ($routeParams.branchId && $scope.project.branchList) {
				var branch = _.find($scope.project.branchList, function (b) { return b.id == $routeParams.branchId });
				if (branch) {
					$scope.branch = new SrirachaResource.branch(branch);
				}
			}
			if (!$scope.branch) {
				$scope.branch = new SrirachaResource.branch({ projectId: $routeParams.projectId });
			}

			if ($routeParams.environmentId && $scope.project.environmentList) {
				var environment = _.find($scope.project.environmentList, function (e) { return e.id == $routeParams.environmentId });
				if (environment) {
					$scope.environment = new SrirachaResource.environment(environment);
				}
			}
			if (!$scope.environment) {
				$scope.environment = new SrirachaResource.environment({ projectId: $routeParams.projectId });
			}
		});
	}
	else {
		$scope.project = new SrirachaResource.project({});
	}

	$scope.reportError = function (error) {
		alert("ERROR: \r\n" + JSON.stringify(error));
	};

	//Projects
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
	//End Projects

	//Components
	$scope.cancelComponent = function () {
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
	$scope.getCreateComponentUrl = function (project) {
		if (project) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Component.CreateUrl, { projectId: project.id });
		}
	}
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
	//End Components

	//Deployment Steps
	$scope.cancelDeploymentStep = function () {
		Sriracha.Navigation.Component.View($routeParams.projectId, $routeParams.componentId);
	}

	$scope.saveDeploymentStep = function () {
		$scope.deploymentStep.taskOptionsJson = JSON.stringify($scope.deploymentStep.taskOptions);
		$scope.deploymentStep.$save(
			$scope.deploymentStep,
			function () {
				Sriracha.Navigation.Component.View($routeParams.projectId, $routeParams.componentId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.deleteDeploymentStep = function () {
		$scope.deploymentStep.$delete(
			$scope.deploymentStep,
			function () {
				Sriracha.Navigation.Component.View($routeParams.projectId, $routeParams.componentId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.getCreateDeploymentStepUrl = function (component) {
		if (component) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.DeploymentStep.CreateUrl, { projectId: component.projectId, componentId: component.id });
		}
	}

	$scope.getEditDeploymentStepUrl = function (deploymentStep) {
		if (deploymentStep) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.DeploymentStep.EditUrl, { projectId: deploymentStep.projectId, componentId: deploymentStep.componentId, deploymentStepId: deploymentStep.id });
		}
	}
	$scope.getDeleteDeploymentStepUrl = function (deploymentStep) {
		if (deploymentStep) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.DeploymentStep.DeleteUrl, { projectId: deploymentStep.projectId, componentId: deploymentStep.componentId, deploymentStepId: deploymentStep.id });
		}
	}
	//End Deployment Steps

	//Branches
	$scope.getCreateBranchUrl = function (project) {
		if (project) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Branch.CreateUrl, { projectId: project.id });
		}
	}
	$scope.getEditBranchUrl = function(branch) {
		if (branch) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Branch.EditUrl, { projectId: branch.projectId, branchId: branch.id });
		}
	}
	$scope.getDeleteBranchUrl = function(branch) {
		if(branch) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Branch.DeleteUrl, { projectId: branch.projectId, branchId: branch.id });
		}
	}
	$scope.cancelBranch = function () {
		Sriracha.Navigation.Project.View($routeParams.projectId);
	}
	$scope.saveBranch = function () {
		$scope.branch.$save(
			$scope.branch,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	$scope.deleteBranch = function () {
		$scope.branch.$delete(
			$scope.branch,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Branches

	//Environments
	$scope.getCreateEnvironmentUrl = function (project) {
		if (project) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.CreateUrl, { projectId: project.id });
		}
	}
	$scope.getViewEnvironmentUrl = function (environment) {
		if (environment) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.ViewUrl, { projectId: environment.projectId, environmentId: environment.id });
		}
	}
	$scope.getEditEnvironmentUrl = function (environment) {
		if (environment) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.EditUrl, { projectId: environment.projectId, environmentId: environment.id });
		}
	}
	$scope.getDeleteEnvironmentUrl = function (environment) {
		if (environment) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.DeleteUrl, { projectId: environment.projectId, environmentId: environment.id });
		}
	}
	$scope.cancelEnvironment = function () {
		Sriracha.Navigation.Project.View($routeParams.projectId);
	}
	$scope.saveEnvironment = function () {
		$scope.environment.$save(
			$scope.environment,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	$scope.deleteEnvironment = function () {
		$scope.environment.$delete(
			$scope.environment,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Environments
});

