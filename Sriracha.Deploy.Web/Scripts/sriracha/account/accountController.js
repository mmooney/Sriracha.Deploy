ngSriracha.controller("AccountController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {

	$scope.navigator = SrirachaNavigator;

	$scope.serviceData = SrirachaResource.account.get(
		{},
		function() {
			$scope.accountSettings = $scope.serviceData.accountSettings,
			$scope.effectivePermissions = $scope.serviceData.effectivePermissions
		},
		function (err) {
			ErrorReporter.handleResourceError(err);
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

	$scope.getProjectPermissionEnvironmentList = function (projectPermission) {
		if (projectPermission) {
			var list = _.union(
				_.pluck(projectPermission.requestDeployPermissionList, 'environmentName'),
				_.pluck(projectPermission.approveRejectDeployPermissionList, 'environmentName'),
				_.pluck(projectPermission.runDeploymentPermissionList, 'environmentName'),
				_.pluck(projectPermission.editEnvironmentPermissionList, 'environmentName'),
				_.pluck(projectPermission.managePermissionsPermissionList, 'environmentName')
			);
			return list;
		}
	}

	$scope.getEnvironmentPermission = function (permissionList, environment) {
		var item = _.findWhere(permissionList, { environmentName: environment });
		if (item) {
			return item.access;
		}
		else {
			return "None";
		}
	}
}]);

