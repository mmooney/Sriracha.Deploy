ngSriracha.controller("deployBatchListController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.listOptions = {};
	if ($routeParams.pageNumber) $scope.listOptions.pageNumber = $routeParams.pageNumber;
	if ($routeParams.pageSize) $scope.listOptions.pageSize = $routeParams.pageSize;
	if ($routeParams.sortField) $scope.listOptions.sortField = $routeParams.sortField;
	if ($routeParams.sortAscending) $scope.listOptions.sortAscending = $routeParams.sortAscending;

	$scope.deploymentList = SrirachaResource.deployBatchStatus.get($scope.listOptions,
		function () {
		},
		function (err) {
			ErrorReporter.handleResourceError(err);
		});
	
	$scope.getDeployStatusDescription = function (status) {
		var description;
		switch (status) {
			case "Requested":
				description = "Requested";
				break;
			case "NotStarted":
				description = "Not Started";
				break;
			case "InProcess":
				description = "In Process";
				break;
			case "Warning":
				description = "Warning";
				break;
			case "Success":
				description = "Success";
				break;
			case "Error":
				description = "Error";
				break;
			default:
				description = status;
				break;
		}
		return description;
	}

	$scope.getDeployStatusIconClass = function (status) {
		switch (status) {
			case "InProcess":
				return "deployStatus-icon in-process";
			case "Warning":
				return "deployStatus-icon warning";
			case "Success":
				return "deployStatus-icon ok";
			case "Error":
				return "deployStatus-icon error";
			default:
				return "deployStatus-icon";
		}
	}

	$scope.goToPage = function (pageNumber) {
		$scope.navigator.deployment.batchList.go(pageNumber, $scope.deploymentList.pageSize, $scope.deploymentList.sortField, $scope.deploymentList.sortAscending);
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