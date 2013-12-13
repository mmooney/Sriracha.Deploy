﻿ngSriracha.controller("systemSettingsController",
	['$scope', '$location', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter','PermissionVerifier',
	function ($scope, $location,$routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

		$scope.navigator = SrirachaNavigator;
		$scope.permissionVerifier = PermissionVerifier;
	}]);

