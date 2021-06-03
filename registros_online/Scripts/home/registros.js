function actualizarLista(e) {
    $('#newRecordModal').modal('hide');
    angular.element(document.getElementById('angularId')).scope().volverCargar();
    if (e.key=="error")
    angular.element(document.getElementById('angularId')).scope().errorAddRecord(e);
}
function eliminarSummary()
{
    summary = $('.validation-summary-errors ul')[0];
    if (summary)
        summary.innerHTML = "";
}
$(function () {
    $("#fecha").datepicker();

    $('.numero').keyup(function (event) {

        // skip for arrow keys
        // if (event.which >= 37 && event.which <= 40) {
            // event.preventDefault();
        // }

        $(this).val(function (index, value) {
            return value
              .replace(/\D/g, "")
              .replace(/([0-9])([0-9]{2})$/, '$1,$2')
              .replace(/\B(?=(\d{3})+(?!\d)\.?)/g, ".")
            ;
        });
    });


});
