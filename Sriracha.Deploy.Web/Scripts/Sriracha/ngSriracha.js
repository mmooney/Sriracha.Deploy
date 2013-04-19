﻿var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload"]);

ngSriracha.config(function ($routeProvider) {
	$routeProvider
		.when("/", {
			templateUrl: "/templates/home-template.html",
			controller: "HomeController"
		})
		.when(Sriracha.Navigation.Project.CreateUrl, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.ViewUrl, {
			templateUrl: "templates/project-view-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.EditUrl, {
			templateUrl: "templates/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Project.DeleteUrl, {
			templateUrl: "templates/project-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.CreateUrl, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.ViewUrl, {
			templateUrl: "templates/component-view-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.EditUrl, {
			templateUrl: "templates/component-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Component.DeleteUrl, {
			templateUrl: "templates/component-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.DeploymentStep.CreateUrl, {
			templateUrl: "templates/deploymentstep-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.DeploymentStep.EditUrl, {
			templateUrl: "templates/deploymentstep-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.DeploymentStep.DeleteUrl, {
			templateUrl: "templates/deploymentstep-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Branch.CreateUrl, {
			templateUrl: "templates/branch-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Branch.EditUrl, {
			templateUrl: "templates/branch-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Branch.DeleteUrl, {
			templateUrl: "templates/branch-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.CreateUrl, {
			templateUrl: "templates/environment-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.ViewUrl, {
			templateUrl: "templates/environment-view-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.EditUrl, {
			templateUrl: "templates/environment-edit-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Environment.DeleteUrl, {
			templateUrl: "templates/environment-delete-template.html",
			controller: "ProjectController"
		})
		.when(Sriracha.Navigation.Build.SubmitUrl, { 
			templateUrl: "templates/build-submit-template.html",
			controller: "BuildController"
		})
		.otherwise({
			template: "<h1>Not Found</h1>"
		})
	;
});

ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project"),
		component: $resource("/api/project/:projectId/component"),
		branch: $resource("/api/project/:projectId/branch"),
		environment: $resource("/api/project/:projectId/environment"),
		deploymentStep: $resource("/api/project/:projectId/component/:componentId/step", { projectId: "@projectId", componentId: "@componentId" }),
		taskMetadata: $resource("/api/taskmetadata"),
		build: $resource("/api/build/:buildId")
	}
});

ngSriracha.directive("taskConfig", function () {
	return {
		templateUrl: "templates/deploymentstep-options-edit-template.html",
		link: function (scope, element, attrs) {
			scope.taskTypeName = attrs.taskTypeName;
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

ngSriracha.controller("HomeController", function ($scope, $routeParams, SrirachaResource) {
	$scope.projectList = SrirachaResource.project.query({});
	$scope.buildList = SrirachaResource.build.query({});
	$scope.getSubmitBuildUrl = function() {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.SubmitUrl);
	}
});

//ngSriracha.controller("ProjectListController", function ($scope, $routeParams, SrirachaResource) {
//	$scope.projectList = SrirachaResource.project.query({});
//	$scope.goToDeleteProject = function (project) {
//		Sriracha.Navigation.Project.Delete(project.id);
//	}
//});

