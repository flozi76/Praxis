FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# install ssl certificate

COPY localhost.conf /app
COPY pwdfile /app
RUN apt-get update
RUN apt install libnss3-tools -y

# RUN mkdir -p $HOME/.pki/nssdb
# # RUN certutil -d $HOME/.pki/nssdb -N -f pwdfile
# RUN certutil -d $HOME/.pki/nssdb -N -password pass:duftfinder1

# RUN openssl req -config localhost.conf -new -x509 -sha256 -newkey rsa:2048 -nodes \
#     -keyout localhost.key -days 3650 -out localhost.crt
# # RUN openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt
# RUN openssl pkcs12 -export -out localhost.pfx -inkey localhost.key -in localhost.crt -password pass:duftfinder1

# RUN certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n 'dev cert' -i localhost.crt -password pass:duftfinder1

ENV certPassword duftfinder1

# Use opnssl to generate a self signed certificate cert.pfx with password $env:certPassword
RUN openssl genrsa -des3 -passout pass:${certPassword} -out server.key 2048
RUN openssl rsa -passin pass:${certPassword} -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=localhost'
RUN openssl x509 -req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out localhost.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:${certPassword}


##


FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . ./

RUN dotnet restore "Duftfinder.Web/Duftfinder.Web.csproj"
WORKDIR "/src/Duftfinder.Web"
RUN dotnet build "Duftfinder.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Duftfinder.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "duftfinder.web.dll"]