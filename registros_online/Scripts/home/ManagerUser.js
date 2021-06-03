
function actualizarLista(e) {
    $('#modalUser').modal('hide');
    angular.element(document.getElementById('angularId')).scope().LoadUsers();
}