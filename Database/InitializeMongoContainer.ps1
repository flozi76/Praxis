docker stop duftfinder.init
docker rm duftfinder.init
docker rmi duftfinder.db.init
docker build  --no-cache -t flozi76/duftfinder.db.init -f Dockerfile ./
docker run -d -p 27017:27017 --name duftfinder.db duftfinder.db.init
docker push flozi76/duftfinder.db.init

# docker exec -it duftfinder.init bash

# docker pull mongo
# docker rm duftfinder.init
# docker run -d -p 27017:27017 --name duftfinder.init mongo

# $fileDirectory = ".";
# #$parse_results = New-Object System.Collections.ArrayList;

# foreach($file in Get-ChildItem $fileDirectory -Filter *.json)
# {
    
#     $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file);

#     Write-Host "Initializing Collection $fileName with data from file: $file";

#     mongoimport --db duftfinder --collection "$fileName" --drop --file "$file"
# }

#docker ps
#docker commit c16378f943fe duftfinder.db
#docker images
#docker tag bb38976d03cf flozi76/duftfinder.db:latest
#docker push flozi76/duftfinder.db