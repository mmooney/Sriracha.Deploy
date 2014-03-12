ngSriracha.controller("SystemLogController",
		['$scope', '$routeParams', 'SrirachaResource', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, ErrorReporter) {
	$scope.systemLog = SrirachaResource.systemLog.get({  }, function () {
	}, ErrorReporter.handleResourceError);
}]);