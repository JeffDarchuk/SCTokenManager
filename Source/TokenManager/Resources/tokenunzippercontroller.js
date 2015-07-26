(function () {
    'use strict';

    angular
        .module('app')
        .controller('tokenunzippercontroller', tokenunzippercontroller);

    tokenunzippercontroller.$inject = ['tokenfactory'];

    function tokenunzippercontroller(tokenfactory) {
        /* jshint validthis:true */
    	var vm = this;
    	vm.tokenCategories = [];
    	vm.tokens = [];
    	vm.selectedToken = "";
    	vm.selectedTokenCategory = "";
	    tokenfactory.tokenCategories().then(function (response) {
    		vm.tokenCategories = response.data;
    	});
    	vm.loadTokens = function () {
    		tokenfactory.tokens(vm.selectedTokenCategory).then(function (response) {
    			vm.tokens = response.data;
    		});
    	}
		vm.formFilled = function() {
			return vm.selectedToken && vm.selectedTokenCategory;
		}
		vm.unzipToken = function () {
			vm.spinner = true;
			tokenfactory.unzipToken(vm.selectedTokenCategory, vm.selectedToken).then(function(response) {
				vm.response = response.data;
				vm.spinner = false;
			});
		}
    }
})();
