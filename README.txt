Converts files with T-SQL quieries to the files with the same names but with *.txt extension and which contains results of quieries.

Supports simple parameters, which can be added like this:

	DECLARE @NumDataSource int;
	SELECT @NumDataSource = /*$DataSourceParam*/28282/*$*/
	
Here we create param "DataSourceParam" with value "28282". 
You can change value of param later from program.