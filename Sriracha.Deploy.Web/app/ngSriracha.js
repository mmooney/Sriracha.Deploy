var ngSriracha = angular.module("ngSriracha", ["ngResource", "SharedServices", "ngUpload", "ngRoute", "ui.bootstrap", "angularBootstrapNavTree", "ui.ace"]);

ngSriracha.filter("displayDate", function () {
	return function (input) {
		if (input) {
			var date = new Date(input);
			return date.toString();
		}
	}
});

ngSriracha.filter("displayDateTimeShort", function () {
	return function (input) {
		if (input) {
			var date = new Date(input);
			var amPm;
			var twelveHourHour;
			if (date.getHours() > 12) {
				amPm = "PM";
				twelveHourHour = date.getHours() - 12;
			}
			else {
				amPm = "AM";
				twelveHourHour = date.getHours();
			}
			var returnValue = date.getFullYear() + "-" + lpad(date.getMonth()+1, 2) + "-" + lpad(date.getDate(), 2) + " " + lpad(twelveHourHour, 2) + ":" + lpad(date.getMinutes(), 2) + ":" + lpad(date.getSeconds(), 2) + " " + amPm + " " + getTimezoneShort(date);
			return returnValue;
		}

		function lpad(data, size, padChar) {
			padChar = padChar || "0";
			var returnValue = data.toString() || "";
			while (returnValue.length < size) {
				returnValue = padChar + returnValue;
			}
			return returnValue;
		}
		function getTimezoneShort(now) { //now is expected as a Date object
			if (now == null)
				return '';
			var str = now.toString();
			// Split on the first ( character
			var s = str.split("(");
			if (s.length == 2) {
				// remove the ending ')'
				var n = s[1].replace(")", "");
				// split on words
				var parts = n.split(" ");
				var abbr = "";
				for (i = 0; i < parts.length; i++) {
					// for each word - get the first letter
					abbr += parts[i].charAt(0).toUpperCase();
				}
				return abbr;
			}
		}
	}
});


ngSriracha.directive("projectList", function () {
	return {
		restrict: "E",
		templateUrl: "app/project/project-list-template.html"
	}
});

ngSriracha.directive("buildList", function () {
	return {
		restrict: "E",
		templateUrl: "app/build/build-list-template.html"
	}
});

ngSriracha.directive("systemLogList", function () {
	return {
		restrict: "E",
		templateUrl: "app/systemlog-list-template.html"
	}
});

