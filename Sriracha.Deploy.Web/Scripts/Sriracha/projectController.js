ngSriracha.controller("ProjectController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator) {
	$scope.navigator = SrirachaNavigator;
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

					_.each($scope.project.componentList, function (component) {
						var environmentComponent = _.findWhere($scope.environment.componentList, { componentId: component.id });
						var configDefinition = SrirachaResource.componentConfiguration.get({ projectId: $routeParams.projectId, componentId: component.id }, function () {
							$scope.configDefinitionList = $scope.configDefinitionList || {};
							$scope.configDefinitionList[component.id] = configDefinition;

							var oldConfigurationValueList = environmentComponent.configurationValueList || {};
							environmentComponent.configurationValueList = {};

							_.each(configDefinition.environmentTaskParameterList, function (taskParameter) {
								var existingItem = oldConfigurationValueList[taskParameter.fieldName];
								if (existingItem) {
									environmentComponent.configurationValueList[taskParameter.fieldName] = existingItem;
								}
								else {
									environmentComponent.configurationValueList[taskParameter.fieldName] = null;
								}
							});

							_.each(environmentComponent.machineList, function (machine) {
								var oldMachineConfigurationValueList = machine.configurationValueList;
								machine.configurationValueList = {};
								_.each(configDefinition.machineTaskParameterList, function (taskParameter) {
									var existingItem = oldMachineConfigurationValueList[taskParameter.fieldName];
									if (existingItem) {
										machine.configurationValueList[taskParameter.fieldName] = existingItem;
									}
									else {
										machine.configurationValueList[taskParameter.fieldName] = null;
									}
								});
							});
						});
					});
				}
			}
			if (!$scope.environment) {
				$scope.environment = new SrirachaResource.environment({ projectId: $routeParams.projectId });
			}
		});
	}
	else {
		$scope.projectList = SrirachaResource.project.query({});
		$scope.project = new SrirachaResource.project({});
	}

	$scope.editMachine = function (machine) {
		var newValue = prompt("Enter machine name:", machine.machineName);
		if (newValue !== null) {
			machine.machineName = newValue;
		}
	}

	$scope.deleteMachine = function (environmentComponent, machine) {
		if (confirm("Are you SURE you want to delete this machine (" + machine.machineName + ")?")) {
			environmentComponent.machineList = _.reject(environmentComponent.machineList, function (x) { return x.id == machine.id; });
		}
	}

	$scope.editConfigurationItem = function (configItemDefinition, configurationValueList) {
		var newValue = prompt("Edit Value for " + configItemDefinition.fieldName, configurationValueList[configItemDefinition.fieldName]);
		if (newValue !== null) {
			configurationValueList[configItemDefinition.fieldName] = newValue;
		}
	}

	$scope.addMachine = function (component) {
		var newMachineName = prompt("Please enter machine name:");
		if (newMachineName) {
			var machine = {
				machineName: newMachineName,
				componentId: component.componentId,
				configurationValueList: {}
			};
			component.machineList = component.machineList || [];
			component.machineList.push(machine);
		}
	}

	$scope.reportError = function (error) {
		alert("ERROR: \r\n" + JSON.stringify(error));
	};

	//Projects
	$scope.deleteProject = function () {
		SrirachaResource.project.delete(
			{ id: $routeParams.projectId },
			function () {
				$scope.navigator.project.list.go();
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	};

	$scope.saveProject = function () {
		var saveParams = {
			projectId: $routeParams.projectId
		};
		$scope.project.$save(
			saveParams,
			function (item) {
				$scope.navigator.project.view.go(item.id);
			},
			function (error) {
				$scope.reportError("ERROR: " + JSON.stringify(error));
			}
		);
	};
	//End Projects

	//Components
	$scope.saveComponent = function () {
		$scope.component.$save(
			$scope.component,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
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
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Components

	//Deployment Steps
	$scope.saveDeploymentStep = function () {
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
				$scope.navigator.component.view.go($scope.project.id, $scope.component.id);
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
				$scope.navigator.component.view.go($scope.project.id, $scope.component.id);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Deployment Steps

	//Branches
	$scope.saveBranch = function () {
		var saveParams = {
			projectId: $routeParams.projectId,
			branchId: $routeParams.branchId
		};		
		$scope.branch.$save(
			saveParams,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
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
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Branches

	//Environments
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
					$scope.navigator.project.view.go($scope.project.id);
				}
				else {
					$scope.navigator.environment.edit.go($scope.project.id, $scope.environment.id);
				}
				
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.deleteEnvironment = function () {
		var deleteParams = {
			id: $routeParams.environmentId,
			projectId: $routeParams.projectId
		};
		$scope.environment.$delete(
			deleteParams,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
	//End Environments
}]);

