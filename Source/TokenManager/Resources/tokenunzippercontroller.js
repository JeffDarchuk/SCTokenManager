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
        vm.replaceWithValue = false;
    	vm.tokens = [];
    	vm.selectedToken = "";
    	vm.selectedTokenCategory = "";
    	vm.preview = true;
    	vm.itemChosen = function () { return typeof (vm.events.selected) != "undefined" }
    	vm.events = {
    	    'click': function (val) {
    	        vm.events.selected = val;
    	    },
            'selected': function() {
                return vm.events.selected.Id;
            }
    	};
	    tokenfactory.tokenCategories().then(function (response) {
    		vm.tokenCategories = response.data;
    	});
    	vm.loadTokens = function () {
    		tokenfactory.tokens(vm.selectedTokenCategory.Label).then(function (response) {
    			vm.tokens = response.data;
    		});
    	}
		vm.formFilled = function() {
			return vm.itemChosen() && vm.selectedToken && vm.selectedTokenCategory;
		}
		vm.unzipToken = function () {
			vm.spinner = true;
			tokenfactory.unzipToken(vm.events.selected.Id, vm.preview, vm.selectedTokenCategory.Label, vm.selectedToken.Label, vm.replaceWithValue).then(function(response) {
				vm.response = response.data;
				vm.spinner = false;
			});
		}
    }
})();
