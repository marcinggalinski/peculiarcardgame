# peculiarcardgame

Cards Against Humanity (https://www.cardsagainsthumanity.com) clone

## Setup

Application consists of two main parts: backend and frontend. Both of these, along with their dependencies, can be run standalone or in Docker, e.g. using provided Dockerfiles and `Docker Compose` files.

### Backend

First, follow instructions provided in `backend/README.md` file. In case of running backend using provided `docker-compose` file, you have to provide `ASPNETCORE_ENVIRONMENT`, `SqlServer__ConnectionString` and, if environment is other than development, `Authentication__Bearer__Key` environmental variables in `.env` file.

### Frontend

Follow instructions provided in `frontend/README.md` file.
