-- DECLARE @tmpl NVARCHAR(100) = N'ol'
-- DECLARE @count INT = 3
-- DECLARE @pageOffset INT = 0

CREATE PROCEDURE FindUsersByTemplatePlusOneRow @template NVARCHAR(100), @pageOffset INT, @count INT
AS
BEGIN
	SELECT
		r.[Id], r.[UserName], r.[Email], r.[FirstName], r.[LastName]--, MIN(r.[type]) AS [type_min]
		FROM (
		SELECT u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName], 1 AS 'type'
			FROM [AspNetUsers] u
			WHERE CHARINDEX(@template, u.[UserName], 0) > 0
		UNION
		SELECT u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName], 2 AS 'type'
			FROM [AspNetUsers] u
			WHERE CHARINDEX(@template, u.[FirstName], 0) > 0
		UNION
		SELECT u.[Id], u.[UserName], u.[Email], u.[FirstName], u.[LastName], 3 AS 'type'
			FROM [AspNetUsers] u
			WHERE CHARINDEX(@template, u.[LastName], 0) > 0
	) AS r
		GROUP BY r.[Id], r.[UserName], r.[Email], r.[FirstName], r.[LastName]
		ORDER BY MIN(r.[type]), r.[UserName], r.[FirstName], r.[LastName]
		OFFSET (@pageOffset*@count) ROWS FETCH NEXT (@count+1) ROWS ONLY
END
