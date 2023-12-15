$(document).ready(function () {
    //localStorage.removeItem("token")
    if (!localStorage.getItem("token")) {
        window.location.href = "https://localhost:44386/Login/Index";
    }
})
$("#buscar").on('click', function () {

    var formData = $("#form-buscar").serialize();
    $.get("/Reserva/listadoTablaJs?" + formData, function (data) {
        var contenido = "";


        for (var i = 0; i < data.length; i++) {
            var fechaFormateada = data[i].dmeDateReservation != null ? moment(data[i].dmeDateReservation).format("DD/MM/YYYY HH:mm") : "";
            contenido += "<tr >"
            contenido += "<th>" + data[i].idItem + "</th>"
            contenido += "<td>" + data[i].varCode + "</td>"
            contenido += "<td>" + data[i].varTitle + "</td>"
            contenido += "<td>" + data[i].varStatus + "</td>"
            contenido += "<td>" + fechaFormateada + "</td>"
            contenido += "</tr>"
        }

        document.getElementById("data").innerHTML = contenido;
    })
});

var data = [];
var titulo = $("#titulo-modal");
var cuerpo = $("#cuerpo-modal");
$("table tbody").on('click', 'tr', function () {
    data = [];
    $(this).find('td').each(function () {
        data.push($(this).text());
    });
    
    $.get("/Reserva/validarReservaJs?varCode=" + data[0], function (data2) {
        if (data2.resultado === 0) {
            $(titulo).text("Confirmar");
            $(cuerpo).text("¿ESTA SEGURO QUE DESEA RESERVAR ESTE LIBRO: " + data[1] + "?");
            $("#modalConfirmacion").find(".modal-footer button").removeClass("d-none");
            $("#modalConfirmacion").modal("show");
        } else {
            $(titulo).text("Error");
            $(cuerpo).text("NO ES POSIBLE RESERVAR EL LIBRO TITULO DEL LIBRO.");
            //$("#buscar").trigger("click");
            $("#modalConfirmacion").find(".modal-footer button").addClass("d-none");
            $("#modalConfirmacion").modal("show");
        }
    })
});

$("#reservar").on("click", function () {
    var dataPost = {};

    
    dataPost = {
        varCode: data[0],
        idUser: localStorage.getItem("idUser") //Añadir el local storage
    }
    $.post("/Reserva/registrarReserva", dataPost, function (data3) {
        if (data3.resultado !== 1) {
            $(titulo).text("Error");
            $(cuerpo).text(data3.resultado);

            $("#modalConfirmacion").find(".modal-footer button").addClass("d-none");
            $("#modalConfirmacion").modal("show");
        }
        else {
            cerrarModal();
        }
        $("#buscar").trigger("click");
    })
});

function cerrarModal() {
    $("#modalConfirmacion").modal("hide");
}

$("#cerrar").on("click", function () {
    cerrarModal();
})





