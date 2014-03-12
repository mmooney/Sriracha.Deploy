angular.module("ngSriracha").directive("selectEnvironmentMachines",
		['SrirachaResource', 'ErrorReporter', 'SrirachaNavigator',
		function (SrirachaResource, ErrorReporter, SrirachaNavigator) {
			return {
				restrict: "E",
				templateUrl: "app/directives/selectEnvironmentMachines.html",
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
							scope.environmentSelector = null;
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
														if (scope.selection.preselectedMachineIds) {
															_.each(scope.selection.preselectedMachineIds, function (id) {
																var machine = _.findWhere(scope.selection.machineList, { id: id });
																if (machine) {
																	machine.selected = true;
																}
															});
														}
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
								function (err) {
									ErrorReporter.handleResourceError(err);
								}
							);
						}
					});
				}
			}
		}]);
