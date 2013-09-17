var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload"]);

ngSriracha.directive("taskConfig", function () {
	return {
		templateUrl: "templates/project/deploymentStep/deploymentstep-options-edit-template.html",
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

ngSriracha.directive("selectEnvironmentMachines",
		['SrirachaResource', 'ErrorReporter', 'SrirachaNavigator',
		function (SrirachaResource, ErrorReporter, SrirachaNavigator) {
	return {
		restrict: "E",
		templateUrl: "templates/directives/selectEnvironmentMachines.html",
		scope: {
			buildid: '@',
			environmentid: '@',
			selection: '='
		},
		link: function postLink(scope, element, attrs) {
			scope.navigator = SrirachaNavigator;
			scope.errorReporter = ErrorReporter;
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
									scope.environmentSelector = new EnvironmentSelector(scope.build, scope.project, scope.environmentid, SrirachaResource, ErrorReporter,
										function () {
											if (scope.environmentSelector.environmentComponent) {
												scope.selection.machineList = scope.environmentSelector.environmentComponent.machineList;
											}
											if (!scope.selection.machineList || !scope.selection.machineList.length) {
												scope.selection.machineList = [];
											}
										});
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
}]);

ngSriracha.directive("projectList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/project-list-template.html"
	}
});

ngSriracha.directive("buildList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/builds/build-list-template.html"
	}
});

ngSriracha.directive("systemLogList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/systemlog-list-template.html"
	}
});

