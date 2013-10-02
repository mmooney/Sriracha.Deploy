ngSriracha.controller("ProjectComponentController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if (!$routeParams.componentId) {
			$scope.component = new SrirachaResource.component({ projectId: $routeParams.projectId });
		}
		else if ($scope.project.componentList) {
			var component = _.find($scope.project.componentList, function (c) { return c.id == $routeParams.componentId; });
			if (component) {
				$scope.component = new SrirachaResource.component(component);
				$scope.component.useConfigurationGroup = $scope.component.useConfigurationGroup || false;
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

	//Components
	$scope.saveComponent = function () {
		var saveParams = {
			projectId: $routeParams.projectId
		};
		if ($routeParams.componentId) {
			saveParams.id = $routeParams.componentId;
		}
		$scope.component.$save(
			saveParams,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	$scope.deleteComponent = function () {
		SrirachaResource.component.delete(
			{
				projectId: $routeParams.projectId,
				id: $routeParams.componentId
			},
			function () {
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}

	$scope.startCopyDeploymentSteps = function () {
		$scope.copyingDeploymentSteps = true;
	}

	$scope.copyDeploymentSteps = function(item) {
		$scope.component.deploymentStepList = [];
		if (item.deploymentStepList && item.deploymentStepList) {
			$scope.copyNextDeploymentStep(item.deploymentStepList, item.deploymentStepList[0]);
		}
	}

	$scope.copyNextDeploymentStep = function(list, step) {
		var saveParams = {
			projectId: $routeParams.projectId,
			componentId: $routeParams.componentId
		};
		step.id = null;
		var x = new SrirachaResource.deploymentStep(step);
		x.$save(
			saveParams,
			function () {
				var found = false;
				for (var i = 0; i < list.length - 1; i++) {
					if (step == list[i]) {
						var nextStep = list[i + 1];
						found = true;
						$scope.copyNextDeploymentStep(list, nextStep)
					}
				}
				if (!found) {
					$scope.navigator.component.view.go($routeParams.projectId,$routeParams.componentId);
				}
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	//End Components

}]);

