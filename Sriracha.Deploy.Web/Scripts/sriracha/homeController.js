ngSriracha.controller("HomeController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator) {
	$scope.navigator = SrirachaNavigator;
	$scope.projectList = SrirachaResource.project.query({});
	$scope.buildList = SrirachaResource.build.query({});
	$scope.getCreateProjectUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Project.CreateUrl);
	}
	$scope.getSubmitBuildUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.SubmitUrl);
	}
	$scope.getBuildListUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.ListUrl);
	}
	$scope.getViewBuildUrl = function (build) {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.ViewUrl, { buildId: build.id });
	}
});

