(function() {
    'use strict';

    angular
        .module('app')
        .directive('tokenselector', tokenselector);

    tokenselector.$inject = ['$window'];
    
    function tokenselector($window) {
        // Usage:
        //     <directive1></directive1>
        // Creates:
        // 
        var directive = {
            templateUrl: "tokenmanager/tokenselector.html",
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