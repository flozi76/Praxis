docker-compose build -> buildet die containers neu
docker-compose up --force-recreate --> für dev

docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d