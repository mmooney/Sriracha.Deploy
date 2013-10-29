angular.module('SharedServices', [])
    .config(['$httpProvider',function ($httpProvider) {
    	$httpProvider.responseInterceptors.push('myHttpInterceptor');
    	var spinnerFunction = function (data, headersGetter) {
    		// todo start the spinner here
    		$.blockUI();
    		return data;
    	};
    	$httpProvider.defaults.transformRequest.push(spinnerFunction);
    }])
// register the interceptor as a service, intercepts ALL angular ajax http calls
    .factory('myHttpInterceptor', ['$q', '$window', function ($q, $window) {
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
    }])
	.service("ErrorReporter", function () {
		this.reportError = function (errorMessage) {
			alert("Error: " + errorMessage);
		};
		this.handleResourceError = function (response) {
			var displayMessage;
			if (!response.data) {
				displayMessage = "<b>Unknown Error</b><br/>" + JSON.stringify(response);
			}
			else if (response.data.responseStatus && response.data.responseStatus.message) {
				displayMessage = "<b>Error: " + response.data.responseStatus.message + "</b><pre>" + JSON.stringify(response.data.responseStatus,null,4);
			}
			else if(typeof(response.data == "string")) {
				displayMessage = "<b>Error</b><br/>" + response.data;
			}
			else  {
				displayMessage = "<b>Error</b><br/>" + JSON.stringify(response.data);
			}
			console.log(displayMessage);
			var dialogHtml = "<div style='display:none'>" + displayMessage + "</div>";
			var dialogElement = $(dialogHtml);
			$("body").append(dialogElement);
			dialogElement.dialog({
				height: 'auto',
				width: "800",
				fluid:true,
				modal: true,
				close: function (x) {
					dialogElement.dialog("destroy").remove();
				}
			});
		}
	});

