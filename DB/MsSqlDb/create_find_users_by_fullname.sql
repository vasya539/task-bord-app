CREATE PROCEDURE  FindUsersByFullNameTemplatePlusOneRow
		@firstName NVARCHAR(100), @lastName NVARCHAR(100), @pageOffset INT, @count INT
AS
BEGIN
	SELECT
		r.[Id], r.[UserName], r.[Email], r.[FirstName], r.[LastName]
		FROM (
		SELECT u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName], 1 AS 'type'
			FROM [AspNetUsers] u
			WHERE CHARINDEX(@firstName, u.[FirstName], 0) = 1
				AND CHARINDEX(@lastName, u.[LastName], 0) = 1
	) AS r
		GROUP BY r.[Id], r.[UserName], r.[Email], r.[FirstName], r.[LastName]
		ORDER BY MIN(r.[type]), r.[UserName], r.[FirstName], r.[LastName]
		OFFSET (@pageOffset*@count) ROWS FETCH NEXT (@count+1) ROWS ONLY
END
