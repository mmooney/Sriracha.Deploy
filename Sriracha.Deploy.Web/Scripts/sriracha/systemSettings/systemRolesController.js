ngSriracha.controller("systemRolesController",
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
		    $scope.systemRole = SrirachaResource.systemRole.get(
				{ id: $routeParams.userId },
				function (data) {
                    console.log($scope.systemRole)
				    //$scope.editForm.userName = data.userName;
				    //$scope.editForm.emailAddress = data.emailAddress;
				},
				function (err) {
				    ErrorReporter.handleResourceError(err);
				}
			);
		}
		else {
		    $scope.systemRoleList = SrirachaResource.systemRole.get(
				$scope.listOptions,
				function (data) {
				    //console.log(data);
				},
				function (err) {
				    ErrorReporter.handleResourceError(err);
				}
			)
		}

		//$scope.goToPage = function (pageNumber) {
		//    $scope.navigator.systemSettings.systemRole.list.go(pageNumber, $scope.userList.pageSize, $scope.userList.sortField, $scope.userList.sortAscending);
		//};
		//$scope.applySort = function (sortField, sortAscending) {
		//    $scope.navigator.systemSettings.systemRole.list.go(1, $scope.userList.pageSize, sortField, sortAscending);
		//}

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

		//$scope.saveUser = function () {
		//    if (!$scope.editForm) {
		//        alert("Error: scope.editForm is null")
		//        return;
		//    }
		//    var isValid = true;
		//    if (!$scope.editForm.userName) {
		//        $scope.editForm.userNameError = "User Name required";
		//        isValid = false;
		//    }
		//    if (!$routeParams.userId && !$scope.editForm.password) {
		//        $scope.editForm.passwordError = "Password required";
		//        isValid = false;
		//    }
		//    else if ($scope.editForm.password != $scope.editForm.confirmPassword) {
		//        $scope.editForm.confirmPasswordError = "Passwords do not match";
		//        isValid = false;
		//    }
		//    if (isValid) {
		//        var item = new SrirachaResource.users();
		//        item.userName = $scope.editForm.userName;
		//        item.emailAddress = $scope.editForm.emailAddress;
		//        item.password = $scope.editForm.password;
		//        var saveParams = {};
		//        if ($routeParams.userId) {
		//            saveParams.id = $routeParams.userId;
		//        }
		//        var result = item.$save(saveParams,
		//			function (data) {
		//			    if ($routeParams.userId) {
		//			        $scope.navigator.systemSettings.users.list.go();
		//			    }
		//			    else {
		//			        $scope.navigator.systemSettings.users.list.go();
		//			        //$scope.navigator.systemSettings.credentials.edit.go(data.id);
		//			    }
		//			},
		//			function (err) {
		//			    ErrorReporter.handleResourceError(err);
		//			}
		//		)
		//    }
		//}
	}]);

