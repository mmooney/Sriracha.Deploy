Sriracha.View = Backbone.View.extend({
	render: function () {
		var source = $(this.template).html();
		var data = {};
		if (this.model)
			data = this.model.toJSON();
		var compiled = _.template(source, data);
		$(this.el).html(compiled);
		return this;
	}
});
Sriracha.ItemView = Sriracha.View.extend({
	remove: function () {
		var confirmed = confirm("Delete this?  Are you sure ?");
		if (confirmed) {
			this.model.destroy();
		}
	}
});

Sriracha.ListView = Backbone.View.extend({
	initialize: function () {
		this.collection.bind("reset", this.render, this);
		this.collection.bind("add", this.render, this);
		this.collection.bind("remove", this.render, this);
	},
	render: function (evt) {
		var self = this;
		var els = [];
		this.collection.each(function (item) {
			if (!self.ItemView) {
				throw "Missing implementation of ItemView method";
			}
			var itemView = new self.ItemView({ model: item });
			els.push(itemView.render().el);
		});
		$(this.el).html(els);
		return this;
	}
});