(function() {
    'use strict';

    angular
        .module('app')
        .directive('contenttree', contenttree);

    contenttree.$inject = ['tokenfactory', '$compile'];

    
    function contenttree(tokenfactory, $compile) {

        var directive = {
            templateUrl: "tokenmanager/contenttree.html",
            restrict: 'E',
            scope: {
                parent: '=',
                events: '=',
                selected: '='
            },
            compile: function(tElement, tAttr) {
                var contents = tElement.contents().remove();
                var compiledContents;
                return function(scope, iElement, iAttr) {
                    if (!compiledContents) {
                        compiledContents = $compile(contents);
                    }
                    compiledContents(scope, function(clone, scope) {
                        iElement.append(clone);
                    });
                };
            }
        }
        return directive;

        function link(scope, element, attrs) {
        }
    }

})();