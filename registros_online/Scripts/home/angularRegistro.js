var myModule = angular.module('myApp', ['ui.bootstrap','ngRoute',  'ngMaterial', 'scrollable-table', 'ngAnimate']);


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

        myModule.filter("myNumber", function () {
            return function (y) {
                var x = y.toString();
                x = x.replace(/,/g, "x");
                x = x.replace(".", ",");
                x = x.replace(/x/g, ".")
                return x;
            };
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

        myModule.service('registrosService', function ($http, $q) {
            this.getRegistros = function (url,data) {
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
            this.getAnios = function (tipo)
            {
                var deferred = $q.defer();
                $http.get('/home/getAnios', { params: { "tipo": tipo } })
                .success(function success(response)
                {
                    deferred.resolve(response);
                }
                    )
                .error(
                        function (e) {
                            console.log("error getting data array");
                            deferred.reject();
                        }
                       );
                return deferred.promise;
            }

            this.getMeses= function (anio, tipo) {
                var deferred = $q.defer();
                $http.get('/home/getMeses', { params: { "anio": anio, "tipo": tipo } })
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


        myModule.controller('egresosController', function ($scope, registrosService, $mdDialog) {
            $scope.tipo = 0;
            $scope.meses = [
                    {id:'1', value:'Enero'}, 
                    {id:'2', value:'Febrero'}, 
                    {id:'3', value:'Marzo'}, 
                    {id:'4', value:'Abril'}, 
                    {id:'5', value:'Mayo'}, 
                    {id:'6', value:'Junio'}, 
                    {id:'7', value:'Julio'}, 
                    {id:'8', value:'Agosto'}, 
                    {id:'9', value:'Septiembre'}, 
                    {id:'10', value:'Octubre'}, 
                    {id:'11', value:'Noviembre'}, 
                    {id:'12', value:'Diciembre'}
            ];
            $scope.categoriaSeleccionada = { descripcion: "" };
            $scope.cargarAnios = function () {
                registrosService.getAnios($scope.tipo)
                                  .then(function (data) {
                                      $scope.anios = data;
                                      $scope.selectAnios = data[data.length - 1];
                                  });
            }

            $scope.exportar = function () {
                data = { anio: $scope.selectAnios, mes: $scope.selectMeses.id }
                registrosService.getRegistros('/home/GetRegisters', data);
            }
            
            $scope.cargarMeses = function () {
                registrosService.getMeses($scope.selectAnios, $scope.tipo)
                      .then(function (data) {
                          $scope.mesesAnio = {};
                          for (i = 0; i < data.length; i++) {
                              $scope.mesesAnio[i] = $scope.meses[data[i] - 1];
                          }
                          $scope.selectMeses = $scope.mesesAnio[i - 1];
                          cargar();
                      });
            }
            $scope.registroEditar = { id:0, producto:"", descripcion:""};

            $scope.agregar = function () {
                eliminarSummary();

                //deselecciono todas las opciones 
                $scope.categorias.filter(function (obj) { obj.seleccionado = false; });
                $scope.categoriaSeleccionada = { descripcion: "" };

                document.getElementById("id").value = 0;
                //document.getElementById("categoria").value = "";
                document.getElementById("precio").value = "";
                document.getElementById("descripcion").value = "";
                document.getElementById("fecha").value = document.getElementById("fechaHoy").value;
            };

            function eliminarSummary() {
                summary = $('.validation-summary-errors ul')[0];
                if (summary)
                    summary.innerHTML = "";
            }

            $scope.editar = function (data, precio)
            {
                //borro el validation summary 
                eliminarSummary();
                $scope.categoriaSeleccionada.descripcion = data.movimiento;
                document.getElementById("id").value = data.id;
                document.getElementById("descripcion").value = data.descripcion;
                document.getElementById("precio").value =precio;
                var fechaString= data.fecha.substr(6);
                var newFormattedDate = $.datepicker.formatDate('dd/mm/yy', new Date(parseInt(fechaString)));
                document.getElementById("fecha").value = newFormattedDate;
                //$("#agregarRegistro").dialog('open');
            }
  
            $scope.showConfirm = function (ev, id, html) {

                var confirm = $mdDialog.confirm()
                    .title('Eliminar registro')
                    .textContent('Está seguro que desea eliminar el registro?')
                    .ariaLabel('Lucky day')
                    .targetEvent(ev)
                    .ok('Eliminar')
                    .cancel('Cancelar');

                $mdDialog.show(confirm).then(function () {
                    eliminarRegistro(id);
                }, function () {
          
                });
            }

            $scope.opciones = [

            { id: '20', value: 20, name: '20' },
            { id: '50', value: 50, name: '50' },
            { id: '100', value: 100, name: '100' },
            { id: 'todo', value: 'todo', name: 'Todo' }
            ];
            $scope.selectedOption = $scope.opciones[3];

            $scope.volverCargar = function()
            {
                cargar();
            }
  
            $scope.checkGeneral = false;
            $scope.IsRequiredPay = false;
            $scope.sumar = function (valor, activo) {

                if (activo)
                    $scope.total = $scope.total + valor;
                else
                    $scope.total = $scope.total - valor;
            }

            $scope.cambiarEstado = function (estado, users) {
                $scope.total = 0;
                if (estado) {
                    var total = $scope.itemsPerPage == "todo" ? $scope.cantidad : $scope.itemsPerPage;
                    total = total > $scope.registros.length ? $scope.registros.length : total;
         
                    for (var i = 0;( i < total ) ; i++) {
                        $scope.registros[i].seleccionado = estado;
                        $scope.total = $scope.total + ($scope.registros[i].precio);
                    }
                }

                else
                    angular.forEach($scope.registros, function (usuario) {
                        $scope.itemsPerPage
                        usuario.seleccionado = false;
                    });

            }
            $scope.total = 0;
            $scope.predicate = 'name';
            $scope.reverse = true;
            $scope.order = function (predicate) {

                $scope.reverse = ($scope.predicate === predicate) ? !$scope.reverse : false;
                $scope.predicate = predicate;

            };

            $scope.filas = function (pagina) {
    
                if ($scope.selectedOption.value == "todo")
                    $scope.itemsPerPage = $scope.cantidad;
                else
                    $scope.itemsPerPage = $scope.selectedOption.value;
        
                cargar();

            }

            $scope.setPage = function (pagina) {
                if (pagina != null)
                    $scope.currentPage = pagina;
                cargar();
            }

            function eliminarRegistro(id)
            {
                data = { id: id };
                registrosService.getRegistros('/home/eliminarRegistro', data).then(function (data) {
                    cargar();
                });
            }

            function cargar() {
        
                $scope.currentPage  =($scope.currentPage == null)? 1: $scope.currentPage;
                $scope.itemsPerPage =($scope.itemsPerPage == null)? "todo" :$scope.itemsPerPage ;
            
                data = {

                    anio: ($scope.selectAnios == null) ? new Date().getFullYear() : $scope.selectAnios.toString(),
                    mes: ($scope.selectMeses == null) ? new Date().getMonth() + 1 : $scope.selectMeses.id,
                    columna: "fecha",
                    orden: "fecha",
                    cantidadRegistros: $scope.itemsPerPage,
                    numeroPagina: $scope.currentPage,
                    tipo: $scope.tipo
                };
                registrosService.getRegistros('/home/GetRegisters', data).then(function (data) {
                    $scope.registros = data.registros;
                    $scope.cantidad = data.cantidad;
                    configurar();
                });
            }

            function configurar() {

                if ($scope.currentPage == null)
                    $scope.currentPage = 1;
                if ($scope.itemsPerPage == null)
                    $scope.itemsPerPage = 20;
                $scope.totalItems = $scope.cantidad;
                $scope.maxSize = 5; //maximo de paginas en el paginador
                //$scope.numPerPage = 20; //Math.ceil($scope.totalItems / $scope.maxSize);
                $scope.itemsPerPage = $scope.selectedOption.id; 
                $scope.paginaActualPrincipal = 1;
                $scope.checkGeneral = false;
                $scope.total = 0;
            }

            $scope.cambiarCategoria = function (data)
            {
                $scope.categorias.filter(function (obj) { obj.seleccionado = false; });
                $scope.categoriaSeleccionada = data;
                data.seleccionado = true;
            }
            $scope.cargarCategorias = cargarCategorias;
            
            function cargarCategorias() {
                data = { tipo: $scope.tipo }
                aux = [];
                registrosService.getRegistros('/Category/getCategorias', data).
                then(function (data) {

                    for (i = 0; i < data.length ; i++) {
                        aux.push(data[i]);
                        aux[i].seleccionado = false;

                    }
                  
                    aux.itemSeleccionado = {};
                    $scope.categorias = aux;
                });

            };

            $scope.categoriaaEliminar = {};
            $scope.editarCategoria = function (dato) {
                $scope.categorias.itemSeleccionado = angular.copy(dato);
          
            }
            $scope.seleccionarCategoria = function (dato) {
                $scope.categoriaaEliminar = angular.copy(dato);

            }

            $scope.eliminarCategoria = function () {
                var data = {id_categoria :$scope.categoriaaEliminar.id}
                registrosService.getRegistros('/Category/eliminarCategoria', data).then(function (data) {
                    cargarCategorias();
                    $scope.reset();
                    cargar();
                });
            }
            
            // gets the template to ng-include for a table row / item
            $scope.getTemplate = function (contact) {
                if (contact.id === $scope.categorias.itemSeleccionado.id) return 'edit';
                else return 'display';
            };



            $scope.categoriaNueva = { id: "0", descripcion: "", id_tipo: "" };
            $scope.guardarCategoria = function (categoria)
            {
                var auxCategoria = null;
                //Asigno a auxCategoria el parametro categoria (si no viene vacio), si no asigno la variable nueva
                auxCategoria = categoria ? categoria : $scope.categoriaNueva;

                if (auxCategoria.id == 0 & $scope.categoriaNueva.descripcion == "" ) {
                    $scope.messageCategory = "El campo está vacío";
                    $scope.titleMessageCategory = "Error";
                    document.getElementById("openModalMessageCategory").click();
                }
                else {

                    registrosService.getRegistros('/Category/agregarCategoria', auxCategoria).then(function (data) {

                 
                        $scope.messageCategory = data.message;
                        $scope.titleMessageCategory = data.key == "ok" ? "Successful" : "Error";
                        document.getElementById("openModalMessageCategory").click();
                        if (data.key == "ok") {
                            cargarCategorias();
                            $scope.reset();
                            $scope.categoriaNueva.descripcion = "";
                        }
                        
                     });
                }
            }
            $scope.errorAddRecord = function (e) {
                $scope.messagePayMP = e.description;
                $scope.IsRequiredPay = e.IsRequiredPay;   
                $("#PayMercadoPago").modal('show');
            }
            $scope.reset = function () {
                $scope.categorias.itemSeleccionado = {};
            };

        });

      