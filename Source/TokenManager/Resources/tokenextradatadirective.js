(function() {
    'use strict';

    angular
        .module('app')
        .directive('tokenextradata', tokenextradata);

    tokenextradata.$inject = ['tokenfactory'];
    
    function tokenextradata(tokenfactory) {

        var directive = {
            templateUrl: "tokenmanager/tokenextradata.html",
            restrict: 'EA',
			scope: {
			    master: '=',
                token: '='
			}
        };
        return directive;

        function link(scope, element, attrs) {
        }
    }

})();