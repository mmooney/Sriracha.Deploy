ngSriracha.service("PermissionVerifier",
	["SrirachaResource","ErrorReporter",
	function (SrirachaResource, ErrorReporter) {
		var accountData = SrirachaResource.account.get(
			{},
			function () {
			},
			function (err) {
				ErrorReporter.handleResourceException(err);
			}
		);

		this.canEditComponentConfiguration = function (projectId) {
			var projectPermissions = this.getUserProjectPermissions(projectId);
			if (projectPermissions) {
				return (projectPermissions.editComponentConfigurationAccess == "Grant")
			}
		};

		this.canEditEnvironment = function (projectId, environmentId) {
			var projectPermissions = this.getUserProjectPermissions(projectId);
			if (projectPermissions) {
				var environmentPermission = _.findWhere(projectPermissions.editEnvironmentPermissionList, { environmentId: environmentId });
				if (environmentPermission) {
					return (environmentPermission.access == "Grant");
				}
			}

		}

		this.getUserProjectPermissions = function (projectId) {
			if (accountData && accountData.effectivePermissions) {
				return _.findWhere(accountData.effectivePermissions.projectPermissionList, { projectId: projectId });
			}
		}
		this.getUserPermissions = function () {
			return this.effectivePermissions;
		}
}]);