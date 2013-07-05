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
						if (deploymentStepItem) {
							deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
							$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
						}
					}
				}
				if (!$scope.deploymentStep) {
					$scope.deploymentStep = new SrirachaResource.deploymentStep({ projectId: $routeParams.projectId, componentId: $routeParams.componentId });
				}
			}
			if (!$scope.component) {
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
					if (!$scope.environment.componentList) {
						$scope.enviornment.componentList = [];
					}
					var oldEnvironmentComponentList = $scope.environment.componentList;
					$scope.environment.componentList = [];
					_.each($scope.project.componentList, function (component) {
						var environmentComponentItem = _.findWhere(oldEnvironmentComponentList, { componentId: component.id });
						if (!environmentComponentItem) {
							environmentComponentItem = {
								componentId: component.id
							};
						}
						environmentComponentItem.componentName = component.componentName;
						$scope.environment.componentList.push(environmentComponentItem);
					});
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
	$scope.cancelDeleteProject = function () {
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
		var saveParams = {
			projectId: $routeParams.projectId
		};
		$scope.project.$save(
			saveParams,
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
		console.log($scope.deploymentStep.taskOptions);
		$scope.deploymentStep.taskOptionsJson = JSON.stringify($scope.deploymentStep.taskOptions);
		var saveParams = {
			projectId: $routeParams.projectId, 
			componentId: $routeParams.componentId
		};
		if ($routeParams.deploymentStepId) {
			saveParams.deploymentStepId = $routeParams.deploymentStepId;
		}
		$scope.deploymentStep.$save(
			saveParams,
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
	$scope.getEditBranchUrl = function (branch) {
		if (branch) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Branch.EditUrl, { projectId: branch.projectId, branchId: branch.id });
		}
	}
	$scope.getDeleteBranchUrl = function (branch) {
		if (branch) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Branch.DeleteUrl, { projectId: branch.projectId, branchId: branch.id });
		}
	}
	$scope.cancelBranch = function () {
		Sriracha.Navigation.Project.View($routeParams.projectId);
	}
	$scope.saveBranch = function () {
		var saveParams = {
			projectId: $routeParams.projectId,
			branchId: $routeParams.branchId
		};		
		$scope.branch.$save(
			saveParams,
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
		var saveParams = {
			projectId: $routeParams.projectId
		};
		if ($routeParams.environmentId) {
			saveParams.id = $routeParams.environmentId;
		}
		$scope.environment.$save(
			saveParams,
			function () {
				if (saveParams.id) {
					Sriracha.Navigation.Project.View($routeParams.projectId);
				}
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

	$scope.editEnvironmentComponentMachine = function (environmentComponent, machine) {
		var existingMachineName = machine.machineName;
		var newMachineName = prompt("Please enter machine name", existingMachineName);
		if (newMachineName != existingMachineName) {
			var duplicate = _.find(environmentComponent.machineList, function (s) { return s.machineName == newMachineName });
			if (duplicate) {
				alert(newMachineName + " already exists");
				return;
			}
			machine.machineName = newMachineName;
		}
	}

	$scope.deleteEnvironmentMachine = function (environmentComponent, machine) {
		var index = environmentComponent.machineList.indexOf(machine);
		if (index >= 0) {
			environmentComponent.machineList.splice(index, 1);
		}
	}

	$scope.addEnvironmentComponentMachine = function (environmentComponent) {
		var machineName = prompt("Please enter machine name");
		if (machineName) {
			if (environmentComponent.machineList) {
				var duplicate = _.find($scope.environment.machineList, function (s) { return s.machineName == machineName });
				if (duplicate) {
					alert(machineName + " already exists");
					return;
				}
			}
			else {
				environmentComponent.machineList = [];
			}
			var newMachine = {
				machineName: machineName
			};
			environmentComponent.machineList.push(newMachine);
		}
	}
	//End Environments
});

