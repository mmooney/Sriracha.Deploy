angular.module("ngSriracha").directive("sortHeader",
		['$compile',
		function ($compile) {
			return {
				restrict: "A",
				scope: {
					sortField: '@',
					applySort: '=',
					pagedList: '='
				},
				link: function postLink(scope, element, attrs) {
					scope.$watch("pagedList.items", function () {
						if (scope.pagedList && scope.pagedList.items) {
							element.addClass("sortHeader");
							if (scope.pagedList.sortField == scope.sortField) {
								element.addClass("active");
								if (!element.children(".sortHeaderArrow").length) {
									if (scope.pagedList.sortAscending) {
										element.append($compile("<i class='icon-arrow-up sortHeaderArrow'></i>")(scope));
									}
									else {
										element.append($compile("<i class='icon-arrow-down sortHeaderArrow'></i>")(scope));
									}
								}
							}
							element.click(function () {
								if (scope.applySort) {
									if (scope.pagedList.sortField == scope.sortField) {
										scope.applySort(scope.sortField, !scope.pagedList.sortAscending);
									}
									else {
										scope.applySort(scope.sortField);
									}
								}
								else {
									console.error("Missing sortHeaderDirective missing applySort function");
								}
							})
						}
					});
				}
			};
		}]);
