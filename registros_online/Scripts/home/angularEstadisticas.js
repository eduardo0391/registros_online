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


myModule.filter("miFecha", function () {
    var re = /\/Date\(([0-9]*)\)\//;
    return function (x) {
        var m = x.match(re);
        if (m) return new Date(parseInt(m[1]));
        else return null;
    };
});

myModule.filter("myNumber", function () {
    return function (y) {
        var x = y.toString();
        x = x.replace(/,/g, "x");
        x  = x.replace(".", ",");
        x = x.replace(/x/g, ".")
        return x;
    };
});

myModule.service('registrosService', function ($http, $q) {
    this.getRegistros = function (url, data) {
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


myModule.controller('egresosController', function ($scope, $filter, registrosService) {
    //paginacion
    $scope.itemsPerPage = 10;
    $scope.maxPaginas = 5; //maximo de paginas en el paginador
    $scope.paginaActualPrincipal = 1;
    $scope.checkGeneral = false;

    //paginacion de la tabla detalla
    $scope.itemsPerPageDet = 10;
    $scope.maxPaginasDet = 5; //maximo de paginas en el paginador
    $scope.paginaActualDet = 1;
    $scope.checkGeneralDet = {
        valor: false
           };


    $scope.mesesSeleccionados = [];
    $scope.detallesRegistros = [];
    $scope.totalDetalles =0;
    $scope.meses = [
            { id: '1', value: 'Enero' },
            { id: '2', value: 'Febrero' },
            { id: '3', value: 'Marzo' },
            { id: '4', value: 'Abril' },
            { id: '5', value: 'Mayo' },
            { id: '6', value: 'Junio' },
            { id: '7', value: 'Julio' },
            { id: '8', value: 'Agosto' },
            { id: '9', value: 'Septiembre' },
            { id: '10', value: 'Octubre' },
            { id: '11', value: 'Noviembre' },
            { id: '12', value: 'Diciembre' }
    ];

    var placeholder = $("#placeholder");
    placeholder.bind("plotclick", function (event, pos, obj) {
        if (!obj) {
            return;
        }
        var cantidadRegistros = 0;
        cantidadRegistros = angular.element(document.getElementById('angularId')).scope().mesesSeleccionados.length;
        $("#informacionPromedio")[0].innerHTML= "$" + Math.round(obj.datapoint[1][0][1] / cantidadRegistros);
        $("#informacion")[0].innerHTML= "$" + Math.round(obj.datapoint[1][0][1]);
        $("#msjDialog").dialog("open");
     
    });

    $scope.opciones = [
    { id: '20', value: 10, name: '20' },
    { id: '50', value: 20, name: '50' },
        { id: '100', value: 100, name: '100' },
    { id: 'todo', value: 'todo', name: 'Todo' }
    ];
    $scope.selectedOption = $scope.opciones[0];
    $scope.selectedOptionDet = $scope.opciones[0];

    $scope.cargar = function () {
        cargar();
    }

    $scope.seleccionarCantDet = function (pagina)
    {
        if ($scope.selectedOptionDet.value == "todo")
            $scope.itemsPerPageDet = $scope.detallesRegistros.length;
        else
            $scope.itemsPerPageDet = $scope.selectedOptionDet.value;
        $scope.paginaActualDet = ($scope.paginaActualDet == null) ? 1 : $scope.paginaActualDet;
    }

    $scope.seleccionarCantidad = function (pagina) {
        if ($scope.selectedOption.value == "todo")
            $scope.itemsPerPage = $scope.cantidad;
        else
            $scope.itemsPerPage = $scope.selectedOption.value;
        $scope.currentPage = ($scope.currentPage == null) ? 1 : $scope.currentPage;
    }

    $scope.totalCheck = false;
    $scope.total = 0;
    $scope.sumarTodo = function (valorBoolean) {
        $scope.mesesSeleccionados = [];
        $scope.total = 0;


        if (valorBoolean) {
            for (var i = 0; i < $scope.registros.length; i++) {
                $scope.registros[i].seleccionado = true;
                $scope.mesesSeleccionados[i] = $scope.registros[i][0].toString() + $scope.registros[i][1].id;
                $scope.total += $scope.registros[i][2];
            }
            //armo el grafico
                  getGrafico()
        }
        else {
            for (var i = 0; i < $scope.registros.length; i++)
                $scope.registros[i].seleccionado = false;
            //no muestro nada
            $scope.detallesRegistros = [];
        }
    }

    $scope.sumarTodoDetalle = function (valorBoolean) {
        $scope.checkGeneralDet.valor = valorBoolean;
        $scope.totalDetalles = 0;
        if (valorBoolean) {
            for (var i = 0; i < $scope.detallesRegistros.length; i++) {
                $scope.detallesRegistros[i].seleccionado = true;
                $scope.totalDetalles += $scope.detallesRegistros[i].total;
            }
        }
        else {
            for (var i = 0; i < $scope.detallesRegistros.length; i++)
                $scope.detallesRegistros[i].seleccionado = false;
        }

    }

    $scope.sumarDetalle = function (valor, activo)
    {
        if (activo)
            $scope.totalDetalles = $scope.totalDetalles + valor;
        else
            $scope.totalDetalles = $scope.totalDetalles - valor;
    }
    $scope.sumar = function (valor, activo) {
        $scope.totalDetalles = 0;
        $scope.checkGeneralDet.valor = false;
        if (activo)
            $scope.total = $scope.total + valor;
        else
            $scope.total = $scope.total - valor;
        var cont = 0;
        $scope.mesesSeleccionados = [];
        for (var i = 0; i < $scope.registros.length; i++)
        {
            if ($scope.registros[i].seleccionado)
            {
                $scope.mesesSeleccionados[cont] = $scope.registros[i][0].toString() + $scope.registros[i][1].id;
            cont++;
            }
        }
        if ($scope.mesesSeleccionados.length > 0) {
            getGrafico()
        }
        else {
            $scope.detallesRegistros = [];
        }
    }

   
    function getGrafico()
    {
        data = { fechas: $scope.mesesSeleccionados.join(), tipo: $scope.tipoMovimiento };
        registrosService.getRegistros('/home/getEstadisticasProductos', data).then(function (data) {
            var placeholder = $("#placeholder");
            $scope.detallesRegistros = data.full;
            var estadisticas = [];
            var vectorGrafico = data.top10;
            for (var i = 0; i < vectorGrafico.length; i++) {

                estadisticas[i] = { data: vectorGrafico[i].total, label: vectorGrafico[i].movimiento }
            }
            $.plot(placeholder, estadisticas, {
                series: {
                    pie: {
                        show: true,
                        combine: {
                            color: "#999"
                            , threshold: 0.035
                        }
                    }
                },
                grid: {
                    hoverable: true,
                    clickable: true
                },
                legend: {
                    show: false
                }
            });
        });
    }

    $scope.cambiarEstado = function (estado, users) {
        $scope.total = 0;
        if (estado) {
            var total = $scope.itemsPerPage == "todo" ? $scope.cantidad : $scope.itemsPerPage;
            total = total > $scope.items.length ? $scope.items.length : total;

            for (var i = 0; (i < total) ; i++) {
                $scope.items[i].seleccionado = estado;
                $scope.total = $scope.total + ($scope.registros[i][2]);
            }
        }

        else
            angular.forEach($scope.registros, function (usuario) {
                $scope.itemsPerPage
                usuario.seleccionado = false;
            });

    }
    $scope.total = 0;
    $scope.predicate = 'anio';
    $scope.reverse = false;

    $scope.predicateCategories = '1';
    $scope.reverseCategories = false;

    $scope.order = function (predicate) {
        $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
        $scope.predicate = predicate;
    };

    $scope.orderCategory = function (predicateCategories) {
        $scope.reverseCategories = ($scope.predicateCategories === predicateCategories) ? !$scope.reverseCategories : false;
        $scope.predicateCategories = predicateCategories;
    };


    
    $scope.setPage = function (pagina) {
        if (pagina != null)
            $scope.paginaActualPrincipal = pagina;
    }


    function cargar() {
    
        data = {

            //anio: ($scope.selectAnios == null) ? new Date().getFullYear() : $scope.selectAnios.toString(),
            //mes: ($scope.selectMeses == null) ? new Date().getMonth() + 1 : $scope.selectMeses.id,
            //columna: "fecha",
            //orden: "fecha",
            //cantidadRegistros: $scope.itemsPerPage,
            //numeroPagina: $scope.currentPage,
            tipo : $scope.tipoMovimiento
        };
        registrosService.getRegistros('/home/getRegistrosxMes', data).then(function (data) {
            $scope.registros = data;
            $scope.cantidad = data.length;

            for (i = 0; i < $scope.cantidad; i++) {
                $scope.registros[i][1] = $scope.meses[$scope.registros[i][1] - 1];
            }

         
        });
    }


});