(function () {
	'use strict';

	angular
        .module('app')
        .controller('tokenincorporatorcontroller', tokenincorporatorcontroller);

	tokenincorporatorcontroller.$inject = ['tokenfactory'];

	function tokenincorporatorcontroller(tokenfactory) {
		/* jshint validthis:true */
		var vm = this;
		vm.tokenCategories = [];
		vm.tokens = [];
		vm.selectedToken = "";
		vm.selectedTokenCategory = "";
		vm.spinner = false;
		vm.response = "";
		vm.tokenName = "";
		vm.tokenValue = "";
		vm.preview = true;
	    vm.type = "plain";
		vm.events = {
		    'click': function (val) {
		        vm.events.selected = val;
		    },
		    'selected': function () {
		        return vm.events.selected.Id;
		    }
		};
		vm.itemChosen = function () { return typeof (vm.events.selected) != "undefined" }

		tokenfactory.tokenCategories().then(function (response) {
			vm.tokenCategories = response.data;
		});
		vm.existingTokenSelected = function () {
			if (!vm.sitecoreBasedTokenCollection)
				return true;
			return typeof (vm.existingToken) != "undefined";
		}
		vm.loadTokens = function () {
			vm.existingToken = undefined;
			tokenfactory.isSitecoreBasedCollection(vm.selectedTokenCategory.Label).then(function(response) {
				vm.sitecoreBasedTokenCollection = response.data;
				if (!vm.sitecoreBasedTokenCollection)
					vm.existingToken = true;
			});
			tokenfactory.tokens(vm.selectedTokenCategory.Label).then(function(response) {
				vm.tokens = response.data;
			});
		}
		vm.incorporateToken = function () {
			vm.spinner = true;
			tokenfactory.incorporateToken(vm.events.selected.Id, vm.preview, vm.type, vm.selectedTokenCategory.Label, vm.tokenName.Label, vm.tokenValue).then(function(response) {
				vm.response = response.data;
				vm.spinner = false;
			});
		}
		vm.formFilled = function() {
			return vm.itemChosen() && vm.selectedTokenCategory && vm.tokenName && vm.tokenValue ;
		}

	}
})();
