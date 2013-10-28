ngSriracha.controller("ProjectController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter','PermissionVerifier',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;
	if ($routeParams.projectId) {
		$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId },
			function () {
				$scope.projectRoleList = SrirachaResource.projectRole.query(
					{projectId: $routeParams.projectId},
					function () {
						//merry christmas
					},
					function (err) {
						ErrorReporter.handleResourceError(err)
					}
				)
			},
			function (error) { ErrorReporter.handleResourceError(error); }
		);
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
}]);

