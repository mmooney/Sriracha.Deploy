ngSriracha.controller("ProjectBranchController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter','PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;
	if (!$routeParams.projectId) {
		console.error("Missing $routeParams.projectId");
		return;
	}
	$scope.project = SrirachaResource.project.get({ id: $routeParams.projectId }, function () {
		if ($routeParams.branchId && $scope.project.branchList) {
			var branch = _.find($scope.project.branchList, function (b) { return b.id == $routeParams.branchId });
			if (branch) {
				$scope.branch = new SrirachaResource.branch(branch);
			}
		}
		if (!$scope.branch) {
			$scope.branch = new SrirachaResource.branch({ projectId: $routeParams.projectId });
		}
	});

	//Branches
	$scope.saveBranch = function () {
		var saveParams = {
			projectId: $routeParams.projectId,
			branchId: $routeParams.branchId
		};		
		$scope.branch.$save(
			saveParams,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	$scope.deleteBranch = function () {
		$scope.branch.$delete(
			$scope.branch,
			function () {
				$scope.navigator.project.view.go($scope.project.id);
			},
			function (error) {
				ErrorReporter.handleResourceError(error);
			}
		);
	}
	//End Branches

}]);

