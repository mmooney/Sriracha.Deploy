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

	$scope.$on("$destroy", function () {
		if ($scope.refreshInterval) {
			clearInterval($scope.refreshInterval);
			$scope.refreshInterval = null;
		}
	});

	$scope.refreshData = function () {
		$scope.deployState = SrirachaResource.deployState.get({ id: $routeParams.deployStateId },
			function () {
				if ($scope.deployState.status == "Success" || $scope.deployState.status == "Error") {
					if ($scope.refreshInterval) {
						clearInterval($scope.refreshInterval);
						$scope.refreshInterval = null;
					}
				}
			},
			function (error) {
				if ($scope.refreshInterval) {
					clearInterval($scope.refreshInterval);
					$scope.refreshInterval = null;
				}
				ErrorReporter.handleResourceError(error);
			});
	}

	if ($routeParams.deployStateId) {
		$scope.refreshData();
		$scope.refreshInterval = setInterval($scope.refreshData, 10000);
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