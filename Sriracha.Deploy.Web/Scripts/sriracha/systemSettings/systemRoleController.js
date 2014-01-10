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
		if ($routeParams.userId) {
		    $scope.systemRole = SrirachaResource.systemSettings.systemRole.get(
				{ id: $routeParams.userId },
				function (data) {
				    $scope.editForm.roleName = data.roleName;
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
	}]);

