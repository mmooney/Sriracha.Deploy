ngSriracha.controller("deployBatchRequestController",
		['$scope', '$routeParams', '$rootElement', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, $rootElement, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.selection = {};
	$scope.idValues = {
		buildId: $routeParams.buildId,
		environmentId: $routeParams.environmentId
	};

	$scope.$on("$destroy", function () {
		angular.element(".promoteBuildDialog").dialog("destroy").remove();
		angular.element(".editBuildDialog").dialog("destroy").remove();
	});

	if ($routeParams.sourceDeployBatchRequestId) {
		$scope.sourceDeployBatchRequest = SrirachaResource.deployBatchRequest.get({ id: $routeParams.sourceDeployBatchRequestId },
			function () {
				$scope.selectedItems = $scope.selectedItems || [];
				_.each($scope.sourceDeployBatchRequest.itemList, function (x) {
					var selectedBuild = x.build;
					var queryParameters = {
						projectId: x.build.projectId,
						projectBranchId: x.build.projectBranchId,
						projectComponentId: x.build.projectComponentId
					};
					$scope.selectedItems.push(x);
				});
			},
			function (err) {
				ErrorReporter.handleResourceError()
			});
	}

	$scope.displayPromoteBuildScreen = function () {
		$scope.promoteDeployment = {};
		$scope.promoteDeployment.allEnvironmentNameList = [];
		_.each($scope.projectList, function (project) {
			_.each(project.environmentList, function (environment) {
				if (environment.componentList) {
					_.each(environment.componentList, function (component) {
						if (component.machineList && component.machineList.length) {
							if (!_.contains($scope.promoteDeployment.allEnvironmentNameList, environment.environmentName)) {
								$scope.promoteDeployment.allEnvironmentNameList.push(environment.environmentName);
							}
						}
					});
				}
			});
		});
		angular.element(".promoteBuildDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}

	$scope.canPromoteDeployment = function () {
		if ($scope.promoteDeployment) {
			return ($scope.promoteDeployment.environmentName);
		}
		return false;
	}

	$scope.anyfailedValidationBuilds = function () {
		if ($scope.promoteDeployment && $scope.promoteDeployment.failedValidationBuilds && $scope.promoteDeployment.failedValidationBuilds.length) {
			return true;
		}
		else {
			return false;
		}
	}
	$scope.promoteEnvironmentSelected = function () {
		$scope.promoteDeployment.buildsWithNoMachines = [];
		$scope.promoteDeployment.failedValidationBuilds = [];
		_.each($scope.selectedItems, function (item) {
			var project = _.findWhere($scope.projectList, { id: item.build.projectId });
			var environment = _.findWhere(project.environmentList, { environmentName: $scope.promoteDeployment.environmentName });
			if (environment == null) {
				$scope.promoteDeployment.buildsWithNoMachines.push(item);
			}
			else {
				var hasConfig = false;
				var component = _.findWhere($scope.project.componentList, { id: item.build.projectComponentId });
				if (component.useConfigurationGroup && component.configurationId) {
					var environmentComponent = _.findWhere(environment.configurationList, { parentId: component.configurationId });
					if (!environmentComponent || !environmentComponent.machineList || !environmentComponent.machineList) {
						$scope.promoteDeployment.buildsWithNoMachines.push(item);
						hasConfig = false;
					}
					else {
						hasConfig = true;
					}
				}
				else {
					var environmentComponent = _.findWhere(environment.componentList, { parentId: item.build.projectComponentId });
					if (!environmentComponent || !environmentComponent.machineList || !environmentComponent.machineList) {
						$scope.promoteDeployment.buildsWithNoMachines.push(item);
						hasConfig = false;
					}
					else {
						hasConfig = true;
					}
				}
				if (hasConfig) {
					var validationResult = SrirachaResource.validateEnvironment.get(
						{ buildId: item.build.id, environmentId: environment.id },
						function () {
							_.each(validationResult.validationResult.resultList, function (resultItem) {
								if (resultItem.taskValidationResult.status != "Success") {
									if (!_.contains($scope.promoteDeployment.failedValidationBuilds, item)) {
										$scope.promoteDeployment.failedValidationBuilds.push(item);
									}
								}
							});
						},
						function (err) {
							ErrorReporter.handleResourceError(err);
						}
					);
				}
			}
		});

	}

	$scope.completeBuildPromotion = function () {
		if ($scope.promoteDeployment.failedValidationBuilds) {
			_.each($scope.promoteDeployment.failedValidationBuilds, function (failedBuild) {
				if (_.contains($scope.selectedItems, failedBuild)) {
					$scope.selectedItems = _.without($scope.selectedItems, failedBuild);
				}
			});
		}
		if ($scope.promoteDeployment.buildsWithNoMachines) {
			_.each($scope.promoteDeployment.buildsWithNoMachines, function (badBuild) {
				$scope.selectedItems = _.reject($scope.selectedItems, function (x) { return x == badBuild; });
			});
		}
		_.each($scope.selectedItems, function (item) {
			var project = _.findWhere($scope.projectList, { id: item.build.projectId });
			var environment = _.findWhere(project.environmentList, { environmentName: $scope.promoteDeployment.environmentName });
			var component = _.findWhere(project.componentList, { id: item.build.projectComponentId });
			if (component.useConfigurationGroup && component.configurationId) {
				var environmentComponent = _.findWhere(environment.configurationList, { parentId: component.configurationId });
				item.machineList = environmentComponent.machineList.slice(0);
			}
			else {
				var environmentComponent = _.findWhere(environment.componentList, { parentId: item.build.projectComponentId });
				item.machineList = environmentComponent.machineList.slice(0);
			}
		});
		//$(".promoteBuildDialog").dialog("close");
		angular.element(".promoteBuildDialog").dialog("destroy");
	}

	$scope.takeLatestBuilds = function () {
		if (confirm("Are you sure you want the latest version of all builds?")) {
			_.each($scope.selectedItems, function (item) {
				var queryParameters = {
					projectId: item.build.projectId,
					projectBranchId: item.build.projectBranchId,
					projectComponentId: item.build.projectComponentId
				};
				var buildList = SrirachaResource.build.query(queryParameters,
					function (data) {
						var latest = _.first(_.sortBy(data, function (build) { return build.version; }).reverse());
						item.build = latest;
					},
					function (error) {
						Error.handleResourceError(error);
					});
			})
		}
	}

	$scope.displayAddBuildScreen = function () {
		$scope.resetAddEditBuildForm();
		angular.element(".editBuildDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}
	$scope.projectList = SrirachaResource.project.query({},
		function () {
		},
		function (error) {
			ErrorReporter.handleResourceError(error);
		});

	$scope.isLatestBuild = function (item) {
		return false;
	}
	$scope.refreshBuildAndEnvironmentList = function (selectedBuildId, selectedEnvironmentId, selectedMachineIds) {
		queryParameters = {};
		if($scope.project) {
			queryParameters.projectId = $scope.project.id;
		}
		if($scope.branch) {
			queryParameters.projectBranchId = $scope.branch.id;
		}
		if($scope.component) {
			queryParameters.projectComponentId = $scope.component.id;
		}
		$scope.buildList = SrirachaResource.build.query(queryParameters,
			function () {
				if (selectedBuildId) {
					$scope.build = _.findWhere($scope.buildList, { id: selectedBuildId });
					$scope.selection.preselectedMachineIds = selectedMachineIds;
					$scope.updateEnvironmentMachines();
				}
			},
			function(error) {
				Error.handleResourceError(error);
			});
		$scope.environmentList = null;
		if ($scope.project && $scope.component) {
			if ($scope.component.useConfigurationGroup && $scope.component.configurationId) {
				$scope.environmentList = _.filter($scope.project.environmentList,
											function (env) {
												if (!env || !env.configurationList) {
													return false;
												}
												var anyItems = _.any(env.configurationList,
													function (x) {
														return x.parentId == $scope.component.configurationId;
													}
												);
												return anyItems;
											});
			}
			else {
				$scope.environmentList = _.filter($scope.project.environmentList,
											function (env) {
												if (!env || !env.componentList) {
													return false;
												}
												var anyItems = _.any(env.componentList,
													function (x) {
														return x.parentId == $scope.component.id;
													}
												);
												return anyItems;
											});
			}
		}
		if (selectedEnvironmentId) {
			$scope.environment = _.findWhere($scope.environmentList, { id: selectedEnvironmentId });
		}
	}

	$scope.buildSelected = function () {
		$scope.updateEnvironmentMachines();
	}

	$scope.environmentSelected = function () {
		//$scope.machineList = null;
		//$scope.selectedMachines = null;
		//var environmentComponent = _.find($scope.environment.componentList, function (component) { return component.componentId == $scope.component.id; });
		//if (environmentComponent) {
		//	$scope.machineList = environmentComponent.machineList;
		//	$scope.selectedMachines = [];
		//}
		$scope.updateEnvironmentMachines();
	}

	$scope.canAddBuild = function () {
		if ($scope.build && $scope.selection.machineList && $scope.selection.machineList.length) {
			return _.any($scope.selection.machineList, function (x) { return x.selected; });
		}
		else {
			return false;
		}
	}

	$scope.updateEnvironmentMachines = function () {
		if($scope.build && $scope.environment) {
			$scope.idValues.buildId = $scope.build.id;
			$scope.idValues.environmentId = $scope.environment.id;
		}
		else  {
			$scope.idValues.buildId = null;
			$scope.idValues.environmentId = null;
		}
	}

	$scope.addBuildToBatch = function () {
		$scope.selectedItems = $scope.selectedItems || [];
		if ($scope.editingItem) {
			$scope.editingItem.build = $scope.build,
			$scope.editingItem.machineList = _.filter($scope.selection.machineList, function (x) { return x.selected; })
		}
		else {
			var deploymentItem = {
				build: $scope.build,
				machineList: _.filter($scope.selection.machineList, function (x) { return x.selected; })
			};

			$scope.selectedItems.push(deploymentItem);

		}
		//angular.element(".editBuildDialog").dialog("close");
		angular.element(".editBuildDialog").dialog("destroy");
		$scope.resetAddEditBuildForm();
	}

	$scope.resetAddEditBuildForm = function () {
		$scope.editingItem = null;
		$scope.project = null;
		$scope.environment = null;
		$scope.build = null;
		$scope.branch = null;
	}

	$scope.moveItemUp = function (item) {
		var index = $scope.selectedItems.indexOf(item);
		$scope.selectedItems.splice(index - 1, 2, $scope.selectedItems[index], $scope.selectedItems[index-1]);
	}

	$scope.moveItemDown = function (item) {
		var index = $scope.selectedItems.indexOf(item);
		$scope.selectedItems.splice(index, 2, $scope.selectedItems[index+1], $scope.selectedItems[index]);
	}

	$scope.removeItem = function (item) {
		if (confirm("Are you sure you want to remove this item (" + item.build.displayValue + ")?")) {
			var index = $scope.selectedItems.indexOf(item);
			$scope.selectedItems.splice(index, 1);
		}
	}

	$scope.editItem = function (item) {
		$scope.resetAddEditBuildForm();
		$scope.project = _.findWhere($scope.projectList, { id: item.build.projectId });
		$scope.branch = _.findWhere($scope.project.branchList, { id: item.build.projectBranchId });
		$scope.component = _.findWhere($scope.project.componentList, { id: item.build.projectComponentId });
		var environmentId;
		if (item.machineList) {
			environmentId = item.machineList[0].environmentId;
		}
		var selectedMachineIds = [];
		if (item.machineList) {
			selectedMachineIds = _.pluck(item.machineList, "id");
		}
		$scope.refreshBuildAndEnvironmentList(item.build.id, environmentId, selectedMachineIds);
		$scope.editingItem = item;
		angular.element(".editBuildDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}

	$scope.submitBuildRequest = function () {
		var request = new SrirachaResource.deployBatchRequest();
		request.itemList = $scope.selectedItems;
		request.status = "Requested";
		request.$save(null,
			function () {
				$scope.navigator.deployment.batchStatus.go(request.id);
			},
			function (err) {
				ErrorReporter.handleResourceError(err);
			});
	}

}]);
