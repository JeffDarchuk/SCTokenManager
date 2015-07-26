(function() {
    'use strict';

    angular
        .module('app')
        .directive('tokenconverter', tokenconverter);

    tokenconverter.$inject = ['tokenfactory'];
    
    function tokenconverter(tokenfactory) {

        var directive = {
            templateUrl: "tokenmanager/tokenconverter.html",
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