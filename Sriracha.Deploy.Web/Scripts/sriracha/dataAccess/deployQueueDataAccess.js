angular.module("ngSriracha").factory("DeployQueueDataAccess",
	['SrirachaResource','ErrorReporter',
	function (SrirachaResource, ErrorReporter) {
		return {
			get: function () {
				return SrirachaResource.deployQueue.get(
					{},
					function (x) {
					},
					function (err) {
						ErrorReporter.handleResourceError(err);
					}
				)
			}
		}
	}
]);
