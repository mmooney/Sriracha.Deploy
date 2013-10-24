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
			}
		};
	}]);
