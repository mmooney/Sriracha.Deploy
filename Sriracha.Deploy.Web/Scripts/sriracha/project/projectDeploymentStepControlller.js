ngSriracha.controller("ProjectDeploymentStepController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter','PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	if (!$routeParams.componentId && !$routeParams.configurationId) {
		console.error("Missing $routeParams.componentId and $routeParams.configurationId, one must be provided");
		return;
	}
	if ($routeParams.componentId && $routeParams.configurationId) {
		console.error("Both $routeParams.componentId and $routeParams.configurationId, only one may be provided");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if ($routeParams.componentId) {
			if ($scope.project.componentList) {
				var component = _.findWhere($scope.project.componentList, { id: $routeParams.componentId });
				if (component) {
					$scope.component = new SrirachaResource.component(component);
					$scope.taskMetadataList = SrirachaResource.taskMetadata.query({});

					if ($routeParams.deploymentStepId && $scope.component.deploymentStepList) {
						var deploymentStepItem = _.findWhere(component.deploymentStepList, { id: $routeParams.deploymentStepId });
						if (deploymentStepItem) {
							deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
							$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
						}
					}
				}
			}
		}
		if ($routeParams.configurationId) {
			if ($scope.project.configurationList) {
				var configuration = _.findWhere($scope.project.configurationList, { id: $routeParams.configurationId });
				if(configuration) {
					$scope.configuration = new SrirachaResource.configuration(configuration);
					$scope.taskMetadataList = SrirachaResource.taskMetadata.query({});
				}
				if ($routeParams.deploymentStepId && $scope.configuration.deploymentStepList) {
					var deploymentStepItem = _.findWhere(configuration.deploymentStepList, { id: $routeParams.deploymentStepId });
					if (deploymentStepItem) {
						deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
						$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
					}
				}
			}
		}
		if (!$scope.deploymentStep) {
			$scope.deploymentStep = new SrirachaResource.deploymentStep({ projectId: $routeParams.projectId, componentId: $routeParams.componentId });
		}
	});

	$scope.goBack = function () {
		if ($routeParams.componentId) {
			$scope.navigator.component.view.go($scope.project.id, $routeParams.componentId);
		}
		else if ($routeParams.configurationId) {
			$scope.navigator.configuration.view.go($scope.project.id, $routeParams.configurationId);
		}
		else {
			$scope.navigator.project.view.go($scope.project.id);
		}
	}
	//Deployment Steps
	$scope.saveDeploymentStep = function () {
		$scope.deploymentStep.taskOptionsJson = JSON.stringify($scope.deploymentStep.taskOptions);
		var saveParams = {
			projectId: $routeParams.projectId
		};
		if ($routeParams.componentId) {
			saveParams.parentId = $routeParams.componentId;
			saveParams.parentType = "Component";
		}
		if ($routeParams.configurationId) {
			saveParams.parentId = $routeParams.configurationId;
			saveParams.parentType = "Configuration";
		}
		if ($routeParams.deploymentStepId) {
			saveParams.deploymentStepId = $routeParams.deploymentStepId;
		}
		$scope.deploymentStep.$save(
			saveParams,
			function () {
				if ($routeParams.componentId) {
					$scope.navigator.component.view.go($scope.project.id, $routeParams.componentId);
				}
				else if ($routeParams.configurationId) {
					$scope.navigator.configuration.view.go($scope.project.id, $routeParams.configurationId);
				}
				else {
					$scope.navigator.project.view.go($scope.project.id);
				}
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}

	$scope.deleteDeploymentStep = function () {
		var deleteParms = {
			projectId: $routeParams.projectId,
			id: $routeParams.deploymentStepId
		};
		if ($routeParams.componentId) {
			deleteParms.parentId = $routeParams.componentId;
			deleteParms.parentType = "Component";
		}
		if ($routeParams.configurationId) {
			deleteParms.parentId = $routeParams.configurationId;
			deleteParms.parentType = "Configuration";
		}
		$scope.deploymentStep.$delete(
			deleteParms,
			function () {
				$scope.navigator.component.view.go($scope.project.id, $scope.component.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	//End Deployment Steps
}]);

