FROM mongo AS base
WORKDIR /app
EXPOSE 27017

COPY ./*.json /app/
COPY ImportDataToDatabase.sh /docker-entrypoint-initdb.d/
RUN apt-get install -y mongodb-org;

#RUN cd app;
#RUN sed -i 's/\r$//' ImportDataToDatabase.sh
#RUN chmod +x ImportDataToDatabase.sh
#RUN ./ImportDataToDatabase.sh