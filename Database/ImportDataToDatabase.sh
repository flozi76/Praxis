until mongo --eval "print(\"waited for connection\")"
  do
    sleep 5s
  done

FILES=./*

#chmod +x ImportDataToDatabase.sh
#./ImportDataToDatabase.sh

    #cd setup/DuftfinderData
    for f in $FILES
    do
    fullfilename=$f
        filename=$(basename "$fullfilename")
        fname="${filename%.*}"
        ext="${filename##*.}"

        echo $filename
        echo $fname
        #echo $ext
        echo "importing file $f to collection $fname... "
        mongoimport --db duftfinder --collection "$fname" --drop --file "$f"

    # take action on each file. $f store current file name
    #cat $f
    done