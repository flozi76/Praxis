## docker-compose build -> buildet die containers neu
## docker-compose build --no-cache -> buildet die containers neu ohne cache
## 
## docker-compose up --force-recreate --> für dev
## 
## docker-compose up -d -> Startet prod umgebung
## docker-compose  -f docker-compose.dev.yml up -d -> startet dev Umgebung
## 
## docker-compose -f docker-compose.dev.yml stop

# build duftfinder docker container
docker build . -t duftfinder

#start duftfinder normal mode
docker run -d -p 8080:80 -p 44380:443 -e MongoDatabaseName=duftfinder -e MongoConnectionString='mongodb+srv://duftfinderweb:aed72ziGn0RE209oAbcR@duftfinder-vdzwu.mongodb.net/test?retryWrites=true' duftfinder

# connect container bash
docker exec -it duftfinder bash
