(function () {
	'use strict';

	angular
        .module('app')
        .controller('contenttreecontroller', contenttreecontroller);

	contenttreecontroller.$inject = ['tokenfactory', '$scope'];

	function contenttreecontroller(tokenfactory, $scope) {
		/* jshint validthis:true */
		var vm = this;
		vm.Open = false;
		vm.selected = "";
		vm.selectNode = function (id) {
			vm.selected = id;
		}
		$scope.init = function (nodeId, selectedId, events) {
			tokenfactory.contentTree(nodeId, "master").then(function (response) {
				vm.data = response.data;
				if (selectedId === nodeId) {
					events.selectedItem = vm.data.Id;
					events.click(vm.data);
				}
			}, function (response) {
				vm.error = response.data;
			});
			if (typeof (selectedId) !== "undefined" && selectedId.length > 0)
				tokenfactory.contentTreeSelectedRelated(nodeId, selectedId).then(function (response) {
					if (response.data)
						vm.Open = true;
				}, function (response) {
					vm.error = response.data;
				});

		}
	}
})();
