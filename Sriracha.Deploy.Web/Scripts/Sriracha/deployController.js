ngSriracha.controller("DeployController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.selection = {

	};
	$scope.idValues = {
		buildId: $routeParams.buildId,
		environmentId: $routeParams.environmentId
	};

	if ($routeParams.deployStateId) {
		$scope.deployState = SrirachaResource.deployState.get({ id: $routeParams.deployStateId }, 
			function () {
				$scope.project = SrirachaResource.project.get({ id: $scope.deployState.projectId },
					function () {
						$scope.refreshStatus();
					},
					function (error) {
						ErrorReporter.handleResourceError(error);
					});
			},
			function(error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}

	$scope.refreshStatus = function () {
		$scope.deployState = SrirachaResource.deployState.get({ id: $routeParams.deployStateId },
			function () {
				if ($scope.deployState.status == "NotStarted" || $scope.deployState.status == "InProcess") {
					setTimeout($scope.refreshStatus, 10000);
				}
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			});
	}

	$scope.canSubmitDeployment = function () {
		if ($scope.selection && $scope.selection.machineList) {
			return _.any($scope.selection.machineList, function (x) { return x.selected; });
		}
		else {
			return false;
		}
	}

	$scope.submitDeployment = function () {
		var selectedMachines = _.select($scope.selection.machineList, function (x) { return x.selected; });
		if (!selectedMachines || !selectedMachines.length) {
			alert("No target machines selected");
		}
		var saveParams = {
			environmentId: $scope.idValues.environmentId,
			buildId: $scope.idValues.buildId,
			machineIdList: _.pluck(selectedMachines, 'id')
		};
		var deployRequestTemplate = new SrirachaResource.deployRequest();
		deployRequestTemplate.$save(
			saveParams,
			function (result) {
				alert("Deployment submitted!");
				$scope.navigator.deployment.view.go(result.deployStateId);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
}]);