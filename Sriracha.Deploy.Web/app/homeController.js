ngSriracha.controller("HomeController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator) {
	$scope.navigator = SrirachaNavigator;
	$scope.projectList = SrirachaResource.project.query({});
	$scope.buildList = SrirachaResource.build.get({});
}]);

