ngSriracha.controller("deployBatchStatusController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;

	$scope.$on("$destroy", function () {
		if ($scope.refreshInterval) {
			clearInterval($scope.refreshInterval);
			$scope.refreshInterval = null;
		}
	});

	$scope.refreshData = function () {
		$scope.deployBatchStatus = SrirachaResource.deployBatchStatus.get({ id: $routeParams.deployBatchRequestId },
			function () {
				if ($scope.deployBatchStatus.request.status == "Success" || $scope.deployBatchStatus.request.status == "Error") {
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
	if ($routeParams.deployBatchRequestId) {
		$scope.refreshData();
		$scope.refreshInterval = setInterval($scope.refreshData, 10000);
	}

	$scope.getMachineDeployStateId = function (item, machine) {
		if ($scope.deployBatchStatus.deployStateList && item && machine) {
			for (var i = 0; i < $scope.deployBatchStatus.deployStateList.length; i++) {
				var state = $scope.deployBatchStatus.deployStateList[i];
				if (state.build && state.build.id == item.build.id) {
					if (state.machineList && _.any(state.machineList, function (x) { return x.id == machine.id; })) {
						return state.id;
					}
				}
			}
		}
		return "NotStarted";
	}

	$scope.getMachineDeployStatus = function (item, machine) {
		if ($scope.deployBatchStatus.deployStateList && item && machine) {
			for (var i = 0; i < $scope.deployBatchStatus.deployStateList.length; i++)
			{
				var state = $scope.deployBatchStatus.deployStateList[i];
				if (state.build && state.build.id == item.build.id) {
					if (state.machineList && _.any(state.machineList, function (x) { return x.id == machine.id; })) {
						return state.status;
					}
				}
			}
		}
		return "NotStarted";
	}
}]);