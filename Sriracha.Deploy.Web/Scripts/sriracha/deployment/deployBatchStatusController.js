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
		//var deployState = _.find($scope.deployBatchStatus.deployStateList,
		//	function (x) {
		//		if(x && x.build && x.build.id == item.build.id 
		//			&& x.)
		//	});
		return "NotStarted";
	}
});