angular.module("ngSriracha").factory("DeployQueueDataAccess",
	['SrirachaResource','ErrorReporter',
	function (SrirachaResource, ErrorReporter) {
		return {
			get: function () {
				return SrirachaResource.deployQueue.get(
					{},
					function (x) {
						console.log(x);
					},
					function (err) {
						ErrorReporter.handleResourceError(err);
					}
				)
			}
		}
	}
]);
