ngSriracha.controller("ProjectConfigurationController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter','PermissionVerifier',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
			$scope.navigator = SrirachaNavigator;
			$scope.permissionVerifier = PermissionVerifier;
			if (!$routeParams.projectId) {
				console.error("Missing $routeParams.projectId");
				return;
			}
			$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
				if (!$routeParams.configurationId) {
					$scope.configuration = new SrirachaResource.configuration({ projectId: $routeParams.projectId });
				}
				else if ($scope.project.configurationList) {
					var configuration = _.findWhere($scope.project.configurationList, { id: $routeParams.configurationId });
					if (configuration) {
						$scope.configuration = new SrirachaResource.configuration(configuration);
						$scope.taskMetadataList = SrirachaResource.taskMetadata.query({});

						if ($routeParams.deploymentStepId && $scope.configuration.deploymentStepList) {
							var deploymentStepItem = _.find(configuration.deploymentStepList, function (d) { return d.id == $routeParams.deploymentStepId });
							if (deploymentStepItem) {
								deploymentStepItem.taskOptions = JSON.parse(deploymentStepItem.taskOptionsJson);
								$scope.deploymentStep = new SrirachaResource.deploymentStep(deploymentStepItem);
							}
						}
					}
					if (!$scope.deploymentStep) {
						$scope.deploymentStep = new SrirachaResource.deploymentStep({ projectId: $routeParams.projectId, configurationId: $routeParams.configurationId });
					}
				}
			});

			//configurations
			$scope.saveConfiguration = function () {
				var saveParams = {
					projectId: $routeParams.projectId
				};
				if ($routeParams.configurationId) {
					saveParams.id = $routeParams.configurationId;
				}
				$scope.configuration.$save(
					saveParams,
					function () {
						$scope.navigator.project.view.go($scope.project.id);
					},
					function (error) {
						ErrorReporter.handleResourceError(error);
					}
				);
			}
			$scope.deleteConfiguration = function () {
				SrirachaResource.configuration.delete(
					{
						projectId: $routeParams.projectId,
						id: $routeParams.configurationId
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

			$scope.copyDeploymentSteps = function (item) {
				$scope.configuration.deploymentStepList = [];
				if (item.deploymentStepList && item.deploymentStepList) {
					$scope.copyNextDeploymentStep(item.deploymentStepList, item.deploymentStepList[0]);
				}
			}

			$scope.copyNextDeploymentStep = function (list, step) {
				var saveParams = {
					projectId: $routeParams.projectId,
					parentId: $routeParams.configurationId,
					parentType: "Configuration"
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
							$scope.navigator.configuration.view.go($routeParams.projectId, $routeParams.configurationId);
						}
					},
					function (error) {
						ErrorReporter.handleResourceError(error);
					}
				);
			}
			//End Configurations

		}]);

