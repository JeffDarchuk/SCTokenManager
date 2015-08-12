(function () {
    'use strict';

    angular
        .module('app')
        .factory('tokenfactory', tokenfactory);

    tokenfactory.$inject = ['$http'];

    function tokenfactory($http) {
        var service = {
        	tokenCategories: function() {
        		return $http.get("tokenmanager/categories.json?sc_itemid=" + window.parent.scItemID);
	        },
        	tokens: function (category) {
        		var data = { "category": category };
				return $http.post("tokenmanager/tokens.json", data);
			},
        	tokenStats: function (category, token) {
        		var data = { "category": category, "token": token };
        		return $http.post("tokenmanager/tokenstats.json", data);
			},
        	tokenIdentifier: function (category, token) {
        		var data = { "category": category, "token": token };
        		return $http.post("tokenmanager/tokenidentifier.json", data);
			},
        	tokenConvert: function (prefix, delimiter, suffix) {
        		var data = { "prefix": prefix, "delimiter": delimiter, "suffix": suffix };
        		return $http.post("tokenmanager/tokenconvert.json", data);
			},
			databases: function() {
				return $http.post("tokenmanager/databases.json");
			},
			sitecoreTokenCategories: function (database) {
				var data = { "database": database};
				return $http.post("tokenmanager/sitecoretokencollections.json", data);
			},
			incorporateToken: function (category, tokenName, tokenValue) {
				var data = { "category": category, "tokenName": tokenName, "tokenValue": tokenValue };
				return $http.post("tokenmanager/incorporatetokens.json", data);
			},
			isSitecoreBasedCollection: function (category) {
				var data = { "category": category };
				return $http.post("tokenmanager/issitecorecollection.json", data);
			},
			unzipToken: function (category, token) {
				var data = { "category": category, "token": token };
				return $http.post("tokenmanager/unziptoken.json", data);
			}

        };

        return service;

        function getData() { }
    }
})();