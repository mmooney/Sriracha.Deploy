ngSriracha.factory("SrirachaResource",
	['$resource',
	function ($resource) {
		return {
			account: $resource("/api/account"),
			project: $resource("/api/project"),
			configuration: $resource("/api/project/:projectId/configuration"),
			component: $resource("/api/project/:projectId/component"),
			componentConfiguration: $resource("api/project/:projectId/:parentType/:parentId/configuration"),
			branch: $resource("/api/project/:projectId/branch"),
			environment: $resource("/api/project/:projectId/environment"),
			deploymentStep: $resource("/api/project/:projectId/:parentType/:parentId/step", { projectId: "@projectId", parentType: "@parentType", parentId: "@parentId" }),
			projectRole: $resource("/api/project/:projectId/role"),
			deployHistory: $resource("/api/deploy/history"),
			taskMetadata: $resource("/api/taskmetadata"),
			build: $resource("/api/build/:buildId"),
			deployRequest: $resource("/api/deployRequest/:deployRequestId"),
			deployState: $resource("/api/deployState/:deployState"),
			deployBatchRequest: $resource("/api/deploybatchRequest/:deployBatchRequestId"),
			deployBatchStatus: $resource("/api/deploybatchStatus/:id"),
			validateEnvironment: $resource("/api/validateEnvironment"),
			systemLog: $resource("/api/systemLog")
		}
	}
]);
