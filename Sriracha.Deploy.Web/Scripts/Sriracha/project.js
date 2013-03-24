Sriracha.Project = Backbone.Model.extend({
	url: function () {
		return "/api/project" + this.Id;
	},
	validate: function (atts) {
		alert("validate");
	},
	idAttribute: "Id"
});

Sriracha.ProjectCollection = Backbone.Collection.extend({
	model: Sriracha.Project,
	url: "/api/project"
});

Sriracha.ProjectItemView = Sriracha.ItemView.extend({
	tagName: "tr",
	template: "#project-list-template",
	events: {
		"click button": "remove",
		"click a": "showDb"
	},
	showDb: function (ev) {
		ev.preventDefault();
		var db = $(ev.currentTarget).data("db");
	}
});
Sriracha.ProjectListView = Sriracha.ListView.extend({
	tagName: "table",
	className: "table table-striped",
	ItemView: Sriracha.ProjectItemView
});

