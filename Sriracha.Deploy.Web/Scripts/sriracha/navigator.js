ngSriracha.provider("SrirachaNavigator", function () {
	this.$get = function () {
		var root = {
			getUrl: function (clientUrl, routeParams, queryParams) {
				var url = clientUrl;
				if (routeParams) {
					for (var paramName in routeParams) {
						url = url.replace(":" + paramName, encodeURIComponent(routeParams[paramName]));
					}
				}
				if (queryParams) {
					var queryUrl = "";
					for (var paramName in queryParams) {
						if (queryParams[paramName]) {
							if (queryUrl) {
								queryUrl += "&";
							}
							queryUrl += encodeURIComponent(paramName) + "=" + encodeURIComponent(queryParams[paramName]);
						}
					}
					if (queryUrl) {
						url += "?" + queryUrl;
					}
				}
				return "/#" + url;
			},
			goTo: function (clientUrl, routeParams, queryParams) {
				var url = this.getUrl(clientUrl, routeParams, queryParams);
				console.log(url);
				window.location.href = url;
			}
		};
		root.systemLog = {
			list: {
				url: "/systemlog",
				clientUrl: function () { return root.getUrl(this.url); },
				go: function () { root.goTo(this.url); }
			}
		};
		root.account = {
			edit: {
				url: "/account",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function () { return root.goTo(this.url) }
			},
		};
		root.project = {
			list: {
				url: "/project",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function() { root.goTo(this.url) }
			},
			create: {
				url: "/project/create",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function() { root.goTo(this.url) }
			},
			view: {
				url: "/project/:projectId",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function(projectId) { root.goTo(this.url, { projectId: projectId}) }
			},
			edit: {
				url: "/project/edit/:projectId",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			},
			remove: {
				url: "/project/delete/:projectId",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			}
		};
		root.environment = {
			create: {
				url: "/project/:projectId/environment/create",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			},
			edit: {
				url: "/project/:projectId/environment/edit/:environmentId",
				clientUrl: function (projectId, environmentId) { return root.getUrl(this.url, { projectId: projectId, environmentId: environmentId }) },
				go: function (projectId, environmentId) { root.goTo(this.url, { projectId: projectId, environmentId: environmentId }) }
			},
			remove: {
				url: "/project/:projectId/environment/delete/:environmentId",
				clientUrl: function (projectId, environmentId) { return root.getUrl(this.url, { projectId: projectId, environmentId: environmentId }) },
				go: function (projectId, environmentId) { root.goTo(this.url, { projectId: projectId, environmentId: environmentId }) }
			},
		};
		root.configuration = {
			create: {
				url: "/project/:projectId/configuration/create",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			},
			view: {
				url: "/project/:projectId/configuration/:configurationId",
				clientUrl: function (projectId, configurationId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId }) },
				go: function (projectId, configurationId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId }) }
			},
			edit: {
				url: "/project/:projectId/configuration/edit/:configurationId",
				clientUrl: function (projectId, configurationId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId }) },
				go: function (projectId, configurationId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId }) }
			},
			remove: {
				url: "/project/:projectId/configuration/remove/:configurationId",
				clientUrl: function (projectId, configurationId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId }) },
				go: function (projectId, configurationId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId }) }
			},
		};
		root.component = {
			create: {
				url: "/project/:projectId/component/create",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			},
			view: {
				url: "/project/:projectId/component/:componentId",
				clientUrl: function (projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function (projectId, componentId) { root.goTo(this.url, { projectId: projectId, componentId: componentId }) }
			},
			edit: {
				url: "/project/:projectId/component/edit/:componentId",
				clientUrl: function (projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function (projectId, componentId) { root.goTo(this.url, { projectId: projectId, componentId: componentId }) }
			},
			remove: {
				url: "/project/:projectId/component/delete/:componentId",
				clientUrl: function (projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function (projectId, componentId) { root.goTo(this.url, { projectId: projectId, componentId: componentId }) }
			}
		};
		root.deploymentStep = {
			componentCreate: {
				url: "/project/:projectId/component/:componentId/step/create",
				clientUrl: function (projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }); },
				go: function (projectId, componentId) { root.goTo(this.url, { projectId: projectId, componentId: componentId }); }
			},
			configurationCreate: {
				url: "/project/:projectId/configuration/:configurationId/step/create",
				clientUrl: function (projectId, configurationId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId }); },
				go: function (projectId, configurationId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId }); }
			},
			componentEdit: {
				url: "/project/:projectId/component/:componentId/step/edit/:deploymentStepId",
				clientUrl: function (projectId, componentId, deploymentStepId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId }); },
				go: function (projectId, componentId, deploymentStepId) { root.goTo(this.url, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId }); }
			},
			configurationEdit: {
				url: "/project/:projectId/configuration/:configurationId/step/edit/:deploymentStepId",
				clientUrl: function (projectId, configurationId, deploymentStepId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId, deploymentStepId: deploymentStepId }); },
				go: function (projectId, configurationId, deploymentStepId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId, deploymentStepId: deploymentStepId }); }
			},
			componentRemove: {
				url: "/project/:projectId/component/:componentId/step/delete/:deploymentStepId",
				clientUrl: function (projectId, componentId, deploymentStepId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId }); },
				go: function (projectId, componentId, deploymentStepId) { root.goTo(this.url, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId }); }
			},
			configurationRemove: {
				url: "/project/:projectId/configuration/:configurationId/step/delete/:deploymentStepId",
				clientUrl: function (projectId, configurationId, deploymentStepId) { return root.getUrl(this.url, { projectId: projectId, configurationId: configurationId, deploymentStepId: deploymentStepId }); },
				go: function (projectId, configurationId, deploymentStepId) { root.goTo(this.url, { projectId: projectId, configurationId: configurationId, deploymentStepId: deploymentStepId }); }
			}
		};
		root.branch = {
			create: {
				url: "/project/:projectId/branch/create",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }); },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }); }
			},
			edit: {
				url: "/project/:projectId/branch/edit/:branchId",
				clientUrl: function (projectId, branchId) { return root.getUrl(this.url, { projectId: projectId, branchId: branchId }); },
				go: function (projectId, branchId) { root.goTo(this.url, { projectId: projectId, branchId: branchId }); }
			},
			remove: {
				url: "/project/:projectId/branch/delete/:branchId",
				clientUrl: function (projectId, branchId) { return root.getUrl(this.url, { projectId: projectId, branchId: branchId }); },
				go: function (projectId, branchId) { root.goTo(this.url, { projectId: projectId, branchId: branchId }); }
			}
		}
		root.projectRole = {
			list: {
				url: "/project/:projectId/role",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }); },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId });}
			},
			edit: {
				url: "/project/:projectId/role/:projectRoleId",
				clientUrl: function (projectId, projectRoleId) { return root.getUrl(this.url, { projectId: projectId, projectRoleId: projectRoleId }); },
				go: function (projectId, projectRoleId) { root.goTo(this.url, { projectId: projectId, projectRoleId: projectRoleId });}
			}
		}
		root.build = {
			list: {
				url: "/build",
				clientUrl: function() { return root.getUrl(this.url) },
				go: function() { root.goTo(this.url) }
			},
			submit: {
				url: "/build/submit",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function () { root.goTo(this.url) }
			},
			view: {
				url: "/build/:buildId",
				clientUrl: function (buildId) { return root.getUrl(this.url, { buildId: buildId }); },
				go: function (buildId) { root.goTo(this.url, { buildId: buildId }); }
			},
			remove: {
				url: "/build/delete/:buildId",
				clientUrl: function (buildId) { return root.getUrl(this.url, { buildId: buildId }); },
				go: function (buildId) { root.goTo(this.url, { buildId: buildId }); }
			}
		};
		root.deployment = {
			batchRequest: {
				url: "/deploy/batchRequest",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function() { root.goTo(this.url) }
			},
			batchCopy: {
				url: "/deploy/batchCopy/:sourceDeployBatchRequestId",
				clientUrl: function (sourceDeployBatchRequestId) { return root.getUrl(this.url, { sourceDeployBatchRequestId: sourceDeployBatchRequestId }) },
				go: function (sourceDeployBatchRequestId) { root.goTo(this.url, { sourceDeployBatchRequestId: sourceDeployBatchRequestId }) }
			},
			batchList: {
				url: "/deploy/batchList",
				clientUrl: function (pageNumber, pageSize, sortField, sortAscending) 
				{ 
					return root.getUrl(this.url, null, { pageNumber: pageNumber, pageSize: pageSize, sortField: sortField, sortAscending: sortAscending}); 
				},
				go: function (pageNumber, pageSize, sortField, sortAscending)  
				{ 
					root.goTo(this.url, null, { pageNumber: pageNumber, pageSize: pageSize, sortField: sortField, sortAscending: sortAscending}); 
				}
			},
			batchStatus: {
				url: "/deploy/batchStatus/:deployBatchRequestId",
				clientUrl: function (deployBatchRequestId) { return root.getUrl(this.url, { deployBatchRequestId: deployBatchRequestId }); },
				go: function (deployBatchRequestId) { root.goTo(this.url, { deployBatchRequestId: deployBatchRequestId }); }
			},
			submit: {
				url: "/deploy/submit/:buildId/:environmentId",
				clientUrl: function (buildId, environmentId) { return root.getUrl(this.url, { buildId: buildId, environmentId: environmentId }); },
				go: function (buildId, environmentId) { root.goTo(this.url, { buildId: buildId, environmentId: environmentId }); }
			},
			view: {
				url: "/deploy/:deployStateId",
				clientUrl: function (deployStateId) { return root.getUrl(this.url, { deployStateId: deployStateId }) },
				go: function (deployStateId) { root.goTo(this.url, { deployStateId: deployStateId }) }
			}
		};
		return root;
	}
});
