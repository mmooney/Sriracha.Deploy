ngSriracha.controller("deployOfflineStatusController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter','PermissionVerifier',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;

	$scope.refreshData = function () {
		$scope.offlineDeployment = SrirachaResource.offlineDeployment.get({ id: $routeParams.offlineDeploymentId},
			function () {
			    if (!$scope.batchRequest && $scope.offlineDeployment.deployBatchRequestId) {
			        $scope.batchRequest = SrirachaResource.deployBatchRequest.get({ id: $scope.offlineDeployment.deployBatchRequestId },
                        function () {
                            console.log($scope.batchRequest);
                        },
                        function (error) {
                            ErrorReporter.handleResourceError(error);
                        }
                    );
			    }
			    console.log($scope.offlineDeployment);
				//if ($scope.deployBatchStatus.request.status == "Success" ||
				//		($scope.deployBatchStatus.request.status == "Error" && !$scope.deployBatchStatus.request.resumeRequested)) {
				//	if ($scope.refreshInterval) {
				//		clearInterval($scope.refreshInterval);
				//		$scope.refreshInterval = null;
				//	}
				//}
			},
			function (error) {
				if ($scope.refreshInterval) {
					clearInterval($scope.refreshInterval);
					$scope.refreshInterval = null;
				}
				ErrorReporter.handleResourceError(error);
			});
	}
	if ($routeParams.offlineDeploymentId) {
		$scope.refreshData();
		$scope.refreshInterval = setInterval($scope.refreshData, 10000);
	}
}]);