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
                        },
                        function (error) {
                            ErrorReporter.handleResourceError(error);
                        }
                    );
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
	if ($routeParams.offlineDeploymentId) {
		$scope.refreshData();
		$scope.refreshInterval = setInterval($scope.refreshData, 10000);
	}
}]);