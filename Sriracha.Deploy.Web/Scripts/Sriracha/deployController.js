ngSriracha.controller("DeployController", function ($scope, $routeParams, SrirachaResource) {
	$scope.build = SrirachaResource.build.get({ buildId: $routeParams.buildId }, function() {
		$scope.project = SrirachaResource.project.get({ id: $scope.build.projectId }, function () {
			$scope.environment = _.findWhere($scope.project.environmentList, { id: $routeParams.environmentId });
			console.log($scope.environment);
			console.log()
			if($scope.environment.componentList) {
				$scope.environmentComponent = _.findWhere($scope.environment.componentList, { componentId: $scope.build.componentId });
				console.log($scope.environmentComponent);
			}	
		});
	});
});