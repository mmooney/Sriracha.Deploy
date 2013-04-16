
var Sriracha = Sriracha || {};

Sriracha.Navigation = {
	HomeUrl: "/#",
	Home: function () {
		window.location.href = this.HomeUrl;
	},

	GetUrl: function (clientUrl, parameters) {
		var url = clientUrl;
		if (parameters) {
			for (var paramName in parameters) {
				url = url.replace(":" + paramName, parameters[paramName]);
			}
		}
		return "/#" + url;
	},

	GoTo: function (clientUrl, parameters) {
		var url = this.GetUrl(clientUrl, parameters);
		window.location.href = url;
	},

	Project : {
		ListUrl: "/",
		List: function () {
			Sriracha.Navigation.GoTo(this.ListUrl);
		},
		CreateUrl: "/project/create",
		Create: function () {
			Sriracha.Navigation.GoTo(this.CreateUrl)
		},
		ViewUrl: "/project/:projectId",
		View: function (id) {
			Sriracha.Navigation.GoTo(this.ViewUrl, { projectId: id });
		},
		EditUrl: "/project/edit/:projectId",
		Edit: function (id) {
			Sriracha.Navigation.GoTo(this.EditUrl, { projectId: id });
		},
		DeleteUrl: "/project/delete/:projectId",
		Delete: function (id) {
			Sriracha.Navigation.GoTo(this.DeleteUrl, { projectId: id });
		}
	},

	Component: {
		CreateUrl: "/project/:projectId/component/create",
		Create: function (projectId) {
			Sriracha.Navigation.GoTo(this.CreateUrl, { projectId: projectId });
		},
		ViewUrl: "/project/:projectId/component/:componentId",
		View: function (projectId, componentId) {
			Sriracha.Navigation.GoTo(this.ViewUrl, { projectId: projectId, componentId: componentId });
		},
		EditUrl: "/project/:projectId/component/edit/:componentId",
		Edit: function (projectId, componentId) {
			Sriracha.Navigation.GoTo(this.EditUrl, { projectId: projectId, componentId: componentId });
		},
		DeleteUrl: "/project/:projectId/component/delete/:componentId",
		Delete: function (projectId, componentId) {
			Sriracha.Navigation.GoTo(this.DeleteUrl, { projectId: projectId, componentId: componentId });
		}
	},

	DeploymentStep: {
		CreateUrl: "/project/:projectId/component/:componentId/step/create",
		Create: function (projectId, componentId) {
			Sriracha.Navigation.GoTo(this.CreateUrl, { projectId: projectId, componentId: componentId });
		},
		EditUrl: "/project/:projectId/component/:componentId/step/edit/:deploymentStepId",
		Edit: function (projectId, componentId, deploymentStepId) {
			Sriracha.Navigation.GoTo(this.EditUrl, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId });
		},
		DeleteUrl: "/project/:projectId/component/:componentId/step/delete/:deploymentStepId",
		Delete: function (projectId, componentId, deploymentStepId) {
			Sriracha.Navigation.GoTo(this.DeleteUrl, { projectId: projectId, componentId: componentId, deploymentStepId: deploymentStepId });
		}
	}

};