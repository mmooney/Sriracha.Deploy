ngSriracha.controller("DeployController", function ($scope, $routeParams, SrirachaResource) {
	$scope.build = SrirachaResource.build.get({ buildId: $routeParams.buildId }, function() {
		$scope.project = SrirachaResource.project.get({ id: $scope.build.projectId }, function () {
			$scope.environment = _.findWhere($scope.project.environmentList, { id: $routeParams.environmentId });
			if($scope.environment.componentList) {
				$scope.environmentComponent = _.findWhere($scope.environment.componentList, { componentId: $scope.build.projectComponentId });
			}	
		});
		$scope.deployRequestTemplate = SrirachaResource.deployRequest.get({ buildId: $routeParams.buildId, environmentId: $routeParams.environmentId }, function () {
			$scope.environmentResults = $scope.getEnvironmentResults($scope.deployRequestTemplate);
			$scope.environmentResultsIncomplete = _.any($scope.environmentResults, function (x) { return !x.present; });
			console.log($scope.environmentResultsIncomplete);
		});
	});

	$scope.getEditEnvironmentUrl = function (environment) {
		if (environment) {
			return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Environment.EditUrl, { projectId: environment.projectId, environmentId: environment.id });
		}
	}
	$scope.getEnvironmentResults = function (deployRequestTemplate) {
		var returnValue = [];
		if (deployRequestTemplate && deployRequestTemplate.validationResult) {
			for (var i = 0; i < deployRequestTemplate.validationResult.resultList.length; i++) {
				var resultItem = deployRequestTemplate.validationResult.resultList[i];
				if (resultItem.taskValidationResult.environmentResultList.length) {
					console.log(resultItem.taskValidationResult.environmentResultList.length);
					for (var j = 0; j < resultItem.taskValidationResult.environmentResultList.length; j++) {
						var environmentResultItem = resultItem.taskValidationResult.environmentResultList[j];
						var item = {
							taskName: resultItem.deploymentStep.stepName,
							settingName: environmentResultItem.fieldName,
							present: environmentResultItem.present
						};
						if (environmentResultItem.present) {
							if (environmentResultItem.sensitive) {
								item.settingValue = "*******************";
							}
							else {
								item.settingValue = environmentResultItem.fieldValue;
							}
						}
						else {
							item.settingValue = "N/A";
						}
						returnValue.push(item);
					}
				}
			}
		}
		console.log(returnValue);
		return returnValue;
	}
});