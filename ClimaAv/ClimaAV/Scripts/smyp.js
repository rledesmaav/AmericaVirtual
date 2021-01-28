function SoloNumeros(e) {

    var keynum = window.event ? window.event.keyCode : e.which;

    if ((keynum == 0) || (keynum == 8) )
        return true;
    return /\d/.test(String.fromCharCode(keynum));
}

function CartelitoOk(msg) {

    alertify.set({ delay: 7000 });
    alertify.success(msg);
    return false;
}

function CartelitoError(msg) {

    alertify.set({ delay: 5000 });
    alertify.error(msg);
    return false;
}

function validarFechas(fecha1, fecha2) {

    fechaD = new Date(fecha1);
    fechaH = new Date(fecha2);

    if (fechaD > fechaH) {
        $("#FechaD").focus();
        return false;
    }
    else {
        return true;
    }
}

function IsNumeric(valor) {
    var log = valor.length; var sw = "S";
    for (x = 0; x < log; x++) {
        v1 = valor.substr(x, 1);
        v2 = parseInt(v1);
        //Compruebo si es un valor numérico 
        if (isNaN(v2)) { sw = "N"; }
    }
    if (sw == "S") { return true; } else { return false; }
}

var primerslap = false;
var segundoslap = false;

function formateafecha(fecha) {

    var long = fecha.length;
    var dia;
    var mes;
    var ano;
    if ((long >= 2) && (primerslap == false)) {
        dia = fecha.substr(0, 2);
        if ((IsNumeric(dia) == true) && (dia <= 31) && (dia != "00")) { fecha = fecha.substr(0, 2) + "/" + fecha.substr(3, 7); primerslap = true; }
        else { fecha = ""; primerslap = false; }
    }
    else {
        dia = fecha.substr(0, 1);
        if (IsNumeric(dia) == false) { fecha = ""; }
        if ((long <= 2) && (primerslap = true)) { fecha = fecha.substr(0, 1); primerslap = false; }
    }
    if ((long >= 5) && (segundoslap == false)) {
        mes = fecha.substr(3, 2);
        if ((IsNumeric(mes) == true) && (mes <= 12) && (mes != "00")) { fecha = fecha.substr(0, 5) + "/" + fecha.substr(6, 4); segundoslap = true; }
        else { fecha = fecha.substr(0, 3);; segundoslap = false; }
    }
    else { if ((long <= 5) && (segundoslap = true)) { fecha = fecha.substr(0, 4); segundoslap = false; } }
    if (long >= 7) {
        ano = fecha.substr(6, 4);
        if (IsNumeric(ano) == false) { fecha = fecha.substr(0, 6); }
        else { if (long == 10) { if ((ano == 0) || (ano < 1900) || (ano > 2100)) { fecha = fecha.substr(0, 6); } } }
    }
    if (long >= 10) {
        fecha = fecha.substr(0, 10);
        dia = fecha.substr(0, 2);
        mes = fecha.substr(3, 2);
        ano = fecha.substr(6, 4);
        // Año no viciesto y es febrero y el dia es mayor a 28 
        if ((ano % 4 != 0) && (mes == 02) && (dia > 28)) { fecha = fecha.substr(0, 2) + "/"; }
    }
    return (fecha);
}

function Mayuscula(el) {
    if (!el || !el.value) return;
    el.value = el.value.toUpperCase();
}

function showCantidades() {
    entra = document.getElementById("Entrada");
    sale = document.getElementById("Salida");

    check = document.getElementById("checkTipo");
    if (check.checked) {
        //entra.style.display = 'block';
        sale.value = "";
        sale.readOnly = true;
        entra.readOnly = false;
    }
    else {
        entra.value = "";
        entra.readOnly = true;
        sale.readOnly = false;
    }
}