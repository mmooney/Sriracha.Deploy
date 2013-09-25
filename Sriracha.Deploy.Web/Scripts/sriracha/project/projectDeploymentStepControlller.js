ngSriracha.controller("ProjectDeploymentStepController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	if (!$routeParams.componentId) {
		console.error("$routeParams.componentId");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if ($scope.project.componentList) {
			var component = _.find($scope.project.componentList, function (c) { return c.id == $routeParams.componentId; });
			if (component) {
				$scope.component = new SrirachaResource.component(component);
				$scope.taskMetadataList = SrirachaResource.taskMetadata.query({});

				if ($routeParams.deploymentStepId && $scope.component.deploymentStepList) {
					var deploymentStepItem = _.find(component.deploymentStepList, function (d) { return d.id == $routeParams.deploymentStepId });
					if (deploymentStepItem) {
						deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
						$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
					}
				}
			}
			if (!$scope.deploymentStep) {
				$scope.deploymentStep = new SrirachaResource.deploymentStep({ projectId: $routeParams.projectId, componentId: $routeParams.componentId });
			}
		}
	});

	//Deployment Steps
	$scope.saveDeploymentStep = function () {
		$scope.deploymentStep.taskOptionsJson = JSON.stringify($scope.deploymentStep.taskOptions);
		var saveParams = {
			projectId: $routeParams.projectId, 
			componentId: $routeParams.componentId
		};
		if ($routeParams.deploymentStepId) {
			saveParams.deploymentStepId = $routeParams.deploymentStepId;
		}
		$scope.deploymentStep.$save(
			saveParams,
			function () {
				$scope.navigator.component.view.go($scope.project.id, $scope.component.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}

	$scope.deleteDeploymentStep = function () {
		var deleteParms = {
			projectId: $routeParams.projectId,
			componentId: $routeParams.componentId,
			id: $routeParams.deploymentStepId
		};
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

