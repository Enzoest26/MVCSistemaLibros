$("#ingresar").on('click', function () {

    var data = $("#form-login").serialize();

    $.post("Login/login", data, function (response) {
        if (response.error) {
            localStorage.removeItem("token");
            localStorage.removeItem("idUser");
            return;
        }
        localStorage.setItem("idUser", response.userId);
        localStorage.setItem("token", response.token);
        window.location.href = "https://localhost:44386/Reserva/Index";
    });
});