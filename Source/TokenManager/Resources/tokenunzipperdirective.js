(function() {
    'use strict';

    angular
        .module('app')
        .directive('tokenunzipper', tokenunzipper);

    tokenunzipper.$inject = ['tokenfactory'];
    
    function tokenunzipper(tokenfactory) {

        var directive = {
            templateUrl: "tokenmanager/tokenunzipper.html",
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