/// <reference path="angular.js" />
(function () {
	'use strict';

	angular
        .module('app')
        .controller('tokenselectorcontroller', tokenselectorcontroller);

	tokenselectorcontroller.$inject = ['$http', 'tokenfactory'];

	function tokenselectorcontroller($http, tokenfactory) {
		var vm = this;
		vm.tokens = [];
		tokenfactory.tokenCategories().then(function(response) {
			vm.categories = response.data;
		});
		vm.loadTokens = function(category) {
			if (category === vm.category)
				vm.category = "";
			else {
				tokenfactory.tokens(category).then(function(response) {
					vm.tokens = response.data;
					vm.category = category;
					vm.token = "";
				});
			}
		}
		vm.reset = function() {
			vm.selectedToken = "";
			vm.category = "";
			vm.tokens = [];
			tokenfactory.tokenCategories().then(function (response) {
				vm.categories = response.data;
			});
		}

	vm.getTokenIdentifier = function(category, token) {
			tokenfactory.tokenIdentifier(category, token).then(function(response) {
				vm.tokenIdentifier = response.data;
				vm.token = token;
			});
		}
		vm.returnToken = function(category, token) {
			if (typeof (window.parent.scTokenSelectorCallback) != "undefined") {
				tokenfactory.tokenIdentifier(category, token).then(function(response) {

					window.parent.scTokenSelectorCallback(null, response.data);
					window.close();
				});
			} else {
				tokenfactory.tokenStats(category, token).then(function(response) {
					vm.tokenStats = response.data;
					vm.selectedToken = true;
				});
			}
		}
	}
})();