ngSriracha.controller("ProjectEnvironmentController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if ($routeParams.environmentId && $scope.project.environmentList) {
			var environment = _.findWhere($scope.project.environmentList, { id: $routeParams.environmentId });
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
					var configDefinition = SrirachaResource.componentConfiguration.get(
						{ projectId: $routeParams.projectId, componentId: component.id },
						function () {
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
							},
							function (err) {
								ErrorReporter.handleResourceError(err);
							}
						);

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

	$scope.editMachine = function (machine) {
		var oldMachineName = machine.machineName;
		var newValue = prompt("Enter machine name:", machine.machineName);
		if (newValue !== null) {
			machine.machineName = newValue;
			if ($scope.project.usesSharedComponentConfiguration) {
				_.each($scope.environment.componentList, function (component) {
					console.log("tesT234S");
					var existingMachine = _.findWhere(component.machineList, { machineName: oldMachineName });
					if (existingMachine) {
						existingMachine.machineName = newValue;
					}
				});
			}
		}
	}

	$scope.deleteMachine = function (environmentComponent, machine) {
		if (confirm("Are you SURE you want to delete this machine (" + machine.machineName + ")?")) {
			$scope.deleteMachineFromEnvironmentComponent(environmentComponent, machine.machineName)
			if ($scope.project.usesSharedComponentConfiguration) {
				_.each($scope.environment.componentList, function (x) {
					$scope.deleteMachineFromEnvironmentComponent(x, machine.machineName)
				})
			}
		}
	}

	$scope.deleteMachineFromEnvironmentComponent = function (environmentComponent, machineName) {
		environmentComponent.machineList = _.reject(environmentComponent.machineList, function (x) { return x.machineName == machineName; });
	}

	$scope.editConfigurationItem = function (configItemDefinition, configurationValueList, machineName) {
		var newValue = prompt("Edit Value for " + configItemDefinition.fieldName, configurationValueList[configItemDefinition.fieldName]);
		if (newValue !== null) {
			if ($scope.project.usesSharedComponentConfiguration) {
				_.each($scope.environment.componentList, function (component) {
					if (!machineName) {
						component.configurationValueList[configItemDefinition.fieldName] = newValue;
					}
					else {
						var machine = _.findWhere(component.machineList, { machineName: machineName });
						if (machine) {
							machine.configurationValueList[configItemDefinition.fieldName] = newValue;
						}
					}
				});
			}
			else {
				configurationValueList[configItemDefinition.fieldName] = newValue;
			}
		}
	}

	$scope.addMachine = function (component) {
		var newMachineName = prompt("Please enter machine name:");
		if (newMachineName) {
			$scope.addMachineToEnvironmentComponent(component, newMachineName);
			if ($scope.project.usesSharedComponentConfiguration) {
				_.each($scope.environment.componentList, function (environmentComponent) {
					$scope.addMachineToEnvironmentComponent(environmentComponent, newMachineName);
				});
			}
		}
	}
	$scope.addMachineToEnvironmentComponent = function(environmentComponent, newMachineName) {
		environmentComponent.machineList = environmentComponent.machineList || [];
		var existingMachine = _.findWhere(environmentComponent.machineList, { machineName: newMachineName });
		if (!existingMachine) {
			var machine = {
				machineName: newMachineName,
				componentId: environmentComponent.componentId,
				configurationValueList: {}
			};
			environmentComponent.machineList.push(machine);
		}
	}

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
				ErrorReporter.handleResourceError(error);
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
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	//End Environments
}]);

