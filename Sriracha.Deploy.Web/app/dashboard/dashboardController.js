ngSriracha.controller("dashboardController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
		$scope.navigator = SrirachaNavigator;

		$scope.projectList = SrirachaResource.project.query(
            {},
            function (data) {
                console.log(data);
            },
            function (err) {
                ErrorReporter.handleResourceError(err);
            }
        );

	}]);