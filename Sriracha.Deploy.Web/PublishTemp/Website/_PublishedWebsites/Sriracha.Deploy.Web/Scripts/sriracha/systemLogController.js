ngSriracha.controller("SystemLogController", function ($scope, $routeParams, SrirachaResource, ErrorReporter) {
	$scope.systemLog = SrirachaResource.systemLog.get({  }, function () {
		console.log($scope.systemLog);
	}, ErrorReporter.handleResourceError);
});