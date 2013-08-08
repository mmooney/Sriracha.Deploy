var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload"]);

ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project"),
		component: $resource("/api/project/:projectId/component"),
		componentConfiguration: $resource("api/project/:projectId/component/:componentId/configuration"),
		branch: $resource("/api/project/:projectId/branch"),
		environment: $resource("/api/project/:projectId/environment"),
		deploymentStep: $resource("/api/project/:projectId/component/:componentId/step", { projectId: "@projectId", componentId: "@componentId" }),
		deployHistory: $resource("/api/deploy/history"),
		taskMetadata: $resource("/api/taskmetadata"),
		build: $resource("/api/build/:buildId"),
		deployRequest: $resource("/api/deployRequest/:deployRequestId"),
		deployState: $resource("/api/deployState/:deployState"),
		systemLog: $resource("/api/systemLog")
	}
});

ngSriracha.directive("taskConfig", function () {
	return {
		templateUrl: "templates/deploymentstep-options-edit-template.html",
		link: function (scope, element, attrs) {
			scope.taskTypeName = attrs.taskTypeName;
			scope.addXPathValueListItem = function () {
				scope.deploymentStep.taskOptions.XPathValueList = scope.deploymentStep.taskOptions.XPathValueList || [];
				scope.deploymentStep.taskOptions.XPathValueList.push({});
			}
			scope.deleteXPathValueListItem = function (item) {
				var index = scope.deploymentStep.taskOptions.XPathValueList.indexOf(item);
				if (index >= 0) {
					scope.deploymentStep.taskOptions.XPathValueList.splice(index, 1);
				}
			}
		}
	}
});

ngSriracha.directive("projectList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/project-list-template.html"
	}
});

ngSriracha.directive("buildList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/build-list-template.html"
	}
});

ngSriracha.directive("systemLogList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/systemlog-list-template.html"
	}
});

ngSriracha.controller("HomeController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator) {
	$scope.navigator = SrirachaNavigator;
	$scope.projectList = SrirachaResource.project.query({});
	$scope.buildList = SrirachaResource.build.query({});
	$scope.getCreateProjectUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Project.CreateUrl);
	}
	$scope.getSubmitBuildUrl = function() {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.SubmitUrl);
	}
	$scope.getBuildListUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.ListUrl);
	}
	$scope.getViewBuildUrl = function (build) {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.ViewUrl, { buildId: build.id });
	}
});

