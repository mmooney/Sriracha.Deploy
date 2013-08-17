﻿var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload"]);

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

ngSriracha.directive("selectEnvironmentMachines", function (SrirachaResource, ErrorReporter) {
	return {
		restrict: "E",
		templateUrl: "templates/directives/selectEnvironmentMachines.html",
		scope: {
			buildid: '@',
			environmentid: '@',
			selection: '='
		},
		link: function postLink(scope, element, attrs) {
			scope.$watch("buildid + environmentid", function () {
				if (!scope.buildid || !scope.environmentid) {
					scope.build = null;
					scope.environment = null;
				}
				else {
					scope.build = SrirachaResource.build.get({ id: scope.buildid },
						function () {
							scope.project = SrirachaResource.project.get({ id: scope.build.projectId },
								function () {
									scope.environmentSelector = new EnvironmentSelector(scope.build, scope.project, scope.environmentid, SrirachaResource, ErrorReporter);
									scope.selection.machineList = scope.environmentSelector.environmentComponent.machineList;
									console.log(scope);
								},
								function (err) {
									ErrorReporter.handleResourceError(error);
								});
						},
						function(err) {
							ErrorReporter.handleResourceError(err);
						}
					);
				}
			});
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

