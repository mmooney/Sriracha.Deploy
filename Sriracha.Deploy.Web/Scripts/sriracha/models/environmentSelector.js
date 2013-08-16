function EnvironmentSelector(build, environment) {
	if (!(this instanceof EnvironmentSelector)) {
		return new EnvironmentSelector(build, environment)
	}
	this.build = build;
	this.environment = environment;
};