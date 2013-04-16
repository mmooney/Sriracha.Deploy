var Sriracha = Sriracha || {};

Sriracha.Navigation = {
	HomeUrl: "/#",
	Home: function () {
		window.location.href = this.HomeUrl;
	},

	GoTo: function (clientUrl, parameters) {
		var url = clientUrl;
		if (parameters) {
			for (var paramName in parameters) {
				url = url.replace(":" + paramName, parameters[paramName]);
			}
		}
		window.location.href = "/#" + url;
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
	}

};