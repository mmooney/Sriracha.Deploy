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
				var url = this.GetUrl(clientUrl, parameters);
				window.location.href = url;
			}
		};
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
