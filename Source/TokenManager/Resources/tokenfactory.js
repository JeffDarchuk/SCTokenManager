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
        	tokenIdentifier: function (category, token, tokenData) {
        	    var data = { "category": category, "token": token, "data": tokenData };
        		return $http.post("tokenmanager/tokenidentifier.json", data);
        	},
        	tokenConvert: function (root, preview, prefix, delimiter, suffix) {
        		var data = { "root":root, "preview" :preview, "prefix": prefix, "delimiter": delimiter, "suffix": suffix };
        		return $http.post("tokenmanager/tokenconvert.json", data);
			},
			databases: function() {
				return $http.post("tokenmanager/databases.json");
			},
			sitecoreTokenCategories: function (database) {
				var data = { "database": database};
				return $http.post("tokenmanager/sitecoretokencollections.json", data);
			},
			incorporateToken: function (root, preview, type, category, tokenName, tokenValue) {
			    var data = { "root": root, "preview": preview, "type":type, "category": category, "tokenName": tokenName, "tokenValue": tokenValue };
				return $http.post("tokenmanager/incorporatetokens.json", data);
			},
			isSitecoreBasedCollection: function (category) {
				var data = { "category": category };
				return $http.post("tokenmanager/issitecorecollection.json", data);
			},
			unzipToken: function (root, preview, category, token, replaceWithValue) {
			    var data = { "root": root, "preview": preview, "category": category, "token": token, "replaceWithValue": replaceWithValue };
				return $http.post("tokenmanager/unziptoken.json", data);
			},
            contentTree: function(id, database) {
                var data = { "id": id, "database": database };
                return $http.post("tokenmanager/contenttree.json", data);
            },
            contentTreeSelectedRelated: function(currentId, selectedId) {
                var data = { "currentId": currentId, "selectedId": selectedId };
                return $http.post("tokenmanager/contenttreeselectedrelated.json", data);
            },
            tokenExtraData: function(category, token) {
                var data = { "category": category, "token": token };
                return $http.post("tokenmanager/tokenextradata.json", data);
            },
            getSelectedToken: function() {
                return $http.post("tokenmanager/tokenselected.json");
            }

        };
        
        return service;

        function getData() { }
    }
})();