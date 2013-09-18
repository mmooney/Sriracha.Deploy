ngSriracha.controller("deployBatchStatusController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;

	if ($routeParams.deployBatchRequestId) {
		$scope.deployBatchStatus = SrirachaResource.deployBatchStatus.get({ id: $routeParams.deployBatchRequestId },
			function() {
				console.log($scope.deployBatchStatus);
			},
			function(err) {
				ErrorReporter.handleResourceError(err);
			});
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