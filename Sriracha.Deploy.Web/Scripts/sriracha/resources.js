ngSriracha.factory("SrirachaResource", function ($resource) {
	return {
		project: $resource("/api/project"),
		component: $resource("/api/project/:projectId/component"),
		componentConfiguration: $resource("api/project/:projectId/component/:componentId/configuration"),
		branch: $resource("/api/project/:projectId/branch"),
		environment: $resource("/api/project/:projectId/environment"),
		deploymentStep: $resource("/api/project/:projectId/component/:componentId/step", { projectId: "@projectId", componentId: "@componentId" }),
		deployHistory: $resource("/api/deploy/history"),
		taskMetadata: $resource("/api/taskmetadata"),
		build: $resource("/api/build/:buildId"),
		deployRequest: $resource("/api/deployRequest/:deployRequestId"),
		deployState: $resource("/api/deployState/:deployState"),
		validateEnvironment: $resource("/api/validateEnvironment"),
		systemLog: $resource("/api/systemLog")
	}
});
