var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload"]);

ngSriracha.filter("displayDate", function () {
	return function (input) {
		if (input) {
			var date = new Date(input);
			return date.toString();
		}
	}
});
ngSriracha.directive("breadcrumbs",
		['$location','$rootScope','SrirachaResource', 'ErrorReporter', 'SrirachaNavigator',
		function ($location, $rootScope, SrirachaResource, ErrorReporter, SrirachaNavigator) {
	return {
		restrict: "E",
		templateUrl: "templates/directives/breadcrumbs.html",
		scope: {
		},
		link: function (scope, element, attrs) {
			scope.breadcrumbList = [];
			scope.updateBreadcrumbs = function (event) {
				scope.breadcrumbList.length = 0;
				var currentParam;
				var keyValueList = [];
				var splitParams = $location.path().split('/');
				for (i in splitParams) {
					if (currentParam) {
						if (splitParams[i] != "create" && splitParams[i] != "view" && splitParams[i] != "edit" && splitParams[i] != "delete" && splitParams[i] != "remove") {
							var item = { key: currentParam, value: splitParams[i] };
							keyValueList.push(item);
							currentParam = null;
						}
					}
					else {
						if (splitParams[i]) {
							currentParam = splitParams[i];
						}
					}
				}
				if (keyValueList.length && keyValueList[0].key == "project") {
					var project = SrirachaResource.project.get(
						{ id: keyValueList[0].value },
						function () {
							var breadcrumbItem = {
								url: SrirachaNavigator.project.view.clientUrl(keyValueList[0].value),
								displayValue: "Project: " + project.projectName
							};
							scope.breadcrumbList.push(breadcrumbItem);
							var component, step, environment, branch, configuration;
							for (var i = 1; i < keyValueList.length; i++) {
								var item = keyValueList[i];
								if (item.value) {
									switch (item.key) {
										case "configuration":
											configuration = _.findWhere(project.configurationList, { id: item.value });
											x = {
												url: SrirachaNavigator.configuration.view.clientUrl(project.id, configuration.id),
												displayValue: "Configuration: " + configuration.configurationName
											};
											scope.breadcrumbList.push(x);
											break;
										case "component":
											component = _.findWhere(project.componentList, { id: item.value });
											x = {
												url: SrirachaNavigator.component.view.clientUrl(project.id, component.id),
												displayValue: "Component: " + component.componentName
											};
											scope.breadcrumbList.push(x);
											break;
										case "step":
											if (component) {
												step = _.findWhere(component.deploymentStepList, { id: item.value });
												if (step) {
													x = {
														url: SrirachaNavigator.deploymentStep.componentEdit.clientUrl(project.id, component.id, item.value),
														displayValue: "Deployment Step: " + step.stepName
													};
													scope.breadcrumbList.push(x);
												}
											}
											else if (configuration) {
												step = _.findWhere(configuration.deploymentStepList, { id: item.value });
												if (step) {
													x = {
														url: SrirachaNavigator.deploymentStep.configurationEdit.clientUrl(project.id, configuration.id, item.value),
														displayValue: "Deployment Step: " + step.stepName
													};
													scope.breadcrumbList.push(x);
												}
											}
											break;
										case "branch":
											branch = _.findWhere(project.branchList, { id: item.value });
											x = {
												url: SrirachaNavigator.branch.edit.clientUrl(project.id, branch.id),
												displayValue: "Branch: " + branch.branchName
											};
											scope.breadcrumbList.push(x);
											break;
										case "environment":
											environment = _.findWhere(project.environmentList, { id: item.value });
											x = {
												url: SrirachaNavigator.environment.edit.clientUrl(project.id, environment.id),
												displayValue: "Environment: " + environment.environmentName
											};
											scope.breadcrumbList.push(x);
											break;
										case "role":
											//skip for now
											break;
										default:
											console.error("Unrecognized breadcrumb URL parameter: " + item.key);
											console.error(item.key);
									}
								}
							}
						},
						function(err) {
							ErrorReporter.handleResourceError(err);
						}
					);
				}
			};
			$rootScope.$on('$locationChangeSuccess', scope.updateBreadcrumbs);
			scope.updateBreadcrumbs();
		}
	};
}]);

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


ngSriracha.directive("projectList", function () {
	return {
		restrict: "E",
		templateUrl: "templates/project/project-list-template.html"
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

