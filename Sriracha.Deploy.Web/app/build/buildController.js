ngSriracha.controller("BuildController", 
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator','ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.fileContents = [];
	if ($routeParams.buildId) {
		$scope.build = SrirachaResource.build.get({ id: $routeParams.buildId }, function () {
			$scope.project = SrirachaResource.project.get({ id: $scope.build.projectId });
			if ($scope.build.fileId) {
			    var fileData = SrirachaResource.file.get({ id: $scope.build.fileId },
                    function () {
                        if (fileData.manifest && fileData.manifest.itemList && fileData.manifest.itemList.length) {
                            var itemList = fileData.manifest.itemList;
                            var directoryNameList = _.sortBy(_.unique(_.pluck(_.filter(itemList, function (x) { return x.directory }), 'directory')));
                            var directoryDataList = [];
                            var childrenList = _.filter(itemList, function (x) { return !x.directory; });   //Start with root items
                            _.each(childrenList, function (x) { x.label = x.fileName });
                            _.each(directoryNameList, function (directoryName) {
                                var parts = directoryName.split('/');
                                var displayName = _.last(parts);
                                var currentDirectoryName = null;
                                var currentNodeList = childrenList;
                                _.each(parts, function (directoryLabel) {
                                    if (currentDirectoryName) {
                                        currentDirectoryName += "/" + directoryLabel;
                                    }
                                    else {
                                        currentDirectoryName = directoryLabel;
                                    }
                                    var directoryNode = _.findWhere(currentNodeList, { label: directoryLabel });
                                    if (!directoryNode) {
                                        directoryNode = {
                                            label: directoryLabel,
                                            children: _.filter(itemList, function (x) { return (x.fileName && x.directory == currentDirectoryName);})
                                        };
                                        _.each(directoryNode.children, function (x) { x.label = x.fileName });
                                        currentNodeList.push(directoryNode);
                                    }
                                    directoryNode.children = directoryNode.children || [];
                                    currentNodeList = directoryNode.children;
                                });
                            });
                            $scope.fileContents = [{
                                label: 'Files',
                                children: childrenList
                            }];
                        }
                    }
                )
			}
		});

		$scope.selectTreeNode = function (branch) {
		    $scope.selectedTreeNode = branch;
		}
		$scope.deployHistory = SrirachaResource.deployHistory.get(
            { buildIdList: [$routeParams.buildId] },
            function () {
            },
            function (err) {
                ErrorReporter.handleResourceError(err);
            }
        );
	}
	else {
		$scope.buildList = SrirachaResource.build.get(
			{},
			function() {
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

}]);
