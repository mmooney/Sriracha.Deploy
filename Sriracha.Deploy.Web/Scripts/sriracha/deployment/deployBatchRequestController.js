﻿ngSriracha.controller("deployBatchRequestController", function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.selection = {

	};
	$scope.idValues = {
		buildId: $routeParams.buildId,
		environmentId: $routeParams.environmentId
	};

	$scope.projectList = SrirachaResource.project.query({},
		function () {
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
											if(!env || !env.componentList) {
												return false;
											}
											var anyItems = _.any(env.componentList,
												function (x) {
													return x.componentId == $scope.component.id;
												}
											);
											return anyItems;
										})
		}
		$scope.updateEnvironmentMachines();
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
		if ($scope.selectedItems.indexOf($scope.build) >= 0) {
			alert("This build has already been included");
		}
		else {
			var deploymentItem = {
				build: $scope.build,
				machineList: _.filter($scope.selection.machineList, function (x) { return x.selected; })
			};

			$scope.selectedItems.push(deploymentItem);
			$scope.build = null;
		}
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

	$scope.submitBuildRequest = function () {
		var request = new SrirachaResource.deployBatchRequest();
		request.itemList = $scope.selectedItems;
		request.$save(null,
			function () {
				$scope.navigator.deployment.batchStatus.go(request.id);
			},
			function (err) {
				ErrorReporter.handleResourceError(err);
			});
	}
});
