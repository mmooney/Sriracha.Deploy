ngSriracha.controller("dashboardController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter) {
		$scope.navigator = SrirachaNavigator;
		$scope.selectedProjects = {};
		$scope.selectedComponents = {};
		$scope.selectedEnvironments = {};
		$scope.displayEnvironmentList = [];
		$scope.projectList = SrirachaResource.project.query(
            {},
            function (data) {
                _.each($scope.projectList, function (project) { $scope.selectedComponents[project.id] = {} });
                _.each($scope.projectList, function (project) { $scope.selectedEnvironments[project.id] = {} });
            },
            function (err) {
                ErrorReporter.handleResourceError(err);
            }
        );

		$scope.getBuildDisplayClass = function (build) {
		    if (!build || !build.status) {
		        return "alert";
		    }
		    switch (build.status) {
		        case "Success":
		            return "alert alert-success";
		        case "Error":
                    return "alert alert-danger";
		        case "Requested":
		        case "Approved":
		        case "NotStarted":
		        case "InProcess":
		        case "OfflineRequested":
		        case "OfflineComplete":
                    return "alert alert-info";
		        case "Rejected":
		        case "Warning":
                case "Cancelled":
		            return "alert alert-warning";

            }
		}

		$scope.updateReport = function () {
		    var projectList = [];
		    for (var projectId in $scope.selectedProjects) {
		        if (!$scope.selectedProjects[projectId]) {
		            continue;
		        }
		        var project = {
                    projectId: projectId,
		            componentIdList: [],
		            environmentIdList: []
		        };
		        for (var environmentId in $scope.selectedEnvironments[projectId]) {
		            project.environmentIdList.push(environmentId);
		        }
		        for (var componentId in $scope.selectedComponents[projectId]) {
		            project.componentIdList.push(componentId);
		        }
		        projectList.push(project);
		    }
		    $scope.displayEnvironmentList = [];
		    if (!projectList.length) {
		        return;
		    }
		    var request = { projectList: projectList };
            //Angular/ServiceStack don't play nicely when doing an HTTP/GET with a complex object in the URL, so do a POST instead.
		    $scope.dashboardData = SrirachaResource.dashboard.save(
                request,
                function (data) {
                    $scope.displayEnvironmentList = [];
                    if ($scope.dashboardData.projectList) {
                        console.log("projectList", $scope.dashboardData.projectList)
                        _.each($scope.dashboardData.projectList, function (project) {
                            console.log(project);
                            if (project.environmentList) {
                                _.each(project.environmentList, function (environment) {
                                    var displayEnvironment = _.findWhere($scope.displayEnvironmentList, { environmentName: environment.environmentName })
                                    if (!displayEnvironment) {
                                        displayEnvironment = { environmentName: environment.environmentName };
                                        $scope.displayEnvironmentList.push(displayEnvironment);
                                    }
                                    if(environment.componentList)  {
                                        _.each(environment.componentList, function (component) {
                                            displayEnvironment.componentList = displayEnvironment.componentList || [];
                                            displayEnvironment.componentList.push(component);
                                        });
                                    }
                                });
                            }
                        });
                    }
                },
                function (err) {
                    ErrorReporter.handleResourceError(err);
                });
		}
	}]);