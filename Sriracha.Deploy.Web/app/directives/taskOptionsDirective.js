angular.module("ngSriracha").directive("taskOptions",
	['$http', '$templateCache', '$compile', '$parse', 'PermissionVerifier', 'SrirachaResource', 'ErrorReporter',
	function ($http, $templateCache, $compile, $parse, PermissionVerifier, SrirachaResource, ErrorReporter) {
	    return {
	        replace: true,
	        scope: {
	            deploymentStep: '='
	        },
	        template: "<div><div></div></div>",
	        link: function (scope, element, attrs) {
	            scope.permissionVerifier = PermissionVerifier;
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
	            scope.aceChanged = function (event, editor) {
	                var value = editor.getValue();
	                scope.deploymentStep.validation = scope.deploymentStep.validation || [];
	                var validationItem = _.findWhere(scope.deploymentStep.validation, {field:"aceEditor"});
	                if(!validationItem) {
	                    validationItem = {field:"aceEditor"};
	                    scope.deploymentStep.validation.push(validationItem);
	                }
	                try {
	                    scope.deploymentStep.taskOptions = JSON.parse(value)
	                    validationItem.isError = false;
	                }
	                catch (exception) {
	                    validationItem.message = exception.message;
	                    validationItem.isError = true;
	                    console.log(exception);
	                }
	            }
	            scope.$watch("deploymentStep.taskTypeName", function () {
	                if (scope.deploymentStep && scope.deploymentStep.taskTypeName) {
	                    var defaultUrl = "app/directives/deploymentstep-options-default-template.html";
	                    var viewUrl = "api/taskOptionsView/" + scope.deploymentStep.taskTypeName;
	                    $http.get(viewUrl, { cache: $templateCache })
                            .success(function (tplContent) {
                                if (tplContent) {
                                    var x = $compile(tplContent)(scope);
                                    element.children().remove();
                                    element.append(x);
                                }
                                else {
                                    $http.get(defaultUrl, { cache: $templateCache })
                                        .success(function (innerTplContent) {
                                            var x = $compile(innerTplContent)(scope);
                                            element.children().remove();
                                            element.append(x);
                                            scope.jsonEditorTaskOptions = JSON.stringify(scope.deploymentStep.taskOptions, null, 4);
                                        })
                                        .error(function (err) {
                                            ErrorReporter.handleResourceError(err);
                                        });
                                }
                            })
                            .error(function (err) {
                                ErrorReporter.handleResourceError(err);
                            });
	                }
	            });
	        }
	    }
	}]);
