ngSriracha.controller("systemRoleController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter', 'PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

		$scope.navigator = SrirachaNavigator;
		$scope.permissionVerifier = PermissionVerifier;
		$scope.listOptions = {};
		if ($routeParams.pageNumber) $scope.listOptions.pageNumber = $routeParams.pageNumber;
		if ($routeParams.pageSize) $scope.listOptions.pageSize = $routeParams.pageSize;
		if ($routeParams.sortField) $scope.listOptions.sortField = $routeParams.sortField;
		if ($routeParams.sortAscending) $scope.listOptions.sortAscending = $routeParams.sortAscending;

		$scope.editForm = {};
		if ($routeParams.systemRoleId) {
		    $scope.systemRole = SrirachaResource.systemSettings.systemRole.get(
				{ id: $routeParams.systemRoleId },
				function (data) {
				    $scope.editForm.roleName = data.roleName;
				    $scope.editForm.permissions = data.permissions;
				    $scope.editForm.assignments = data.assignments;
				},
				function (err) {
				    ErrorReporter.handleResourceError(err);
				}
			);
		}
		else {
		    $scope.systemRoleList = SrirachaResource.systemSettings.systemRole.get(
				$scope.listOptions,
				function (data) {
				    //console.log(data);
				},
				function (err) {
				    ErrorReporter.handleResourceError(err);
				}
			)
		}

		$scope.canEditRoleName = function () {
		    if ($scope.systemRole) {
		        if ($scope.systemRole.everyoneRoleIndicator) {
		            return false;
		        }
		        else {
		            return $scope.permissionVerifier.canEditSystemRole();
		        }
		    }
		}

		$scope.canEditRole = function () {
		    if ($scope.systemRole) {
		        return $scope.permissionVerifier.canEditSystemRole();
		    }
		}

		$scope.goToPage = function (pageNumber) {
		    $scope.navigator.systemSettings.systemRole.list.go(pageNumber, $scope.systemRoleList.pageSize, $scope.systemRoleList.sortField, $scope.systmeRoleList.sortAscending);
		};
		$scope.applySort = function (sortField, sortAscending) {
		    $scope.navigator.systemSettings.systemRole.list.go(1, $scope.systemRoleList.pageSize, sortField, sortAscending);
		}

		//$scope.deleteUser = function () {
		//    if (!$scope.editForm) {
		//        alert("Error: scope.editForm is null")
		//        return;
		//    }
		//    var item = new SrirachaResource.users();
		//    item.userName = $scope.editForm.userName;
		//    item.emailAddress = $scope.editForm.emailAddress;
		//    item.password = $scope.editForm.password;
		//    var deleteParams = {
		//        id: $routeParams.userId
		//    };
		//    var result = item.$delete(deleteParams,
		//		function (data) {
		//			$scope.navigator.systemSettings.users.list.go();
		//		},
		//		function (err) {
		//			ErrorReporter.handleResourceError(err);
		//		}
		//	)
		//}

		$scope.saveSystemRole = function () {
		    if (!$scope.editForm) {
		        alert("Error: scope.editForm is null")
		        return;
		    }
		    var isValid = true;
		    if (!$scope.editForm.roleName) {
		        $scope.editForm.roleNameError = "Role Name required";
		        isValid = false;
		    }
		    if (isValid) {
		        var item = new SrirachaResource.systemSettings.systemRole();
		        item.roleName = $scope.editForm.roleName;
		        item.permissions = $scope.editForm.permissions,
		        item.assignments = $scope.editForm.assignments
		        var saveParams = {};
		        if ($routeParams.systemRoleId) {
		            saveParams.id = $routeParams.systemRoleId;
		        }
		        var result = item.$save(saveParams,
					function (data) {
					    if ($routeParams.systemRoleId) {
					        $scope.navigator.systemSettings.systemRole.list.go();
					    }
					    else {
					        $scope.navigator.systemSettings.systemRole.list.go();
					        //$scope.navigator.systemSettings.credentials.edit.go(data.id);
					    }
					},
					function (err) {
					    ErrorReporter.handleResourceError(err);
					}
				)
		    }
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
				        if ($scope.editForm && $scope.editForm.assignments && $scope.editForm.assignments.userNameList) {
				            item.selected = _.contains($scope.editForm.assignments.userNameList, userName);
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
		    if ($scope.editForm) {
		        $scope.editForm.assignments = $scope.editForm.assignments || {};
		        $scope.editForm.assignments.userNameList = _.pluck(_.filter($scope.editAssignedUsersData.userAssignList, function (x) { return x.selected; }), "userName");
		    }
		    angular.element(".editAssignedUsers").dialog("destroy");
		}

		$scope.deleteUserAssignment = function (userName) {
		    if ($scope.editForm && editForm.assignments) {
		        if (_.contains($scope.editForm.assignments.userNameList, userName)) {
		            $scope.editForm.assignments.userNameList = _.without($scope.editForm.assignments.userNameList, userName);
		        }
		    }
		}

		$scope.deleteSystemRole = function () {
		    if (!$scope.editForm) {
		        alert("Error: scope.editForm is null")
		        return;
		    }
		    var item = new SrirachaResource.systemSettings.systemRole();
		    var deleteParams = {
		        id: $routeParams.systemRoleId
		    };
		    var result = item.$delete(deleteParams,
				function (data) {
				    $scope.navigator.systemSettings.systemRole.list.go();
				},
				function (err) {
				    ErrorReporter.handleResourceError(err);
				}
			)
		}
	}]);

