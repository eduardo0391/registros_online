$("#actualizarPerfil").addClass("active-menu");

var myModule = angular.module('myApp', ['ui.bootstrap']);
myModule.service('registrosService', function ($http, $q) {
    this.postRegistros = function (url, data) {
        var deferred = $q.defer();

        $http.get(url, { params: data })
               .success(function success(response) {
                   deferred.resolve(response);

               })
               .error(function (e) {
                   console.log("error getting data array");
                   deferred.reject();
               });

        return deferred.promise;
    }
});

myModule.controller('usuarioController', function ($scope)
{
    $scope.email =$("#email").val();
});

function limpiarAvisos()
{
    $("#divError ul").empty();
    $("#divTarget").addClass("hidden");
}

function actualizarUsuario(data) {

    if (data.timeout) {
        alert("su sesión caducó");
        window.location.href = '/home/';
    }
    if (data.key == "error") {
        limpiarAvisos();
        $("#divError ul").append("<li>" + data.descripcion + "</li>")
        //agrego los errores
        $("#divError").removeClass("validation-summary-valid");

    }
    else if (data.key == "ok") {
        if (data.descripcion == "emailDistinto") {
            $('#userModal').modal('show');
        }
        else {
            $("#divError").addClass("validation-summary-valid");
            $("#divTarget").removeClass("hidden");
        }
    }
}


function actualizarPassword(data) {


    if (data.timeout){
        alert("su sesión caducó");
        window.location.href = '/home/';
    }
    else if (data.key == "error") {
        $("#divPasswordError ul").empty();
        $("#divPasswordTarget").addClass("hidden");
        $("#divPasswordError").removeClass("validation-summary-valid");
        for (i = 0; i < data.summary.length; i++) {
            $("#divPasswordError ul").append("<li>" + data.summary[i].error + "</li>")
        }
    }
    else if (data.key == "ok") {
        $("#divPasswordError").addClass("validation-summary-valid");
        $("#divPasswordTarget").removeClass("hidden");

    }
}


