angular.module("ngSriracha").directive("deployActionLinks",
		['SrirachaResource','SrirachaNavigator','ErrorReporter',
		function (SrirachaResource, SrirachaNavigator, ErrorReporter) {
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

				}
			};
		}]);
