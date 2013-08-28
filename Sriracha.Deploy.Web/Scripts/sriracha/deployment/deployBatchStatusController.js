ngSriracha.controller("deployBatchStatusController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;

	if ($routeParams.deployBatchRequestId) {
		$scope.deployBatchStatus = SrirachaResource.deployBatchStatus.get({ id: $routeParams.deployBatchRequestId },
			function() {
				console.log($scope.deployBatchStatus);
			},
			function(err) {
				ErrorReport.handleResourceException(err);
			});
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
		var deployState = _.find($scope.deployBatchStatus.deployStateList,
			function (x) {
				return (x && x.build && x.build.id == item.build.id
					&& x.machineList && _.any(x.machineList, function (y) { return (y.id == machine.id); }));
			});
		return "NotStarted";
	}
});