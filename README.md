# DataBaseManager
simple data base manager, binary files, with indeces in btrees

queries de ejemplo:

		createtable alumno
		id integer
		nombre varchar
		activo boolean
		;

		droptable alumno;

		insert alumno 12 "juan" true;

		delete from alumno
		where nombre = "juan";

		select * from alumno
		where id = 12;

		select id nombre from alumno;

		createindex on alumno id primary;

		createindex on alumno nombre;

		dropindex on alumno nombre;

