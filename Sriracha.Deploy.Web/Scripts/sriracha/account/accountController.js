ngSriracha.controller("AccountController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {

	$scope.navigator = SrirachaNavigator;

	$scope.serviceData = SrirachaResource.account.get(
		{},
		function() {
			$scope.accountSettings = $scope.serviceData.accountSettings;
		},
		function (err) {
			ErrorReporter.handleResourceException(err);
		}
	);

	$scope.getNotificationFlags = function (projectNotificationList) {
		var fieldList = [];
		_.each(projectNotificationList, function (item) {
			for (var fieldName in item.flags) {
				if (!_.contains(fieldList, fieldName)) {
					fieldList.push(fieldName);
				}
			}
		});
		return fieldList;
	};

	$scope.saveSettings = function () {
		$scope.serviceData.$save(
			{},
			function () {
				$scope.navigator.account.edit.go();
			},
			function (err) {
				ErrorReporter.handleResourceError(err);
			}
		)
	}
}]);

