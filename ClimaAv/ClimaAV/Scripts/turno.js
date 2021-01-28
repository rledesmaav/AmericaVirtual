const CargarEspecialidades = async (ente) => {

    var blanco = '<option>SIN DEFINIR</option>';
    $('#IdServicio').html(blanco);
    $('#IdEmpleado').html(blanco);

    try {
        const resPar = await fetch(`../Turno/ObtenerEspecialidades?ente=${ente}`);

        if (resPar.ok) {
            const data = await resPar.text();
            $('#IdEspecialidad').html(data);
        } else {
            throw ("ERROR DE SERVIDOR");
        }
    } catch (e) {
        alert(e);
    }
}

const CargarServicios = async (especialidad) => {

    try {
        const resPar = await fetch(`../Turno/ObtenerServicios?especialidad=${especialidad}`);

        if (resPar.ok) {
            const data = await resPar.text();
            $('#IdServicio').html(data);
        } else {
            throw ("ERROR DE SERVIDOR");
        }
    } catch (e) {
        alert(e);
    }
}

const CargarEmpleados = async (ente, servicio) => {

    try {
        const resPar = await fetch(`../Turno/ObtenerEmpleados?ente=${ente}&servicio=${servicio}`);

        if (resPar.ok) {
            const data = await resPar.text();
            $('#IdEmpleado').html(data);
        } else {
            throw ("ERROR DE SERVIDOR");
        }
    } catch (e) {
        alert(e);
    }
}

const CargarFechas = async (ente,servicio,empleado,sobreturno) => {

    try {
        const resPar = await fetch(`../Turno/ObtenerFechasDisponibles?ente=${ente}&servicio=${servicio}&empleado=${empleado}&sobreturno=${sobreturno}`);

        if (resPar.ok) {
            const data = await resPar.json();
            availableDates = data;
            //response.forEach(function () { });

            $(function () {
                $('#Fecha').datepicker({
                    beforeShowDay:
                        function (dt) {
                            return [Disponible(dt), ""];
                        }
                    , changeMonth: true, changeYear: false,
                    locale: {
                        format: 'dd/MM/yyyy'
                    }
                });
            });

            function Disponible(date) {
                dmy = date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear();
                if ($.inArray(dmy, availableDates) != -1) {
                    return true;
                } else {
                    return false;
                }
            }

        } else {
            throw ("ERROR DE SERVIDOR");
        }
    } catch (e) {
        alert(e);
    }
}

const CargarHorarios = async (ente,servicio,empleado,fecha,sobreturno) => {

    try {
        const resPar = await fetch(`../Turno/ObtenerHorarios?ente=${ente}&servicio=${servicio}&empleado=${empleado}&fecha=${fecha}&sobreturno=${sobreturno}`);
        if (resPar.ok) {
            const data = await resPar.text();
            $('#IdHorario').html(data);
        } else {
            throw ("ERROR DE SERVIDOR");
        }
    } catch (e) {
        alert(e);
    }
}

