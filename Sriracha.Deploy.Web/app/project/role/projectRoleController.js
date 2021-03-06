﻿ngSriracha.controller("ProjectRoleController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter','PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

		$scope.navigator = SrirachaNavigator;
		$scope.permissionVerifier = PermissionVerifier;
	    if (!$routeParams.projectId) {
	        console.error("Missing $routeParams.projectId");
	        return;
	    }

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

	    $scope.isProjectEditable = function () {
	    	return $scope.permissionVerifier.canEditProjectPermissions($routeParams.projectId);
	    }

	    $scope.isEnvironmentEditable = function (environmentId) {
	    	return $scope.permissionVerifier.canEditEnvironmentPermissions($routeParams.projectId, environmentId);
	    }

	    $scope.isAnythingEditable = function () {
	    	if ($scope.isProjectEditable()) {
	    		return true;
	    	}
	    	else {
	    		return _.any($scope.project.environmentList,
							function (x) {
								return $scope.isEnvironmentEditable(x.id);
							});
	    	}
    	}

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

	    $scope.editAssignedUsers = function () {
	    	$scope.editAssignedUsersData = {
	    		userAssignList: []
	    	};
	    	var response = SrirachaResource.user.get(
				{},
				function () {
					_.each(response.userNameList, function (userName) {
						var item = {
							userName: userName,
							selected: false
						}
						if ($scope.projectRole && $scope.projectRole.assignments && $scope.projectRole.assignments.userNameList) {
							item.selected = _.contains($scope.projectRole.assignments.userNameList, userName);
						}
						$scope.editAssignedUsersData.userAssignList.push(item);
					});
					angular.element(".editAssignedUsers").dialog({
						width: 'auto',
						height: 'auto',
						modal: true
					});
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				}
			)
	    }

	    $scope.completeEditAssignedUsers = function () {
	    	if ($scope.projectRole) {
	    		$scope.projectRole.assignments = $scope.projectRole.assignments || {};
	    		$scope.projectRole.assignments.userNameList = _.pluck(_.filter($scope.editAssignedUsersData.userAssignList, function (x) { return x.selected; }), "userName");
	    	}
	    	angular.element(".editAssignedUsers").dialog("destroy");
	    }

	    $scope.deleteUserAssignment = function (userName) {
	    	if ($scope.projectRole) {
	    		if (_.contains($scope.projectRole.assignments.userNameList, userName)) {
	    			$scope.projectRole.assignments.userNameList = _.without($scope.projectRole.assignments.userNameList, userName);
	    		}
	    	}
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

	    $scope.beginDeleteRole = function (role) {
	    	if (role && confirm("Are you sure you want to delete the role \"" + role.roleName + "\"?  This cannot be undone!")) {
	    	    SrirachaResource.projectRole['delete'](
					{ id:role.id, projectId: $routeParams.projectId },
					function () {
						if (role.id == $routeParams.projectRoleId) {
							$scope.navigator.projectRole.edit.go($scope.project.id);
						}
						else {
							$scope.projectRoleList = SrirachaResource.projectRole.query(
								{ projectId: $routeParams.projectId },
								function () {
									if ($routeParams.projectRoleId) {
										$scope.projectRole = SrirachaResource.projectRole.get(
											{ projectId: $routeParams.projectId, id: $routeParams.projectRoleId },
											function () {
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
						}
					},
					function (error) {
						ErrorReporter.handleResourceError(error);
					}
				);
	    	}
	    }
}]);