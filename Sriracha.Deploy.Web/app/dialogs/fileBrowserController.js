ngSriracha.controller("FileBrowserController",
    ['$scope', '$modalInstance', 'SrirachaResource', 'ErrorReporter','buildList','$rootScope', 
	function ($scope, $modalInstance, SrirachaResource, ErrorReporter, buildList, $rootScope) {
	    $scope.buildList = buildList;

	    $scope.loadFiles = function (build) {
	        $scope.fileList = [];
	        var file = SrirachaResource.file.get(
                { id: build.fileId },
                function () {
                    if (file && file.manifest && file.manifest.itemList) {
                        $scope.fileList = _.filter(file.manifest.itemList, function (x) { return x.fileName });
                    }
                },
                function (err) {
                    ErrorReporter.handleResourceError(err);
                }
            );
	    }

	    if (buildList && buildList.length) {
	        $scope.selectedBuild = _.first(buildList);
	        $scope.loadFiles($scope.selectedBuild);
	    }

	    $scope.selectedBuildChanged = function (x) {
            console.log($scope, $rootScope)
	        $scope.loadFiles($scope.selectedBuild);
	    }


	    $scope.selectFile = function(item) {
	        $modalInstance.close(item);
	    }

	    $scope.close = function () {
	        $modalInstance.dismiss();
	    }

	}
]);

