ngSriracha.controller("BuildListController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
		$scope.listOptions = {};
		if ($routeParams.pageNumber) $scope.listOptions.pageNumber = $routeParams.pageNumber;
		if ($routeParams.pageSize) $scope.listOptions.pageSize = $routeParams.pageSize;
		if ($routeParams.sortField) $scope.listOptions.sortField = $routeParams.sortField;
		if ($routeParams.sortAscending) $scope.listOptions.sortAscending = $routeParams.sortAscending;

		$scope.navigator = SrirachaNavigator;
		$scope.buildList = SrirachaResource.build.get(
			$scope.listOptions,
			function () {
			},
			function (err) {
				ErrorReporter.handleResourceError(err);
			}
		);
		$scope.goToPage = function (pageNumber) {
			$scope.navigator.build.list.go(pageNumber, $scope.buildList.pageSize, $scope.buildList.sortField, $scope.buildList.sortAscending);
		};
		$scope.applySort = function (sortField, sortAscending) {
			$scope.navigator.build.list.go(1, $scope.buildList.pageSize, sortField, sortAscending);
		}
	}]
);
