angular.module("ngSriracha").directive("taskOptions",
	['$http', '$templateCache', '$compile', '$parse', '$modal', 'PermissionVerifier', 'SrirachaResource', 'ErrorReporter',
	function ($http, $templateCache, $compile, $parse, $modal, PermissionVerifier, SrirachaResource, ErrorReporter) {
	    return {
	        replace: true,
	        scope: {
	            deploymentStep: '=',
                browseContents: '&'
	        },
	        template: "<div><div></div></div>",
	        link: function (scope, element, attrs) {
	            scope.permissionVerifier = PermissionVerifier;
	            scope.addListItem = function (listParent, listName, item) {
	                listParent = listParent || scope;
	                listParent[listName] = listParent[listName] || [];
	                item = item || {};
	                listParent[listName].push(item);
	            }
	            scope.deleteListItem = function (listParent, listName, item) {
	                listParent = listParent || scope;
	                if (listParent[listName]) {
	                    var index = listParent[listName].indexOf(item);
	                    if (index >= 0) {
	                        listParent[listName].splice(index, 1);
	                    }
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
	                }
	            }
	            scope.browseContents = function (targetObject, targetFieldName) {
	                console.log("browseContents")
	                var queryParameters = {
	                    projectId: scope.deploymentStep.projectId,
	                    projectComponentId: scope.deploymentStep.parentId,
	                    sortField: "UpdatedDateTimeUtc",
	                    sortAscending: false,
	                    pageSize: 10
	                };
	                var buildList = SrirachaResource.build.get(
                        queryParameters,
                        function () {
                            var modalInstance = $modal.open({
                                templateUrl: 'app/dialogs/filebrowser-template.html',
                                controller: "FileBrowserController",
                                resolve: {
                                    buildList: function () {
                                        return buildList.items;
                                    }
                                }
                            });
                            modalInstance.result.then(
                                function (selectedFile) {
                                    if (selectedFile && selectedFile.fileName) {
                                        var fullPath = "${deploy:directory}\\";
                                        if (selectedFile.directory) {
                                            var reformattedDirectory = selectedFile.directory;
                                            if (reformattedDirectory[0] == '/' || reformattedDirectory[0] == '\\') {
                                                reformattedDirectory = reformattedDirectory.substring(1);
                                            }
                                            if (reformattedDirectory && reformattedDirectory.length &&
                                                (reformattedDirectory[reformattedDirectory.length - 1] == '/' || reformattedDirectory[reformattedDirectory.length - 1] == '\\')) {
                                                reformattedDirectory = reformattedDirectory.substring(0, reformattedDirectory.length - 1);
                                            }
                                            if (reformattedDirectory && reformattedDirectory.length) {
                                                var re = /\//g;
                                                reformattedDirectory = reformattedDirectory.replace(re, '\\');
                                            }
                                            if (reformattedDirectory && reformattedDirectory.length) {
                                                fullPath += reformattedDirectory + "\\";
                                            }
                                        }
                                        fullPath += selectedFile.fileName;
                                        targetObject[targetFieldName] = fullPath;
                                    }
                                }
                            );
                        },
                        function (err) {
                            ErrorReporter.handleResourceError(err);
                        }
                    );
	            };
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
