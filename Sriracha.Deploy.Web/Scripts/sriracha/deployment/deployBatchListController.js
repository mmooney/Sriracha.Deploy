ngSriracha.controller("deployBatchListController",
		['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
		function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
	$scope.navigator = SrirachaNavigator;
	$scope.listOptions = {};
	if ($routeParams.pageNumber) $scope.listOptions.pageNumber = $routeParams.pageNumber;
	if ($routeParams.pageSize) $scope.listOptions.pageSize = $routeParams.pageSize;
	if ($routeParams.sortField) $scope.listOptions.sortField = $routeParams.sortField;
	if ($routeParams.sortAscending) $scope.listOptions.sortAscending = $routeParams.sortAscending;

	$scope.deploymentList = SrirachaResource.deployBatchStatus.get($scope.listOptions,
		function () {
			console.log($scope.deploymentList);
		},
		function (err) {
			ErrorReporter.handleResourceError(err);
		});
		
}]);