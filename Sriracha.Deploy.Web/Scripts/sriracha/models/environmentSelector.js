function EnvironmentSelector(build, project, environmentId, srirachaResource, errorReporter) {
	if (!(this instanceof EnvironmentSelector)) {
		return new EnvironmentSelector(build, project, environmentId)
	}

	var self = this;
	var getEnvironmentResults = function (validationResult) {
		var returnValue = [];
		if (validationResult && validationResult.validationResult) {
			for (var i = 0; i < validationResult.validationResult.resultList.length; i++) {
				var resultItem = validationResult.validationResult.resultList[i];
				if (resultItem.taskValidationResult.environmentResultList.length) {
					for (var j = 0; j < resultItem.taskValidationResult.environmentResultList.length; j++) {
						var environmentResultItem = resultItem.taskValidationResult.environmentResultList[j];
						var item = {
							taskName: resultItem.deploymentStep.stepName,
							settingName: environmentResultItem.fieldName,
							present: environmentResultItem.present
						};
						if (environmentResultItem.present) {
							if (environmentResultItem.sensitive) {
								item.settingValue = "*******************";
							}
							else {
								item.settingValue = environmentResultItem.fieldValue;
							}
						}
						else {
							item.settingValue = "N/A";
						}
						returnValue.push(item);
					}
				}
			}
		}
		return returnValue;
	};
	var getMachineResults = function (validationResult, machineId) {
		var returnValue = [];
		if (validationResult && validationResult.validationResult) {
			for (var i = 0; i < validationResult.validationResult.resultList.length; i++) {
				var resultItem = validationResult.validationResult.resultList[i];
				var machineResultList = resultItem.taskValidationResult.machineResultList[machineId];
				if (machineResultList && machineResultList.length) {
					for (var j = 0; j < machineResultList.length; j++) {
						var machineResultItem = machineResultList[j];
						var item = {
							taskName: resultItem.deploymentStep.stepName,
							settingName: machineResultItem.fieldName,
							present: machineResultItem.present
						};
						if (machineResultItem.present) {
							if (machineResultItem.sensitive) {
								item.settingValue = "*******************";
							}
							else {
								item.settingValue = machineResultItem.fieldValue;
							}
						}
						else {
							item.settingValue = "N/A";
						}
						returnValue.push(item);
					}
				}
			}
		}
		return returnValue;
	};


	self.build = build;
	self.project = project;
	self.environment = _.findWhere(project.environmentList, { id: environmentId });
	self.selectedMachines = {};

	if (self.environment.componentList) {
		self.environmentComponent = _.findWhere(self.environment.componentList, { componentId: self.build.projectComponentId });
	}

	self.validationResult = srirachaResource.validateEnvironment.get({ buildId: self.build.id, environmentId: self.environment.id },
		function () {
			self.environmentResults = getEnvironmentResults(self.validationResult);
			self.environmentResultsIncomplete = _.any(self.environmentResults, function (x) { return !x.present; });

			self.machineResults = {};
			self.machineResultsIncomplete = {};
			_.each(self.environmentComponent.machineList, function (machine) {
				self.machineResults[machine.id] = getMachineResults(self.validationResult, machine.id);
				self.machineResultsIncomplete[machine.id] = _.any(self.machineResults[machine.id], function (x) { return !x.present; });
			});
		},
		function (err) {
			errorRepoter.handleResourceError(err);
		});


};

