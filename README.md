# Course application

A website with a C# programming tutorial. Lessons include descriptions and tests. Depending on the type of question, you can choose pre-prepared answers or write your own. Score is visible after the test. Administrator can create new courses for users and see the statistics.

## Running app locally

Execute a following command in the project root directory:

`export ConnectionStrings__StudyContext="" && dotnet run --project Server && unset ConnectionStrings__StudyContext`

Insert a correct Postgres database connection string

## Building and running a production Docker image

- `docker build --build-arg ConnectionStrings__StudyContext="" -t course .`

- `docker run -p 8080:80 --name course course`

## Database migrations

- adding a migration after a model change: `export ConnectionStrings__StudyContext="" && dotnet ef migrations add [name] --project Server && unset ConnectionStrings__StudyContext`
- removing the migration: `export ConnectionStrings__StudyContext="" && dotnet ef migrations remove --project Server && unset ConnectionStrings__StudyContext`
- updating the database: `export ConnectionStrings__StudyContext="" && dotnet ef database update --project Server && unset ConnectionStrings__StudyContext`
