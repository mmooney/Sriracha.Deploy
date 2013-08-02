angular.module('SharedServices', [])
    .config(function ($httpProvider) {
    	$httpProvider.responseInterceptors.push('myHttpInterceptor');
    	var spinnerFunction = function (data, headersGetter) {
    		// todo start the spinner here
    		$.blockUI();
    		return data;
    	};
    	$httpProvider.defaults.transformRequest.push(spinnerFunction);
    })
// register the interceptor as a service, intercepts ALL angular ajax http calls
    .factory('myHttpInterceptor', function ($q, $window) {
    	return function (promise) {
    		return promise.then(function (response) {
    			// do something on success
    			// todo hide the spinner
    			$.unblockUI();
    			return response;

    		}, function (response) {
    			// do something on error
    			// todo hide the spinner
    			$.unblockUI();
    			return $q.reject(response);
    		});
    	};
    })
	.service("ErrorReporter", function () {
		this.reportError = function (errorMessage) {
			alert("Error: " + errorMessage);
		};
		this.handleResourceError = function (response) {
			alert("Error: " + JSON.stringify(response.data.responseStatus.message));
		}
	});

