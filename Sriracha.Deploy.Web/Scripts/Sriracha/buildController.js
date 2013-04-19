ngSriracha.controller("BuildController", function ($scope, $routeParams, SrirachaResource) {
	$scope.projectList = SrirachaResource.project.query({});
	$scope.uploadMessage = "Please upload the deploy package file first";
	$scope.build = new SrirachaResource.build({});

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
		$scope.build.$save(
			$scope.build,
			function () {
				Sriracha.Navigation.Project.View($routeParams.projectId);
			},
			function (error) {
				$scope.reportError(error);
			}
		);
	}
});
