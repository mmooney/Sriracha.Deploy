/// <reference path="systemSettings/deploymentTools/deploymentToolsController.js" />
/// <reference path="systemSettings/deploymentTools/deploymentToolsController.js" />
ngSriracha.config(
		['$routeProvider', 'SrirachaNavigatorProvider',
		function ($routeProvider, SrirachaNavigatorProvider) {
	var navigator = SrirachaNavigatorProvider.$get();
	$routeProvider
		.when("/", {
			templateUrl: "app/home-template.html",
			controller: "HomeController"
		})

		.when(navigator.account.edit.url, {
		    templateUrl: "app/account/account-edit-template.html",
			controller: "AccountController"
		})

		//Projects
		.when(navigator.project.list.url, {
			templateUrl: "app/project/project-list-template.html",
			controller: "ProjectController",
		})
		.when(navigator.project.create.url, {
			templateUrl: "app/project/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.view.url, {
			templateUrl: "app/project/project-view-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.edit.url, {
			templateUrl: "app/project/project-edit-template.html",
			controller: "ProjectController"
		})
		.when(navigator.project.remove.url, {
			templateUrl: "app/project/project-delete-template.html",
			controller: "ProjectController"
		})

		//Configurations
		.when(navigator.configuration.create.url, {
			templateUrl: "app/project/configuration/configuration-edit-template.html",
			controller: "ProjectConfigurationController"
		})
		.when(navigator.configuration.view.url, {
			templateUrl: "app/project/configuration/configuration-view-template.html",
			controller: "ProjectConfigurationController"
		})
		.when(navigator.configuration.edit.url, {
			templateUrl: "app/project/configuration/configuration-edit-template.html",
			controller: "ProjectConfigurationController"
		})
		.when(navigator.configuration.remove.url, {
			templateUrl: "app/project/configuration/configuration-delete-template.html",
			controller: "ProjectConfigurationController"
		})

		//Components
		.when(navigator.component.create.url, {
			templateUrl: "app/project/component/component-edit-template.html",
			controller: "ProjectComponentController"
		})
		.when(navigator.component.view.url, {
			templateUrl: "app/project/component/component-view-template.html",
			controller: "ProjectComponentController"
		})
		.when(navigator.component.edit.url, {
			templateUrl: "app/project/component/component-edit-template.html",
			controller: "ProjectComponentController"
		})
		.when(navigator.component.remove.url, {
			templateUrl: "app/project/component/component-delete-template.html",
			controller: "ProjectComponentController"
		})

		//Deployment Steps
		.when(navigator.deploymentStep.componentCreate.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-edit-template.html",
			controller: "ProjectDeploymentStepController"
		})
		.when(navigator.deploymentStep.configurationCreate.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-edit-template.html",
			controller: "ProjectDeploymentStepController"
		})
		.when(navigator.deploymentStep.componentEdit.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-edit-template.html",
			controller: "ProjectDeploymentStepController"
		})
		.when(navigator.deploymentStep.configurationEdit.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-edit-template.html",
			controller: "ProjectDeploymentStepController"
		})
		.when(navigator.deploymentStep.componentRemove.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-delete-template.html",
			controller: "ProjectDeploymentStepController"
		})
		.when(navigator.deploymentStep.configurationRemove.url, {
			templateUrl: "app/project/deploymentStep/deploymentstep-delete-template.html",
			controller: "ProjectDeploymentStepController"
		})

		//Branches
		.when(navigator.branch.create.url, {
			templateUrl: "app/project/branch/branch-edit-template.html",
			controller: "ProjectBranchController"
		})
		.when(navigator.branch.edit.url, {
			templateUrl: "app/project/branch/branch-edit-template.html",
			controller: "ProjectBranchController"
		})
		.when(navigator.branch.remove.url, {
			templateUrl: "app/project/branch/branch-delete-template.html",
			controller: "ProjectBranchController"
		})

		//Environments
		.when(navigator.environment.create.url, {
			templateUrl: "app/project/environment/environment-edit-template.html",
			controller: "ProjectEnvironmentController"
		})
		.when(navigator.environment.edit.url, {
			templateUrl: "app/project/environment/environment-edit-template.html",
			controller: "ProjectEnvironmentController"
		})
		.when(navigator.environment.remove.url, {
			templateUrl: "app/project/environment/environment-delete-template.html",
			controller: "ProjectEnvironmentController"
		})

		//Project Roles
		.when(navigator.projectRole.list.url, {
			templateUrl: "app/project/role/role-list-template.html",
			controller: "ProjectRoleController"
		})
		.when(navigator.projectRole.edit.url, {
			templateUrl: "app/project/role/role-list-template.html",
			controller: "ProjectRoleController"
		})

		//Builds
		.when(navigator.build.list.url, {
			templateUrl: "app/build/build-list-template.html",
			controller: "BuildListController"
		})
		.when(navigator.build.submit.url, {
			templateUrl: "app/build/build-submit-template.html",
			controller: "BuildController"
		})
		.when(navigator.build.view.url, {
			templateUrl: "app/build/build-view-template.html",
			controller: "BuildController"
		})
		.when(navigator.build.remove.url, {
			templateUrl: "app/build/build-delete-template.html",
			controller: "BuildController"
		})

		//Deployments
		.when(navigator.deployment.batchRequest.url, {
			templateUrl: "app/deployment/deployment-batchRequest-template.html",
			controller: "deployBatchRequestController"
		})
		.when(navigator.deployment.batchCopy.url, {
		    templateUrl: "app/deployment/deployment-batchRequest-template.html",
			controller: "deployBatchRequestController"
		})
		.when(navigator.deployment.batchList.url, {
		    templateUrl: "app/deployment/deployment-batchList-template.html",
			controller: "deployBatchListController"
		})
		.when(navigator.deployment.batchStatus.url, {
		    templateUrl: "app/deployment/deployment-batchStatus-template.html",
			controller: "deployBatchStatusController"
		})
        .when(navigator.deployment.offlineStatus.url, {
            templateUrl: "app/deployment/deployment-offlineStatus-template.html",
            controller: "deployOfflineStatusController"
        })
		.when(navigator.deployment.view.url, {
		    templateUrl: "app/deployment/deployment-view-template.html",
			controller: "DeployController"
		})

		//System Log
		.when(navigator.systemLog.list.url, {
			templateUrl: "templates/systemlog-list-template.html",
			controller: "systemLogController"
		})

		//System Settings
		.when(navigator.systemSettings.list.url, {
			templateUrl: "templates/systemSettings/systemSettings-list.html",
			controller: "systemSettingsController"
		})
            //Credentials
		    .when(navigator.systemSettings.credentials.list.url, {
			    templateUrl: "templates/systemSettings/systemSettings-credentials-list.html",
			    controller: "credentialsController"
		    })
		    .when(navigator.systemSettings.credentials.create.url, {
			    templateUrl: "templates/systemSettings/systemSettings-credentials-create.html",
			    controller: "credentialsController"
		    })
		    .when(navigator.systemSettings.credentials.edit.url, {
		        templateUrl: "templates/systemSettings/systemSettings-credentials-edit.html",
		        controller: "credentialsController"
		    })
		    .when(navigator.systemSettings.credentials.remove.url, {
		        templateUrl: "templates/systemSettings/systemSettings-credentials-delete.html",
		        controller: "credentialsController"
		    })
            //Users
            .when(navigator.systemSettings.user.list.url, {
                templateUrl: "templates/systemSettings/systemSettings-user-list.html",
                controller: "userController"
            })
            .when(navigator.systemSettings.user.create.url, {
                templateUrl: "templates/systemSettings/systemSettings-user-create.html",
                controller: "userController"
            })
            .when(navigator.systemSettings.user.edit.url, {
                templateUrl: "templates/systemSettings/systemSettings-user-edit.html",
                controller: "userController"
            })
            .when(navigator.systemSettings.user.remove.url, {
                templateUrl: "templates/systemSettings/systemSettings-user-delete.html",
                controller: "userController"
            })
            //System Roles
            .when(navigator.systemSettings.systemRole.list.url, {
                templateUrl: "templates/systemSettings/systemSettings-systemRole-list.html",
                controller: "systemRoleController"
            })
            .when(navigator.systemSettings.systemRole.create.url, {
                templateUrl: "templates/systemSettings/systemSettings-systemRole-create.html",
                controller: "systemRoleController"
            })
            .when(navigator.systemSettings.systemRole.edit.url, {
                templateUrl: "templates/systemSettings/systemSettings-systemRole-edit.html",
                controller: "systemRoleController"
            })
            .when(navigator.systemSettings.systemRole.remove.url, {
                templateUrl: "templates/systemSettings/systemSettings-systemRole-delete.html",
                controller: "systemRoleController"
            })
            // Deployment Tools
            .when(navigator.systemSettings.deploymentTool.list.url, {
                templateUrl: "scripts/sriracha/systemSettings/deploymentTools/list.html",
                controller: "deploymentToolsController"
            })
            .when(navigator.systemSettings.deploymentTool.create.url, {
                templateUrl: "scripts/sriracha/systemSettings/deploymentTools/create.html",
                controller: "systemRoleController"
            })

		.otherwise({
			template: "<h1>Not Found</h1>"
		})

	;
}]);

