ngSriracha.factory("SrirachaResource",
	['$resource',
	function ($resource) {
		return {
			//Account
			account: $resource("/api/account"),

			//Project
			project: $resource("/api/project"),
			configuration: $resource("/api/project/:projectId/configuration"),
			component: $resource("/api/project/:projectId/component"),
			componentConfiguration: $resource("api/project/:projectId/:parentType/:parentId/configuration"),
			branch: $resource("/api/project/:projectId/branch"),
			deploymentStep: $resource("/api/project/:projectId/:parentType/:parentId/step", { projectId: "@projectId", parentType: "@parentType", parentId: "@parentId" }),
			environment: $resource("/api/project/:projectId/environment"),
			projectRole: $resource("/api/project/:projectId/role"),

			//Build
			build: $resource("/api/build/:buildId"),
            file: $resource("/api/file/:fileId"),

			//Deploy
			deployQueue: $resource("/api/deploy/queue"),
			deployBatchRequest: $resource("/api/deploy/batch/request/:deployBatchRequestId"),
			deployState: $resource("/api/deploy/state/:deployState"),
			deployBatchStatus: $resource("/api/deploy/batch/:id/status"),
			deployBatchAction: $resource("/api/deploy/batch/:id/action"),
			deployHistory: $resource("/api/deploy/history"),
			offlineDeployment: $resource("/api/deploy/offline"),

			validateEnvironment: $resource("/api/validateEnvironment"),

			//Other
			systemLog: $resource("/api/systemLog"),
			taskMetadata: $resource("/api/taskmetadata"),
			taskOptionsView: $resource("/api/taskOptionsView"),
			user: $resource("/api/user"),

		    //System settings
			systemSettings: {
			    credentials: $resource("/api/systemSettings/credentials"),
			    credentialsTest: $resource("/api/systemSettings/credentials/test/:id"),
			    user: $resource("/api/systemSettings/user/:id"),
			    systemRole: $resource("/api/systemSettings/systemRole/:id"),
                buildPurgeRule: $resource("/api/systemSettings/buildPurgeRule/:id")
			}
		}
	}
]);
