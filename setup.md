# Documentation for setting up the project locally.

## Prerequisites
Before setting up the project itself you must ensure that you have the following items setup:
- WSL (Windows Subsystem for Linux) installed and configured - required due to use of Linux docker containers
- Docker
- Local instance of Microsoft SQL Server (Express works as well) - make sure you have a login created which you will use to connect the application, if not, you can follow the guide below

### Creating a login using SQL

Server-level - sufficient for local development. A login can also be made to be database-specific with only specific permissions.
However we assume that the local development base will only be used for our application so we do not need to go into such depths.

**NOTE**: This should **NOT** be used in publically-used databases under any circumstances. Especially if using a non-secure password
```sql
CREATE LOGIN my_user with PASSWORD = N'MyPassword'
```

## Setting up proxy.
Since the application uses an nginx container acting as a reverse proxy we need to configure our computer to know that requests to a specific address should point back to itself.

In order to do this you need to open `Notepad` with **Administrator** privileges and open the following file `hosts` (**NOTE** the lack of extension on the file) located at : `C:\Windows\System32\drivers\etc`
- In case the folder appears empty select `All Files (*.*)` in the menu for searching for files

Once you have opened the file you need to do the following:
- At the end of the file add `127.0.0.1` press `TAB` once and write `local-dev.homeowners.com`
- Save the file and close it
- Open it again to ensure that everything is saved correctly

## Setting up the application configuration

Inside the docker-compose project files you can find a `Sample-Config` folder and a `sample.env` file.
They hold the configuration files for both the API and WEB projects with as much data as possible without exposing sensitive information in order to allow the application to be setup more quickly for development

Begin by copying the `sample.env` file and renaming it to `.env`. Currently there are no variables inside it for changing but this could change in the future

Next up create a folder called `Config` copy the contents of `Sample-Config` inside it.
- You will need to update the `api-configuration.json` file's connection string located at `DatabaseSetings:ConnectionString`.
  - Currently the sample-config is configured to work with a local instance of Microsoft SQL server running on the same machine. In case it is running on a different port change `1433` with the corresponding port
    - The following SQL can be used to determine the port after you have connected to the server instance:
        ```sql
        USE MASTER
        GO
        xp_readerrorlog 0, 1, N'Server is listening on'
        GO
        ```
  - You need to change the `Database`, `User Id` and `Password` with account you have created as a Prerequisite
- The configurations come ready with the Observability (OpenSearch) configurations already setup. While not mandatory it is recommended to setup the OpenSearch instance as [documented](./lib/HomeOwners.Lib.Observability/Docs/OpenSearch/LocalSetup.md)

## Validating correct setup
After completing the setup you should check the healthcheck endpoints of the application with GET requests to validate that they are working as expected.
Endpoints:
- http://local-dev.homeowners.com:443/app/hc
- http://local-dev.homeowners.com:443/api/hc

## Initial Account setup
In order to have access to the application you need to manually insert a temporary administrator account in the database. Afterwards you can change its password to what you desire.

The default password provided in the SQL is "password"

```sql
DECLARE @Username NVARCHAR(MAX) = 'Admin';
DECLARE @FirstName NVARCHAR(MAX) = '...';
DECLARE @LastName NVARCHAR(MAX) = '...';
DECLARE @Email NVARCHAR(MAX) = 'myemail@email.com';

INSERT INTO homeowners.users ([username], [first_name], [last_name], [email], [password], [role], [is_deleted])
VALUES (
    @Username,
    @FirstName,
    @LastName,
    @Email,
    N'5E-88-48-98-DA-28-04-71-51-D0-E5-6F-8D-C6-29-27-73-60-3D-0D-6A-AB-BD-D6-2A-11-EF-72-1D-15-42-D8',
    'Administrator',
    0
);
```