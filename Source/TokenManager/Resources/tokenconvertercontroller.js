(function () {
    'use strict';

    angular
        .module('app')
        .controller('tokenconvertercontroller', tokenconvertercontroller);

    tokenconvertercontroller.$inject = ['tokenfactory']; 

    function tokenconvertercontroller(tokenfactory) {
        /* jshint validthis:true */
        var vm = this;
        vm.prefix = "";
        vm.suffix = "";
        vm.delimiter = "";
	    vm.response = { "Count": 0 };
	    vm.convertToken = function () {
		    vm.spinner = true;
			tokenfactory.tokenConvert(vm.prefix, vm.suffix, vm.delimiter).then(function (response) {
				vm.response = response.data;
				vm.spinner = false;
			});
		}
        activate();

        function activate() { }
    }
})();
