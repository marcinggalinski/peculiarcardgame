# peculiarcardgame

Cards Against Humanity (https://www.cardsagainsthumanity.com) clone

## Setup

Application consists of two main parts: backend and frontend. Both of these, along with their dependencies, can be run standalone or in Docker, e.g. using provided Dockerfiles and `Docker Compose` files. Both parts are configured to use HTTPS, so additional certificates setup is required.

### Backend

First, follow instructions provided in `backend/README.md` file. In case of running backend using provided `docker-compose` file, you have to provide the following environment variables (e.g. using a `.env` file):
- `ASPNETCORE_ENVIRONMENT` - name of the environment
- `ASPNETCORE_Kestrel__Certificates__Default__Password` - HTTPS certificate password
- `Authentication__Bearer__Key` - secret key used to sign and validate JWTs
- `MSSQL_SA_PASSWORD` - password of the `sa` db user
- `SqlServer__ConnectionString` - connection string used by backend

There are also 3 volumes used by `docker compose`:
- `.docker/mssql/data/` - contains db data
- `.docker/mssql/log/` - contains db logs
- `.docker/https/` - has to contain `peculiarcardgame.cfx` certificate with password matching value stored in `ASPNETCORE_Kestrel__Certificates__Default__Password`

DB-related two don't require any setup, but the third one has to be populated with a valid certificate. One can generate a self-signed dev certificate using `dotnet dev-cert https -ep .docker/https/peculiarcardgame.pfx -p [password]`.

### Frontend

Follow instructions provided in `frontend/README.md` file.
