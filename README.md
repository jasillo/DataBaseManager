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

el archivo query1.txt contiene queries para insercion de 10000 filas en una tabla, no tiene indeces, el campo id es unico asi que se le puede aplicar un indice primario a ese campo.
