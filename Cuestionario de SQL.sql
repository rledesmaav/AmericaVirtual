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

Echo
