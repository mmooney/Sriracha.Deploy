ngSriracha.controller("BuildController", function ($scope, $routeParams, SrirachaResource) {
	if ($routeParams.buildId) {
		$scope.build = SrirachaResource.build.get({id: $routeParams.buildId});
	}
	else {
		$scope.buildList = SrirachaResource.build.query({});
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

	$scope.cancelBuild = function () {
		Sriracha.Navigation.Build.List();
	}

	$scope.saveBuild = function () {
		$scope.build.projectId = $scope.project.id;
		$scope.build.projectComponentId = $scope.component.id;
		$scope.build.projectBranchId = $scope.branch.id;
		$scope.build.$save(
			$scope.build,
			function () {
				Sriracha.Navigation.Build.List();
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
				Sriracha.Navigation.Build.List();
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}

	$scope.getSubmitBuildUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.SubmitUrl);
	}
	$scope.getBuildListUrl = function () {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.ListUrl);
	}
	$scope.getDeleteBuildUrl = function (build) {
		return Sriracha.Navigation.GetUrl(Sriracha.Navigation.Build.DeleteUrl, { buildId: build.id });
	}
});
