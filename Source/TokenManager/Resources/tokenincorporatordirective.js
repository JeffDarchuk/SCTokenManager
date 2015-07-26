(function() {
    'use strict';

    angular
        .module('app')
        .directive('tokenincorporator', tokenincorporator);

    tokenincorporator.$inject = ['tokenfactory'];
    
    function tokenincorporator(tokenfactory) {

        var directive = {
        	templateUrl: "tokenmanager/tokenincorporator.html",
            restrict: 'EA',
			scope: {
				master: '='
			}
        };
        return directive;

        function link(scope, element, attrs) {
        }
    }

})();