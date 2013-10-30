ngSriracha.controller("ProjectEnvironmentController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter','PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if ($routeParams.environmentId && $scope.project.environmentList) {
			var environment = _.findWhere($scope.project.environmentList, { id: $routeParams.environmentId });
			if (environment) {
				$scope.environment = new SrirachaResource.environment(environment);
				$scope.environment.componentList = environment.componentList || [];
				$scope.environment.configurationList = environment.configurationList || [];

				$scope.validateList(environment.componentList, $scope.project.componentList, "Component");
				$scope.validateList(environment.configurationList, $scope.project.configurationList, "Configuration");
			}
		}
		if (!$scope.environment) {
			$scope.environment = new SrirachaResource.environment({ projectId: $routeParams.projectId });
		}
	});
	$scope.credentials = SrirachaResource.credentials.get(
		{pageSize:1000},
		function (data) {
			console.log(data);
		},
		function (err) {
			ErrorReporter.handleResourceError(err);
		}
	)

	$scope.isEditable = function () {
		if ($routeParams.environmentId) {
			return $scope.permissionVerifier.canEditEnvironment($routeParams.projectId, $routeParams.environmentId);
		}
		else  {
			return $scope.permissionVerifier.canCreateEnvironment($routeParams.projectId);
		}
	}

	$scope.getConfigurationName = function (configurationId) {
		if(configurationId && $scope.project) {
			var configuration = _.findWhere($scope.project.configurationList, { id: configurationId });
			if (configuration) {
				return configuration.configurationName;
			}
		}
		return "???";
	}

	$scope.validateList = function (environmentComponentList, projectComponentList, parentType) {
		var oldEnvironmentComponentList = environmentComponentList.slice(0);
		environmentComponentList.length = 0;
		_.each(projectComponentList, function (component) {
			var environmentComponentItem = _.findWhere(oldEnvironmentComponentList, { parentId: component.id });
			if (!environmentComponentItem) {
				environmentComponentItem = {
					parentId: component.id
				};
			}
			environmentComponentItem.itemName = component.componentName || component.configurationName;
			environmentComponentItem.useConfigurationGroup = component.useConfigurationGroup || false;
			environmentComponentItem.configurationId = component.configurationId;
			if (environmentComponentItem.configurationId) {
				environmentComponentItem.configurationName = $scope.getConfigurationName(component.configurationId);
			}
			environmentComponentList.push(environmentComponentItem);
		});

		_.each(projectComponentList, function (component) {
			var environmentComponent = _.findWhere(environmentComponentList, { parentId: component.id });
			var configDefinition = SrirachaResource.componentConfiguration.get(
				{ projectId: $routeParams.projectId, parentId: component.id, parentType: parentType },
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
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				});
		});
	};

	$scope.editMachine = function (machine) {
		var oldMachineName = machine.machineName;
		var newValue = prompt("Enter machine name:", machine.machineName);
		if (newValue !== null) {
			machine.machineName = newValue;
			if ($scope.project.usesSharedComponentConfiguration) {
				_.each($scope.environment.componentList, function (component) {
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
				parentId: environmentComponent.parentId,
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
				$scope.permissionVerifier.reload();
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

