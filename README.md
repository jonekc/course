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

## User credentials

This project is for demonstration purposes only so the basic user credentials are explicit:

- login - student
- password - S%342)+sA

## Admin panel screenshots

![image](https://github.com/user-attachments/assets/ba2ac18b-b354-44b8-ab78-10bd9d89cb6e)
![image](https://github.com/user-attachments/assets/6e9cbd40-01d1-4d20-9719-8195c0df016f)
![image](https://github.com/user-attachments/assets/04acb2d9-bc09-435d-ae00-543b7dc4c6a6)
![image](https://github.com/user-attachments/assets/1ca05b84-2d92-478b-8460-86ccd7e836c9)



