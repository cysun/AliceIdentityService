CREATE UNIQUE INDEX "UserEmailIndex" ON "AspNetUsers" (LOWER("Email") varchar_pattern_ops);
CREATE INDEX "UserFirstNameIndex" ON "AspNetUsers" (LOWER("FirstName") varchar_pattern_ops);
CREATE INDEX "UserLastNameIndex" ON "AspNetUsers" (LOWER("LastName") varchar_pattern_ops);
CREATE INDEX "UserFullNameIndex" ON "AspNetUsers" (LOWER("FirstName" || ' ' || "LastName") varchar_pattern_ops);

CREATE OR REPLACE FUNCTION "SearchUsersByPrefix"(prefix varchar) RETURNS SETOF "AspNetUsers" AS
$BODY$
BEGIN
    RETURN QUERY SELECT * FROM "AspNetUsers" WHERE
        LOWER("FirstName") LIKE prefix || '%' OR
        LOWER("LastName") LIKE prefix || '%' OR
        LOWER("FirstName" || ' ' || "LastName") LIKE prefix || '%' OR
        LOWER("Email") LIKE prefix || '%'
        ORDER BY "FirstName", "LastName";
    RETURN;
 END
$BODY$
LANGUAGE plpgsql;
