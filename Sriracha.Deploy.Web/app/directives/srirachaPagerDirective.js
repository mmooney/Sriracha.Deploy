angular.module("ngSriracha").directive("srirachaPager",
		[
		function () {
			return {
				restrict: "E",
				templateUrl: "templates/directives/srirachaPager.html",
				scope: {
					pagedList: '=',
					goToPage: '='
				},
				link: function postLink(scope, element, attrs) {
					scope.pageLinks = [];
					scope.$watch("pagedList.items", function () {
						if (scope.pagedList && scope.pagedList.items) {
							var startLink = 1;
							if (scope.pagedList.pageNumber > 5) {
								startLink = scope.pagedList.pageNumber - 5;
								scope.morePreviousPages = true;
							}
							var endLink = scope.pagedList.pageCount;
							if (scope.pagedList.pageNumber < scope.pagedList.pageCount - 5) {
								endLink = scope.pagedList.pageNumber + 5;
								scope.moreNextPages = true;
							}
							for (var i = startLink; i <= endLink; i++) {
								var pageLink = {
									pageNumber: i
								};
								if (i == scope.pagedList.pageNumber) {
									pageLink.currentPage = true;
								}
								scope.pageLinks.push(pageLink);
							}
						}
					});
				}
			};
		}]);
