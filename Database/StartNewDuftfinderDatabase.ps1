docker stop duftfinder.db
docker rm duftfinder.db
docker network create duftfinderNet
docker pull flozi76/duftfinder.db
docker run -d -p 27017:27017 --net duftfinderNet --name duftfinder.db flozi76/duftfinder.db