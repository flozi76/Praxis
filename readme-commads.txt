docker-compose build -> buildet die containers neu
docker-compose build --no-cache -> buildet die containers neu ohne cache

docker-compose up --force-recreate --> für dev

docker-compose up -d -> Startet prod umgebung
docker-compose  -f docker-compose.dev.yml up -d -> startet dev Umgebung

docker-compose -f docker-compose.dev.yml stop