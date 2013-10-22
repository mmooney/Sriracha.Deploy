ngSriracha.controller("BuildController", 
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	if ($routeParams.buildId) {
		$scope.build = SrirachaResource.build.get({ id: $routeParams.buildId }, function () {
			$scope.project = SrirachaResource.project.get({ id: $scope.build.projectId });
		});
		$scope.deployHistory = SrirachaResource.deployHistory.query({ buildId: $routeParams.buildId });
	}
	else {
		$scope.buildList = SrirachaResource.build.get(
			{},
			function() {
				console.log($scope.buildList);
			},
			function(err) {
				ErrorHandler.handleResourceError(err);
			}
		);
		$scope.projectList = SrirachaResource.project.query({});
		$scope.uploadMessage = "Please upload the deploy package file first";
		$scope.build = new SrirachaResource.build({});
	}

	$scope.reportError = function (error) {
		alert("ERROR: \r\n" + JSON.stringify(error));
	};

	$scope.uploadComplete = function (content, isComplete) {
		if (isComplete) {
			$scope.uploadMessage = "File Uploaded!";
			var fileObject = JSON.parse(content);
			$scope.build.fileId = fileObject.id;
		}
		else {
			$scope.uploadMessage = content;
		}
	}

	$scope.saveBuild = function () {
		var saveParams = {
			projectId: $scope.project.id,
			projectComponentId: $scope.component.id,
			projectBranchId: $scope.branch.id
		};
		$scope.build.$save(
			saveParams,
			function () {
				$scope.navigator.build.list.go();
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.deleteBuild = function() {
		$scope.build.$delete(
			$scope.build,
			function () {
				$scope.navigator.build.list.go();
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.getDeployHistory = function (build, environment) {
		if (build && environment) {
			return _.find($scope.deployHistory, function (x) { return (x.buildId == build.id && x.environmentId == environment.Id); });
		}
		return null;
	}

	$scope.getDeployHistoryStatus = function (build, environment) {
		var item = $scope.getDeployHistory(build, environment);
		if(item) {
			return item.status;
		}
		return "none";
	}

	$scope.anyDeployment = function (build, environment) {
		var item = $scope.getDeployHistory(build, environment);
		if(item) {
			switch(item.status) {
				case "Unknown":
				case "NotStarted":
					return false;
				default:
					return true;
			}
		}
		return false;
	}


}]);
