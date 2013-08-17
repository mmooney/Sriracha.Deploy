﻿ngSriracha.controller("DeployController", function ($scope, $routeParams, SrirachaResource, ErrorReporter) {
	$scope.selection = {

	};
	$scope.idValues = {
		buildId: $routeParams.buildId,
		environmentId: $routeParams.environmentId
	};

	//if ($routeParams.buildId) {
	//	$scope.build = SrirachaResource.build.get({ buildId: $routeParams.buildId }, function () {
	//		$scope.project = SrirachaResource.project.get({ id: $scope.build.projectId }, function () {
	//			$scope.environment = _.findWhere($scope.project.environmentList, { id: $routeParams.environmentId });
	//			if ($scope.environment.componentList) {
	//				$scope.environmentComponent = _.findWhere($scope.environment.componentList, { componentId: $scope.build.projectComponentId });
	//			}
	//		});
	//		$scope.deployRequestTemplate = SrirachaResource.deployRequest.get({ buildId: $routeParams.buildId, environmentId: $routeParams.environmentId }, function () {
	//			$scope.environmentResults = $scope.getEnvironmentResults($scope.deployRequestTemplate);
	//			$scope.environmentResultsIncomplete = _.any($scope.environmentResults, function (x) { return !x.present; });

	//			$scope.machineResults = {};
	//			$scope.machineResultsIncomplete = {};
	//			_.each($scope.environmentComponent.machineList, function (machine) {
	//				$scope.machineResults[machine.id] = $scope.getMachineResults($scope.deployRequestTemplate, machine.id);
	//				$scope.machineResultsIncomplete[machine.id] = _.any($scope.machineResults[machine.id], function (x) { return !x.present; });
	//			});
	//		});
	//	});
	//}
	if ($routeParams.deployStateId) {
		$scope.deployState = SrirachaResource.deployState.get({ id: $routeParams.deployStateId }, 
			function () {
				$scope.project = SrirachaResource.project.get({ id: $scope.deployState.projectId },
					function () {
						$scope.refreshStatus();
					},
					function (error) {
						$scope.reportError(error);
					});
			},
			function(error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.refreshStatus = function () {
		$scope.deployState = SrirachaResource.deployState.get({ id: $routeParams.deployStateId },
			function () {
				if ($scope.deployState.status == "NotStarted" || $scope.deployState.status == "InProcess") {
					setTimeout($scope.refreshStatus, 10000);
				}
			},
			function (error) {
				$scope.reportError(error);
			});
	}

	$scope.getEditEnvironmentUrl = function (environment) {
		if (environment) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.EditUrl, { projectId: environment.projectId, environmentId: environment.id });
		}
	}

	$scope.canSubmitDeployment = function () {
		if ($scope.selection && $scope.selection.machineList) {
			return _.any($scope.selection.machineList, function (x) { return x.selected; });
		}
		else {
			return false;
		}
	}

	$scope.submitDeployment = function () {
		var selectedMachines = _.select($scope.selection.machineList, function (x) { return x.selected; });
		if (!selectedMachines || !selectedMachines.length) {
			alert("No target machines selected");
		}
		var saveParams = {
			environmentId: $scope.idValues.environmentId,
			buildId: $scope.idValues.buildId,
			machineIdList: _.pluck(selectedMachines, 'id')
		};
		var deployRequestTemplate = new SrirachaResource.deployRequest();
		deployRequestTemplate.$save(
			saveParams,
			function (result) {
				alert("Deployment submitted!");
				Sriracha.Navigation.Deployment.View(result.deployStateId);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}

	//	var returnValue = [];

	//	if (deployRequestTemplate && deployRequestTemplate.validationResult) {
	//		for (var i = 0; i < deployRequestTemplate.validationResult.resultList.length; i++) {
	//			var resultItem = deployRequestTemplate.validationResult.resultList[i];
	//			var machineResultList = resultItem.taskValidationResult.machineResultList[machineId];
	//			if (machineResultList && machineResultList.length) {
	//				for (var j = 0; j < machineResultList.length; j++) {
	//					var machineResultItem = machineResultList[j];
	//					var item = {
	//						taskName: resultItem.deploymentStep.stepName,
	//						settingName: machineResultItem.fieldName,
	//						present: machineResultItem.present
	//					};
	//					if (machineResultItem.present) {
	//						if (machineResultItem.sensitive) {
	//							item.settingValue = "*******************";
	//						}
	//						else {
	//							item.settingValue = machineResultItem.fieldValue;
	//						}
	//					}
	//					else {
	//						item.settingValue = "N/A";
	//					}
	//					returnValue.push(item);
	//				}
	//			}
	//		}
	//	}
	//	return returnValue;
	//}

	//$scope.getEnvironmentResults = function (deployRequestTemplate) {
	//	var returnValue = [];
	//	if (deployRequestTemplate && deployRequestTemplate.validationResult) {
	//		for (var i = 0; i < deployRequestTemplate.validationResult.resultList.length; i++) {
	//			var resultItem = deployRequestTemplate.validationResult.resultList[i];
	//			if (resultItem.taskValidationResult.environmentResultList.length) {
	//				for (var j = 0; j < resultItem.taskValidationResult.environmentResultList.length; j++) {
	//					var environmentResultItem = resultItem.taskValidationResult.environmentResultList[j];
	//					var item = {
	//						taskName: resultItem.deploymentStep.stepName,
	//						settingName: environmentResultItem.fieldName,
	//						present: environmentResultItem.present
	//					};
	//					if (environmentResultItem.present) {
	//						if (environmentResultItem.sensitive) {
	//							item.settingValue = "*******************";
	//						}
	//						else {
	//							item.settingValue = environmentResultItem.fieldValue;
	//						}
	//					}
	//					else {
	//						item.settingValue = "N/A";
	//					}
	//					returnValue.push(item);
	//				}
	//			}
	//		}
	//	}
	//	return returnValue;
	//}
});