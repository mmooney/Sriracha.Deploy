Sriracha = {
	init: function () {
		Sriracha.projects = new Sriracha.ProjectCollection();

		var projectListView = new Sriracha.ProjectListView({ collection: Sriracha.projects });
		projectListView.render();

		$(".projectList").append(projectListView.el);
		Sriracha.projects.fetch();
	},
	start: function () {
		Sriracha.init();
		//also start router
	}
}

