ngSriracha.controller("HomeController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator) {
	$scope.navigator = SrirachaNavigator;
	$scope.projectList = SrirachaResource.project.query({});
	$scope.buildList = SrirachaResource.build.query({});
});

