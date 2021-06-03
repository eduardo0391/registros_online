var myModule = angular.module('myApp', ['ui.bootstrap']);

myModule.filter('startFromGrid', function () {
    return function (input, start) {
        if (input == null)
            return input;
        else {
            start = +start;
            return input.slice(start);
        }
    }
});
myModule.run(function (paginationConfig) {
    paginationConfig.firstText = '<<';
    paginationConfig.previousText = '<';
    paginationConfig.lastText = ">>";
    paginationConfig.nextText = ">"
})
    
myModule.service('usuarioService', function ($http, $q) {
    this.getUsuarios = function () {
        var deferred = $q.defer();
        $http.get('/UsersManager/GetUsers')
            .success(function success(response) {
                deferred.resolve(response);
            })
            .error(function (error) {
                console.log("error getting data array");
                deferred.reject();
            });

        return deferred.promise;
    }
});


myModule.controller('paginacionController', function ($scope, $filter, usuarioService, $log) {

    $scope.LoadUsers = function () {
        usuarioService.getUsuarios().then(function (data) {
            $scope.usuarios = data;
            configurar();
        });
    }

    $("#creationDate").datepicker();
    $("#shipmentDate").datepicker();
    $("#ExpirationDate").datepicker();

    $scope.predicate = 'name';
    $scope.reverse = true;
    $scope.order = function (predicate) {
        $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
        $scope.predicate = predicate;
    };
    $scope.formatDate = function (date) {
        if (date)
          return date.replace('/Date(', '').replace(')/', '');
        else
          return date;
    };
    function configurar() {

        $scope.totalItems = $scope.usuarios.length;
        $scope.currentPage = 1;
        $scope.itemsPerPage = 5;
        $scope.maxSize = 5; //maximo de paginas en el paginador
        $scope.numPerPage = Math.ceil($scope.totalItems / $scope.maxSize);
        $scope.setPage = function (pageNo) {
            $scope.currentPage = pageNo;
        };


        $scope.paginaActualPrincipal = 1;
        $scope.pageChanged = function () {
            $scope.currentPage = $scope.paginaActualPrincipal;
        };
        $scope.editar = function (data) {
            document.getElementById("email").value = data.email.toString().trim();
            document.getElementById("name").value = data.name.toString().trim();
            document.getElementById("user").value = data.user.toString().trim();
            document.getElementById("ExpirationDate").value = data.ExpirationDate;
            document.getElementById("shipmentDate").value = data.shipmentDate;
            document.getElementById("IsSuperUser").checked  = data.IsSuperUser;
            document.getElementById("creationDate").value = data.creationDate;
            document.getElementById("codActivation").value = data.codActivation;
            document.getElementById("IsConfirm").checked = data.IsConfirm;
            document.getElementById("id_user").value = data.id_user;
        }
        $scope.updateUsers = function () {
            alert('asdas');
        }
        //$scope.formatDateModal = function (date) {
        //    if (date) {
        //        var dateString = date.substr(6);
        //        var newFormattedDate = $.datepicker.formatDate('dd/mm/yy', new Date(parseInt(dateString)));
        //        return newFormattedDate;
        //    }
        //    else
        //        return date;
        //}
    }


});

