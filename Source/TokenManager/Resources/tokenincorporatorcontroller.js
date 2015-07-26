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
			tokenfactory.isSitecoreBasedCollection(vm.selectedTokenCategory).then(function(response) {
				vm.sitecoreBasedTokenCollection = response.data;
				if (!vm.sitecoreBasedTokenCollection)
					vm.existingToken = true;
			});
			tokenfactory.tokens(vm.selectedTokenCategory).then(function(response) {
				vm.tokens = response.data;
			});
		}
		vm.incorporateToken = function () {
			vm.spinner = true;
			tokenfactory.incorporateToken(vm.selectedTokenCategory, vm.tokenName, vm.tokenValue).then(function(response) {
				vm.response = response.data;
				vm.spinner = false;
			});
		}
		vm.formFilled = function() {
			return vm.selectedTokenCategory && vm.tokenName && vm.tokenValue ;
		}
	}
})();
