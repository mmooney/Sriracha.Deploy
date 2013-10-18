ngSriracha.controller("ProjectRoleController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {


		$scope.navigator = SrirachaNavigator;
	    if (!$routeParams.projectId) {
	        console.error("Missing $routeParams.projectId");
	        return;
	    }

	    $scope.$on("$destroy", function () {
	    	
	    });

	    $scope.project = SrirachaResource.project.get(
			{ id: $routeParams.projectId }, 
			function () {
				$scope.projectRoleList = SrirachaResource.projectRole.query(
					{ projectId: $routeParams.projectId },
					function () {
						if ($routeParams.projectRoleId) {
							$scope.projectRole = SrirachaResource.projectRole.get(
								{projectId: $routeParams.projectId, id: $routeParams.projectRoleId},
								function () {
									console.log($scope.projectRole);
									//ok
								},
								function (err) {
									ErrorReporter.handleResourceError(err);
								}
							)
						}
					},
					function (err) {
						ErrorReporter.handleResourceError(err);
					}
				)
			},
			function(err) {
				ErrorReporter.handleResourceError(err);
			}
		);

	    $scope.saveRole = function() {
	    	var saveParams = {
				id: $routeParams.projectRoleId,
	    		projectId: $routeParams.projectId
	    	};
	    	$scope.projectRole.$save(
				saveParams,
				function (response) {
					$scope.projectRole = response;
					$scope.projectRoleList = SrirachaResource.projectRole.query(
						{ projectId: $routeParams.projectId },
						function () {
							//ok
						},
						function (err) {
							ErrorReporter.handleResourceError(err);
						});
					},
				function (error) {
					ErrorReporter.handleResourceError(error);
				}
			);
	    }

	    $scope.beginAddRole = function () {
	    	$scope.addRoleData = {};
	    	angular.element(".addRoleDialog").dialog({
	    		width: 'auto',
	    		height: 'auto',
	    		modal: true
	    	});
	    }

	    $scope.completeAddRole = function () {
	    	var newRole = new SrirachaResource.projectRole({ projectId: $routeParams.projectId });
	    	var saveParams = {
	    		projectId: $routeParams.projectId,
	    		roleName: $scope.addRoleData.roleName
	    	};
	    	var result = newRole.$save(
				saveParams,
				function (response) {
					$scope.navigator.projectRole.edit.go($scope.project.id, response.id);
				},
				function (error) {
					ErrorReporter.handleResourceError(error);
				}
			);

	    	angular.element(".addRoleDialog").dialog("destroy");
	    	$scope.addRoleData = {};
	    }
}]);