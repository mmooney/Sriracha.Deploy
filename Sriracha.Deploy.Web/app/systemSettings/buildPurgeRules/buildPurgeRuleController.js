ngSriracha.controller("buildPurgeRuleController",
	['$scope', '$routeParams', 'SrirachaResource', 'SrirachaNavigator', 'ErrorReporter', 'PermissionVerifier',
	function ($scope, $routeParams, SrirachaResource, SrirachaNavigator, ErrorReporter, PermissionVerifier) {

		$scope.navigator = SrirachaNavigator;
		$scope.permissionVerifier = PermissionVerifier;
		$scope.editForm = {};

		if ($routeParams.buildPurgeRuleId) {
			$scope.buildPurgeRule = SrirachaResource.systemSettings.buildPurgeRule.get(
				{id: $routeParams.buildPurgeRuleId},
				function (data) {
				    //console.log(data);
					//$scope.editForm.userName = data.userName;
					//$scope.editForm.domain = data.domain;
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				}
			);
		}
		else  {
		    $scope.buildPurgeRuleList = SrirachaResource.systemSettings.buildPurgeRule.query(
				{},
				function (data) {
				    //console.log($scope.buildPurgeRuleList);
				},
				function (err) {
					ErrorReporter.handleResourceError(err);
				}
			)
		    $scope.buildPurgeRule = new SrirachaResource.systemSettings.buildPurgeRule({
		        environmentIdList: [],
		        environmentNameList: [],
		        machineIdList: [],
		        machineNameList: []
		    });
        }

		$scope.editItem = function (list, item) {
		    var newValue = prompt("Enter new value:", item);
		    if (newValue && newValue != item) {
		        var existingIndex = _.indexOf(list, newValue)
		        if (existingIndex >= 0) {
		            alert("Value " + newValue + " already exists");
		            return;
		        }
		        var index = _.indexOf(list, item);
		        if (index >= 0) {
		            list[index] = newValue;
		        }
		    }
		}

		$scope.deleteItem = function (list, item) {
		    if(item) {
		        if (confirm("Are you sure you want to delete " + item + "?")) {
		            var index = _.indexOf(list, item)
		            if (index >= 0) {
                        list.splice(index, 1)
                    }
		        }
		    }
		}

		$scope.addItem = function (list) {
		    var item = prompt("Please enter the new item:")
		    if (item) {
		        var index = _.indexOf(list, item);
		        if (index >= 0) {
		            alert(item + " is already in the list")
		            return;
		        }
		        list.push(item);
		    }
		}

		$scope.saveBuildPurgeRule = function () {
		    var isValid = true;
		    $scope.buildRetentionMinutesError = null;
		    //if (!$scope.buildPurgeRule.buildRetentionMinutes) {
		    //    $scope.buildRetentionMinutesError = "Build Retention Minutes required";
		    //	isValid = false;
		    //} 
		    if ($scope.buildPurgeRule.buildRetentionMinutes && isNaN(parseInt($scope.buildPurgeRule.buildRetentionMinutes))) {
		        $scope.buildRetentionMinutesError = "Build Retention Minutes must be a number";
		    	isValid = false;
		    }
		    //console.log($scope.buildPurgeRule.buildRetentionMinutes, parseInt($scope.buildPurgeRule.buildRetentionMinutes))
		    if (isValid) {
		    	var saveParams = {};
		    	if ($routeParams.buildPurgeRuleId) {
		    	    saveParams.id = $routeParams.buildPurgeRuleId;
		    	}
		    	var result = $scope.buildPurgeRule.$save(saveParams,
		    		function (data) {
		    			if ($routeParams.credentialsId) {
		    				$scope.navigator.systemSettings.buildPurgeRule.list.go();
		    			}
		    			else {
		    			    $scope.navigator.systemSettings.buildPurgeRule.list.go();
		    			}
		    		},
		    		function (err) {
		    			ErrorReporter.handleResourceError(err);
		    		}
		    	)
		    }
		}

		$scope.deleteBuildPurgeRule = function () {
		    var item = new SrirachaResource.systemSettings.buildPurgeRule();
		    var deleteParams = {
		        id: $routeParams.buildPurgeRuleId
		    };
		    var result = item.$delete(deleteParams,
		    	function (data) {
		    		$scope.navigator.systemSettings.buildPurgeRule.list.go();
		    	},
		    	function (err) {
		    		ErrorReporter.handleResourceError(err);
		    	}
		    )
		}
	}]);

