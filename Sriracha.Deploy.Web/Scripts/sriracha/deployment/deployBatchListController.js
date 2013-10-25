ngSriracha.controller("deployBatchListController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.listOptions = {};
	if ($routeParams.pageNumber) $scope.listOptions.pageNumber = $routeParams.pageNumber;
	if ($routeParams.pageSize) $scope.listOptions.pageSize = $routeParams.pageSize;
	if ($routeParams.sortField) $scope.listOptions.sortField = $routeParams.sortField;
	if ($routeParams.sortAscending) $scope.listOptions.sortAscending = $routeParams.sortAscending;

	//$scope.deploymentList = SrirachaResource.deployBatchStatus.get($scope.listOptions,
	//	function () {
	//	},
	//	function (err) {
	//		ErrorReporter.handleResourceError(err);
	//	});
	$scope.deployBatchRequestList = SrirachaResource.deployBatchRequest.get($scope.listOptions,
		function () {
			console.log($scope.deployBatchRequestList)
		},
		function (err) {
			ErrorReporter.handleResourceError(err);
		});

	$scope.goToPage = function (pageNumber) {
		$scope.navigator.deployment.batchList.go(pageNumber, $scope.deployBatchRequestList.pageSize, $scope.deployBatchRequestList.sortField, $scope.deployBatchRequestList.sortAscending);
		//alert(pageNumber);
	}

	$scope.getEnvironmentList = function (deployBatchRequest) {
		if (deployBatchRequest) {
			var environmentList = [];
			if (deployBatchRequest.itemList) {
				_.each(deployBatchRequest.itemList, function (item) {
					if (item.machineList) {
						_.each(item.machineList, function (machine) {
							var environmentName = machine.environmentName;
							if (!environmentName) {
								environmentName = machine.environmentId;
							}
							if (!_.contains(environmentList, environmentName)) {
								environmentList.push(environmentName);
							}
						});
					}
				});
			}
			return environmentList.join(", ");
		}
	};

	$scope.getComponentList = function (deployBatchRequest) {
		if (deployBatchRequest) {
			var componentList = [];
			if (deployBatchRequest.itemList) {
				_.each(deployBatchRequest.itemList, function (item) {
					if (item.build) {
						var componentName = item.build.projectComponentName;
						if (!componentName) {
							componentName = item.build.projectComponentId;
						}
						if (!_.contains(componentList, componentName)) {
							componentList.push(componentName)
						}
					}
				});
			}
			return componentList.join(", ");
			return returnValue;
		}
	}

}]);