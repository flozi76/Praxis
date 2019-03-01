
$fileDirectory = ".";
#$parse_results = New-Object System.Collections.ArrayList;

foreach($file in Get-ChildItem $fileDirectory -Filter *.json)
{
    
    $fileName = [System.IO.Path]::GetFileNameWithoutExtension($file);

    Write-Host "Initializing Collection $fileName with data from file: $file";

    ##------- For local usages:
    #mongoimport --db duftfinder --collection "$fileName" --drop --file "$file"

    ##------- atlasdb in mongocloud:
    mongoimport --host duftfinder-shard-0/duftfinder-shard-00-00-vdzwu.mongodb.net:27017,duftfinder-shard-00-01-vdzwu.mongodb.net:27017,duftfinder-shard-00-02-vdzwu.mongodb.net:27017 --ssl --username administrator --password AHOZ5pn18Gari4IZ --authenticationDatabase admin --db duftfinder --collection "$fileName" --drop --file "$file"
}