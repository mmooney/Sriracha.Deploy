ngSriracha.controller("CredentialsController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter', 'PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

		$scope.navigator = SrirachaNavigator;
		$scope.permissionVerifier = PermissionVerifier;
		$scope.editForm = {};
		console.log(SrirachaResource)
		if ($routeParams.credentialsId) {
			$scope.credentialsItem = SrirachaResource.credentials.get(
				{id: $routeParams.credentialsId},
				function (data) {
					$scope.editForm.userName = data.userName;
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				}
			);
		}
		else  {
			$scope.credentialsList = SrirachaResource.credentials.get(
				{},
				function (data) {
					console.log(data);
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				}
			)
		}

		$scope.saveCredentials = function () {
			if (!$scope.editForm) {
				alert("Error: scope.editForm is null")
				return;
			}
			var isValid = true;
			$scope.editForm.userNameError = null;
			$scope.editForm.passwordError = null;
			$scope.editForm.confirmPasswordError = null;
			if (!$scope.editForm.userName) {
				$scope.editForm.userNameError = "User Name Required";
				isValid = false;
			} 
			if (!$scope.editForm.password) {
				$scope.editForm.passwordError = "Password Required";
				isValid = false;
			}
			else if ($scope.editForm.password != $scope.editForm.confirmPassword) {
				$scope.editForm.confirmPasswordError = "Passwords do not match";
				isValid = false;
			}
			if (isValid) {
				var item = new SrirachaResource.credentials();
				item.userName = $scope.editForm.userName;
				item.password = $scope.editForm.password;
				var saveParams = {};
				if ($routeParams.credentialsId) {
					saveParams.id = $routeParams.credentialsId;
				}
				item.$save(saveParams,
					function (data) {
						console.log(data);
						if ($routeParams.credentialsId) {
							$scope.navigator.systemSettings.credentials.list.go();
						}
						else {
							$scope.navigator.systemSettings.credentials.edit.go({ credentialsId: data.id });
						}
					},
					function (err) {
						ErrorReporter.handleResourceError(err);
					}
				)
			}
		}
	}]);

