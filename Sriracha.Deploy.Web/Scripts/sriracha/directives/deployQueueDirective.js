angular.module("ngSriracha").directive("deployQueue",
	["DeployQueueDataAccess","SrirachaNavigator",
	function (DeployQueueDataAccess, SrirachaNavigator) {
		return {
			restrict: "E",
			templateUrl: "templates/directives/deployQueue.html",
			scope: {
			},
			link: function postLink(scope, element, attrs) {
				scope.navigator = SrirachaNavigator;
				scope.queueList = DeployQueueDataAccess.get();

				scope.getEnvironmentList = function (deployBatchRequest) {
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

				scope.getComponentList = function (deployBatchRequest) {
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
			}
		};
	}]);
