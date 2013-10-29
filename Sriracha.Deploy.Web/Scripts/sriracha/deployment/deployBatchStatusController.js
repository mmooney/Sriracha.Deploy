ngSriracha.controller("deployBatchStatusController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter','PermissionVerifier',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {
	$scope.navigator = SrirachaNavigator;
	$scope.permissionVerifier = PermissionVerifier;
	$scope.$on("$destroy", function () {
		if ($scope.refreshInterval) {
			clearInterval($scope.refreshInterval);
			$scope.refreshInterval = null;
		}
		$(".statusChangeDialog").dialog("destroy").remove();
	});

	$scope.refreshData = function () {
		$scope.deployBatchStatus = SrirachaResource.deployBatchStatus.get({ id: $routeParams.deployBatchRequestId },
			function () {
				if ($scope.deployBatchStatus.request.status == "Success" || $scope.deployBatchStatus.request.status == "Error") {
					if ($scope.refreshInterval) {
						clearInterval($scope.refreshInterval);
						$scope.refreshInterval = null;
					}
				}
			},
			function (error) {
				if ($scope.refreshInterval) {
					clearInterval($scope.refreshInterval);
					$scope.refreshInterval = null;
				}
				ErrorReporter.handleResourceError(error);
			});
	}
	if ($routeParams.deployBatchRequestId) {
		$scope.refreshData();
		$scope.refreshInterval = setInterval($scope.refreshData, 10000);
	}

	$scope.getMachineDeployStateId = function (item, machine) {
		if ($scope.deployBatchStatus.deployStateList && item && machine) {
			for (var i = 0; i < $scope.deployBatchStatus.deployStateList.length; i++) {
				var state = $scope.deployBatchStatus.deployStateList[i];
				if (state.build && state.build.id == item.build.id) {
					if (state.machineList && _.any(state.machineList, function (x) { return x.id == machine.id; })) {
						return state.id;
					}
				}
			}
		}
		return "NotStarted";
	}

	$scope.getMachineDeployStatus = function (item, machine) {
		if ($scope.deployBatchStatus.deployStateList && item && machine) {
			for (var i = 0; i < $scope.deployBatchStatus.deployStateList.length; i++)
			{
				var state = $scope.deployBatchStatus.deployStateList[i];
				if (state.build && state.build.id == item.build.id) {
					if (state.machineList && _.any(state.machineList, function (x) { return x.id == machine.id; })) {
						return state.status;
					}
				}
			}
		}
		return "NotStarted";
	}

	$scope.startApproval = function () {
		$scope.statusChange = {
			statusMessage: null,
			newStatus: "Approved",
			promptText: "Approver Notes:",
			buttonText: "Approve"
		};
		$(".statusChangeDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}

	$scope.startRejection = function () {
		$scope.statusChange = {
			statusMessage: null,
			newStatus: "Rejected",
			promptText: "Rejection Notes:",
			buttonText: "Reject"
		};
		$(".statusChangeDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}

	$scope.startBeginDeployment = function () {
		$scope.statusChange = {
			statusMessage: null,
			newStatus: "NotStarted",
			promptText: "Notes:",
			buttonText: "Begin Deployment"
		};
		$(".statusChangeDialog").dialog({
			width: 'auto',
			height: 'auto',
			modal: true
		});
	}

	$scope.completeStatusChange = function () {
		var saveParams = {
			id: $routeParams.deployBatchRequestId,
			newStatus: $scope.statusChange.newStatus,
			statusMessage: $scope.statusChange.statusMessage
		};
		$scope.deployBatchStatus.$save(
			saveParams,
			function () {
				$scope.refreshData();
			},
			function (err) {
				ErrorReporter.handleResourceError(err);
			}
		)
		//$(".statusChangeDialog").dialog("close");
		$(".statusChangeDialog").dialog("destroy");
		$scope.statusChangeNotes = {};
	}
	$scope.cancelStatusChange = function () {
		//$(".statusChangeDialog").dialog("close");
		$(".statusChangeDialog").dialog("destroy");
		$scope.statusChange = {};
	}

	$scope.canApproveReject = function () {
		if ($scope.deployBatchStatus && $scope.deployBatchStatus.request && $scope.permissionVerifier) {
			if ($scope.deployBatchStatus.request.status == 'Requested' || $scope.deployBatchStatus.request.status == 'Rejected') {
				var isValid = $scope.validateEnvironmentPermission($scope.deployBatchStatus.request, function (projectId, environmentId) {
					return $scope.permissionVerifier.canApproveRejectDeployment(projectId, environmentId)
				});
				return isValid;
			}
		}
	}

	$scope.canBeginDeployment = function () {
		if ($scope.deployBatchStatus && $scope.deployBatchStatus.request && $scope.permissionVerifier) {
			if ($scope.deployBatchStatus.request.status == 'Approved') {
				var isValid = $scope.validateEnvironmentPermission($scope.deployBatchStatus.request, function (projectId, environmentId) {
					return $scope.permissionVerifier.canRunDeployment(projectId, environmentId)
				});
				return isValid;
			}
		}
	}

	$scope.validateEnvironmentPermission = function (request, callback) {
		var isValid = true;
		_.each(request.itemList, function (item) {
			_.each(item.machineList, function (machine) {
				if (!callback(item.build.projectId, machine.environmentId)) {
					isValid = false;
					return false;
				}
			})
			if (!isValid) {
				return false;
			}
		});
		return isValid;
	}
}]);