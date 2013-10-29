angular.module("ngSriracha").directive("deployActionLinks",
		['SrirachaResource','SrirachaNavigator','ErrorReporter','PermissionVerifier',
		function (SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
			return {
				restrict: "E",
				templateUrl: "templates/directives/deployActionLinks.html",
				scope: {
					deployBatchRequest: '=',
					refreshDataCallback: '='
				},
				link: function postLink(scope, element, attrs) {
					scope.navigator = SrirachaNavigator;
					scope.startCancelDeployment = function () {
						if (scope.deployBatchRequest) {
							if (confirm("Are you sure you want to cancel this deployment?")) {
								var saveParams = {
									id: scope.deployBatchRequest.id,
									action: "Cancel"//,
									//statusMessage: $scope.statusChange.statusMessage
								};
								var resourceItem = new SrirachaResource.deployBatchAction();
								resourceItem.$save(
									saveParams,
									function () {
										if (scope.refreshDataCallback) {
											scope.refreshDataCallback();
										}
										else {
											scope.navigator.deployment.batchStatus.go(scope.deployBatchRequest.id);
										}
									},
									function (err) {
										ErrorReporter.handleResourceError(err);
									}
								)
							}
						}
					}
					scope.startResumeDeployment = function () {
						if (scope.deployBatchRequest) {
							if (confirm("Are you sure you want to resume this deployment?")) {
								var saveParams = {
									id: scope.deployBatchRequest.id,
									action: "Resume"//,
									//statusMessage: $scope.statusChange.statusMessage
								};
								var resourceItem = new SrirachaResource.deployBatchAction();
								resourceItem.$save(
									saveParams,
									function () {
										if (scope.refreshDataCallback) {
											scope.refreshDataCallback();
										}
										else {
											scope.navigator.deployment.batchStatus.go(scope.deployBatchRequest.id);
										}
									},
									function (err) {
										ErrorReporter.handleResourceError(err);
									}
								)
							}
						}
					}
					scope.canCancel = function () {
						if (scope.deployBatchRequest) {
							if((scope.deployBatchRequest.status == 'NotStarted' || scope.deployBatchRequest.status == 'InProcess') && !scope.deployBatchRequest.cancelRequested) {
								var isValid = scope.validateEnvironmentPermission(scope.deployBatchRequest, function (projectId, environmentId) {
									return PermissionVerifier.canRunDeployment(projectId, environmentId)
								});
								return isValid;
							}
						}
					}
					scope.canResume = function () {
						if (scope.deployBatchRequest) {
							if ((scope.deployBatchRequest.status == 'Cancelled' || scope.deployBatchRequest.status == 'Error') && !scope.deployBatchRequest.resumeRequested) {
								var isValid = scope.validateEnvironmentPermission(scope.deployBatchRequest, function (projectId, environmentId) {
									return PermissionVerifier.canRunDeployment(projectId, environmentId)
								});
								return isValid;
							}
						}
					}
					scope.validateEnvironmentPermission = function (request, callback) {
						var isValid = true;
						_.each(request.itemList, function (item) {
							_.each(item.machineList, function (machine) {
								if (!callback(item.build.projectId, machine.environmentId)) {
									isValid = false;
									return false;
								}
							})
							if (!isValid) {
								return false;
							}
						});
						return isValid;
					}
				}
			};
		}]);
