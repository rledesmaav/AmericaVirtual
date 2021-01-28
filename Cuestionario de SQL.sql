/*Teniendo en cuenta el punto 23 donde pide realizar los puntos  2, 3, 4, 9, 11, 14, 16, 22 solamente*/

--2. Listar nombre y edad de los estudiantes
Select Nombre, Edad from [dbo].[Estudiante]

--3. ¿Qué estudiantes pertenecen a la carrera de Informática?
Select IdLector, Nombre from [dbo].[Estudiante]
Where carrera = 'Informatica'

--4. Listar los nombres de los estudiantes cuyo apellido comience con la letra G?
--NOTA: La tabla Estudiante no contiene el campo apellido, pero de tenerlo:
Select Apellido from [dbo].[Estudiante]
Where Apellido like '%G*'


--9. Listar el nombre del estudiante de menor edad
--NOTA: Como la edad es un integer el select devolvera todos los 
--estudiantes que coincidan con la menor edad registrada
Select Nombre from Estudiante where Edad = (select min(Edad) from Estudiante)


--11. Listar los libros de editorial “ALFA” y de “OMEGA”
select *
from libro
where 
Editorial = 'ALFA' or Editorial = 'OMEGA'

--14. Hallar el promedio de edad de los estudiantes
SELECT 
   AVG(Edad)
FROM
   Estudiante


--16. Crear un Stored Procedure que muestre los libros de un determinado Autor que se
--especique.


-- =============================================
-- Autor:		Santagati
-- Fecha Create:20210127
-- Nota:		Se asume que se proporciona el ID de autor por sistema (no el nombre)
-- =============================================
CREATE PROCEDURE LibrosXAutor 
@Autor int
AS
BEGIN
select l.* 
	from libro l
	join LibAut la on l.IdLibro = la.IdLibro 
	join Autor a on la.IdAutor = a.IdAutor
	where 
	a.IdAutor = @Autor
END


--22. ¿Puedes decir que los valores NULL equivalen a cero? 

/* No! cero es un valor, Null es la falta de valor alguno*/

--23. Responda las preguntas 2, 3, 4, 9, 11, 14, 16, 22 solamente.


/*Resto de los puntos sin tener en cuenta el punto 23*//*Preguntas tecnicas de relevamiento de tablas a realizar
Area en libro podria estar catalogado
Nacionalidad en Autor podria estar catalogado
Carrera en Estudiante podria estar catalogado*/


--1. Listar los datos de los autores que tengan más de un libro publicado
select a.* 
from LibAut la
join Autor a on la.IdAutor = a.IdAutor 
where
la.IdAutor in ( 
select IdAutor from LibAut
group by IdAutor 
having count(IdAutor) > 1 ) 

--5. ¿Quiénes son los autores del libro “Visual Studio Net”, listar solamente los nombres?
select a.Nombre 
from libro l
join LibAut la on l.IdLibro = la.IdLibro 
join Autor a on la.IdAutor = a.IdAutor
where 
l.Titulo = 'Visual Studio Net'

--6. ¿Qué autores son de nacionalidad “USA” o “Francia”?
select nombre from Autor where Nacionalidad in ('USA', 'Francia')

--7. ¿Qué libros NO Son del Area “CABA”?
select Titulo from Libro where Area <> 'CABA'

--8. ¿Qué libros se prestó del Lector “Felipe Loayza Beramendi”?
Select Titulo from Libro l
join Prestamo p on l.IdLibro = p.IdLibro
join Estudiante e on p.IdLector = e.IdLector and e.Nombre = 'Felipe Loayza Beramendi'

--10. Listar los nombres de los estudiantes que se prestaron Libros del área “BUENOS AIRES”
select e.Nombre 
from libro l
join Prestamo p on l.IdLibro = p.IdLibro 
join Estudiante e on p.IdLector = e.IdLector
where 
l.Area = 'BUENOS AIRES'

--12. Listar los libros que pertenecen al autor “FELIPE PIGNA”
select l.* 
from libro l
join LibAut la on l.IdLibro = la.IdLibro 
join Autor a on la.IdAutor = a.IdAutor
where 
a.Nombre = 'FELIPE PIGNA'

--13. Listar los títulos de los libros que debían devolverse el “10/04/2020”
select l.Titulo 
from libro l
join Prestamo p on l.IdLibro = p.IdLibro 
where
p.FechaDevolucion = '10/04/2020'

