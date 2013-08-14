ngSriracha.provider("SrirachaNavigator", function () {
	this.$get = function () {
		var root = {
			getUrl: function (clientUrl, parameters) {
				var url = clientUrl;
				if (parameters) {
					for (var paramName in parameters) {
						url = url.replace(":" + paramName, parameters[paramName]);
					}
				}
				return "/#" + url;
			},
			goTo: function (clientUrl, parameters) {
				var url = this.getUrl(clientUrl, parameters);
				window.location.href = url;
			}
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
			"delete": {
				url: "/project/delete/:projectId",
				clientUrl: function (projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function (projectId) { root.goTo(this.url, { projectId: projectId }) }
			}
		};
		root.component = {
			create: {
				url: "/project/:projectId/component/create",
				clientUrl: function(projectId) { return root.getUrl(this.url, { projectId: projectId }) },
				go: function(projectId) { root.goTo(this.url, { projectId: projectId}) }
			},
			view: {
				url: "/project/:projectId/component/:componentId",
				clientUrl: function(projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function(projectId, componentId) { root.goto(this.url, { projectId: projectId, componentId: componentId}) }
			},
			edit: {
				url: "/project/:projectId/component/edit/:componentId",
				clientUrl: function(projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function(projectId, componentId) { root.goto(this.url, { projectId: projectId, componentId: componentId}) }
			},
			"delete": {
				url: "/project/:projectId/component/delete/:componentId",
				clientUrl: function(projectId, componentId) { return root.getUrl(this.url, { projectId: projectId, componentId: componentId }) },
				go: function(projectId, componentId) { root.goto(this.url, { projectId: projectId, componentId: componentId}) }
			}
		}
		root.deployment = {
			batchRequest: {
				url: "/deploy/batchRequest",
				clientUrl: function () { return root.getUrl(this.url) },
				go: function() { root.goTo(this.url) }
			}
		};
		return root;
	}
});
