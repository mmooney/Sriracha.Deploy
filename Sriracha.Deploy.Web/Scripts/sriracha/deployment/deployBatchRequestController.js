ngSriracha.controller("deployBatchRequestController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.name = "test";
	console.log("hi!");
	$scope.projectList = SrirachaResource.project.query({},
		function () {
			console.log($scope.projectList);
		},
		function (error) {
			ErrorReporter.handleResourceError(error);
		});

	$scope.refreshBuildAndEnvironmentList = function () {
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
			function() {
				//console.log($scope.buildList);
			},
			function(error) {
				Error.handleResourceError(error);
			});
		$scope.environmentList = null;
		if ($scope.project && $scope.component) {
			$scope.environmentList = _.filter($scope.project.environmentList,
										function (env) {
											var anyItems = _.any(env.componentList,
												function (x) {
													return x.componentId == $scope.component.id;
												}
											);
											return anyItems;
										})
		}
	}

	$scope.environmentSelected = function () {
		$scope.machineList = null;
		$scope.selectedMachines = null;
		var environmentComponent = _.find($scope.environment.componentList, function (component) { return component.componentId == $scope.component.id; });
		if (environmentComponent) {
			$scope.machineList = environmentComponent.machineList;
			$scope.selectedMachines = [];
		}
	}

	$scope.addBuildToBatch = function () {
		$scope.selectedItems = $scope.selectedItems || [];
		if ($scope.selectedItems.indexOf($scope.build) >= 0) {
			alert("This build has already been included");
		}
		else {
			$scope.selectedItems.push($scope.build);
			$scope.build = null;
		}
	}
});