--15. Listar los datos de los estudiantes cuya edad es mayor al promedio
Select e.*
from Estudiante e
where
Edad > (SELECT 
   AVG(Edad)
FROM
   Estudiante)

--17. Crear un Stored Procedure que inserte nuevos Estudiantes
-- =============================================
-- Autor:		Santagati
-- Fecha Create:20210127
-- Nota:		Se asume que el ID de Estudiante (IdLector) es un autonumerico 1:1
-- =============================================
CREATE PROCEDURE LibrosXAutor 
    @CI varchar(50)
    ,@Nombre varchar(50)
    ,@Direccion varchar(50)
    ,@Carrera varchar(50)
    ,@Edad int
AS
BEGIN

INSERT INTO [dbo].[Estudiante]
           (
           [CI]
           ,[Nombre]
           ,[Direccion]
           ,[Carrera]
           ,[Edad])
     VALUES
           (
           @CI
           ,@Nombre
           ,@Direccion
           ,@Carrera
           ,@Edad
		   )

END

--18. Crear un Stored Procedure que actualice cualquier Libro especicando su código.

-- =============================================
-- Autor:		Santagati
-- Fecha Create:20210127
-- Nota:		Asignando el ID de libro devuelve el si el libro se encuentra disponible o no
-- =============================================
CREATE PROCEDURE LibrosXAutor 
@IdLibro
AS
BEGIN

Select l.Titulo, isnull(p.devuelto, 1) Disponibilidad
from Libro l
left join Prestamo p on l.IdLibro = p.IdLibro
where l.IdLibro = @IdLibro

END

--19. Crear un disparador DML que permita listar los registros de la Tabla Estudiante luego de
--insertar un nuevo registro.

-- =============================================
-- Author:		Santagati
-- Create date: 20210127

-- =============================================
CREATE TRIGGER TR_EstudianteIns 
   ON  Estudiante 
   AFTER Insert
AS 
BEGIN

Select * from Estudiante

END

--20. Crear una Función (que devuelva una Tabla) que liste los préstamos solicitados por un
--determinado alumno

-- =============================================
-- Author:		Santagati
-- Create date: 20210127
-- =============================================
CREATE FUNCTION GetPrestamos(@IdLector int)
RETURNS TABLE
AS RETURN
SELECT l.titulo, e.Nombre, p.FechaPrestamo
FROM Libro l
join Prestamo p on l.IdLibro = p.IdLibro 
join Estudiante e on p.IdLector = e.IdLector 
WHERE 
	e.IdLector  = ISNULL(@IdLector, e.IdLector)


--21. ¿Cuáles son las diferencias entre los comandos ‘delete’ y ‘truncate’
/*
DELETE
Borra una serie de filas de la tabla. 
Podemos usar una claúsula WHERE para limitar las filas a borrar, a las que cumplan una condición. La sintaxis sería:

DELETE FROM Estudiante WHERE IdLector = 1

TRUNCATE

A diferencia de DELETE, TRUNCATE elimina todas las filas de la tabla sin borrar la tabla

TRUNCATE TABLE Estudiante;

Semejanzas y diferencias:

1. Ambas eliminan los datos, no la estructura.
2. Solo DELETE permite la eliminación condicional de los registros.
3. DELETE es una operación registrada en el log de transacciones, por cada eliminación individual.
4. TRUNCATE se registra como una liberación de las páginas de datos en las cuales existen los datos.
5. Ambas se pueden deshacer con un ROLLBACK.
6. TRUNCATE reiniciará el contador para una tabla que contenga una columna IDENTITY.
7. DELETE mantendrá el contador de la tabla para una columna IDENTITY.
8. TRUNCATE es un comando DDL(lenguaje de definición de datos) mientras que DELETE es un DML(lenguaje de manipulación de datos).
9. TRUNCATE no desencadena un TRIGGER, DELETE sí.

*/


--24. Genere una sentencia para crear una tabla vacía llamada AUTOR2 identica a la tabla
--AUTOR que ya tiene datos.CREATE TABLE [dbo].[Autor2](
	[IdAutor] [int] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Nacionalidad] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Autor2] PRIMARY KEY CLUSTERED 
(
	[IdAutor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
